struct ShorelineData
{
	float4 uv1;
	float4 uv2;
	float4 data1;
	float4 data2;
};

float2 GetAnimatedUV(float2 uv, int _ColumnsX, int _RowsY, float FPS, float time)
{
	float2 size = float2(1.0f / _ColumnsX, 1.0f / _RowsY);
	uint totalFrames = _ColumnsX * _RowsY;
	uint index = time * 1.0f * FPS;
	uint indexX = index % _ColumnsX;
	uint indexY = floor((index % totalFrames) / _ColumnsX);

	float2 offset = float2(size.x * indexX, -size.y * indexY);
	float2 newUV = uv * size;
	newUV.y = newUV.y + size.y * (_RowsY - 1);

	return newUV + offset;
}

float ComputeWaterOrthoDepth(float3 worldPos)
{
	float2 depthUV = (worldPos.xz - KW_ShorelineAreaPos.xz) / KW_ShorelineDepthOrthoSize + 0.5;
	if (depthUV.x < 0.001 || depthUV.x > 0.999 || depthUV.y < 0.001 || depthUV.y > 0.999) return 0;
	float terrainDepth = KW_ShorelineDepthTex.SampleLevel(sampler_linear_clamp, depthUV, 0).r * KW_ShorelineDepth_Near_Far_Dist.z - KW_ShorelineDepth_Near_Far_Dist.y + KW_ShorelineAreaPos.y;
	return terrainDepth;
}

float3 ComputeBeachWaveOffsetForOneLine(float2 wavesMapUV, float terrainDepth, float time, sampler2D tex_UV_angle_alpha, sampler2D tex_timeOffset_scale,
	inout float4 shorelineUVAnim, inout float4 shorelineWaveData)
{

	float fps = 20;
	float4 uv_angle_alpha = tex2Dlod(tex_UV_angle_alpha, float4(wavesMapUV, 0, 0));
	float2 timeOffset_Scale = tex2Dlod(tex_timeOffset_scale, float4(wavesMapUV, 0, 0)).xy;

	shorelineUVAnim.xy = uv_angle_alpha.xy;

	float2 waveUV = uv_angle_alpha.xy;
	float waveAngle = uv_angle_alpha.z;
	float waveAlpha = uv_angle_alpha.w;
	float timeOffset = timeOffset_Scale.x;

	float waveScale = timeOffset_Scale.y;

	UNITY_BRANCH
	if (waveAlpha.x > 0.1)
	{
		time += timeOffset * KW_GlobalTimeOffsetMultiplier;

		float2 uv = GetAnimatedUV(waveUV, 14, 15, fps, time);
		float2 prevUV = GetAnimatedUV(waveUV, 14, 15, fps, time + 1.0 / fps);

		float3 pos = tex2Dlod(KW_ShorelineWaveDisplacement, float4(uv, 0, 0)).xyz;
		float3 pos2 = tex2Dlod(KW_ShorelineWaveDisplacement, float4(prevUV, 0, 0)).xyz;
		pos = lerp(pos, pos2, frac(time * fps));
		pos.y -= 2;
		pos.xy *= pos.z;

		float angle = (360 * waveAngle) * 3.1415 / 180.0;
		float sina, cosa;
		sincos(angle, sina, cosa);
		float3 offsetWave = float3(cosa * pos.x, pos.y, -sina * pos.x);

		offsetWave = offsetWave * waveAlpha * waveScale * 0.3;
		offsetWave.y = max(offsetWave.y, pos.z * (terrainDepth - KW_WaterPosition.y + 0.125));

		shorelineUVAnim.xy = uv;
		shorelineUVAnim.zw = prevUV;

		shorelineWaveData.xy = float2(-sina, cosa);
		shorelineWaveData.z = frac(time * fps);
		shorelineWaveData.w = waveAlpha;

		return offsetWave;
	}
	else return 0;
}

