#ifndef BEAUTIFY_CABERRATION_INCLUDE
#define BEAUTIFY_CABERRATION_INCLUDE

	// Copyright 2020-2021 Kronnect - All Rights Reserved.
	#include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	SAMPLER(sampler_MainTex);
	float4 _MainTex_TexelSize;

	#include "BeautifyDistortion.hlsl"
	
	float4 fragChromaticAberration (VaryingsSimple i) : SV_Target {
	    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv         = UnityStereoTransformScreenSpaceTex(i.uv);
        float4 pixel = GetDistortedColor(i.uv);
  		return pixel;
	}


#endif // BEAUTIFY_CABERRATION_INCLUDE