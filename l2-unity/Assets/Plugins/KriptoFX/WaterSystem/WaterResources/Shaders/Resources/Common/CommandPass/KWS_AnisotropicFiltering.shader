Shader "Hidden/KriptoFX/KWS/AnisotropicFiltering"
{

    HLSLINCLUDE

#include "../KWS_WaterVariables.cginc"
#include "../../PlatformSpecific/KWS_PlatformSpecificHelpers.cginc"

        float KWS_AnisoReflectionsScale;
    float KWS_NormalizedWind;

    Texture2D _SourceRT;
    float4 _SourceRTHandleScale;

    uint KWS_ReverseBits32(uint bits)
    {
#if 0 // Shader model 5
        return reversebits(bits);
#else
        bits = (bits << 16) | (bits >> 16);
        bits = ((bits & 0x00ff00ff) << 8) | ((bits & 0xff00ff00) >> 8);
        bits = ((bits & 0x0f0f0f0f) << 4) | ((bits & 0xf0f0f0f0) >> 4);
        bits = ((bits & 0x33333333) << 2) | ((bits & 0xcccccccc) >> 2);
        bits = ((bits & 0x55555555) << 1) | ((bits & 0xaaaaaaaa) >> 1);
        return bits;
#endif
    }
    //-----------------------------------------------------------------------------
    float KWS_RadicalInverse_VdC(uint bits)
    {
        return float(KWS_ReverseBits32(bits)) * 2.3283064365386963e-10; // 0x100000000
    }

    //-----------------------------------------------------------------------------
    float2 KWS_Hammersley2d(uint i, uint maxSampleCount)
    {
        return float2(float(i) / float(maxSampleCount), KWS_RadicalInverse_VdC(i));
    }

    //-----------------------------------------------------------------------------
    float KWS_HashRand(uint s)
    {
        s = s ^ 2747636419u;
        s = s * 2654435769u;
        s = s ^ (s >> 16);
        s = s * 2654435769u;
        s = s ^ (s >> 16);
        s = s * 2654435769u;
        return float(s) / 4294967295.0f;
    }

    //-----------------------------------------------------------------------------
    float KWS_InitRand(float input)
    {
        return KWS_HashRand(uint(input * 4294967295.0f));
    }

    half4 ReflectionPreFiltering(Texture2D tex, sampler texSampler, float2 uv, const uint SAMPLE_COUNT)
    {
        half4 prefilteredColor = 0.0;
        float randNum = KWS_InitRand(uv.x * uv.y);

        float anisoScale = (KWS_NormalizedWind * 0.85 + 0.15) * -KWS_AnisoReflectionsScale * 0.05;
        float anisoScaleOffset = anisoScale * 0.35;

        UNITY_UNROLL
            for (uint i = 0u; i < SAMPLE_COUNT; ++i)
            {
                float2 u = KWS_Hammersley2d(i, SAMPLE_COUNT);
                u = frac(u + randNum + 0.5f);
                prefilteredColor += tex.SampleLevel(texSampler, uv - float2(0, u.x * anisoScale - anisoScaleOffset), 0);
            }
        prefilteredColor = prefilteredColor / (1 * SAMPLE_COUNT);

        return prefilteredColor;
    }

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    v2f vert(uint vertexID : SV_VertexID)
    {
        v2f o;
        o.vertex = GetTriangleVertexPosition(vertexID);
        o.uv = GetTriangleUV(vertexID) * _SourceRTHandleScale.xy;
        return o;
    }


    ENDHLSL


        SubShader
    {
        Pass //low quality
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define SAMPLE_COUNT 5

            half4 frag(v2f i) : SV_Target
            {
                return ReflectionPreFiltering(_SourceRT, sampler_linear_clamp, i.uv, SAMPLE_COUNT);
            }

            ENDHLSL
        }


        Pass //high quality
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define SAMPLE_COUNT 13

            half4 frag(v2f i) : SV_Target
            {
                return ReflectionPreFiltering(_SourceRT, sampler_linear_clamp, i.uv, SAMPLE_COUNT);
            }

            ENDHLSL
        }
    }
}

