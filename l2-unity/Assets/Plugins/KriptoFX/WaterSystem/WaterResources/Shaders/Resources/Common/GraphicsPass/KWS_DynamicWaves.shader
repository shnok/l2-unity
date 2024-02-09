Shader "Hidden/KriptoFX/KWS/DynamicWaves"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

		HLSLINCLUDE


		#include "UnityCG.cginc"
		#include "../KWS_WaterVariables.cginc"

		struct vertexInputDraw
	{
		float4 vertex : POSITION;
		float3 uv : TEXCOORD0;
		float force : TEXCOORD1;
	};

	struct vertexInput
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2fDraw
	{
		float4 vertex  : POSITION;
		float3 uv : TEXCOORD0;
		float force : TEXCOORD1;
	};


	struct v2f
	{
		float4 vertex  : POSITION;
		float3 worldPos : TEXCOORD0;
		float4 uv[6] : TEXCOORD1;

	};

	sampler2D _MainTex;
	sampler2D KW_DynamicObjectsMap;
	sampler2D _PrevTex;
	float2 _WorldOffset;
	float2 _WorldOffsetPrev;
	float4x4  _WaterMVPDepth;

	float _DropSize;
	float _Damping;
	float4 _TEST;
	float4 _MousePos;
	half KW_RipplesScaleLerped;
	float2 _MouseWorldPos;

	float3 KW_AreaOffset;
	float3 KW_LastAreaOffset;
	float KW_DrawPointForce;
	half KW_InteractiveWavesPixelSpeed;
	float4 _MainTex_TexelSize;
	float KW_DynamicWavesRainStrength;
	float KW_DynamicWavesRainThreshold;


	v2fDraw vertDraw(vertexInputDraw v)
	{
		v2fDraw o;
		if (_ProjectionParams.x < 0) v.vertex.y = 1 - v.vertex.y;
		o.vertex = float4(v.vertex.xy - 0.5, 0, 0.5);

		o.uv = v.uv.xyz * 2 - 1;

		o.force = v.force;

		return o;
	}


	v2f vert(vertexInput v)
	{
		v2f o;
		o.vertex = float4(v.vertex.xy - 0.5, 0, 0.5);
		v.uv = float2(v.uv.x, 1 - v.uv.y); //TODO check if (_ProjectionParams.x < 0)

		float3 size = float3(_MainTex_TexelSize.x, -_MainTex_TexelSize.y, 0) * KW_InteractiveWavesPixelSpeed;

		o.uv[0] = float4(v.uv.xy + KW_LastAreaOffset.xz, 0, 0);
		o.uv[1] = float4(v.uv.xy + size.xz + KW_AreaOffset.xz, 0, 0); //0.004, 0      right
		o.uv[2] = float4(v.uv.xy + size.yz + KW_AreaOffset.xz, 0, 0); //-0.004, 0     left
		o.uv[3] = float4(v.uv.xy + size.zx + KW_AreaOffset.xz, 0, 0); //0, 0.004      up
		o.uv[4] = float4(v.uv.xy + size.zy + KW_AreaOffset.xz, 0, 0); //0, -0.004     
		o.uv[5] = float4(v.uv.xy, 0, 0);

		float2 worldUV = v.uv.xy * KW_DynamicWavesAreaSize - KW_DynamicWavesAreaSize * 0.5;
		o.worldPos = float3(worldUV.x, 0, worldUV.y) + KW_DynamicWavesWorldPos;
		return o;
	}


	half4 fragDraw(v2fDraw i) : COLOR
	{
		float alphaMask = 1 - length(i.uv);
		if (alphaMask < 0) return 0;

		return i.force < 0 ? float4(alphaMask * i.force, 0, 0, 1) : float4(i.force, 0, 0, 1);
	}

	struct FragmentOutput
	{
		half dest0 : SV_Target0;
		half2 dest1 : SV_Target1;
	};


	float RainNoise(float2 p) {

		float t = frac(_Time.x * 0.1) * 4563.0;
		return sin(p.x * t + p.y * t * t * 0.123456) * sin(p.y * t + p.x * t * t * 0.789123);
	}

	FragmentOutput frag(v2f i)
	{
		FragmentOutput o;
	if (i.uv[0].x < 0.01 || i.uv[0].x > 0.99 || i.uv[0].y < 0.01 || i.uv[0].y > 0.99
		|| i.uv[1].x < 0.01 || i.uv[1].x > 0.99 || i.uv[1].y < 0.01 || i.uv[1].y > 0.99)
		{
			o.dest0 = 0.0;
			o.dest1 = 0.5;
			return o;
		}

		float prevFrame = tex2Dlod(_PrevTex, i.uv[0]);

		float right = tex2Dlod(_MainTex, i.uv[1]).x;
		float left = tex2Dlod(_MainTex, i.uv[2]).x;
		float top = tex2Dlod(_MainTex, i.uv[3]).x;
		float down = tex2Dlod(_MainTex, i.uv[4]).x;

		float dynamicObjects = tex2D(KW_DynamicObjectsMap, i.uv[5]);

		//float2 depthUV = (i.worldPos.xz - KW_DepthPos.xz) / KW_DepthOrthographicSize + 0.5;
		//float depth = tex2D(KW_OrthoDepth, depthUV) * KW_DepthNearFarDistance.z - KW_DepthNearFarDistance.y;

		float data = dynamicObjects;

#if KW_USE_RAIN_EFFECT

		data -= KW_DynamicWavesRainStrength * saturate(RainNoise(i.uv[5].xy) > KW_DynamicWavesRainThreshold);
#endif

		data += (right + left + top + down) * 0.5 - prevFrame;
		data *= 0.992;

		data = clamp(data, -10, 200);
		//data = data * 0.5 + 0.5;

		o.dest0 = data;


		//float2 dxy = prevFrame - float2(right, top);

		//o.dest1 = normalize(float3(dxy, 1.0)).xy * 0.5 + 0.5;
		o.dest1 = (20 * float2(right - left, top - down)) * 0.5 + 0.5;

		float2 maskAlphaUV = 1 - abs(i.uv[5] * 2 - 1);
		float maskAlpha = saturate((maskAlphaUV.x * maskAlphaUV.y - 0.001) * 3);
		o.dest1 = lerp(half2(0.5, 0.5), o.dest1, maskAlpha);

		return o;
	}


		ENDHLSL

		Subshader {

		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Blend SrcAlpha One

			HLSLPROGRAM
			#pragma vertex vertDraw
			#pragma fragment fragDraw
			ENDHLSL
		}

			Pass
		{
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ KW_USE_RAIN_EFFECT
			ENDHLSL
		}
	}

}
