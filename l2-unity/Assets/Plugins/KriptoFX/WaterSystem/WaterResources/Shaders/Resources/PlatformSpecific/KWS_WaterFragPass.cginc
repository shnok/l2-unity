inline float GetFogFactor(float3 worldPos)
{
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	float z = mul(UNITY_MATRIX_VP, float4(worldPos, 1)).z;
	return ComputeFogFactor(z);
#else 
	return 0;
#endif
}

inline half3 ComputeStandardFog(half3 color, float fogFactor)
{
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	return MixFog(color, fogFactor);
#else 
	return color;
#endif
}

inline half3 ComputeWaterFog(half3 sourceColor, float3 worldPos, float2 uv, float3 waterZ, float fogFactor)
{
#if defined(ENVIRO_FOG)
	sourceColor = TransparentFog(half4(sourceColor, 0), worldPos.xyz, uv, waterZ);
#elif defined(AZURE_FOG)
	sourceColor = ApplyAzureFog(half4(sourceColor, 1.), worldPos).xyz;
#else
	sourceColor = ComputeStandardFog(sourceColor, fogFactor);
#endif
	return sourceColor;
}


inline half4 ComputeWaterColor(v2fWater i, float facing)
{
	float2 uv = i.worldPos.xz / KW_FFTDomainSize;
	float2 screenUV = i.screenPos.xy / i.screenPos.w;

	float3 viewDir = GetWorldSpaceViewDirNorm(i.worldPos);
	float viewDist = GetWorldToCameraDistance(i.worldPos);
	float fogFactor = GetFogFactor(i.worldPos);

	half3 normal;

	/////////////////////////////////////////////////////////////  NORMAL  ////////////////////////////////////////////////////////////////////////////////////////////////////////

#if USE_FILTERING
	float normalFilteringMask;
	normal = GetFilteredNormal_lod0(uv, viewDist, normalFilteringMask);
#else
	normal = GetNormal_lod0(uv);
#endif

#ifdef USE_MULTIPLE_SIMULATIONS
	normal = GetNormal_lod1_lod2(i.worldPos, normal);
#endif

	normal = normalize(normal);

#if defined(KW_FLOW_MAP) || defined(KW_FLOW_MAP_EDIT_MODE)
	normal = GetFlowmapNormal(i.worldPos, uv, normal);
#endif
#if defined(KW_FLOW_MAP_FLUIDS) && !defined(KW_FLOW_MAP_EDIT_MODE)
	half fluidsFoam;
	normal = GetFluidsNormal(i.worldPos, uv, normal, fluidsFoam);
#endif


#ifdef KW_FLOW_MAP_EDIT_MODE
	return GetFlowmapEditor(i.worldPos, normal);
#endif

#if USE_SHORELINE
	normal = ComputeShorelineNormal(normal, i.worldPos, i.shorelineUVAnim1, i.shorelineUVAnim2, i.shorelineWaveData1, i.shorelineWaveData2);
#endif


#if KW_DYNAMIC_WAVES
	normal = GetDynamicWaves(i.worldPos, normal);
#endif

#if USE_FILTERING
	normal.xz *= normalFilteringMask;
#endif
	/////////////////////////////////////////////////////////////  end normal  ////////////////////////////////////////////////////////////////////////////////////////////////////////


	float sceneZ = GetSceneDepth(screenUV);
	half surfaceTensionFade = GetSurfaceTension(sceneZ, i.screenPos.w);


	/////////////////////////////////////////////////////////////////////  REFRACTION  ///////////////////////////////////////////////////////////////////
	float2 refractionUV;

#if defined(USE_REFRACTION_IOR)
	refractionUV = GetRefractedUV_IOR(viewDir, normal, i.worldPos, surfaceTensionFade);
#else
	refractionUV = GetRefractedUV_Simple(screenUV, normal);
#endif

#if defined(USE_REFRACTION_DISPERSION)
	half3 refraction = GetSceneColorWithDispersion(refractionUV, KWS_RefractionDispersionStrength);
#else 
	half3 refraction = GetSceneColor(refractionUV);
#endif
	/////////////////////////////////////////////////////////////  end refraction  ////////////////////////////////////////////////////////////////////////////////////////////////////////




	/////////////////////////////////////////////////////////////////////  UNDERWATER  ///////////////////////////////////////////////////////////////////
#if USE_VOLUMETRIC_LIGHT
	half4 volumeScattering = GetVolumetricLight(refractionUV);
#else
	half4 volumeScattering = half4(GetAmbientColor(), 1.0);
#endif

	float depthAngleFix;
	float fade = GetWaterRawFade(viewDir, refractionUV, i.screenPos.w, depthAngleFix);
	FixAboveWaterRendering(depthAngleFix, screenUV, i.screenPos.w, sceneZ, fade, refraction, volumeScattering);

	half3 underwaterColor = ComputeUnderwaterColor(refraction, volumeScattering.rgb, fade, KW_Transparent, KW_WaterColor.xyz, KW_Turbidity, KW_TurbidityColor.xyz);

#if defined(KW_FLOW_MAP_FLUIDS) && !defined(KW_FLOW_MAP_EDIT_MODE)
	underwaterColor = GetFluidsColor(underwaterColor, volumeScattering, fluidsFoam);
#endif
	underwaterColor += ComputeSSS(screenUV, underwaterColor, 5, KW_Transparent);
	/////////////////////////////////////////////////////////////  end underwater  ////////////////////////////////////////////////////////////////////////////////////////////////////////




	/////////////////////////////////////////////////////////////  REFLECTION  ////////////////////////////////////////////////////////////////////////////////////////////////////////
	half3 planarReflection = 0;
	half3 skyReflection = 0;
	half4 ssrReflection = 0;
	half3 sunReflection = 0;

	float3 reflDir = reflect(-viewDir, normal);

#if defined(PLANAR_REFLECTION) || defined(SSPR_REFLECTION)
	float2 refl_uv = GetScreenSpaceReflectionUV(normal, viewDir, KWS_CameraProjectionMatrix);
#endif

#if PLANAR_REFLECTION
	planarReflection = GetPlanarReflectionWithClipOffset(refl_uv);
	
#else
	skyReflection = GetCubemapReflection(reflDir);
#if USE_FILTERING
	skyReflection = GetCubemapReflectionFiltered(reflDir, skyReflection);
#endif

#if SSPR_REFLECTION
	ssrReflection = GetScreenSpaceReflectionWithStretchingMask(refl_uv);
	//return float4(ssrReflection.aaa, 1);
#endif

#endif

#if REFLECT_SUN
	sunReflection = ComputeSunlight(normal, viewDir, GetMainLightDir(), GetMainLightColor(), 1, viewDist, KW_WaterFarDistance, KW_Transparent);
#endif

	half3 finalReflection = 0;
#if PLANAR_REFLECTION
	finalReflection = planarReflection;
#else
	finalReflection = skyReflection;
#endif
	finalReflection = ComputeWaterFog(finalReflection + sunReflection, i.worldPos.xyz, uv, i.pos.z, fogFactor);
	finalReflection = lerp(finalReflection.xyz, ssrReflection.xyz, ssrReflection.a);
	/////////////////////////////////////////////////////////////  end reflection  ////////////////////////////////////////////////////////////////////////////////////////////////////////


	underwaterColor = ComputeWaterFog(underwaterColor + sunReflection, i.worldPos.xyz, uv, i.pos.z, fogFactor);
#if USE_SHORELINE
	finalReflection = ApplyShorelineWavesReflectionFix(reflDir, finalReflection, underwaterColor);
#endif

	half waterFresnel = ComputeWaterFresnel(normal, viewDir);
	half3 finalColor = lerp(underwaterColor, finalReflection * (1 - i.underwaterCubeMask), waterFresnel);

	return float4(finalColor, surfaceTensionFade);
}

