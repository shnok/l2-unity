Shader "Lux SRP Displacement/Simple Displacement Debug"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "bump" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend mode", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]
        LOD 100

        Pass
        {
            Name "LuxGrassDisplacementFX"
            Tags{"LightMode" = "LuxGrassDisplacementFX"}
            
            HLSLPROGRAM
            #pragma vertex SimpleDisplacementFXVertex
            #pragma fragment SimpleDisplacementFXFragment

            #pragma shader_feature_local _DYNAMICALPHA
            #pragma shader_feature_local _NORMAL

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                #if defined _NORMAL
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                #endif
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                #if defined _NORMAL
                    half3 normalWS : TEXCOORD1; 
                    half3 tangentWS : TEXCOORD2;
                    half3 bitangentWS : TEXCOORD3;   
                #endif
                half4 color : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                //half _Alpha;
            CBUFFER_END

            #if defined (_DYNAMICALPHA)
                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(half, _Alpha)
                UNITY_INSTANCING_BUFFER_END(Props)
            #endif
            
            sampler2D _MainTex;
            
            
            Varyings SimpleDisplacementFXVertex (Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexPosition = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexPosition.positionCS;
                output.uv = input.uv;
                output.color = input.color;

                #if defined _NORMAL
                    VertexNormalInputs vertexTBN = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = vertexTBN.normalWS;
                    output.tangentWS = vertexTBN.tangentWS;
                    output.bitangentWS = vertexTBN.bitangentWS;
                #endif

                return output;
            }
            
            half4 SimpleDisplacementFXFragment (Varyings input) : SV_Target
            {
                half4 col = tex2D(_MainTex, input.uv);
                col.a *= input.color.a
                #if defined(_DYNAMICALPHA)
                    * UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha)
                #endif
                ;

                #if defined(_NORMAL)
                    col.rgb = col.rgb * 2 - 1; // unpack
                    col.rgb = TransformTangentToWorld(col.rgb, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
                    col.rgb = col.rbg * 0.5 + 0.5; // swizzle to "tangent space" and repack
                #endif
                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Tags{"LightMode" = "LightweightForward"}
            
            HLSLPROGRAM
            #pragma vertex SimpleDisplacementFXVertex
            #pragma fragment SimpleDisplacementFXFragment

            #pragma shader_feature_local _DYNAMICALPHA
            #pragma shader_feature_local _NORMAL

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                #if defined _NORMAL
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                #endif
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                #if defined _NORMAL
                    half3 normalWS : TEXCOORD1; 
                    half3 tangentWS : TEXCOORD2;
                    half3 bitangentWS : TEXCOORD3;   
                #endif
                half4 color : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                //half _Alpha;
            CBUFFER_END

            #if defined (_DYNAMICALPHA)
                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(half, _Alpha)
                UNITY_INSTANCING_BUFFER_END(Props)
            #endif
            
            sampler2D _MainTex;
            
            
            Varyings SimpleDisplacementFXVertex (Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexPosition = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexPosition.positionCS;
                output.uv = input.uv;
                output.color = input.color;

                #if defined _NORMAL
                    VertexNormalInputs vertexTBN = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = vertexTBN.normalWS;
                    output.tangentWS = vertexTBN.tangentWS;
                    output.bitangentWS = vertexTBN.bitangentWS;
                #endif

                return output;
            }
            
            half4 SimpleDisplacementFXFragment (Varyings input) : SV_Target
            {
                half4 col = tex2D(_MainTex, input.uv);
                col.a *= input.color.a
                #if defined(_DYNAMICALPHA)
                    * UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha)
                #endif
                ;

                #if defined(_NORMAL)
                    col.rgb = col.rgb * 2 - 1; // unpack
                    col.rgb = TransformTangentToWorld(col.rgb, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
                    col.rgb = col.rbg * 0.5 + 0.5; // swizzle to "tangent space" and repack
                #endif
                return col;
            }
            ENDHLSL
        }
    }
}