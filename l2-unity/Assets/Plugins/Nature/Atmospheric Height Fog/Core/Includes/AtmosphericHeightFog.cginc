/* Add the following directive

		#include "Assets/BOXOPHOBIC/Atmospheric Height Fog/Core/Library/AtmosphericHeightFog.cginc"

// Apply Atmospheric Height Fog to transparent shaders like this
// Where finalColor is the shader output color, fogParams.rgb is the fog color and fogParams.a is the fog mask

		float4 fogParams = GetAtmosphericHeightFog(i.worldPos);
		return ApplyAtmosphericHeightFog(finalColor, fogParams);

*/

#ifndef ATMOSPHERIC_HEIGHT_FOG_INCLUDED
#define ATMOSPHERIC_HEIGHT_FOG_INCLUDED

//UnityCG is causing issues in shader graph
//#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"

uniform half _FogCat;
uniform half _SkyboxCat;
uniform half _AdvancedCat;
uniform half _NoiseCat;
uniform half _DirectionalCat;
uniform half4 AHF_FogColorStart;
uniform half4 AHF_FogColorEnd;
uniform half AHF_FogDistanceStart;
uniform half AHF_FogDistanceEnd;
uniform half AHF_FogDistanceFalloff;
uniform half AHF_FogColorDuo;
uniform half4 AHF_DirectionalColor;
uniform half3 AHF_DirectionalDir;
uniform half AHF_DirectionalIntensity;
uniform half AHF_DirectionalFalloff;
uniform half3 AHF_FogAxisOption;
uniform half AHF_FogHeightEnd;
uniform half AHF_FarDistanceHeight;
uniform float AHF_FarDistanceOffset;
uniform half AHF_FogHeightStart;
uniform half AHF_FogHeightFalloff;
uniform half AHF_FogLayersMode;
uniform half AHF_NoiseScale;
uniform half3 AHF_NoiseSpeed;
uniform half AHF_NoiseMin;
uniform half AHF_NoiseMax;
uniform half AHF_NoiseDistanceEnd;
uniform half AHF_NoiseIntensity;
uniform half AHF_FogIntensity;

float4 mod289(float4 x)
{
	return x - floor(x * (1.0 / 289.0)) * 289.0;
}

float4 perm(float4 x)
{
	return mod289(((x * 34.0) + 1.0) * x);
}

float SimpleNoise3D(float3 p)
{
	float3 a = floor(p);
	float3 d = p - a;
	d = d * d * (3.0 - 2.0 * d);
	float4 b = a.xxyy + float4(0.0, 1.0, 0.0, 1.0);
	float4 k1 = perm(b.xyxy);
	float4 k2 = perm(k1.xyxy + b.zzww);
	float4 c = k2 + a.zzzz;
	float4 k3 = perm(c);
	float4 k4 = perm(c + 1.0);
	float4 o1 = frac(k3 * (1.0 / 41.0));
	float4 o2 = frac(k4 * (1.0 / 41.0));
	float4 o3 = o2 * d.z + o1 * (1.0 - d.z);
	float2 o4 = o3.yw * d.x + o3.xz * (1.0 - d.x);
	return o4.y * d.y + o4.x * (1.0 - d.y);
}

