Shader "Hidden/KriptoFX/KWS/BlurGaussian"
{

		HLSLINCLUDE

	#include "../../PlatformSpecific/KWS_PlatformSpecificHelpers.cginc"

		struct vertexInput
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct vertexOutput
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	SamplerState sampler_linear_clamp;
	texture2D _SourceRT;
	float2 _SourceRT_TexelSize;
	float _SampleScale;
	float4 _SourceRTHandleScale;

	//vertexOutput vert(vertexInput v)
	//{
	//	vertexOutput o;

	//	o.vertex = UnityObjectToClipPos(v.vertex);
	//	o.texcoord = v.uv;

	//	return o;
	//}

	vertexOutput vert(uint vertexID : SV_VertexID)
	{
		vertexOutput o;
		o.vertex = GetTriangleVertexPosition(vertexID);
		o.uv = GetTriangleUV(vertexID) * _SourceRTHandleScale.xy;
		return o;
	}

	half4 DownsampleFilter(float2 uv)
	{
		//return tex2D(_SourceRT, uv);
		/*float2 texelSize = _SourceRT_TexelSize;
		half4 A = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(-1.0, -1.0)));
		half4 B = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(0.0, -1.0)));
		half4 C = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(1.0, -1.0)));
		half4 D = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(-0.5, -0.5)));
		half4 E = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(0.5, -0.5)));
		half4 F = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(-1.0, 0.0)));
		half4 G = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv));
		half4 H = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(1.0, 0.0)));
		half4 I = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(-0.5, 0.5)));
		half4 J = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(0.5, 0.5)));
		half4 K = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(-1.0, 1.0)));
		half4 L = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(0.0, 1.0)));
		half4 M = tex2D(_SourceRT,  UnityStereoTransformScreenSpaceTex(uv + texelSize * float2(1.0, 1.0)));

		half2 div = (1.0 / 4.0) * half2(0.5, 0.125);

		half4 o = (D + E + I + J) * div.x;
		o += (A + B + G + F) * div.y;
		o += (B + C + H + G) * div.y;
		o += (F + G + L + K) * div.y;
		o += (G + H + M + L) * div.y;

		return o;*/

		float4 d = _SourceRT_TexelSize.xyxy * float4(-1, -1, +1, +1);

		half4 s;
		s = _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.xy), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.zy), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.xw), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.zw), 0);

		return s * (1.0 / 4);
	}

	half4 UpsampleFilter(float2 uv)
	{
		// 4-tap bilinear upsampler
		float4 d = _SourceRT_TexelSize.xyxy * float4(-1, -1, +1, +1) * (_SampleScale * 0.5);

		half4 s;
		s = _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.xy), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.zy), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.xw), 0);
		s += _SourceRT.SampleLevel(sampler_linear_clamp, (uv + d.zw), 0);

		return s * (1.0 / 4);
	}

	half4 frag_downsample(vertexInput i) : SV_Target
	{
		return DownsampleFilter(i.uv);

	}

	half4 frag_upsample(vertexInput i) : SV_Target
	{
		return UpsampleFilter(i.uv);
	}

		ENDHLSL

	SubShader
	{
		ZTest Always Cull Off ZWrite Off

			//0
			Pass
		{
			ZTest Always Cull Off ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_downsample
			ENDHLSL
		}
			//1
			Pass
		{
			ZTest Always Cull Off ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_upsample
			ENDHLSL
		}

	}
}
