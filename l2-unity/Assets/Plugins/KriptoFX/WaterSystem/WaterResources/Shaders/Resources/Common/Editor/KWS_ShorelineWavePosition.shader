Shader "Hidden/KriptoFX/KWS/ShorelineWavePosition"
{
	Properties
	{

	}
	SubShader
	{


		Pass
		{

			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 uv2 : TEXCOORD1;
				float4 uv3 : TEXCOORD2;
				float4 uv4 : TEXCOORD3;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 uv2 : TEXCOORD1;
				float4 uv3 : TEXCOORD2;
				float4 uv4 : TEXCOORD3;
			};

			struct FragmentOutput
			{
				half4 dest0 : SV_Target0;
				half4 dest1 : SV_Target1;
			};

			uint _ShorelineBake_mrtBufferIdx;
			float3 KW_ShorelineAreaPos;
			float KW_ShorelineAreaSize;

			float4 Test4;

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = v.uv;
				o.uv2 = v.uv2;
				o.uv3 = v.uv3;
				o.uv4 = v.uv4;


				o.vertex = float4((v.vertex.xz - KW_ShorelineAreaPos.xz) / KW_ShorelineAreaSize * 2, 0, 1);
				o.vertex.y = -o.vertex.y;

				return o;
			}

			FragmentOutput frag(v2f i) 
			{
				FragmentOutput o;
				float2 waveUVOffset = i.uv2;
				float waveScale = i.uv3.x;
				float timeOffset = i.uv3.y;
				float angle = i.uv4.x;


				float alpha = 0;
				if (i.uv.x > waveUVOffset.x && i.uv.x < 1.0 - waveUVOffset.x && i.uv.y > waveUVOffset.y && i.uv.y < 1.0 - waveUVOffset.y) alpha = 1;

				i.uv.x = 1 - i.uv.x;

				/*if (_ShorelineBake_mrtBufferIdx == 0) return float4(i.uv.xy * (1 + waveUVOffset.xy * 2) - waveUVOffset.xy, angle, alpha);
				if (_ShorelineBake_mrtBufferIdx == 1) return float4(timeOffset, waveScale, 0, 1);
				return 0;*/
				o.dest0 = float4(i.uv.xy * (1 + waveUVOffset.xy * 2) - waveUVOffset.xy, angle, alpha);
				o.dest1 = float4(timeOffset, waveScale, 0, 1);

				//return 1;
				return o;
			}
			ENDCG
		}
	}
}
