#ifndef BEAUTIFY_DISTORTION
#define BEAUTIFY_DISTORTION

#if BEAUTIFY_CABERRATION

float3 _ChromaticAberrationData;
#define CHROMATIC_ABERRATION_INTENSITY _ChromaticAberrationData.x
#define CHROMATIC_ABERRATION_SMOOTHING _ChromaticAberrationData.y
#define CHROMATIC_ABERRATION_SHIFT _ChromaticAberrationData.z
#define CHROMATIC_ABERRATION_MAX_SAMPLES 32

#if BEAUTIFY_TURBO

float4 GetDistortedColor(float2 uv) {

    float2 coords = 2.0 * uv - 1.0;
    float  dst = dot(coords, coords);
    float2 delta = coords * (dst * CHROMATIC_ABERRATION_INTENSITY);
    float r = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv).r;
    float g = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - delta).g;
    float b = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - delta * 2.0).b;
    return float4(r, g, b, 1.0);
}

#else

float4 GetDistortedColor(float2 uv) {

    float2 coords  = 2.0 * uv - 1.0;
    float  dst     = dot(coords, coords);
    float2 delta   = coords * (dst * CHROMATIC_ABERRATION_INTENSITY);
    float4 abColor = float4(0,0,0,1.0);

#if BEAUTIFY_CABERRATION_ALT
    int samples    = clamp( (int)(dst * CHROMATIC_ABERRATION_SMOOTHING) + 1, 1, CHROMATIC_ABERRATION_MAX_SAMPLES);
    UNITY_UNROLL
    for (int k=0;k<CHROMATIC_ABERRATION_MAX_SAMPLES;k++) {
        if (k<samples) {
            float h = (float)(k + 1.0) / samples;
            float r = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, uv, 0).r;
            float g = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, uv - delta * h, 0).g;
            float b = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, uv - delta * 2.0 * h, 0).b;
            abColor.rgb += float3(r, g, b);
        }
    }
    abColor.xyz /= samples;
#else
    int samples    = clamp( (int)(dst * CHROMATIC_ABERRATION_SMOOTHING) + 1, 3, CHROMATIC_ABERRATION_MAX_SAMPLES);
    float3 weight  = 0.0001.xxx;
    UNITY_UNROLL
    for (int k=0;k<CHROMATIC_ABERRATION_MAX_SAMPLES;k++) {
        if (k<samples) {
            float h = (k+0.5) / samples;
            float3 spec = saturate( abs( fmod( (h + CHROMATIC_ABERRATION_SHIFT) * 6.0.xxx + float3(0, 4, 2), 6.0) - 3.0) - 1.0 ); // hue to rgb
		    float3 rgb = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, uv - delta * h, 0).xyz;
		    abColor.xyz += rgb * spec;
		    weight += spec;
        }
    }
    abColor.xyz /= weight;
#endif

    return abColor;
}

#endif // BEAUTIFY_TURBO

#endif // BEAUTIFY_CABERRATION

#endif // BEAUTIFY_DISTORTION