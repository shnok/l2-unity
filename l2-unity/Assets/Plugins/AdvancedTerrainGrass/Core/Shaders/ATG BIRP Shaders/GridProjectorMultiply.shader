Shader "AdvancedTerrainGrass/GridProjector" {
	Properties {
		_MainTex ("Cookie", 2D) = "gray" {}
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			float4 _MainTex_ST;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = mul (unity_Projector, vertex    );
				return o;
			}
			
			sampler2D _MainTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 tex = tex2D (_MainTex, i.uv * _MainTex_ST.xx - float2(0.5, 0.5) );
				tex = saturate( tex + 0.35);
				return tex;
			}
			ENDCG
		}
	}
}
