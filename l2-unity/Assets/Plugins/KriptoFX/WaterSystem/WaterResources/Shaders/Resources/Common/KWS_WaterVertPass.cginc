struct v2fDepth {
	float4  pos  : SV_POSITION;
	float3 worldPos : TEXCOORD0;
	float underwaterCubeMask : COLOR0;
};

struct v2fWater {
	float4  pos  : SV_POSITION;
	float underwaterCubeMask : COLOR0;

	float3 worldPos : TEXCOORD0;
	float4 screenPos : TEXCOORD1;
#if USE_SHORELINE
	float4 shorelineUVAnim1 : TEXCOORD2;
	float4 shorelineUVAnim2 : TEXCOORD3;
	float4 shorelineWaveData1 : TEXCOORD4;
	float4 shorelineWaveData2 : TEXCOORD5;
#endif
};

float3 ComputeWaterOffset(float3 worldPos)
{
	float2 uv = worldPos.xz / KW_FFTDomainSize;
	float3 offset = 0;
#if defined(USE_FILTERING) || defined(USE_CAUSTIC_FILTERING)
	float3 disp = Texture2DSampleLevelBicubic(KW_DispTex, sampler_linear_repeat, uv, KW_DispTex_TexelSize, 0).xyz;
#else
	float3 disp = KW_DispTex.SampleLevel(sampler_linear_repeat, uv, 0).xyz;
#endif

#if defined(KW_FLOW_MAP) || defined(KW_FLOW_MAP_EDIT_MODE)
	float2 flowMapUV = (worldPos.xz - KW_FlowMapOffset.xz) / KW_FlowMapSize + 0.5;
	float2 flowmap = KW_FlowMapTex.SampleLevel(sampler_linear_repeat, flowMapUV, 0) * 2 - 1;
	disp = ComputeDisplaceUsingFlowMap(KW_DispTex, sampler_linear_repeat, flowmap, disp, uv, _Time.y * KW_FlowMapSpeed);
#endif

#if KW_DYNAMIC_WAVES
	float2 dynamicWavesUV = (worldPos.xz - KW_DynamicWavesWorldPos.xz) / KW_DynamicWavesAreaSize + 0.5;
	float dynamicWave = KW_DynamicWaves.SampleLevel(sampler_linear_clamp, dynamicWavesUV, 0);
	disp.y -= dynamicWave * 0.15;
#endif

#if defined(KW_FLOW_MAP_FLUIDS) && !defined(KW_FLOW_MAP_EDIT_MODE)
	float2 fluidsUV_lod0 = (worldPos.xz - KW_FluidsMapWorldPosition_lod0.xz) / KW_FluidsMapAreaSize_lod0 + 0.5;
	float2 fluids_lod0 = tex2Dlod(KW_Fluids_Lod0, float4(fluidsUV_lod0, 0, 0));

	float2 fluidsUV_lod1 = (worldPos.xz - KW_FluidsMapWorldPosition_lod1.xz) / KW_FluidsMapAreaSize_lod1 + 0.5;
	float2 fluids_lod1 = tex2Dlod(KW_Fluids_Lod1, float4(fluidsUV_lod1, 0, 0));

	float2 maskUV_lod0 = 1 - saturate(abs(fluidsUV_lod0 * 2 - 1));
	float lodLevelFluidMask_lod0 = saturate((maskUV_lod0.x * maskUV_lod0.y - 0.01) * 3);
	float2 maskUV_lod1 = 1 - saturate(abs(fluidsUV_lod0 * 2 - 1));
	float lodLevelFluidMask_lod1 = saturate((maskUV_lod1.x * maskUV_lod1.y - 0.01) * 3);

	float2 fluids = lerp(fluids_lod1, fluids_lod0, lodLevelFluidMask_lod0);
	fluids *= lodLevelFluidMask_lod1;
	disp = ComputeDisplaceUsingFlowMap(KW_DispTex, sampler_linear_repeat, fluids * KW_FluidsVelocityAreaScale * 0.75, disp, uv, _Time.y * KW_FlowMapSpeed);
#endif

#ifdef USE_MULTIPLE_SIMULATIONS
	disp += KW_DispTex_LOD1.SampleLevel(sampler_linear_repeat, worldPos.xz / KW_FFTDomainSize_LOD1, 0).xyz;
	disp += KW_DispTex_LOD2.SampleLevel(sampler_linear_repeat, worldPos.xz / KW_FFTDomainSize_LOD2, 0).xyz;
#endif
	offset += disp;

	return offset;
}

v2fDepth vertDepth(float4 vertex : POSITION, float underwaterCubeMask : COLOR0)
{
	v2fDepth o;
	o.worldPos = LocalToWorldPos(vertex.xyz);
	o.underwaterCubeMask = underwaterCubeMask;
	float3 waterOffset = ComputeWaterOffset(o.worldPos);
#if USE_SHORELINE
	ComputeShorelineOffset(o.worldPos, waterOffset, vertex);
#else
	vertex.xyz += waterOffset;
#endif
	o.pos = ObjectToClipPos(vertex);
	return o;
}

v2fWater ComputeVertexInterpolators(v2fWater o, float3 worldPos, float4 vertex : POSITION)
{
	o.pos = ObjectToClipPos(vertex);
	o.screenPos = ComputeScreenPos(o.pos);

	return o;
}

v2fWater vert(float4 vertex : POSITION, float underwaterCubeMask : COLOR0)
{
	v2fWater o;
	o.worldPos = LocalToWorldPos(vertex.xyz);

	float3 waterOffset = ComputeWaterOffset(o.worldPos);

#if USE_SHORELINE
	ShorelineData shorelineData = ComputeShorelineOffset(o.worldPos, waterOffset, vertex);
	o.shorelineUVAnim1 = shorelineData.uv1;
	o.shorelineUVAnim2 = shorelineData.uv2;
	o.shorelineWaveData1 = shorelineData.data1;
	o.shorelineWaveData2 = shorelineData.data2;
#else
	vertex.xyz += waterOffset;
#endif
	o = ComputeVertexInterpolators(o, o.worldPos.xyz, vertex);

	o.underwaterCubeMask = underwaterCubeMask;
	return o;
}
