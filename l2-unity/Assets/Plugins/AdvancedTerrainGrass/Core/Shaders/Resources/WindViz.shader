Shader "Wind Visualization X"
{
	Properties
	{
		[Enum(Main Wind,0,Turbulence,1,All,2)] _VizMode ("Visualization", Float) = 0
		_Transparency ("Opacity", Range(0,1)) = 1
		_Color ("Color", Color) = (0,0.8,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 200

		Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off 

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
	            float4 vertex : SV_POSITION;
	            float3 worldPos : TEXCOORD0;
	        };

			float _VizMode;
			fixed _Transparency;
			fixed4 _Color;

			CBUFFER_START(AtgGrass)
				sampler2D _AtgWindRT;
				float4 _AtgWindDirSize;
				float4 _AtgWindStrength;
				float2 _AtgSinTime;
				float4 _AtgGrassFadeProps;
				float4 _AtgGrassShadowFadeProps;
			CBUFFER_END

			v2f vert (appdata v)
	        {
	            v2f o;
	            o.vertex = UnityObjectToClipPos(v.vertex);
	            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	        //	HDRP
	            #if defined(SHADEROPTIONS_CAMERA_RELATIVE_RENDERING)
	            	#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING == 0)
	            		o.worldPos += _WorldSpaceCameraPos.xyz;
	            	#endif
	            #endif
	            return o;
	        }

	        fixed4 frag (v2f i) : SV_Target
	        {
	        	fixed4 c = tex2D (_AtgWindRT, i.worldPos.xz * _AtgWindDirSize.w).rgba;
	        	fixed3 albedo = _Color.rgb;
				if (_VizMode == 0) {
					albedo *= c.rrr;	
				}
				else if (_VizMode == 1) {
					albedo *= c.ggg;
				}
			//	Combined
				else {
					albedo *= (c.r * c.g).xxx;
					albedo = lerp(fixed3(1,0,0), albedo, saturate(1 + c.r * c.g)); 
				}
				fixed alpha = _Transparency;
				fixed4 col = fixed4(albedo, alpha);
				return col;
	        }
			ENDCG
		}
	}
}