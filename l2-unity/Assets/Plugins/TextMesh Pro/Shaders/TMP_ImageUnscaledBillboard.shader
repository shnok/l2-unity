Shader "TextMeshPro/Mobile/Image Billboard Unscaled"
{
    Properties
    {
        [HDR]_FaceColor     ("Tint Color", Color) = (1,1,1,1)

        _MainTex            ("Image", 2D) = "white" {}

        // Billboard / size controls
        _ScaleX             ("Scale X", float) = 1
        _ScaleY             ("Scale Y", float) = 1

        // Optional vertex offsets in local space
        _VertexOffsetX      ("Vertex OffsetX", float) = 0
        _VertexOffsetY      ("Vertex OffsetY", float) = 0

        // UI clipping
        _ClipRect           ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
        _MaskSoftnessX      ("Mask SoftnessX", float) = 0
        _MaskSoftnessY      ("Mask SoftnessY", float) = 0

        // Stencil & render state
        _StencilComp        ("Stencil Comparison", Float) = 8
        _Stencil            ("Stencil ID", Float) = 0
        _StencilOp          ("Stencil Operation", Float) = 0
        _StencilWriteMask   ("Stencil Write Mask", Float) = 255
        _StencilReadMask    ("Stencil Read Mask", Float) = 255

        _CullMode           ("Cull Mode", Float) = 0
        _ColorMask          ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
            // Important for billboard math – keeps per-object matrices
            "DisableBatching" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_CullMode]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex VertShader
            #pragma fragment PixShader

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _FaceColor;
            float _ScaleX;
            float _ScaleY;
            float _VertexOffsetX;
            float _VertexOffsetY;

            float4 _ClipRect;
            float _MaskSoftnessX;
            float _MaskSoftnessY;

            // Global (set from C#): xy = reference width/height (e.g., 1920,1080)
            // IMPORTANT: not in Properties on purpose -> treat as global.
            float _ReferenceScreenSize;
            float _UnityScreenSize;

            struct vertex_t
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4  vertex      : POSITION;
                float3  normal      : NORMAL;
                fixed4  color       : COLOR;
                float2  texcoord0   : TEXCOORD0;
                float2  texcoord1   : TEXCOORD1; // kept for TMP mesh compatibility, unused
            };

            struct pixel_t
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                float4  vertex      : SV_POSITION;
                fixed4  faceColor   : COLOR;
                float4  texcoord0   : TEXCOORD0;  // uv.xy, maskUV.xy
                half4   mask        : TEXCOORD1;  // clip pos (xy), softness (zw)
            };

            pixel_t VertShader(vertex_t input)
            {
                pixel_t output;
                UNITY_INITIALIZE_OUTPUT(pixel_t, output);
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // Apply optional per-vertex offset in local space
                float4 vert = input.vertex;
                vert.x += _VertexOffsetX;
                vert.y += _VertexOffsetY;

                // Pivot (object origin) in view space
                float4 viewPos = mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0));

                // Distance from camera (Unity view space uses -Z forward)
                float dist = -viewPos.z;

                // Local quad coords
                float2 quad = vert.xy;

                // Extract RectTransform scale from object->world
                float3 worldX = mul((float3x3)unity_ObjectToWorld, float3(1, 0, 0));
                float3 worldY = mul((float3x3)unity_ObjectToWorld, float3(0, 1, 0));

                float2 rectScale = float2(length(worldX), length(worldY)); // scaleX, scaleY

                // Base size in view-space units
                float2 sizeVS = float2(_ScaleX, _ScaleY);

                float ratio = _ReferenceScreenSize / _UnityScreenSize;

                // Offset in view space (unscaled billboard)
                float2 offsetVS = quad * sizeVS * rectScale * dist * ratio;
                viewPos.xy += offsetVS;

                // Project to clip space
                float4 vPosition = mul(UNITY_MATRIX_P, viewPos);

                // Pixel size estimation (used for mask softness)
                float2 pixelSize = vPosition.w;
                pixelSize /= float2(_ScaleX, _ScaleY) *
                             abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                // Generate UV for the masking rect
                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (vert.xy - clampedRect.xy) /
                                (clampedRect.zw - clampedRect.xy);

                output.vertex    = vPosition;
                output.faceColor = input.color * _FaceColor;

                output.texcoord0 = float4(input.texcoord0.xy, maskUV.xy);

                // Store clip position & softness in input.mask
                output.mask = half4(
                    vert.xy * 2 - clampedRect.xy - clampedRect.zw,
                    0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy)
                );

                return output;
            }

            fixed4 PixShader(pixel_t input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                // Sample the image
                fixed4 texCol = tex2D(_MainTex, input.texcoord0.xy);

                // Tint with vertex color & _FaceColor
                fixed4 c;
                c.a   = texCol.a * input.faceColor.a;
                c.rgb = texCol.rgb * input.faceColor.rgb;

                // Pre-multiply RGB by alpha (for Blend One OneMinusSrcAlpha)
                c.rgb *= c.a;

                // UI clip rect with softness
                #if UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) *
                                   input.mask.zw);
                c *= m.x * m.y;
                #endif

                #if UNITY_UI_ALPHACLIP
                clip(c.a - 0.001);
                #endif

                return c;
            }
            ENDCG
        }
    }
}