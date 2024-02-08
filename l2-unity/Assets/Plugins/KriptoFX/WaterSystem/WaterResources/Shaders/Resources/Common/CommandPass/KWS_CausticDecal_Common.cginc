sampler2D KW_CausticLod0;
sampler2D KW_CausticLod1;
sampler2D KW_CausticLod2;
sampler2D KW_CausticLod3;

texture2D KW_CausticDepthTex;

float4 KW_CausticLod0_TexelSize;
float4 KW_CausticLod1_TexelSize;

float KW_CausticDispersionStrength;
float KW_CaustisStrength;
float KW_DecalScale;

float KW_CausticDepthOrthoSize;

float4 KW_CausticLodSettings;
float3 KW_CausticLodOffset;

float3 KW_CausticDepth_Near_Far_Dist;
float3 KW_CausticDepthPos;


half3 GetCausticLod(float2 decalUV, float lodDist, sampler2D tex, half3 lastLodCausticColor)
{
	float2 uv = lodDist * decalUV + 0.5 - KW_CausticLodOffset.xz;
	float caustic = tex2D(tex, uv).x;
	uv = 1 - min(1, abs(uv * 2 - 1));
	float lerpLod = uv.x * uv.y;
	lerpLod = min(1, lerpLod * 3);
	return lerp(lastLodCausticColor, caustic, lerpLod);
}

half3 GetCausticLodWithDynamicWaves(float2 decalUV, float lodDist, sampler2D tex, half3 lastLodCausticColor, float2 offsetUV1, float2 offsetUV2, float flowLerpMask)
{
	float2 uv = lodDist * decalUV + 0.5 - KW_CausticLodOffset.xz;
	float caustic1 = tex2D(tex, uv - offsetUV1).x;
	float caustic2 = tex2D(tex, uv - offsetUV2).x;
	float caustic = lerp(caustic1, caustic2, flowLerpMask);
	uv = 1 - min(1, abs(uv * 2 - 1));
	float lerpLod = uv.x * uv.y;
	lerpLod = min(1, lerpLod * 3);
	return lerp(lastLodCausticColor, caustic, lerpLod);
}

half3 GetCausticLodWithDispersion(float2 decalUV, float lodDist, sampler2D tex, half3 lastLodCausticColor, float texelSize, float dispersionStr)
{
	float2 uv = lodDist * decalUV + 0.5 - KW_CausticLodOffset.xz;
	float3 caustic;
	caustic.r = tex2D(tex, uv).x;
	caustic.g = tex2D(tex, uv + texelSize * dispersionStr * 2).x;
	caustic.b = tex2D(tex, uv + texelSize * dispersionStr * 4).x;

	uv = 1 - min(1, abs(uv * 2 - 1));
	float lerpLod = uv.x * uv.y;
	lerpLod = min(1, lerpLod * 3);
	return lerp(lastLodCausticColor, caustic, lerpLod);
}

float ComputeCausticOrthoDepth(float3 worldPos)
{
	float2 depthUV = (worldPos.xz - KW_CausticDepthPos.xz - KW_WaterPosition.xz * 0) / KW_CausticDepthOrthoSize + 0.5;
	float terrainDepth = KW_CausticDepthTex.SampleLevel(sampler_linear_clamp, depthUV, 0).r * KW_CausticDepth_Near_Far_Dist.z - KW_CausticDepth_Near_Far_Dist.y;
	return terrainDepth;
}

half GetTerrainDepth(float3 worldPos)
{
#if USE_DEPTH_SCALE
	half terrainDepth = ComputeCausticOrthoDepth(worldPos);
#else 
	half terrainDepth = 1;

#endif

	half depthTransparent = max(1, KW_Transparent * 2);
	terrainDepth = clamp(-terrainDepth, 0, depthTransparent) / (depthTransparent);
	return terrainDepth;
}

