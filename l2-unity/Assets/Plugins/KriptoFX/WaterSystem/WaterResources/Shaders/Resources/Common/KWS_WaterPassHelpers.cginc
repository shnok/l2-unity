#ifndef KWS_WATER_PASS_HELPERS
#define KWS_WATER_PASS_HELPERS

#ifndef KWS_WATER_VARIABLES
#include "KWS_WaterVariables.cginc"
#endif


//////////////////////////////////////////////    MaskDepthNormal_Pass    //////////////////////////////////////////////
Texture2D KW_WaterMaskScatterNormals;
Texture2D KW_WaterMaskScatterNormals_Blured;
Texture2D KW_WaterDepth;

float4 KW_WaterMaskScatterNormals_TexelSize;
float4 KW_WaterMaskScatterNormals_Blured_TexelSize;
float4 KW_WaterDepth_TexelSize;
float4 KWS_WaterMask_RTHandleScale;

inline float2 GetWaterDepthNormalizedUV(float2 uv)
{
	return clamp(uv, 0.01, 0.99) * KWS_WaterMask_RTHandleScale.xy;
}

inline float2 GetWaterMaskScatterNormalsNormalizedUV(float2 uv)
{
	return clamp(uv, 0.01, 0.99) * KWS_WaterMask_RTHandleScale.xy;
}

inline half4 GetWaterMaskScatterNormals(float2 uv)
{
	return KW_WaterMaskScatterNormals.SampleLevel(sampler_linear_clamp, GetWaterMaskScatterNormalsNormalizedUV(uv), 0);
}

inline half4 GetWaterMaskScatterNormalsBlured(float2 uv)
{
	return KW_WaterMaskScatterNormals_Blured.SampleLevel(sampler_linear_clamp, GetWaterMaskScatterNormalsNormalizedUV(uv), 0);
}

inline float GetWaterDepth(float2 uv)
{
	return KW_WaterDepth.SampleLevel(sampler_linear_clamp, GetWaterDepthNormalizedUV(uv), 0).x;
}
//////////////////////////////////////////////  END  MaskDepthNormal_Pass    //////////////////////////////////////////////


//////////////////////////////////////////////    VolumetricLighting_Pass    //////////////////////////////////////////////

texture2D KWS_VolumetricLightRT;
float4 KWS_VolumetricLightRT_TexelSize;
float4 KWS_VolumetricLight_RTHandleScale;

inline float2 GetVolumetricLightNormalizedUV(float2 uv)
{
	return clamp(uv, 0.01, 0.99) * KWS_VolumetricLight_RTHandleScale.xy;
}

inline half4 GetVolumetricLight(float2 uv)
{
	return KWS_VolumetricLightRT.SampleLevel(sampler_linear_clamp, GetVolumetricLightNormalizedUV(uv), 0);
}

//////////////////////////////////////////////  END    VolumetricLighting_Pass    //////////////////////////////////////////////


//////////////////////////////////////////////    ScreenSpaceReflection_Pass    //////////////////////////////////////////////
float KWS_ReflectionClipOffset;
texture2D KWS_ScreenSpaceReflectionRT;
float4 KWS_ScreenSpaceReflectionRT_TexelSize;
float4 KWS_ScreenSpaceReflection_RTHandleScale;

inline float2 GetScreenSpaceReflectionNormalizedUV(float2 uv)
{
	return clamp(uv, 0.005, 0.995) * KWS_ScreenSpaceReflection_RTHandleScale.xy;
}

inline half4 GetScreenSpaceReflection(float2 uv)
{
	return KWS_ScreenSpaceReflectionRT.SampleLevel(sampler_linear_clamp, GetScreenSpaceReflectionNormalizedUV(uv), 0);
}

inline half4 GetScreenSpaceReflectionWithStretchingMask(float2 refl_uv)
{
	refl_uv.y -= KWS_ReflectionClipOffset;

	float stretchingMask = 1 - abs(refl_uv.x * 2 - 1);
	stretchingMask = min(1, stretchingMask * 3);
	refl_uv.x = lerp(refl_uv.x * 0.97 + 0.015, refl_uv.x, stretchingMask);
	return GetScreenSpaceReflection(refl_uv);

	//reflection.xyz = lerp(reflection.xyz, ssr.xyz, ssr.a);

}

//////////////////////////////////////////////  END  ScreenSpaceReflection_Pass    ///////////////////////////////////////////


//////////////////////////////////////////////    Reflection_Pass    //////////////////////////////////////////////
texture2D KWS_PlanarReflectionRT;
TextureCube KWS_CubemapReflectionRT;

inline half3 GetPlanarReflection(float2 refl_uv)
{	
	return KWS_PlanarReflectionRT.Sample(sampler_linear_clamp, refl_uv).xyz;
}

inline half3 GetPlanarReflectionWithClipOffset(float2 refl_uv)
{
	refl_uv.y -= KWS_ReflectionClipOffset;
	return KWS_PlanarReflectionRT.Sample(sampler_linear_clamp, refl_uv).xyz;
}

inline half3 GetCubemapReflection(float3 reflDir)
{
	return KWS_CubemapReflectionRT.Sample(sampler_linear_clamp, reflDir).xyz;
}


#endif