// Returns the fog color and alpha based on world position
float4 GetAtmosphericHeightFog(float3 positionWS)
{
	float4 finalColor;

	float3 WorldPosition = positionWS;

	float3 WorldPosition2_g1 = WorldPosition;
	float temp_output_7_0_g1022 = AHF_FogDistanceStart;
	float temp_output_155_0_g1 = saturate(((distance(WorldPosition2_g1, _WorldSpaceCameraPos) - temp_output_7_0_g1022) / (AHF_FogDistanceEnd - temp_output_7_0_g1022)));
#ifdef AHF_DISABLE_FALLOFF
	float staticSwitch467_g1 = temp_output_155_0_g1;
#else
	float staticSwitch467_g1 = (1.0 - pow((1.0 - abs(temp_output_155_0_g1)), AHF_FogDistanceFalloff));
#endif
	half FogDistanceMask12_g1 = staticSwitch467_g1;
	float3 lerpResult258_g1 = lerp((AHF_FogColorStart).rgb, (AHF_FogColorEnd).rgb, ((FogDistanceMask12_g1 * FogDistanceMask12_g1 * FogDistanceMask12_g1) * AHF_FogColorDuo));
	float3 normalizeResult318_g1 = normalize((WorldPosition2_g1 - _WorldSpaceCameraPos));
	float dotResult145_g1 = dot(normalizeResult318_g1, AHF_DirectionalDir);
	half Jitter502_g1 = 0.0;
	float temp_output_140_0_g1 = (saturate(((dotResult145_g1 + Jitter502_g1) * 0.5 + 0.5)) * AHF_DirectionalIntensity);
#ifdef AHF_DISABLE_FALLOFF
	float staticSwitch470_g1 = temp_output_140_0_g1;
#else
	float staticSwitch470_g1 = pow(abs(temp_output_140_0_g1), AHF_DirectionalFalloff);
#endif
	float DirectionalMask30_g1 = staticSwitch470_g1;
	float3 lerpResult40_g1 = lerp(lerpResult258_g1, (AHF_DirectionalColor).rgb, DirectionalMask30_g1);
#ifdef AHF_DISABLE_DIRECTIONAL
	float3 staticSwitch442_g1 = lerpResult258_g1;
#else
	float3 staticSwitch442_g1 = lerpResult40_g1;
#endif
	half3 Input_Color6_g1012 = staticSwitch442_g1;
#ifdef UNITY_COLORSPACE_GAMMA
	float3 staticSwitch1_g1012 = Input_Color6_g1012;
#else
	float3 staticSwitch1_g1012 = (Input_Color6_g1012 * ((Input_Color6_g1012 * ((Input_Color6_g1012 * 0.305306) + 0.6821711)) + 0.01252288));
#endif
	half3 Final_Color462_g1 = staticSwitch1_g1012;
	half3 AHF_FogAxisOption181_g1 = AHF_FogAxisOption;
	float3 break159_g1 = (WorldPosition2_g1 * AHF_FogAxisOption181_g1);
	float temp_output_7_0_g1024 = AHF_FogDistanceEnd;
	float temp_output_643_0_g1 = saturate(((distance(WorldPosition2_g1, _WorldSpaceCameraPos) - temp_output_7_0_g1024) / ((AHF_FogDistanceEnd + AHF_FarDistanceOffset) - temp_output_7_0_g1024)));
	half FogDistanceMaskFar645_g1 = (temp_output_643_0_g1 * temp_output_643_0_g1);
	float lerpResult690_g1 = lerp(AHF_FogHeightEnd, (AHF_FogHeightEnd + AHF_FarDistanceHeight), FogDistanceMaskFar645_g1);
	float temp_output_7_0_g1025 = lerpResult690_g1;
	float temp_output_167_0_g1 = saturate((((break159_g1.x + break159_g1.y + break159_g1.z) - temp_output_7_0_g1025) / (AHF_FogHeightStart - temp_output_7_0_g1025)));
#ifdef AHF_DISABLE_FALLOFF
	float staticSwitch468_g1 = temp_output_167_0_g1;
#else
	float staticSwitch468_g1 = pow(abs(temp_output_167_0_g1), AHF_FogHeightFalloff);
#endif
	half FogHeightMask16_g1 = staticSwitch468_g1;
	float lerpResult328_g1 = lerp((FogDistanceMask12_g1 * FogHeightMask16_g1), saturate((FogDistanceMask12_g1 + FogHeightMask16_g1)), AHF_FogLayersMode);
	float mulTime204_g1 = _Time.y * 2.0;
	float3 temp_output_197_0_g1 = ((WorldPosition2_g1 * (1.0 / AHF_NoiseScale)) + (-AHF_NoiseSpeed * mulTime204_g1));
	float3 p1_g1029 = temp_output_197_0_g1;
	float localSimpleNoise3D1_g1029 = SimpleNoise3D(p1_g1029);
	float temp_output_7_0_g1028 = AHF_NoiseMin;
	float temp_output_7_0_g1027 = AHF_NoiseDistanceEnd;
	half NoiseDistanceMask7_g1 = saturate(((distance(WorldPosition2_g1, _WorldSpaceCameraPos) - temp_output_7_0_g1027) / (0.0 - temp_output_7_0_g1027)));
	float lerpResult198_g1 = lerp(1.0, saturate(((localSimpleNoise3D1_g1029 - temp_output_7_0_g1028) / (AHF_NoiseMax - temp_output_7_0_g1028))), (NoiseDistanceMask7_g1 * AHF_NoiseIntensity));
	half NoiseSimplex3D24_g1 = lerpResult198_g1;
#ifdef AHF_DISABLE_NOISE3D
	float staticSwitch42_g1 = lerpResult328_g1;
#else
	float staticSwitch42_g1 = (lerpResult328_g1 * NoiseSimplex3D24_g1);
#endif
	float temp_output_454_0_g1 = (staticSwitch42_g1 * AHF_FogIntensity);
	half Final_Alpha463_g1 = temp_output_454_0_g1;
	float4 appendResult114_g1 = (float4(Final_Color462_g1, Final_Alpha463_g1));
	float4 appendResult457_g1 = (float4(WorldPosition2_g1, 1.0));
#ifdef AHF_DEBUG_WORLDPOS
	float4 staticSwitch456_g1 = appendResult457_g1;
#else
	float4 staticSwitch456_g1 = appendResult114_g1;
#endif


	finalColor = staticSwitch456_g1;
	return finalColor;
}

// Applies the fog
float3 ApplyAtmosphericHeightFog(float3 color, float4 fog)
{
	return float3(lerp(color.rgb, fog.rgb, fog.a));
}

float4 ApplyAtmosphericHeightFog(float4 color, float4 fog)
{
	return float4(lerp(color.rgb, fog.rgb, fog.a), color.a);
}

// Shader Graph Support
void GetAtmosphericHeightFog_half(float3 positionWS, out float4 Out)
{
	Out = GetAtmosphericHeightFog(positionWS);
}

void ApplyAtmosphericHeightFog_half(float3 color, float4 fog, out float3 Out)
{
	Out = ApplyAtmosphericHeightFog(color, fog);
}

#endif
