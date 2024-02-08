struct appdata
{
	float4 vertex : POSITION;
};

struct v2f
{
	float4 vertex : SV_POSITION;
	float height : TEXCOORD0;
};


float3 KW_FFT_CameraPosition;
float KW_FFT_DomainScale;

v2f vert(appdata v)
{
	v2f o;

	float3 worldPos = 0;
	worldPos.xz = KW_FFT_CameraPosition.xz + float2(1, -1) * v.vertex.xz * KW_FFT_DomainScale;
	float3 offset = ComputeWaterOffset(worldPos);
	v.vertex.xz += offset.xz / KW_FFT_DomainScale;
	o.height = offset.y;
	o.vertex = float4(v.vertex.xz, 0, 0.5);
	return o;
}

float4 frag(v2f i) : SV_Target
{
	return float4(i.height, 0, 0, 1);
}