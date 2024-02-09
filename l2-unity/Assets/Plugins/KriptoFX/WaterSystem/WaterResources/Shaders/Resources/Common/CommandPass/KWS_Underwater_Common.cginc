Texture2D _SourceRT;
float4 KWS_Underwater_RTHandleScale;

inline half4 GetUnderwaterColor(float2 uv)
{
	half mask = GetWaterMaskScatterNormalsBlured(uv).x;
	if (mask < 0.7) return 0;

	float waterDepth = GetWaterDepth(uv - KW_WaterDepth_TexelSize.y * 3);
	float z = GetSceneDepth(uv);
	float linearZ = LinearEyeDepth(z);
	float depthSurface = LinearEyeDepth(waterDepth);
	half waterSurfaceMask = saturate((depthSurface - linearZ));

#if USE_VOLUMETRIC_LIGHT
	half halfMask = 1 - saturate(mask * 2 - 1);
	half3 volumeScattering = GetVolumetricLight(uv.xy - float2(0, halfMask * 0.02)).xyz;
#else
	half4 volumeScattering = half4(GetAmbientColor(), 1.0);
#endif

	half2 normals = GetWaterMaskScatterNormals(uv.xy).zw * 2 - 1;
	half3 waterColorUnder = GetSceneColor(lerp(uv.xy, uv.xy * 1.75, 0));
	half3 waterColorBellow = GetSceneColor(uv.xy + normals);
	half3 refraction = lerp(waterColorBellow, waterColorUnder, waterSurfaceMask);

	float fade = max(0, min(depthSurface, linearZ)) * 0.25;
	half3 underwaterColor = ComputeUnderwaterColor(refraction, volumeScattering.rgb, fade, KW_Transparent, KW_WaterColor.xyz, KW_Turbidity, KW_TurbidityColor.xyz);
	return half4(underwaterColor, 1);
}

inline half4 GetUnderwaterBluredColor(float2 uv)
{
	half mask = GetWaterMaskScatterNormalsBlured(uv).x;
	if (mask < 0.72) return 0;

	half3 underwaterColor = _SourceRT.SampleLevel(sampler_linear_clamp, uv * KWS_Underwater_RTHandleScale.xy, 0).xyz;
	return half4(underwaterColor, 1);
}