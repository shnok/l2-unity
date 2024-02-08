#define BLUR_KERNEL_SIZE 6
#define BLUR_KERNEL_SIZE_FAST_MODE 3
#define UPSAMPLE_DEPTH_THRESHOLD 1.5f
#define GAUSSIAN_BLUR_DEVIATION 2.5     
#define BLUR_DEPTH_FACTOR 0.5

Texture2D _SourceRT;
float4 _SourceRTHandleScale;

Texture2D KWS_CameraDepthTextureLowRes;
float4 KWS_CameraDepthTextureLowRes_TexelSize;

struct appdata
{
	uint vertexID : SV_VertexID;
	float2 uv : TEXCOORD0;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

struct v2fUpsample
{
	float2 uv : TEXCOORD0;
	float2 uv00 : TEXCOORD1;
	float2 uv01 : TEXCOORD2;
	float2 uv10 : TEXCOORD3;
	float2 uv11 : TEXCOORD4;
	float4 vertex : SV_POSITION;
};

v2f vert(appdata v)
{
	v2f o;
	o.vertex = GetTriangleVertexPosition(v.vertexID);
	o.uv = GetTriangleUV(v.vertexID);
	return o;
}
v2fUpsample vertUpsample(appdata v)
{
	v2fUpsample o;
	o.vertex = GetTriangleVertexPosition(v.vertexID);
	o.uv = GetTriangleUV(v.vertexID);

	o.uv00 = o.uv - 0.5 * KWS_CameraDepthTextureLowRes_TexelSize.xy;
	o.uv10 = o.uv00 + float2(KWS_CameraDepthTextureLowRes_TexelSize.x, 0);
	o.uv01 = o.uv00 + float2(0, KWS_CameraDepthTextureLowRes_TexelSize.y);
	o.uv11 = o.uv00 + KWS_CameraDepthTextureLowRes_TexelSize.xy;
	return o;
}

float4 DownsampleDepth(v2f input) : SV_Target
{
	float4 depth = GetSceneDepthGather(input.uv);
	float minDepth = min(min(depth.x, depth.y), min(depth.z, depth.w));
	float maxDepth = max(max(depth.x, depth.y), max(depth.z, depth.w));
	int2 position = input.vertex.xy % 2;
	int index = position.x + position.y;
	return index == 1 ? minDepth : maxDepth;
}

float4 DownsampleDepthFastMode(v2f input) : SV_Target
{
	float4 depth = GetSceneDepthGather(input.uv);
	return min(min(depth.x, depth.y), min(depth.z, depth.w));
}

float GaussianWeight(float offset, float deviation)
{
	float weight = 1.0f / sqrt(2.0f * 3.1415927f * deviation * deviation);
	weight *= exp(-(offset * offset) / (2.0f * deviation * deviation));
	return weight;
}

float4 BilateralBlur(float2 uv, const int2 direction, const int kernelRadius)
{
	const float deviation = kernelRadius / GAUSSIAN_BLUR_DEVIATION;

	float4 centerColor = _SourceRT.Sample(sampler_linear_clamp, uv * _SourceRTHandleScale.xy);
	float3 color = centerColor.xyz;

	UNITY_BRANCH  if (centerColor.r + centerColor.g + centerColor.b < MIN_THRESHOLD) return 0;
	else 
	{


		float rawZ = KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, uv).r;

		float centerDepth = LinearEyeDepth(rawZ);

		float weightSum = 0;

		float weight = GaussianWeight(0, deviation);
		color *= weight;
		weightSum += weight;

		[unroll] for (int idx = -kernelRadius; idx < 0; idx += 1)
		{
			float2 offset = (direction * idx);
			float3 sampleColor = _SourceRT.Sample(sampler_linear_clamp, uv * _SourceRTHandleScale.xy, offset).xyz;
			float depth = LinearEyeDepth(KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, uv, offset).x);

			float depthDiff = abs(centerDepth - depth);
			float dFactor = depthDiff * BLUR_DEPTH_FACTOR;
			float w = exp(-(dFactor * dFactor));

			weight = GaussianWeight(idx, deviation) * w;

			color += weight * sampleColor;
			weightSum += weight;
		}

		[unroll] for (idx = 1; idx <= kernelRadius; idx += 1)
		{
			float2 offset = (direction * idx);
			float3 sampleColor = _SourceRT.Sample(sampler_linear_clamp, uv * _SourceRTHandleScale.xy, offset).xyz;
			float depth = LinearEyeDepth(KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, uv, offset).x);

			float depthDiff = abs(centerDepth - depth);
			float dFactor = depthDiff * BLUR_DEPTH_FACTOR;
			float w = exp(-(dFactor * dFactor));

			weight = GaussianWeight(idx, deviation) * w;

			color += weight * sampleColor;
			weightSum += weight;
		}

		color /= weightSum;
		return float4(color, centerColor.w);
	}
}

inline float4 LinearEyeDepth4(float4 z)
{
	return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}

float4 BilateralUpsample(v2fUpsample input) : SV_Target
{
	const float threshold = UPSAMPLE_DEPTH_THRESHOLD;
	float4 highResDepth = LinearEyeDepth(GetSceneDepth(input.uv));
	float4 lowResDepth;

	lowResDepth[0] = KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, input.uv00).r;
	lowResDepth[1] = KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, input.uv10).r;
	lowResDepth[2] = KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, input.uv01).r;
	lowResDepth[3] = KWS_CameraDepthTextureLowRes.Sample(sampler_linear_clamp, input.uv11).r;

	lowResDepth = LinearEyeDepth4(lowResDepth);

	float4 depthDiff = abs(lowResDepth - highResDepth);

	float accumDiff = dot(depthDiff, float4(1, 1, 1, 1));

	UNITY_BRANCH
	if (accumDiff < threshold)
	{
		return _SourceRT.Sample(sampler_linear_clamp, input.uv * _SourceRTHandleScale.xy);
	}

	float minDepthDiff = depthDiff[0];
	float2 nearestUv = input.uv00;

	if (depthDiff[1] < minDepthDiff)
	{
		nearestUv = input.uv10;
		minDepthDiff = depthDiff[1];
	}

	if (depthDiff[2] < minDepthDiff)
	{
		nearestUv = input.uv01;
		minDepthDiff = depthDiff[2];
	}

	if (depthDiff[3] < minDepthDiff)
	{
		nearestUv = input.uv11;
		minDepthDiff = depthDiff[3];
	}

	return _SourceRT.Sample(sampler_linear_clamp, nearestUv * _SourceRTHandleScale.xy);
}