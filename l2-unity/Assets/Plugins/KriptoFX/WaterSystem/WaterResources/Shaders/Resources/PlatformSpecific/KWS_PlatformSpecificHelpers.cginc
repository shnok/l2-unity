#define _FrustumCameraPlanes unity_CameraWorldClipPlanes

float4 _CameraDepthTexture_TexelSize;

Texture2D _CameraOpaqueTexture;
SamplerState sampler_CameraOpaqueTexture;
float4 _CameraOpaqueTexture_TexelSize;

float3 KWS_AmbientColor;

float4x4 KWS_MATRIX_I_VP;

#ifndef UNIVERSAL_PIPELINE_CORE_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

#ifndef UNITY_DECLARE_DEPTH_TEXTURE_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#endif

#ifndef UNIVERSAL_LIGHTING_INCLUDED
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

inline float4 ObjectToClipPos(float4 vertex)
{
	return TransformObjectToHClip(vertex.xyz);
}

inline float2 GetTriangleUV(uint vertexID)
{
#if UNITY_UV_STARTS_AT_TOP
	return float2((vertexID << 1) & 2, 1.0 - (vertexID & 2));
#else
	return float2((vertexID << 1) & 2, vertexID & 2);
#endif
}

inline float4 GetTriangleVertexPosition(uint vertexID, float z = UNITY_NEAR_CLIP_VALUE)
{
	float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
	return float4(uv * 2.0 - 1.0, z, 1.0);
}

inline float3 LocalToWorldPos(float3 localPos)
{
	return mul(UNITY_MATRIX_M, float4(localPos, 1)).xyz;
}

inline float3 WorldToLocalPos(float3 worldPos)
{
	return mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
}

inline float3 GetWorldSpaceViewDirNorm(float3 worldPos)
{
	return normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
}

inline float GetWorldToCameraDistance(float3 worldPos)
{
	return length(_WorldSpaceCameraPos.xyz - worldPos.xyz);
}

float3 GetWorldSpacePositionFromDepth(float2 uv, float deviceDepth)
{
	float4 positionCS = float4(uv * 2.0 - 1.0, deviceDepth, 1.0);
#if UNITY_UV_STARTS_AT_TOP
	positionCS.y = -positionCS.y;
#endif
	float4 hpositionWS = mul(KWS_MATRIX_I_VP, positionCS);
	return hpositionWS.xyz / hpositionWS.w;
}

inline float GetSceneDepth(float2 uv)
{
	return _CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, uv, 0).x;
}

inline float4 GetSceneDepthGather(float2 uv)
{
	return _CameraDepthTexture.Gather(sampler_CameraDepthTexture, uv);
}

inline float3 GetAmbientColor()
{
    return KWS_AmbientColor;
}

inline float3 GetSceneColor(float2 uv)
{
	return _CameraOpaqueTexture.SampleLevel(sampler_CameraOpaqueTexture, uv, 0).xyz;
}

inline half3 GetSceneColorWithDispersion(float2 uv, float dispersionStrength)
{
	half3 refraction;
	refraction.r = GetSceneColor(uv - _CameraOpaqueTexture_TexelSize.xy * dispersionStrength).r;
	refraction.g = GetSceneColor(uv).g;
	refraction.b = GetSceneColor(uv + _CameraOpaqueTexture_TexelSize.xy * dispersionStrength).b;
	return refraction;
}


inline float3 GetMainLightDir()
{
	return _MainLightPosition.xyz;
}

inline float3 GetMainLightColor()
{
	return _MainLightColor.rgb;
}

inline float4 ComputeNonStereoScreenPos(float4 pos) {
	float4 o = pos * 0.5f;
	o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
	o.zw = pos.zw;
	return o;
}


inline float4 ComputeGrabScreenPos(float4 pos) {
#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
#else
	float scale = 1.0;
#endif
	float4 o = pos * 0.5f;
	o.xy = float2(o.x, o.y * scale) + o.w;
#ifdef UNITY_SINGLE_PASS_STEREO
	o.xy = TransformStereoScreenSpaceTex(o.xy, pos.w);
#endif
	o.zw = pos.zw;
	return o;
}

inline float LinearEyeDepth(float z)
{
	return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}
