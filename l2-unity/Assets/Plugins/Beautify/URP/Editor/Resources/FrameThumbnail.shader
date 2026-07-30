Shader "Hidden/Beautify/FrameThumbnail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", color) = (1,1,1,1)
        _FrameMask ("Frame Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float2 clipUV : TEXCOORD1;
            };

            sampler2D _LUTPreview;
            float4 _LUTPreview_ST;
			sampler2D _GUIClipTexture;
			uniform float4x4 unity_GUIClipTextureMatrix;

            sampler2D _FrameMask;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				float3 eyePos = UnityObjectToViewPos(v.vertex);
				o.clipUV = mul(unity_GUIClipTextureMatrix, float4(eyePos.xy, 0, 1.0));
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
			    clip( tex2D(_GUIClipTexture, i.clipUV).a - 0.1);

                half3 rgb = tex2D(_LUTPreview, i.uv);

    			half4 frameMask  = tex2D(_FrameMask, i.uv);
		    	rgb       = lerp(rgb, frameMask.rgb, frameMask.a);

                return half4(rgb, 1.0);
            }
            ENDCG
        }
    }
}