float3 ComputeBeachWaveOffset(float3 worldPos, inout ShorelineData shorelineData, float timeOffset = 0)
{
	shorelineData.uv1 = 0;
	shorelineData.uv2 = 0;
	shorelineData.data1 = 0;
	shorelineData.data2 = 0;
	float3 offset = 0;

	float2 wavesMapUV = (worldPos.xz - KW_ShorelineAreaPos.xz) / KW_WavesMapSize + 0.5;
	if (wavesMapUV.x < 0.001 || wavesMapUV.x > 0.999 || wavesMapUV.y < 0.001 || wavesMapUV.y > 0.999) return 0;
	//float2 depthUV = (worldPos.xz - KW_DepthPos.xz) / KW_DepthOrthographicSize + 0.5;
	//float terrainDepth = tex2Dlod(KW_OrthoDepth, float4(depthUV, 0, 0)).r * KW_DepthNearFarDistance.z - KW_DepthNearFarDistance.y + KW_DepthPos.y;
	float terrainDepth = ComputeWaterOrthoDepth(worldPos);
	//_Time.y = TEST;

	const float timeLimit = (14.0 * 15.0) / 20.0; //(frameX * frameY) / fps
	//KW_Time = Test4.x;
	float time = frac((KW_GlobalTimeSpeedMultiplier * _Time.y) / timeLimit) * timeLimit;
	time += timeOffset;

	float3 offsetWave1 = ComputeBeachWaveOffsetForOneLine(wavesMapUV, terrainDepth, time, KW_BakedWaves1_UV_Angle_Alpha, KW_BakedWaves1_TimeOffset_Scale, shorelineData.uv1, shorelineData.data1);
	float3 offsetWave2 = ComputeBeachWaveOffsetForOneLine(wavesMapUV, terrainDepth, time, KW_BakedWaves2_UV_Angle_Alpha, KW_BakedWaves2_TimeOffset_Scale, shorelineData.uv2, shorelineData.data2);
	offset.xyz += offsetWave1;
	offset.xyz += offsetWave2;
	return offset;
}

inline ShorelineData ComputeShorelineOffset(float3 worldPos, float3 waterOffset, inout float4 vertex)
{
	ShorelineData shorelineData;
	float3 beachOffset = ComputeBeachWaveOffset(worldPos, shorelineData);
	float terrainDepth = ComputeWaterOrthoDepth(worldPos);
	vertex.xyz += lerp(waterOffset, 0, saturate(terrainDepth - KW_WaterPosition.y + 0.85));
	vertex.xyz += beachOffset;
	return shorelineData;
}

inline float3 ComputeBeachWaveNormal(float4 shorelineUVAnim, float4 shorelineWaveData)
{
	float4 waveNorm = tex2D(KW_ShorelineWaveNormal, shorelineUVAnim.xy).xyzw;
	float4 waveNorm2 = tex2D(KW_ShorelineWaveNormal, shorelineUVAnim.zw).xyzw;
	waveNorm = lerp(waveNorm, waveNorm2, shorelineWaveData.z);
	waveNorm.xyz = waveNorm.xyz * 2 - 1;
	waveNorm.xz *= -1;

	float2x2 m = float2x2(shorelineWaveData.y, -shorelineWaveData.x, shorelineWaveData.x, shorelineWaveData.y);
	waveNorm.xz = mul(m, waveNorm.xz);
	float wavesAlpha = shorelineWaveData.w > 0.999 ? 1 : 0;
	waveNorm.a *= wavesAlpha;
	return lerp(float3(0, 1, 0), waveNorm.xyz, waveNorm.a);
}

inline float3 ComputeShorelineNormal(half3 normal, float3 worldPos, float4 uv1, float4 uv2, float4 data1, float4 data2)
{
	float3 shorelineWave1 = ComputeBeachWaveNormal(uv1, data1);
	float3 shorelineWave2 = ComputeBeachWaveNormal(uv2, data2);
	float terrainDepth = ComputeWaterOrthoDepth(worldPos);
	float shorelineNearDepthMask = saturate(terrainDepth - KW_WaterPosition.y + 0.85);

	normal = lerp(normal, float3(0, 1, 0), shorelineNearDepthMask);
	normal = BlendNormals(normal, shorelineWave1, shorelineWave2);
	return normal;
}
