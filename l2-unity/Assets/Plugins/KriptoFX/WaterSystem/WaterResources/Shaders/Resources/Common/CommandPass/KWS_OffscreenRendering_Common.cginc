texture2D _SourceRT;
float4 _SourceRTHandleScale;

half3 GetOffscreenColor(float2 uv)
{
    half3 sceneColor = GetSceneColor(uv);
    half4 waterColor = _SourceRT.SampleLevel(sampler_linear_clamp, uv * _SourceRTHandleScale.xy, 0);
    return lerp(sceneColor.rgb, waterColor.rgb, waterColor.a);
}