Shader "Hidden/KriptoFX/KWS/Underwater_URP"
{
	HLSLINCLUDE
	#pragma enable_d3d11_debug_symbols
	#include "../Common/KWS_WaterVariables.cginc"
	#include "../Common/KWS_WaterPassHelpers.cginc"
	#include "../Common/KWS_CommonHelpers.cginc"
	#include "KWS_PlatformSpecificHelpers.cginc"
	#include "../Common/KWS_WaterHelpers.cginc"
	#include "../Common/CommandPass/KWS_Underwater_Common.cginc"


	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};


	v2f vert(uint vertexID : SV_VertexID)
	{
		v2f o;
		o.vertex = GetTriangleVertexPosition(vertexID);
		o.uv = GetTriangleUV(vertexID);
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		half4 underwaterColor = GetUnderwaterColor(i.uv);
		return underwaterColor;
	}

	half4 fragPostFX(v2f i) : SV_Target
	{
		half4 underwaterColor = GetUnderwaterBluredColor(i.uv);
		return underwaterColor;
	}

	ENDHLSL

	SubShader
	{
		Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
		ZWrite Off

		Stencil{
				Ref 230
				Comp Greater
				Pass keep
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ USE_VOLUMETRIC_LIGHT

			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment fragPostFX

			ENDHLSL
		}

	}
}
