Shader "AdvancedTerrainGrass/Grass Array Shader" {
	Properties {

		[Space(8)]
		[Header(For the terrain engine only)]
		[NoScaleOffset] _MainTex 					("Albedo Tex (RGB) Alpha (A)", 2D) = "white" {}
		
		[Header(Actually used texture array)]
		[NoScaleOffset] _MainTexArray 				("Albedo Array (RGB) Alpha (A)", 2DArray) = "white" {}
		_Layers									 	("Number of Layers - 1 (int)", Float) = 1
		[KeywordEnum(Random,BySize,SoftMerge)] _MixMode("Texture Mix Mode", Float) = 0
		_SizeThreshold								("    Size Threshold", Range(0,1)) = 0.5

		_Cutoff 									("Alpha cutoff", Range(0,1)) = 0.5

		[Space(8)]
		[HideInInspector] _MinMaxScales 			("MinMaxScale Factors", Vector) = (1,1,1,1)
		_HealthyColor 								("Healthy Color (RGB) Bending (A)", Color) = (1,1,1,1)
		_DryColor 									("Dry Color (RGB) Bending (A)", Color) = (1,1,1,1)

		[Header(Lighting)]
		[Space(8)]
		[Toggle(_NORMAL)] _SampleNormal 			("Use NormalBuffer", Float) = 0
		_NormalBend									("Bend Normal", Range(0,1)) = 0.5
		[Toggle(_SPECULARHIGHLIGHTS_OFF)] _Spec 	("Enable specular highlights", Float) = 0
		[NoScaleOffset]_SpecTexArray 				("    Trans (R) Spec Mask (G) Smoothness (B)", 2DArray) = "black" {}

		[Space(8)]
		_TransStrength 								("Translucency Strength", Range(0, 1)) = 1.0
// 		Grass has not TranslucencyPower
//		_TransPower ("TransPower", Range(0, 1)) = 0.8

		[Header(Two Step Culling)]
        [Space(8)]
		_Clip 										("Clip Threshold", Range(0.0, 1.0)) = 0.3
		[Toggle(_PARALLAXMAP)] _EnableDebug 		("    Enable Debug", Float) = 0
		_DebugColor									("    Debug Color", Color) = (1,0,0,1)
		[Enum(XYZ,0,XY,1)]
        _ScaleMode                          		("Scale Mode", Float) = 0

		[Header(Wind)]
		[Space(8)]
		_WindMultiplier 							("Strength Main (X) Jitter (Y)", Vector) = (1, 0.5, 0, 0)
		[Toggle(_METALLICGLOSSMAP)] _SamplePivot 	("Sample Wind at Pivot", Float) = 0
		_WindLOD 									("Wind LOD (int)", Float) = 0
	}

	SubShader {
		Tags { 
			"Queue" = "Geometry+200"
			"IgnoreProjector" = "True"
			"RenderType" = "ATGrassArray"
		//	In order to adjust the wind settings on single instances:
			"DisableBatching"="True"
		}
		LOD 200
		Cull Off

		CGPROGRAM
// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does, but cuases shader warnings. and an error on xbox one?!
		#pragma surface surf ATGSpecular vertex:vertgrass addshadow nodynlightmap nometa     
		//nolppv              
// nolightmap
		#pragma target 3.5
		#pragma multi_compile_instancing
		// Specular Highlights
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		// Debug
		#pragma shader_feature _PARALLAXMAP
		// Wind sample mode
		#pragma shader_feature _METALLICGLOSSMAP
		// Array Mix Mode
		#pragma shader_feature _MIXMODE_RANDOM _MIXMODE_BYSIZE _MIXMODE_SOFTMERGE
		// NormalBuffer
		#pragma shader_feature _NORMAL


	//	assumeuniformscaling --> so we do not need the proper WorldToObject matrix for the normals
		#pragma instancing_options assumeuniformscaling procedural:setup
		#define ISGRASS
		#include "Includes/AtgPBSLighting.cginc"

	//	Inputs for vertex shader	
		float _Clip;
		float _WindLOD;
		#if defined(_MIXMODE_BYSIZE)
			float _SizeThreshold;
		#endif
		#if defined(_PARALLAXMAP)
			fixed4 _DebugColor;
		#endif
		half2 _MinMaxScales;
		fixed4 _HealthyColor;
		fixed4 _DryColor;
		
		float3 terrainNormal;
		float TextureLayer;
		float InstanceScale;
		//float2 randPivot;

		half _NormalBend;
		half _Layers;

		half _ScaleMode;

	//	Include all general inputs and vertex functions
		#define GRASSUSESTEXTUREARRAYS
		#include "Includes/GrassInstancedIndirect_Inputs.cginc"
		#include "Includes/GrassInstancedIndirect_Vertex.cginc"

	//	Inputs for the pixelshader
		UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
		float4 _MainTexArray_TexelSize;
		UNITY_DECLARE_TEX2DARRAY(_SpecTexArray);
		half _TransPower;
		half _TransStrength;
		fixed4 _Color;
		half _Glossiness;
		fixed _Cutoff;

		void surf (Input IN, inout SurfaceOutputATGSpecular o) {
			#if defined(_MIXMODE_SOFTMERGE)
				fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(IN.uv_MainTexArray, TextureLayer ));
			#else
				fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(IN.uv_MainTexArray, IN.layer ));
			#endif
			clip(c.a - _Cutoff);

			#if defined(_SPECULARHIGHLIGHTS_OFF)
				#if defined(_MIXMODE_SOFTMERGE)
					half3 rest = UNITY_SAMPLE_TEX2DARRAY(_SpecTexArray, float3(IN.uv_MainTexArray, TextureLayer ));
				#else
					half3 rest = UNITY_SAMPLE_TEX2DARRAY(_SpecTexArray, float3(IN.uv_MainTexArray, IN.layer ));
				#endif
			#endif
			o.Albedo = c.rgb * IN.color;
			o.Alpha = c.a;
			o.Occlusion = IN.occ;
			//o.Specular = 0;
			#if defined(_SPECULARHIGHLIGHTS_OFF)
				o.Smoothness = rest.b * IN.scale;
				o.Translucency = _TransStrength * rest.r * IN.scale;
			//	Grass does not have any specPower (fixed value) but we write out the spec mask here
				o.TranslucencyPower = rest.g;
			//	Enable grass spec lighting
				o.Specular = half3(1,0,0);
			#else
				o.Smoothness = 0;
				o.Translucency = _TransStrength * o.Albedo.g;
			//	Enable grass trans lighting
				o.Specular = half3(1, 0, 0);
			#endif
		}
		ENDCG
	}
}