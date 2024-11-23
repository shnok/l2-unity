Shader "AdvancedTerrainGrass/Grass Base Shader" {
	Properties {

		[Space(8)]
		[NoScaleOffset] _MainTex 					("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff 									("Alpha Cutoff", Range(0,1)) = 0.5

		[Space(8)]
		[HideInInspector] _MinMaxScales 			("MinMaxScale Factors", Vector) = (1,1,1,1)
		_HealthyColor 								("Healthy Color (RGB) Bending (A)", Color) = (1,1,1,1)
		_DryColor 									("Dry Color (RGB) Bending (A)", Color) = (1,1,1,1)

		[Header(Lighting)]
		[Space(8)]
		[Toggle(_NORMAL)] _SampleNormal 			("Use NormalBuffer", Float) = 0
		_NormalBend									("Bend Normal", Range(0,1)) = 0.5
		[Toggle(_SPECULARHIGHLIGHTS_OFF)] _Spec 	("Enable specular Highlights ", Float) = 0
		[NoScaleOffset]_SpecTex 					("    Trans (R) Spec Mask (G) Smoothness (B)", 2D) = "black" {}

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
		[Space(4)]
		_WindMultiplier 							("Strength Main (X) Jitter (Y)", Vector) = (1, 0.5, 0, 0)
		[Toggle(_METALLICGLOSSMAP)] _SamplePivot 	("Sample Wind at Pivot", Float) = 0
		_WindLOD 									("Wind LOD (int)", Float) = 0
	}


	SubShader {
		Tags { 
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="ATGrass"
		//	In order to adjust the wind settings on single instances:
			"DisableBatching"="True"
		}
		LOD 200
		Cull Off
		
		CGPROGRAM
// noshadowmask does not fix the problem with baked shadows in deferred
// removing nolightmap does
		#pragma surface surf ATGSpecular vertex:vertgrass addshadow nodynlightmap nometa
		// nolppv 
// nolightmap
		#pragma target 3.5
		#pragma multi_compile_instancing
		// Specular Highlights
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		// Debug
		#pragma shader_feature _PARALLAXMAP
		// Wind sample mode
		#pragma shader_feature _METALLICGLOSSMAP
		// NormalBuffer
		#pragma shader_feature _NORMAL

	//	assumeuniformscaling --> so we do not need the proper WorldToObject matrix for the normals
		#pragma instancing_options assumeuniformscaling procedural:setup
		#define ISGRASS
		#include "Includes/AtgPBSLighting.cginc"


	//	Inputs for vertex shader	
		float _Clip;
		float _WindLOD;
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

		half _ScaleMode;

	//	Include all general inputs and vertex functions
		#include "Includes/GrassInstancedIndirect_Inputs.cginc"
		#include "Includes/GrassInstancedIndirect_Vertex.cginc"


	//	Inputs for the pixelshader	
		sampler2D _MainTex;
		sampler2D _SpecTex;
		half _TransPower;
		half _TransStrength;
		fixed4 _Color;
		half _Glossiness;
		fixed _Cutoff;

		void surf (Input IN, inout SurfaceOutputATGSpecular o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			clip(c.a - _Cutoff);

			#if defined(_SPECULARHIGHLIGHTS_OFF)
				half3 rest = tex2D (_SpecTex, IN.uv_MainTex);
			#endif
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a;
//			o.Occlusion = IN.occ;
			//o.Specular = 0;
			#if defined(_SPECULARHIGHLIGHTS_OFF)
				o.Smoothness = rest.b * IN.scale; //lerp(rest.b, _Glossiness, rest.r);
				o.Translucency = _TransStrength * rest.r /*saturate(c.g *3)*/  * IN.scale;
			//	Grass does not have any specPower (fixed value) but we write out the spec mask here
				o.TranslucencyPower = rest.g;
			//	Enable grass spec lighting
				o.Specular = half3(1, 0, 0);
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