inline half3 BlendNormals(half3 n1, half3 n2)
{
	return (half3(n1.xz + n2.xz, n1.y * n2.y).xzy);
}

inline half3 BlendNormals(half3 n1, half3 n2, half3 n3)
{
	return (half3(n1.xz + n2.xz + n3.xz, 1).xzy);
}


float Pow5(float x)
{
	return x * x * x * x * x;
}

// filtering

inline float4 Texture2DSampleAA(texture2D tex, SamplerState state, float2 uv)
{
	half4 color = tex.Sample(state, uv.xy);

	float2 uv_dx = ddx(uv);
	float2 uv_dy = ddy(uv);

	color += tex.Sample(state, uv.xy + (0.25) * uv_dx + (0.75) * uv_dy);
	color += tex.Sample(state, uv.xy + (-0.25) * uv_dx + (-0.75) * uv_dy);
	color += tex.Sample(state, uv.xy + (-0.75) * uv_dx + (0.25) * uv_dy);
	color += tex.Sample(state, uv.xy + (0.75) * uv_dx + (-0.25) * uv_dy);

	color /= 5.0;

	return color;
}

float4 cubic(float v) {
	float4 n = float4(1.0, 2.0, 3.0, 4.0) - v;
	float4 s = n * n * n;
	float x = s.x;
	float y = s.y - 4.0 * s.x;
	float z = s.z - 4.0 * s.y + 6.0 * s.x;
	float w = 6.0 - x - y - z;
	return float4(x, y, z, w) * (1.0 / 6.0);
}

inline float4 Texture2DSampleLevelBicubic(texture2D tex, SamplerState state, float2 uv, float4 texelSize, float level)
{
	uv = uv * texelSize.zw - 0.5;
	float2 fxy = frac(uv);
	uv -= fxy;

	float4 xcubic = cubic(fxy.x);
	float4 ycubic = cubic(fxy.y);

	float4 c = uv.xxyy + float2(-0.5, +1.5).xyxy;
	float4 s = float4(xcubic.xz + xcubic.yw, ycubic.xz + ycubic.yw);
	float4 offset = c + float4(xcubic.yw, ycubic.yw) / s;
	offset *= texelSize.xxyy;

	half4 sample0 = tex.SampleLevel(state, offset.xz, level);
	half4 sample1 = tex.SampleLevel(state, offset.yz, level);
	half4 sample2 = tex.SampleLevel(state, offset.xw, level);
	half4 sample3 = tex.SampleLevel(state, offset.yw, level);

	float sx = s.x / (s.x + s.y);
	float sy = s.z / (s.z + s.w);

	return lerp(lerp(sample3, sample2, sx), lerp(sample1, sample0, sx), sy);
}

inline float4 Texture2DSampleBicubic(texture2D tex, SamplerState state, float2 uv, float4 texelSize)
{
	uv = uv * texelSize.zw - 0.5;
	float2 fxy = frac(uv);
	uv -= fxy;

	float4 xcubic = cubic(fxy.x);
	float4 ycubic = cubic(fxy.y);

	float4 c = uv.xxyy + float2(-0.5, +1.5).xyxy;
	float4 s = float4(xcubic.xz + xcubic.yw, ycubic.xz + ycubic.yw);
	float4 offset = c + float4(xcubic.yw, ycubic.yw) / s;
	offset *= texelSize.xxyy;

	half4 sample0 = tex.Sample(state, offset.xz);
	half4 sample1 = tex.Sample(state, offset.yz);
	half4 sample2 = tex.Sample(state, offset.xw);
	half4 sample3 = tex.Sample(state, offset.yw);

	float sx = s.x / (s.x + s.y);
	float sy = s.z / (s.z + s.w);

	return lerp(lerp(sample3, sample2, sx), lerp(sample1, sample0, sx), sy);
}

inline float4 Texture2DSampleBilinear(texture2D tex, SamplerState state, float2 uv, float4 texelSize)
{
	uv = uv * texelSize.zw + 0.5;
	float2 iuv = floor(uv);
	float2 fuv = frac(uv);
	uv = iuv + fuv * fuv * (3.0 - 2.0 * fuv); // fuv*fuv*fuv*(fuv*(fuv*6.0-15.0)+10.0);;
	uv = (uv - 0.5) / texelSize.zw;
	return tex.Sample(state, uv);
}

// end filtering