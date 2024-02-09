float4 _Color;
float _Size;
float4 _Offset;
float _FPS;
float3 _Scale;

texture2D KWS_FoamParticle;
sampler2D KW_VAT_Position;
float4 KW_VAT_Position_TexelSize;
sampler2D KW_VAT_RangeLookup;
float4 KW_VAT_RangeLookup_TexelSize;
sampler2D KW_VAT_Offset;
float KW_SizeAdditiveScale;

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float, KW_WaveTimeOffset)
UNITY_INSTANCING_BUFFER_END(Props)


struct appdata_foam
{
    float4 vertex : POSITION;
    float3 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

inline float4 DecodeParticleData(float idx)
{
    //_Time.y = 0;
    float timeOffset = UNITY_ACCESS_INSTANCED_PROP(Props, KW_WaveTimeOffset);
    float timeLimit = (14.0 * 15.0) / 20.0; //(frameX * frameY) / fps

    float time = frac((KW_GlobalTimeSpeedMultiplier * _Time.y) / timeLimit) * timeLimit;
    time += timeOffset * KW_GlobalTimeOffsetMultiplier;
    time = frac(time * KW_VAT_RangeLookup_TexelSize.x) * KW_VAT_RangeLookup_TexelSize.z;

    float height = frac(idx / KW_VAT_Position_TexelSize.w);
    float offset = (floor(idx / KW_VAT_Position_TexelSize.z)) * KW_VAT_Position_TexelSize.x; //todo check w instead of z

    float4 lookup = tex2Dlod(KW_VAT_RangeLookup, float4((time * _FPS) * KW_VAT_RangeLookup_TexelSize.x, 0, 0, 0));

    float offsetMin = min(lookup.y, offset);
    float4 uv1 = float4((float2(offsetMin + lookup.x - KW_VAT_Position_TexelSize.x * 0.75, height)), 0, 0);
    float4 texturePos1 = tex2Dlod(KW_VAT_Position, uv1);

    float offsetMin2 = min(lookup.w, offset);
    float4 uv2 = float4((float2(offsetMin2 + lookup.z - KW_VAT_Position_TexelSize.x * 0.75, height)), 0, 0);
    float4 texturePos2 = tex2Dlod(KW_VAT_Position, uv2);
    {
        texturePos1 = lerp(texturePos1, texturePos2, frac(time * _FPS));
    }
    texturePos1.z = 1 - texturePos1.z;

    float heightOffset = tex2Dlod(KW_VAT_Offset, float4(texturePos1.xz, 0, 0)).x;

    texturePos1.xyz *= _Scale;
    texturePos1.xyz += _Offset.xyz;
    texturePos1.y -= heightOffset * 1.25;
    return texturePos1;
}

inline float3 ParticleDataToLocalPosition(float3 textureLocalPos, float heightOffset, out float depth)
{
    float3 waterOffset = 0;
    float3 textureWorldPos = LocalToWorldPos(textureLocalPos);

    float terrainDepth = ComputeWaterOrthoDepth(textureWorldPos);

#if FOAM_COMPUTE_WATER_OFFSET
    waterOffset += ComputeWaterOffset(textureWorldPos);
    float3 scale = float3(length(UNITY_MATRIX_M._m00_m10_m20), length(UNITY_MATRIX_M._m01_m11_m21), length(UNITY_MATRIX_M._m02_m12_m22));
    waterOffset /= scale;
    textureWorldPos += waterOffset;
#endif 

    textureWorldPos.y = max(textureWorldPos.y, (terrainDepth + 0.05)) + heightOffset;
    textureLocalPos.y = WorldToLocalPos(textureWorldPos).y;

    depth = textureWorldPos.y - terrainDepth;

    return textureLocalPos;
}

inline float3 CreateBillboardParticle(float size, float2 uv, float3 localPos)
{
    float3 cameraF = float3(uv.x - 0.5, uv.y - 0.5, 0);
    _Size += KW_SizeAdditiveScale;
    _Size /= length(UNITY_MATRIX_M._m01_m11_m21);

    size *= _Size;
    cameraF *= float3(size, size, 1);
    //cameraF = mul(cameraF, (float3x3)matrixMV);
    cameraF = mul(mul((float3x3)UNITY_MATRIX_M, cameraF), (float3x3)UNITY_MATRIX_V).xyz;
    return cameraF + localPos;
}

inline float3 CreateBillboardParticleRelativeToAlpha(float size, float2 uv, float3 localPos, float alpha)
{
    float3 cameraF = float3(uv.x - 0.5, uv.y - 0.5, 0);
    _Size += KW_SizeAdditiveScale;
    _Size /= length(UNITY_MATRIX_M._m01_m11_m21);

    size *= _Size * alpha;
    cameraF *= float3(size, size, 1);
    //cameraF = mul(cameraF, (float3x3)matrixMV);
    cameraF = mul(mul((float3x3)UNITY_MATRIX_M, cameraF), (float3x3)UNITY_MATRIX_V).xyz;
    return cameraF + localPos;
}

inline float2 GetParticleUV(float2 uv)
{
    //return uv.xy;
    return uv.xy * float2(3, 4) - float2(2, 1);
}

inline half GetParticleAlpha(half vertexAlpha, half colorAlpha, float2 uv)
{
    /*float particle = KWS_FoamParticle.Sample(sampler_linear_repeat, uv).x;
    particle *= vertexAlpha;
    particle *= 0.1;*/
    //return saturate(particle);

    half uvAlpha = max(0, 1 - length(uv));
    //half uvAlpha = 1 - max(0, length(uv * 2 - 1));
    uvAlpha = saturate(uvAlpha * 2);
    half alpha = colorAlpha;
    alpha *= 1 + KW_SizeAdditiveScale * 5;
    alpha *= vertexAlpha;
    alpha *= uvAlpha;
    return alpha;
}

inline half GetParticleAlphaShadow(half vertexAlpha, half colorAlpha, float2 uv)
{
    half uvAlpha = max(0, 1 - length(uv));
   // half uvAlpha = 1 - max(0, length(uv * 2 - 1));
    half alpha = colorAlpha;
    alpha *= 1 + KW_SizeAdditiveScale * 5;
    alpha *= vertexAlpha;
    alpha *= uvAlpha;
    return alpha;
}