half4 fragDepth(v2fDepth i, float facing : VFACE) : SV_Target
{
		//FragmentOutput o;

		float2 uv = i.worldPos.xz / KW_FFTDomainSize;
		
		half3 norm = KW_NormTex.Sample(sampler_linear_repeat, uv).xyz;
		half3 normScater = KW_NormTex.SampleLevel(sampler_linear_repeat, uv, KW_NormalScattering_Lod).xyz;
		
		#ifdef USE_MULTIPLE_SIMULATIONS
			half3 normScater_lod1 = KW_NormTex_LOD1.SampleLevel(sampler_linear_repeat, i.worldPos.xz / KW_FFTDomainSize_LOD1, 2).xyz;
			half3 normScater_lod2 = KW_NormTex_LOD2.SampleLevel(sampler_linear_repeat, i.worldPos.xz / KW_FFTDomainSize_LOD2, 1).xyz;
			normScater = normalize(half3(normScater.xz + normScater_lod1.xz + normScater_lod2.xz, normScater.y * normScater_lod1.y * normScater_lod2.y)).xzy;

			half3 norm_lod1 = KW_NormTex_LOD1.Sample(sampler_linear_repeat, i.worldPos.xz / KW_FFTDomainSize_LOD1).xyz;
			half3 norm_lod2 = KW_NormTex_LOD2.Sample(sampler_linear_repeat,  i.worldPos.xz / KW_FFTDomainSize_LOD2).xyz;
			norm = normalize(half3(norm.xz + norm_lod1.xz + norm_lod2.xz, norm.y * norm_lod1.y * norm_lod2.y)).xzy;
		#endif


		float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
		float distance = length(viewDir);
		viewDir = normalize(viewDir);
		int idx;
		half sss = 0;
		half windLimit = clamp(KW_WindSpeed - 0.25, -0.25, 1);
		windLimit -= clamp((KW_WindSpeed - 4) * 0.25, 0, 0.8);
			
		float3 lightDir = KW_DirLightForward.xyz;
			
		half zeroScattering = saturate(dot(viewDir, -(lightDir + float3(0, 1, 0))));

		float3 H = (lightDir + norm * float3(-1, 1, -1));
		float scattering = (dot(viewDir, -H));
		sss += windLimit * (scattering - zeroScattering  * 0.95);

		
		norm.xz *= 1-i.underwaterCubeMask;
		return  half4(0.75 - facing * 0.25 , saturate(sss - 0.1), norm.xz * 0.5 + 0.5);
}

half4 frag(v2fWater i, float facing : VFACE) : SV_Target
{
	return ComputeWaterColor(i, facing);
}


