Shader "Hidden/KriptoFX/KWS/Caustic_Pass"
{
	HLSLINCLUDE
	#pragma enable_d3d11_debug_symbols
	//#include "UnityCG.cginc"
	#include "../KWS_WaterVariables.cginc"
	#include "../KWS_WaterPassHelpers.cginc"
	#include "../KWS_CommonHelpers.cginc"
	#include "../../PlatformSpecific/KWS_PlatformSpecificHelpers.cginc"
	#include "../KWS_WaterHelpers.cginc"
	#include "../Shoreline/KWS_Shoreline_Common.cginc"
	#include "../KWS_WaterVertPass.cginc"


	float KW_MeshScale;
	float KW_CausticDepthScale;

	struct appdata_caustic
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f_caustic
	{
		float4 vertex : SV_POSITION;
		float3 oldPos : TEXCOORD0;
		float3 newPos : TEXCOORD1;
	};

	texture2D KW_CausticDepthTex;
	float KW_CausticDepthOrthoSize;
	float3 KW_CausticDepth_Near_Far_Dist;
	float3 KW_CausticDepthPos;
	float3 KW_CausticCameraOffset;
	

	float ComputeCausticOrthoDepth(float3 worldPos)
	{
		float2 depthUV = (worldPos.xz - KW_CausticDepthPos.xz - KW_WaterPosition.xz * 0) / KW_CausticDepthOrthoSize + 0.5;
		float terrainDepth = KW_CausticDepthTex.SampleLevel(sampler_linear_clamp, depthUV, 0).r * KW_CausticDepth_Near_Far_Dist.z - KW_CausticDepth_Near_Far_Dist.y;
		return terrainDepth;
	}

	v2f_caustic vert_caustic(appdata_caustic v)
	{
		v2f_caustic o;

		float3 offset = 0;
		float3 shorelineOffset = 0;
		float2 uv;
		ShorelineData shorelineData;

		float3 worldPos = 0;
		worldPos.xz = KW_CausticCameraOffset.xz + float2(1, -1) * v.vertex.xy * KW_MeshScale;

		offset += ComputeWaterOffset(worldPos) * KW_CausticDepthScale * float3(1, 0.25, 1);
		offset.z *= -1;
		
#if USE_SHORELINE
		shorelineOffset += ComputeBeachWaveOffset(worldPos, shorelineData, -0.35) * 0.25;
		shorelineOffset.z *= -1;
#endif

		offset.xz += shorelineOffset.xz;

#if USE_DEPTH_SCALE
		float terrainDepth = ComputeCausticOrthoDepth(worldPos);
		terrainDepth = clamp(-terrainDepth * 5, 0.0, 20);
		

		float windStr = clamp(KW_WindSpeed, 0.0, 6.0) / 6.0;
		float depthScaleByWindFix = lerp(terrainDepth, terrainDepth * 0.1, windStr);
#else 
		float terrainDepth = 4;
		float depthScaleByWindFix = 4;
#endif
		o.oldPos = v.vertex.xyz;
		v.vertex.xy += (offset.xz / KW_MeshScale) * depthScaleByWindFix;
		v.vertex.xz += (shorelineOffset.xz / KW_MeshScale) * 1; //todo check why xz instead of xy?
		o.newPos = v.vertex.xyz;

		o.vertex = float4(v.vertex.xy, 0, 0.5);
		return o;
	}

	half4 frag_caustic(v2f_caustic i) : SV_Target
	{
		float oldArea = length(ddx(i.oldPos.xyz)) * length(ddy(i.oldPos.xyz));
		float newArea = length(ddx(i.newPos.xyz)) * length(ddy(i.newPos.xyz));

		float color = oldArea / newArea * 0.1;
		return  float4(color.xxx, 1);
	}

		ENDHLSL

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass
		{
			Blend One One

			ZWrite Off
			ZTest Always
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert_caustic
			#pragma fragment frag_caustic

			#pragma multi_compile _ KW_DYNAMIC_WAVES
			#pragma multi_compile _ KW_FLOW_MAP KW_FLOW_MAP_FLUIDS
			#pragma multi_compile _ USE_MULTIPLE_SIMULATIONS
			#pragma multi_compile _ USE_CAUSTIC_FILTERING
			#pragma multi_compile _ USE_DEPTH_SCALE
			#pragma multi_compile _ USE_SHORELINE
			#pragma multi_compile _ USE_FILTERING

			ENDHLSL
		}

	}
}
