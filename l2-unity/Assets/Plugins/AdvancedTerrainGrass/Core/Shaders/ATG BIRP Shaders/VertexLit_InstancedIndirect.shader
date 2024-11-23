Shader "AdvancedTerrainGrass/VertexLit Shader" {
	Properties {
		
		[NoScaleOffset] _MainTex 				("Albedo (RGB) Smoothness (A)", 2D) = "white" {}
		[HideInInspector] _MinMaxScales 		("MinMaxScale Factors", Vector) = (1,1,1,1)
		_HealthyColor 							("Healthy Color", Color) = (1,1,1,1)
		_DryColor 								("Dry Color", Color) = (1,1,1,1)

		[Space(6)]
		_SpecularReflectivity					("Specular Reflectivity", Color) = (0.2,0.2,0.2)

		[Space(6)]
		[Toggle(_NORMALMAP)] _EnableNormal		("Enable Normal Map ", Float) = 0
		[NoScaleOffset] _BumpMap				("    Normal Map", 2D) = "bump" {}

	}
	SubShader {
		Tags { 
			"RenderType"="ATGVertexLit"
			"IgnoreProjector"="True"
		}
		LOD 200
		
		CGPROGRAM
// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does	
		#pragma surface surf StandardSpecular vertex:vert fullforwardshadows addshadow nodynlightmap nolppv nometa
// nolightmap
		#pragma target 3.0
		#pragma multi_compile_instancing
	//	assumeuniformscaling --> so we do not need the WorldToObject matrix!!!!! for the normals
		#pragma instancing_options assumeuniformscaling procedural:setup

		#pragma shader_feature _NORMALMAP

		sampler2D _MainTex;
		float InstanceScale;

		struct Input {
			float2 uv_MainTex;
			fixed3 instanceColor;
		};

	//	Define AtgGrass CBUFFER as we might use FadeProps in the future
		CBUFFER_START(AtgGrass)
			float4 _AtgWindDirSize;
			float4 _AtgWindStrength;
			float _AtgSinTime;
			float _AtgSinTime1;
			float4 _AtgGrassFadeProps;
			float4 _AtgGrassShadowFadeProps;

			float3 _AtgSurfaceCameraPosition;
		CBUFFER_END
		
		//sampler2D _AtgWindRT;
		float4 _AtgTerrainShiftSurface;

		fixed4 _HealthyColor;
		fixed4 _DryColor;
		float2 _MinMaxScales;

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<float4x4> GrassMatrixBuffer;
		#endif

		void setup() {
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float4x4 data = GrassMatrixBuffer[unity_InstanceID];
			unity_ObjectToWorld = data;

		//	Handle Floating Origin
			float3 shift = _AtgTerrainShiftSurface.xyz * _AtgTerrainShiftSurface.w;
			unity_ObjectToWorld[0].w -= shift.x;
			unity_ObjectToWorld[1].w -= shift.y;
			unity_ObjectToWorld[2].w -= shift.z;

		//	Restore matrix as it could contain layer data here!
			InstanceScale = frac(unity_ObjectToWorld[3].w);
			InstanceScale *= 100.0f;
			unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);

		//	Bullshit!
		//	unity_WorldToObject = unity_ObjectToWorld;
		//	unity_WorldToObject._14_24_34 *= -1;
		//	unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
		
		// 	Not entirely correct but good enough? to get the wind direction in objectspace
		//	Not needed here
		/*	unity_WorldToObject = unity_ObjectToWorld;
			unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
			unity_WorldToObject._11_22_33 *= -1; */

		//	Seems to be rather cheap - on: 34 / off 36fps
			//unity_WorldToObject = inverseMat(unity_ObjectToWorld); //inverspositionBuffer[unity_InstanceID];
		
		#endif
		}

		void vert(inout appdata_tan v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
		
		// 	Scale contains some perlin noise
			//float3 unitvec = mul( (float3x3 )unity_ObjectToWorld, float3(1,0,0)); // float4 would be 0,1,0, 0 !!!!!
			//float scale = length( unitvec );
			float scale = InstanceScale;
		//	Lerp instanceColor according to scale (which has to be normalized)
			o.instanceColor = lerp(_HealthyColor, _DryColor, (scale - _MinMaxScales.x) * _MinMaxScales.y);
		}

		#if defined(_NORMALMAP)
			sampler2D _BumpMap;
		#endif
		half3 _SpecularReflectivity;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.instanceColor;
			#if defined(_NORMALMAP)
				o.Normal = UnpackNormal( tex2D(_BumpMap, IN.uv_MainTex));
			#endif
			o.Specular = _SpecularReflectivity;
			o.Smoothness = c.a;
			o.Alpha = 1;
		}
		ENDCG
	}
}
