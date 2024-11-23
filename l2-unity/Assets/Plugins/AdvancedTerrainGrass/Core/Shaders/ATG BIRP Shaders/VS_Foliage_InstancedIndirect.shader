Shader "AdvancedTerrainGrass/VegetationStudio/Foliage Shader" {
	Properties {

		[Space(8)]
		[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0

		[NoScaleOffset] _MainTex 					("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff 									("Alpha Cutoff", Range(0,1)) = 0.3

		[Space(8)]
		[HideInInspector]_MinMaxScales 				("MinMaxScale Factors", Vector) = (1,1,1,1)
		_HealthyColor 								("Healthy Color", Color) = (1,1,1,1)
		_DryColor 									("Dry Color", Color) = (1,1,1,1)


		[Header(Lighting)]
		[Space(8)]
		_NormalBend									("Bend Normal", Range(0,1)) = 0.5
		[Toggle(_SPECULARHIGHLIGHTS_OFF)] _Spec 	("Enable specular Highlights ", Float) = 0
		[NoScaleOffset]_SpecTex 					("    Normal (GA) Trans(R) Smoothness(B)", 2D) = "bump" {}
		_Glossiness									("Smoothness", Range(0,1)) = 0.5
		_SpecularReflectivity						("Specular Reflectivity", Color) = (0.2,0.2,0.2)
		_HorizonFade								("Horizon Fade", Range(0.0, 5.0)) = 1.0
		[Space(8)]
		_TransStrength 								("Translucency Strength", Range(0, 1)) = 1.0
		_TransPower 								("View Dependency", Range(0, 1)) = 0.8

		[Header(Wind)]
		[Space(8)]
		_WindMultiplier 							("Strength Primary (X) Secondary (Y)", Vector) = (1,1,0,0)
		[Toggle(_METALLICGLOSSMAP)] _SamplePivot 	("Sample Wind at Pivot", Float) = 0
		_WindLOD 									("Wind LOD (int)", Float) = 0

		[Header(VegetationStudio Specials)]
//		Needed by VS Pro
		[Toggle (_IGNOREGRASSFADE)] _IgnoreGrassFade("Ignore Fading (VS Pro)", Float) = 0
		[Toggle] TOUCH_BEND("Use touch bend", Float) = 1
		_ForceRadius("    Force (X) Radius (Y)", Vector) = (1,2,0,0)

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
		#pragma surface surf ATGSpecular fullforwardshadows vertex:vertfoliage addshadow nodynlightmap nolppv nometa
// nolightmap
		#pragma target 3.0

		#pragma multi_compile_instancing

		// Specular Highlights
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		// Wind Sample Pos
		#pragma shader_feature _METALLICGLOSSMAP

		// VS Pro
		#pragma shader_feature _IGNOREGRASSFADE

float InstanceScale;

#pragma multi_compile GPU_FRUSTUM_ON __
#pragma multi_compile TOUCH_BEND_ON __
#pragma multi_compile FAR_CULL_ON __

#pragma instancing_options procedural:setup

		#include "Includes/AtgPBSLighting.cginc"
		#include "TerrainEngine.cginc"
		#include "Includes/VS_InstancedIndirect_Inputs.cginc"
		#include "Includes/VS_FoliageInstancedIndirect_Vertex.cginc"

		sampler2D _MainTex;
		#if defined(_SPECULARHIGHLIGHTS_OFF)
			sampler2D _SpecTex;
		#endif
		
				
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
