Shader "Hidden/KriptoFX/KWS/FFT_ToHeightMap_URP"
{
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			ZWrite Off
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ USE_MULTIPLE_SIMULATIONS
			
			#include "../Common/KWS_WaterVariables.cginc"
			#include "../Common/KWS_WaterPassHelpers.cginc"
			#include "../Common/KWS_CommonHelpers.cginc"
			#include "../PlatformSpecific/KWS_PlatformSpecificHelpers.cginc"
			#include "../Common/KWS_WaterHelpers.cginc"

			#include "../Common/KWS_WaterVertPass.cginc"
			#include "../Common/FFT/KWS_FFT_ToHeightMap_Core.cginc"
			
			ENDHLSL
		}
	}
}
