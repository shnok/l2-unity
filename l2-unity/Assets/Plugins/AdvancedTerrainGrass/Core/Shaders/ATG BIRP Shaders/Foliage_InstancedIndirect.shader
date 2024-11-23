Shader "AdvancedTerrainGrass/Foliage Shader" {
	Properties {

		[Space(6)]
		[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0

		[NoScaleOffset] _MainTex 					("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff 									("Alpha Cutoff", Range(0,1)) = 0.3

		[Space(6)]
		[HideInInspector]_MinMaxScales 				("MinMaxScale Factors", Vector) = (1,1,1,1)
		_HealthyColor 								("Healthy Color", Color) = (1,1,1,1)
		_DryColor 									("Dry Color", Color) = (1,1,1,1)


		[Header(Lighting)]
		[Space(4)]
		_NormalBend									("Bend Normal", Range(0,1)) = 0.5
		[Toggle(_SPECULARHIGHLIGHTS_OFF)] _Spec 	("Enable specular Highlights ", Float) = 0
		[NoScaleOffset]_SpecTex 					("    Normal (GA) Trans(R) Smoothness(B)", 2D) = "bump" {}
		_Glossiness									("Smoothness", Range(0,1)) = 0.5
		_SpecularReflectivity						("Specular Reflectivity", Color) = (0.2,0.2,0.2)
		_HorizonFade								("Horizon Fade", Range(0.0, 5.0)) = 1.0
		[Space(6)]
		_TransStrength 								("Translucency Strength", Range(0, 1)) = 1.0
		_TransPower 								("View Dependency", Range(0, 1)) = 0.8

		[Header(Wind)]
		[Space(4)]
		_WindMultiplier 							("Strength Primary (X) Secondary (Y)", Vector) = (1,1,0,0)
		[Toggle(_METALLICGLOSSMAP)] _SamplePivot 	("Sample Wind at Pivot", Float) = 0
		_WindLOD 									("Wind LOD (int)", Float) = 0

	}
	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="ATGFoliage"
		//	In order to adjust the wind settings on single instances:
			"DisableBatching"="True"
		}
		LOD 200
		Cull [_Culling]
		
		CGPROGRAM
// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does
		#pragma surface surf ATGSpecular vertex:vertfoliage fullforwardshadows  addshadow nodynlightmap nolppv nometa
// nolightmap
		#pragma target 3.0

		#pragma multi_compile_instancing
	//	assumeuniformscaling --> so we do not need the proper WorldToObject matrix for the normals
		#pragma instancing_options assumeuniformscaling procedural:setup

		// Specular Highlights
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		// Wind Sample Pos
		#pragma shader_feature _METALLICGLOSSMAP


float InstanceScale;

		#include "Includes/AtgPBSLighting.cginc"
		#include "TerrainEngine.cginc"
		#include "Includes/FoliageInstancedIndirect_Vertex.cginc"

		sampler2D _MainTex;
		#if defined(_SPECULARHIGHLIGHTS_OFF)
			sampler2D _SpecTex;
		#endif
		
				
		void setup()
		{
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

		// 	not entirely correct but good enough? to get the wind direction in objectspace
			unity_WorldToObject = unity_ObjectToWorld;
			unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
			unity_WorldToObject._11_22_33 *= -1;

		//	Seems to be rather cheap - on: 34 / off 36fps
			//unity_WorldToObject = inverseMat(unity_ObjectToWorld); //inverspositionBuffer[unity_InstanceID];
		#endif
		}

		fixed4 _Color;
		fixed _Glossiness;
		fixed _Cutoff;
		half _TransPower;
		half _TransStrength;
		fixed3 _SpecularReflectivity;

		void surf (Input IN, inout SurfaceOutputATGSpecular o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;

			clip(c.a - _Cutoff);

			#if defined(_SPECULARHIGHLIGHTS_OFF)
				fixed4 trngls = tex2D(_SpecTex, IN.uv_MainTex.xy);
				o.Smoothness = trngls.b;
				#if defined(SHADER_API_METAL)
					o.Normal = UnpackNormalDXT5nm(trngls) * half3(1, 1, -IN.facingSign);
					o.VertexNormal = WorldNormalVector(IN, half3(0, 0, -IN.facingSign));
				#else
					o.Normal = UnpackNormalDXT5nm(trngls) * half3(1, 1, IN.facingSign);
					o.VertexNormal = WorldNormalVector(IN, half3(0, 0, IN.facingSign));
				#endif
				o.Translucency = trngls.r * _TransStrength; // * IN.color.a;
				o.Specular = _SpecularReflectivity;
			#else
				o.Smoothness = _Glossiness;
				#if defined(SHADER_API_METAL)
					o.Normal = half3(0,0,1) * float3(1, 1, -IN.facingSign);
					o.VertexNormal = WorldNormalVector(IN, half3(0, 0, -IN.facingSign));
				#else
					o.Normal = half3(0,0,1) * half3(1, 1, IN.facingSign);
					o.VertexNormal = WorldNormalVector(IN, half3(0, 0, IN.facingSign));
				#endif
				o.Translucency = _TransStrength; // * IN.color.a;
			#endif
			o.Alpha = c.a;
			o.TranslucencyPower = _TransPower;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
