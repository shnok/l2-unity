Shader "KriptoFX/KWS/WaterTesselated" {
	Properties{
	}

		SubShader{

			Tags{  "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" } //light mode required for AmbientLight 

			Blend SrcAlpha OneMinusSrcAlpha
			
			Stencil {
				Ref 230
				Comp Greater
				Pass keep
			}


			Pass
			{
				ZWrite On
				//ZTest LEqual

				Cull Back
				HLSLPROGRAM
				
				//#define ENVIRO_FOG
				//#define AZURE_FOG

				#if defined(AZURE_FOG)
					#include "Assets/Azure[Sky] Dynamic Skybox/Shaders/Transparent/AzureFogCore.cginc"
				#endif

				#if defined(ENVIRO_FOG)
					#include "Assets/Enviro - Sky and Weather/Core/Resources/Shaders/Core/EnviroFogCore.cginc"
				#endif

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

				//#include "UnityCG.cginc"
				#include "../Common/KWS_WaterVariables.cginc"
				#include "../Common/KWS_WaterPassHelpers.cginc"
				#include "../Common/KWS_CommonHelpers.cginc"
				#include "KWS_PlatformSpecificHelpers.cginc"
				#include "../Common/Shoreline/KWS_Shoreline_Common.cginc"
				#include "../Common/KWS_WaterHelpers.cginc"
				#include "../Common/KWS_WaterVertPass.cginc"
				#include "KWS_WaterFragPass.cginc"
				#include "../Common/KWS_Tessellation.cginc"

				#pragma enable_d3d11_debug_symbols
				#pragma shader_feature  KW_FLOW_MAP_EDIT_MODE
				#pragma multi_compile _ KW_FLOW_MAP KW_FLOW_MAP_FLUIDS
				#pragma multi_compile _ KW_DYNAMIC_WAVES
				#pragma multi_compile _ USE_MULTIPLE_SIMULATIONS
				#pragma multi_compile _ PLANAR_REFLECTION SSPR_REFLECTION
				#pragma multi_compile _ USE_REFRACTION_IOR
				#pragma multi_compile _ USE_REFRACTION_DISPERSION
				#pragma multi_compile _ USE_SHORELINE
				#pragma multi_compile _ REFLECT_SUN
				#pragma multi_compile _ USE_VOLUMETRIC_LIGHT
				#pragma multi_compile _ USE_FILTERING
			    #pragma multi_compile_fog
				
				#pragma target 4.6

				#pragma vertex vertHull
				#pragma fragment frag
				#pragma hull HS
				#pragma domain DS

			ENDHLSL
			}
		}
}
