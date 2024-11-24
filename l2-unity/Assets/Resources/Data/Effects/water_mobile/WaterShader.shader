// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Zhanyou/Water"
{
	Properties
	{
		[NoScaleOffset]  _WaterMap("BumpMap", 2D) = "black" { }
	[NoScaleOffset]  _Skybox("Skybox", CUBE) = "black" { }
	[HideInInspector][NoScaleOffset]  _ReflectionTexture("ReflectionTexture", 2D) = "black" { }
		_LightDirection("LightDirection", Vector) = (1,2,1,1)
		_WaveFreqSpeed1("WaveFreqSpeed1", Vector) = (5,5,2,0)
		_WaveFreqSpeed2("WaveFreqSpeed2", Vector) = (25,25,0.6,0)
		_WaveSpeed("WaveSpeed", Range(0.001,0.01)) = 0.005
		_WaterColor("WaterColor", Color) = (0.3,0.4,0.5,0.7)
		_WaterClarity("WaterClarity", Range(0.01,1)) = 0.8
		_SpecularColor("SpecularColor", Color) = (1,1,1,1)
		_SpecularGlossy("SpecularGlossy", Range(1,100)) = 50
		_SpecularIntensity("SpecularIntensity", Range(0,2)) = 1
		_ReflectionIntensity("ReflectionIntensity", Range(0,1)) = 0.7
		_ReflectionClarity("ReflectionClarity", Range(0,1)) = 1
		_ReflectionDistort("ReflectionDistort", Range(0,1)) = 0.05
		_DepthAlpha("Inverse Alpha, Depth and Color ranges", Range(0,1)) = 0.17
	}

		SubShader
	{
		Tags{ "queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  
#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF

#include "UnityCG.cginc"  

		uniform sampler2D _WaterMap;
	uniform samplerCUBE  _Skybox;
	uniform float3 _LightDirection;
	uniform float4 _WaveFreqSpeed1;
	uniform float4 _WaveFreqSpeed2;
	uniform float4 _WaterColor;
	uniform float _WaterClarity;
	uniform float3 _SpecularColor;
	uniform float _SpecularGlossy;
	uniform float _SpecularIntensity;
	uniform float _ReflectionIntensity;
	uniform float _ReflectionClarity;
	uniform float _ReflectionDistort;
	uniform float _DepthAlpha;
	uniform float _WaveSpeed;

	sampler2D _CameraDepthTexture;

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float4 color : COLOR;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 texcoord0 : TEXCOORD0;
		float4 texcoord1 : TEXCOORD1;
		float2 texcoord2 : TEXCOORD2;
		float4 texcoord3 : TEXCOORD3;
		float4 proj0 :TEXCOORD4;
		UNITY_FOG_COORDS(3)
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		float4 tmpvar_1;
		float4 tmpvar_2;
		tmpvar_2 = mul(unity_ObjectToWorld , v.vertex);
		tmpvar_1.xyz = ((_WaterColor.xyz * v.color.xyz) * 2.0);
		float tmpvar_3;
		tmpvar_3 = clamp(((
			(v.color.w * 2.0)
			- 1.0) / _WaterClarity), 0.0, 1.0);
		tmpvar_1.w = (_WaterColor.w * tmpvar_3);
		float2 tmpvar_4;
		tmpvar_4 = (tmpvar_2.xz * 0.01);
		float2 tmpvar_5;
		tmpvar_5 = (_Time.yy * _WaveSpeed);
		float4 tmpvar_6;
		tmpvar_6.zw = float2(0.0, 0.0);
		tmpvar_6.xy = ((tmpvar_4 * _WaveFreqSpeed1.xy) + (tmpvar_5 * _WaveFreqSpeed1.zw));

		o.pos = UnityObjectToClipPos(v.vertex);
		o.texcoord0 = (_WorldSpaceCameraPos - tmpvar_2.xyz);
		o.texcoord1 = tmpvar_6;
		o.texcoord2 = ((tmpvar_4 * _WaveFreqSpeed2.xy) + (tmpvar_5 * _WaveFreqSpeed2.zw));
		o.texcoord3 = tmpvar_1;

		o.proj0 = ComputeScreenPos(o.pos);
		//COMPUTE_EYEDEPTH(o.proj0.z);

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float4 color_1;
	float reflectIntensity_2;
	float3 waterColor_3;
	float specular_4;
	float3 normal_5;
	float3 normal1_6;
	float3 normal0_7;
	float3 tmpvar_8;

	tmpvar_8 = tex2D(_WaterMap, i.texcoord1.xy).xyz;
	normal0_7 = tmpvar_8;
	float3 tmpvar_9;
	tmpvar_9 = tex2D(_WaterMap, i.texcoord2).xyz;
	normal1_6 = tmpvar_9;
	float3 tmpvar_10;
	tmpvar_10 = normalize(((2.0 *
		(normal0_7 + normal1_6)
		) - 2.0));
	normal_5.xz = tmpvar_10.xz;
	normal_5.y = ((tmpvar_10.y * 0.5) + 1.0);
	float3 tmpvar_11;
	tmpvar_11 = normalize(normal_5);
	normal_5 = tmpvar_11;
	float3 tmpvar_12;
	tmpvar_12 = normalize(i.texcoord0);
	float tmpvar_13;
	tmpvar_13 = max(0.0, dot(tmpvar_12, tmpvar_11));
	float3 tmpvar_14;
	tmpvar_14 = normalize(_LightDirection);
	float tmpvar_15;
	tmpvar_15 = (pow(max(0.0,
		dot(-(normalize((tmpvar_14 -
			(2.0 * (dot(tmpvar_11, tmpvar_14) * tmpvar_11))
			))), tmpvar_12)
		), _SpecularGlossy) * _SpecularIntensity);
	specular_4 = tmpvar_15;
	float3 tmpvar_16;
	tmpvar_16 = (-(normalize(
		(tmpvar_12 - (tmpvar_12.y * float3(0.0, 2.0, 0.0)))
		)) + ((tmpvar_11 * _ReflectionDistort) * 0.5));
	float4 tmpvar_17;
	tmpvar_17 = texCUBE(_Skybox, tmpvar_16);
	float3 tmpvar_18;
	tmpvar_18 = (2.0 * tmpvar_17.xyz);
	float3 tmpvar_19;
	tmpvar_19 = ((1.0 - (0.25 * tmpvar_13)) * i.texcoord3.xyz);
	waterColor_3 = tmpvar_19;
	float tmpvar_20;
	tmpvar_20 = max((_ReflectionIntensity + (
		(1.0 - _ReflectionIntensity)
		*
		pow(max(0.0, (1.0 - tmpvar_13)), 5.0)
		)), 0.0);
	reflectIntensity_2 = tmpvar_20;
	color_1.xyz = (((waterColor_3 * reflectIntensity_2) * (
		((tmpvar_18 * 2.0) * _ReflectionClarity)
		+
		(1.0 - _ReflectionClarity)
		)) + (specular_4 * _SpecularColor));
	color_1.w = i.texcoord3.w;

#ifdef WATER_EDGEBLEND_ON
	half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);

	half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.proj0));
	depth = LinearEyeDepth(depth);
	edgeBlendFactors = saturate(_DepthAlpha * (depth - i.proj0.w));

	color_1.a = color_1.a * edgeBlendFactors.x;
#endif

	return color_1;
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
}