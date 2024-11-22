#ifndef BEAUTIFY_PPSOUTLINE
#define BEAUTIFY_PPSOUTLINE
	// Copyright 2016-2021 Ramiro Oliva (Kronnect) - All Rights Reserved.
	
	#include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	float4 _MainTex_TexelSize;
	float4 _MainTex_ST;

	TEXTURE2D_X_FLOAT(_OutlineDepth);
	TEXTURE2D_X_FLOAT(_OutlineObjectId);

	float4 _Outline;
	#define OUTLINE_EDGE_THRESHOLD _Outline.a

	float4 _OutlineData;
	#define OUTLINE_INTENSITY_MULTIPLIER _OutlineData.x
	#define OUTLINE_DISTANCE_FADE _OutlineData.y
	#define OUTLINE_MIN_DEPTH_THRESHOLD _OutlineData.z
    #define OUTLINE_MIN_SATURATION_THRESHOLD _OutlineData.w

	half   _BlurScale;
	#define OUTLINE_MIN_SEPARATION _OutlineData.z
   
	struct VaryingsOutline {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsOutline VertOutline(AttributesSimple input) {
	    VaryingsOutline output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = input.positionOS;
		output.positionCS.y *= _ProjectionParams.x * _FlipY;
        output.uv = input.uv;
        return output;
	}

	float OutlinePass(VaryingsOutline i) {

		float3 uvInc      = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);

		#if BEAUTIFY_DEPTH_FADE || !BEAUTIFY_OUTLINE_SOBEL
			#if BEAUTIFY_OUTLINE_CUSTOM_DEPTH || BEAUTIFY_OUTLINE_OBJECT_ID
				float  depth      = BEAUTIFY_GET_CUSTOM_DEPTH_01(_OutlineDepth, i.uv);
				float  sceneDepth = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
				if (sceneDepth < depth * 0.999) return 0;
			#else
				float  depth      = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
				if (depth >= 1.0) return 0;
			#endif
		#endif

		float outline = 0;

		#if BEAUTIFY_OUTLINE_OBJECT_ID && !BEAUTIFY_OUTLINE_SOBEL
			float objS = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv - uvInc.zy).x;
		   	float objN = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv + uvInc.zy).x;
			float objW = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv - uvInc.xz).x;
   			float objE = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv + uvInc.xz).x;
			float  maxObj   = max( max(objS, objN), max(objW, objE) );
			float  minObj   = min( min(objS, objN), min(objW, objE) );
			float  objDiff  = maxObj - minObj;
			outline = objDiff > 0.01;

			// check object is big enough
			#if BEAUTIFY_OUTLINE_MIN_SEPARATION
				uvInc *= OUTLINE_MIN_SEPARATION;
				float objS2 = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv - uvInc.zy).x;
			   	float objN2 = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv + uvInc.zy).x;
				if (abs(objN2 - objN) > 0.01 && abs(objS2 - objS) > 0.01) outline *= 0.25 / OUTLINE_INTENSITY_MULTIPLIER;
				float objW2 = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv - uvInc.xz).x;
   				float objE2 = SAMPLE_TEXTURE2D_X(_OutlineObjectId, sampler_PointClamp, i.uv + uvInc.xz).x;
				if (abs(objE2 - objE) > 0.01 && abs(objW2 - objW) > 0.01) outline *= 0.25 / OUTLINE_INTENSITY_MULTIPLIER;
			#endif

		#else
			float3 rgbS   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.zy).rgb;
   			float3 rgbN   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.zy).rgb;
			float3 rgbW   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.xz).rgb;
   			float3 rgbE   = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.xz).rgb;

			#if BEAUTIFY_OUTLINE_CUSTOM_DEPTH || !BEAUTIFY_OUTLINE_SOBEL
				#if BEAUTIFY_OUTLINE_CUSTOM_DEPTH
					float  depthS     = BEAUTIFY_GET_CUSTOM_DEPTH_01(_OutlineDepth, i.uv - uvInc.zy);
					float  depthW     = BEAUTIFY_GET_CUSTOM_DEPTH_01(_OutlineDepth, i.uv - uvInc.xz);
					float  depthE     = BEAUTIFY_GET_CUSTOM_DEPTH_01(_OutlineDepth, i.uv + uvInc.xz);		
					float  depthN     = BEAUTIFY_GET_CUSTOM_DEPTH_01(_OutlineDepth, i.uv + uvInc.zy);
				#else
					float  depthS     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv - uvInc.zy);
					float  depthW     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv - uvInc.xz);
					float  depthE     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv + uvInc.xz);		
					float  depthN     = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv + uvInc.zy);
				#endif
				float  maxDepth   = max(depth, max(max(depthS, depthN), max(depthW, depthE)));
				float  minDepth   = min(depth, min(min(depthS, depthN), min(depthW, depthE)));
				float  depthDiff  = maxDepth - minDepth;
	   			float3 normalNW   = getNormal(depth, depthN, depthW, uvInc.zy, float2(-uvInc.x, -uvInc.z));
   				float3 normalSE   = getNormal(depth, depthS, depthE, -uvInc.zy,  uvInc.xz);
				float  dnorm      = dot(normalNW, normalSE);
				float satN        = getSaturation(rgbN);
	            float satS        = getSaturation(rgbS);
		        float satW        = getSaturation(rgbW);
			    float satE        = getSaturation(rgbE);
				float maxSat      = max( max(satN, satS), max(satW, satE) );
	            float minSat      = min( min(satN, satS), min(satW, satE) );
		        float dSat        = maxSat - minSat;
				outline = dSat > OUTLINE_MIN_SATURATION_THRESHOLD && dnorm < OUTLINE_EDGE_THRESHOLD && depthDiff > OUTLINE_MIN_DEPTH_THRESHOLD;
			#else
				float3 rgbSW  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv - uvInc.xy).rgb;
				float3 rgbNE  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + uvInc.xy).rgb;
				float3 rgbSE  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + float2( uvInc.x, -uvInc.y)).rgb;
				float3 rgbNW  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + float2(-uvInc.x,  uvInc.y)).rgb;
				float3 gx  = rgbSW * -1.0;
 				       gx += rgbSE *  1.0;
			       	   gx += rgbW  * -2.0;
				       gx += rgbE  *  2.0;
				       gx += rgbNW * -1.0;
				       gx += rgbNE *  1.0;
				float3 gy  = rgbSW * -1.0;
				       gy += rgbS  * -2.0;
				       gy += rgbSE * -1.0;
					   gy += rgbNW *  1.0;
				       gy += rgbN  *  2.0;
				       gy += rgbNE *  1.0;
				outline = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
   			#endif
		#endif

		#if BEAUTIFY_DEPTH_FADE
			float factor = max(0, (OUTLINE_DISTANCE_FADE - depth) / OUTLINE_DISTANCE_FADE);
			outline *= factor;
		#endif

		return outline;
	}
	
	float4 fragOutline (VaryingsOutline i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
   		float outline = OutlinePass(i);
  		return outline;
	}


	VaryingsCross VertBlur(AttributesSimple v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x * _FlipY;
    	o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_GAUSSIAN_UV(o)

    	return o;
	}
	
	half4 FragBlur (VaryingsCross i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i)

		half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv) * 0.2270270270
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2)) * 0.3162162162
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4)) * 0.0702702703;
   		return pixel;
	}	

	half4 FragCopy (VaryingsSimple i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		half outline = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv).r;
		half4 color = half4(_Outline.rgb, _Outline.a * outline);
		color *= OUTLINE_INTENSITY_MULTIPLIER;
		color.a = saturate(color.a);
		return color;
	}

#endif