half ComputeUVFade(float2 screenUV)
{
	if ((screenUV.x <= 0 || screenUV.y > 1.0)) return 0;
	float fringeY = 1 - screenUV.y;
	float fringeX = fringeY * (0.5 - abs(screenUV.x - 0.5)) * 300;
	fringeY = fringeY * 7;
	return saturate(fringeY) * saturate(fringeX);
}

///////////////////////////////////////////////////////////////////////////////// kernels ////////////////////////////////////////////////////////////////////////////////////////

[numthreads(NUMTHREAD_X, NUMTHREAD_Y, 1)]
void Clear_kernel(uint3 id : SV_DispatchThreadID)
{
	HashRT[id.xy] = MaxUint;
	ColorRT[uint2(id.xy)] = half4(0, 0, 0, 0);
}

[numthreads(NUMTHREAD_X, NUMTHREAD_Y, 1)]
void RenderHash_kernel(uint3 id : SV_DispatchThreadID)
{
	float3 posWS = ScreenToWorldPos(id.xy);

	if (posWS.y <= _HorizontalPlaneHeightWS)
		return;

	float3 reflectPosWS = posWS;

	reflectPosWS.y = -reflectPosWS.y + 2 * _HorizontalPlaneHeightWS;
	float2 reflectUV = WorldToScreenPos(reflectPosWS);

	if (reflectUV.x > 0.999 || reflectUV.x > 0.999 || reflectUV.x < 0.001 || reflectUV.y < 0.001) return;
	uint2 reflectedScreenID = reflectUV * _RTSize.xy;//from screen uv[0,1] to [0,RTSize-1]
	float2 screenUV = id.xy * _RTSize.zw;
	uint hash = id.y << 20 | id.x << 8;

#ifndef SHADER_API_METAL
	InterlockedMin(HashRT[reflectedScreenID], hash);
#endif
}
float4 Test4;
[numthreads(NUMTHREAD_X, NUMTHREAD_Y, 1)]
void RenderColorFromHash_kernel(uint3 id : SV_DispatchThreadID)
{
	ColorRT[uint2(id.xy)] = 1;

	uint hashData = HashRT[id.xy];

	uint left = HashRT[id.xy + uint2(1 + _DepthHolesFillDistance * 0.1, 0)].x;
	uint right = HashRT[id.xy - uint2(1 + _DepthHolesFillDistance * 0.1, 0)].x;
	uint up = HashRT[id.xy + uint2(0, 1)].x;
	uint down = HashRT[id.xy - uint2(0, 1 + _DepthHolesFillDistance)].x;

	hashData = min(left, min(right, min(up, down)));

	if (hashData == MaxUint)
	{
		ColorRT[id.xy] = 0;
		return;
	}

	uint2 sampleID = uint2((hashData >> 8) & 0xFFF, hashData >> 20);

	float2 sampleUV = sampleID.xy * _RTSize.zw;
	half3 sampledColor = GetCameraColor(sampleUV);

	float fade = ComputeUVFade(sampleUV);
	half4 finalColor = half4(sampledColor, fade);
	ColorRT[id.xy] = finalColor;
}

[numthreads(NUMTHREAD_X, NUMTHREAD_Y, 1)]
void RenderSinglePassSSR_kernel(uint3 id : SV_DispatchThreadID)
{
	PosWSyRT[id.xy] = 999999;

	float3 posWS = ScreenToWorldPos(id.xy);
	if (posWS.y <= _HorizontalPlaneHeightWS)
	{
		ColorRT[id.xy] = 0;
		return;
	}
	ColorRT[id.xy] = _PrevReflectionRT[id.xy];

	float3 reflectPosWS = posWS;
	reflectPosWS.y = -reflectPosWS.y + 2 * _HorizontalPlaneHeightWS;
	float2 reflectUV = WorldToScreenPos(reflectPosWS);

	if (reflectUV.x > 0.999 || reflectUV.x > 0.999 || reflectUV.x < 0.001 || reflectUV.y < 0.001) return;

	float2 screenUV = id.xy * _RTSize.zw;
	float2 reflectPixelIndex = reflectUV * _RTSize.xy;

	if (posWS.y < PosWSyRT[reflectPixelIndex])
	{
		float2 prevUV = PrevWorldToScreenPos(reflectPosWS);
		half3 prevColor = GetPrevReflection(prevUV);
		half3 currentColor = GetCameraColor(screenUV);

		half4 color;
		color.rgb = lerp(currentColor, prevColor.xyz, 0.8);
		color.a = ComputeUVFade(screenUV);

		ColorRT[reflectPixelIndex] = color;
		PosWSyRT[reflectPixelIndex] = posWS.y;
	}
}


[numthreads(NUMTHREAD_X, NUMTHREAD_Y, 1)]
void FillHoles_kernel(uint3 id : SV_DispatchThreadID)
{
	id.xy *= 2;

	half4 center = ColorRT[id.xy + uint2(0, 0)];
	half4 right = ColorRT[id.xy + uint2(0, 1)];
	half4 bottom = ColorRT[id.xy + uint2(1, 0)];
	half4 bottomRight = ColorRT[id.xy + uint2(1, 1)];

	half4 best = center;
	best = right.a > best.a + 0.5 ? right : best;
	best = bottom.a > best.a + 0.5 ? bottom : best;
	best = bottomRight.a > best.a + 0.5 ? bottomRight : best;

	ColorRT[id.xy + uint2(0, 0)] = best.a > center.a + 0.5 ? best : center;
	ColorRT[id.xy + uint2(0, 1)] = best.a > right.a + 0.5 ? best : right;
	ColorRT[id.xy + uint2(1, 0)] = best.a > bottom.a + 0.5 ? best : bottom;
	ColorRT[id.xy + uint2(1, 1)] = best.a > bottomRight.a + 0.5 ? best : bottomRight;

}