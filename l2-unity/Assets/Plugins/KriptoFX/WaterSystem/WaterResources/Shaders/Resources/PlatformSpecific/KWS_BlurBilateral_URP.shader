Shader "Hidden/KriptoFX/KWS/BlurBilateral_URP"
{
	SubShader
	{
		Cull Off 
		ZWrite Off 
		ZTest Always

		HLSLINCLUDE

		#include "../Common/KWS_WaterVariables.cginc"
		#include "KWS_PlatformSpecificHelpers.cginc"
		#include "../Common/CommandPass/KWS_BlurBilateral_Common.cginc"

		ENDHLSL

		Pass // pass 0 - downsample depth 
		{
			HLSLPROGRAM

			#pragma target 4.6
			#pragma vertex vert
			#pragma fragment DownsampleDepth

			ENDHLSL
		}

		Pass // pass 1 - downsample depth fast mode
		{
			HLSLPROGRAM

			#pragma target 4.6
			#pragma vertex vert
			#pragma fragment DownsampleDepthFastMode

			ENDHLSL
		}

		// pass 2 - horizontal blur
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment horizontalFrag
			#pragma target 4.0

			half4 horizontalFrag(v2f i) : SV_Target
			{
				return BilateralBlur(i.uv, int2(1, 0), BLUR_KERNEL_SIZE);
			}

			ENDHLSL
		}

		Pass // pass 3 - horizontal blur  Fast mode
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment horizontalFrag
			#pragma target 4.0

			half4 horizontalFrag(v2f i) : SV_Target
			{
				return BilateralBlur(i.uv, int2(1, 0), BLUR_KERNEL_SIZE_FAST_MODE);
			}

			ENDHLSL
		}

		Pass	// pass 4 - vertical blur
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment verticalFrag
			#pragma target 4.0

			half4 verticalFrag(v2f i) : SV_Target
			{
				return BilateralBlur(i.uv, int2(0, 1), BLUR_KERNEL_SIZE);
			}

			ENDHLSL
		}

		Pass // pass 5 - vertical blur  Fast mode
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment verticalFrag
			#pragma target 4.0

			half4 verticalFrag(v2f i) : SV_Target
			{
				return BilateralBlur(i.uv, int2(0, 1), BLUR_KERNEL_SIZE_FAST_MODE);
			}

			ENDHLSL
		}


		Pass // pass 6 - bilateral upsample
		{
			Blend One Zero

			HLSLPROGRAM
			#pragma vertex vertUpsample
			#pragma fragment BilateralUpsample		
			#pragma target 4.6

			ENDHLSL
		}
	}
}