half3 GetCaustic(float3 worldPos, float3 localPos)
{
	half3 caustic = 0.1;

#if KW_DYNAMIC_WAVES
	float2 dynamicWavesUV = (worldPos.xz - KW_DynamicWavesWorldPos.xz) / KW_DynamicWavesAreaSize + 0.5;
	half2 dynamicWavesNormals = KW_DynamicWavesNormal.SampleLevel(sampler_linear_clamp, dynamicWavesUV, 0) * 2 - 1;


	half time1 = frac(_Time.x + 0.5);
	half time2 = frac(_Time.x);
	half flowLerpMask = abs((0.5 - time1) / 0.5);

	float2 uvOffset1 = 0.25 * dynamicWavesNormals * time1;
	float2 uvOffset2 = 0.25 * dynamicWavesNormals * time2;

#if defined(USE_LOD3)
	caustic = GetCausticLodWithDynamicWaves(localPos.xz, KW_DecalScale / KW_CausticLodSettings.w, KW_CausticLod3, caustic, uvOffset1, uvOffset2, flowLerpMask);
#endif
#if defined(USE_LOD2) || defined(USE_LOD3)
	caustic = GetCausticLodWithDynamicWaves(localPos.xz, KW_DecalScale / KW_CausticLodSettings.z, KW_CausticLod2, caustic, uvOffset1, uvOffset2, flowLerpMask);
#endif


#if defined(USE_LOD1) || defined(USE_LOD2) || defined(USE_LOD3)
	caustic = GetCausticLodWithDynamicWaves(localPos.xz, KW_DecalScale / KW_CausticLodSettings.y, KW_CausticLod1, caustic, uvOffset1, uvOffset2, flowLerpMask);
#endif

	caustic = GetCausticLodWithDynamicWaves(localPos.xz, KW_DecalScale / KW_CausticLodSettings.x, KW_CausticLod0, caustic, uvOffset1, uvOffset2, flowLerpMask);

	half dynamicWaves = KW_DynamicWaves.SampleLevel(sampler_linear_clamp, dynamicWavesUV, 0) * 2 - 1;
	half3 dynWavesNormalized = normalize(half3(dynamicWavesNormals.x, 1, dynamicWavesNormals.y));
	half dynWavesCaustic = dot(dynWavesNormalized, half3(0.5, 1, 0.5));
	caustic *= 1 + clamp(dynamicWaves, -0.03, 1) * 20;
#else 


#if defined(USE_LOD3)
	caustic = GetCausticLod(localPos.xz, KW_DecalScale / KW_CausticLodSettings.w, KW_CausticLod3, caustic);
#endif
#if defined(USE_LOD2) || defined(USE_LOD3)
	caustic = GetCausticLod(localPos.xz, KW_DecalScale / KW_CausticLodSettings.z, KW_CausticLod2, caustic);
#endif

#if USE_DISPERSION
#if defined(USE_LOD1) || defined(USE_LOD2) || defined(USE_LOD3)
	caustic = GetCausticLodWithDispersion(localPos.xz, KW_DecalScale / KW_CausticLodSettings.y, KW_CausticLod1, caustic, KW_CausticLod0_TexelSize.x, KW_CausticDispersionStrength);
#endif
	caustic = GetCausticLodWithDispersion(localPos.xz, KW_DecalScale / KW_CausticLodSettings.x, KW_CausticLod0, caustic, KW_CausticLod0_TexelSize.x, KW_CausticDispersionStrength);

#else
#if defined(USE_LOD1) || defined(USE_LOD2) || defined(USE_LOD3)
	caustic = GetCausticLod(localPos.xz, KW_DecalScale / KW_CausticLodSettings.y, KW_CausticLod1, caustic);
#endif
	caustic = GetCausticLod(localPos.xz, KW_DecalScale / KW_CausticLodSettings.x, KW_CausticLod0, caustic);
#endif
#endif
	return caustic;

}

half3 ApplyCausticFade(half3 caustic, half3 worldPos, float terrainDepth)
{
	caustic = lerp(1, caustic * 10, saturate(KW_CaustisStrength));
	caustic += caustic * caustic * caustic * saturate(KW_CaustisStrength - 1);
	float dist = length(worldPos - _WorldSpaceCameraPos);
	float distFade = 1 - saturate(dist / KW_DecalScale * 2);
	caustic = lerp(1, caustic, distFade);

	float fade = saturate((KW_WaterPosition.y - worldPos.y) * 2);
	caustic = lerp(1, caustic, fade);
	return lerp(caustic, 1, terrainDepth);
}

half4 GetCaustic(float2 uv, out float depth, out float3 worldPos)
{
	depth = GetSceneDepth(uv);
	float waterDepth = GetWaterDepth(uv);
	bool isUnderwater = GetWaterMaskScatterNormalsBlured(uv).x > 0.7;
	bool causticMask = isUnderwater ? waterDepth < depth : waterDepth > depth;

	if (causticMask < 0.0001) discard;

	worldPos = GetWorldSpacePositionFromDepth(uv, depth);
	float3 localPos = WorldToLocalPos(worldPos);

	half terrainDepth = GetTerrainDepth(worldPos);
	half3 caustic = GetCaustic(worldPos, localPos);
	caustic = ApplyCausticFade(caustic, worldPos, terrainDepth);

	return float4(caustic, 1);
}
