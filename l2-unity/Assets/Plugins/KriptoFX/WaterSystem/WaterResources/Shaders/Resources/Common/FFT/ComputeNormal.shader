// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "KriptoFX/Water/ComputeNormal" {


	CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _NormalTex;
		sampler2D _DispTex;
		float4 _DispTex_TexelSize;
		float4 _NormalTex_TexelSize;
		half _Choppines;
		half _WindSpeed;
		half _SizeLog;
		float KW_FFTDomainSize;
		float4 Test4;

		struct appdata_t {
			float4 vertex : POSITION;
			float2 texcoord: TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 uv2 : TEXCOORD1;
			float4 uv3 : TEXCOORD2;
		};

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;

			o.uv2.xy = v.texcoord + float2(_DispTex_TexelSize.x, 0);
			o.uv2.zw = v.texcoord + float2(0, _DispTex_TexelSize.y);
			o.uv3.xy = v.texcoord + float2(-_DispTex_TexelSize.x, 0);
			o.uv3.zw = v.texcoord + float2(0, -_DispTex_TexelSize.y);
			return o;
		}


		float3 ComputeSobelNormal(float2 uv)
		{
			float3 uvStart = (float3(uv, 0)).xzy;
			//float3 nScale =  float3(_Choppines, 1, _Choppines) / 20;
			float texel = _DispTex_TexelSize.x;

			float3 p0 = tex2D(_DispTex, uv).xyz;
			float3 p1 = float3(texel, 0, 0) + tex2D(_DispTex, uv + float2(texel, 0)).xyz;
			float3 p2 = float3(0, 0, texel) + tex2D(_DispTex, uv + float2(0, texel)).xyz;

			float3 b = float3(0, 0, -texel) + tex2D(_DispTex, uv + float2(0, -texel)).xyz;
			float3 l = float3(-texel, 0, 0) + tex2D(_DispTex, uv + float2(-texel, 0)).xyz;

			float3 tr = float3(texel, 0, texel) + tex2D(_DispTex, uv + float2(texel, texel)).xyz;
			float3 rr = float3(texel*2, 0, 0) + tex2D(_DispTex, uv + float2(texel*2, 0)).xyz;
			float3 bb = float3(texel, 0, -texel) + tex2D(_DispTex, uv + float2(texel, -texel)).xyz;

			float3 tt = float3(0, 0, texel*2) + tex2D(_DispTex, uv + float2(0, texel*2)).xyz;
			float3 tl = float3(-texel, 0, texel) + tex2D(_DispTex, uv + float2(-texel, texel)).xyz;

			p0 = uvStart + p0 * 1 + p2*0 + p1*0 + b + l + tt;
			p1 = uvStart + p1 * 1 + tr + rr + bb + p0*0;
			p2 = uvStart + p2 * 1 + tt + tr + p0*0 + tl;

			float3 surfNorm = normalize(cross((p2 - p0), (p1 - p0)));
			surfNorm.y = abs(surfNorm.y);
			return surfNorm;
		}

		float4 frag(v2f i) : SV_Target
		{
			//simple normal
			/*
			float colorScale = 2 * _NormalMapScale64;
			float3 dispL = tex2D(_DispTex, i.uv +  float2(-_DispTex_TexelSize.x, 0)) * colorScale;
			float3 dispR = tex2D(_DispTex, i.uv +  float2(_DispTex_TexelSize.x, 0)) * colorScale;
			float3 dispT = tex2D(_DispTex, i.uv +  float2(0, -_DispTex_TexelSize.x)) * colorScale;
			float3 dispB = tex2D(_DispTex, i.uv +  float2(0, _DispTex_TexelSize.x )) * colorScale;

			float3 diffH = dispR - dispL;
			float3 diffV = dispB - dispT;

			float2 Dx = diffH.xz;
			float2 Dy = diffV.xz;
			float J = (1.0f + Dx.x) * (1.0f + Dy.y) - Dx.y * Dy.x;
			float fold = saturate(J/5);
			float3 normal = normalize(float3(-diffH.y, 1, -diffV.y));
			*/

			float3 uvStart = (float3(i.uv, 0)).xzy;
			float3 right = tex2D(_DispTex, i.uv2.xy).xyz / KW_FFTDomainSize;
			float3 left = tex2D(_DispTex, i.uv3.xy).xyz / KW_FFTDomainSize;
			float3 top = tex2D(_DispTex, i.uv2.zw).xyz / KW_FFTDomainSize;
			float3 down = tex2D(_DispTex, i.uv3.zw).xyz / KW_FFTDomainSize;

			/*float3 diffH = right - left;
			float3 diffV = top - down;
			_SizeLog *= 2;*/

			//fixed normalizedWindSpeed = 1.9 - (_WindSpeed - 2) / 3;
			//float J = (1.0f + diffH.x * _SizeLog) * (1.0f + diffV.z * _SizeLog) * normalizedWindSpeed - diffH.z * diffV.x * _SizeLog * _SizeLog;
			//J = 1 - (saturate(J ));
			right += float3(_DispTex_TexelSize.x, 0, 0);
			left += float3(-_DispTex_TexelSize.x, 0, 0);
			top += float3(0, 0, _DispTex_TexelSize.y);
			down += float3(0, 0, -_DispTex_TexelSize.y);

			float3 norm = normalize(cross(top - down, right - left));

			//norm.y = abs(norm.y) * 2;
			//norm.y = 1;
			//norm = normalize(norm);

			return float4(norm, 1);
			//return float4(norm, J);

		}

		float gaussian_weight(float2 p)
		{
			float sigma = Test4.y;
			float pi = 3.1415927;
			float v = 2.0 * sigma * sigma;
			return exp(-(dot(p, p)) / v) / (pi * v);
		}

		float4 fetch(float2 uv, float2 offset)
		{
			
			float3 n = tex2D(_NormalTex, uv + offset * _NormalTex_TexelSize.xy).xyz;
			return float4(n, 1.0) * gaussian_weight(offset);
		}


		float4 fragLean(v2f i) : SV_Target
		{
			float4 n;
			n = fetch(i.uv, float2(-1, -1));
			n += fetch(i.uv, float2(0, -1));
			n += fetch(i.uv, float2(1, -1));

			n += fetch(i.uv, float2(-1, 0));
			n += fetch(i.uv, float2(0, 0));
			n += fetch(i.uv, float2(1, 0));

			n += fetch(i.uv, float2(-1, 1));
			n += fetch(i.uv, float2(0, 1));
			n += fetch(i.uv, float2(1, 1));

			// Divide by weight sum
			n.xyz /= n.w;

			

			float rlen = 1.0 / saturate(length(n.xyz));
			float gloss = 1.0 / (1.0 + 100 * (rlen - 1.0));
			// Toksvig Factor
			
			return gloss;
		}

		ENDCG

			SubShader
		{
			ZTest Always Cull Off ZWrite Off

				//0
				Pass
			{
				ZTest Always Cull Off ZWrite Off
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				ENDCG
			}
				//1
				Pass
			{
				ZTest Always Cull Off ZWrite Off
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragLean
				ENDCG
			}

		}



	}

