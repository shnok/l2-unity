#ifndef BEAUTIFY_PPSSF_FX
#define BEAUTIFY_PPSSF_FX		

	// Copyright 2020-2021 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D(_FlareTex);
    TEXTURE2D(_OcclusionTex);

	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	float4 _SunPos;
	float3 _SunDir;
	float4 _SunData;	// x = sunIntensity, y = disk size, z = ray difraction, w = ray difraction amount
	float4 _SunCoronaRays1;  // x = length, y = streaks, z = spread, w = angle offset
	float4 _SunCoronaRays2;  // x = length, y = streaks, z = spread, w = angle offset
	float4 _SunGhosts1;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts2;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts3;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
	float4 _SunGhosts4;  // x = reserved, y = size, 2 = pos offset, 3 = brightness
   	float3 _SunHalo;  // x = offset, y = amplitude, z = intensity
   	float4 _SunTint;
	float  _SunFlaresAspectRatio;
	float  _SunOcclusionThreshold;
	#define SUN_TINT_COLOR _SunTint.rgb
	#define OCCLUSION_SPEED _SunTint.a
   	#define OCCLUSION_THRESHOLD _SunOcclusionThreshold

	struct VaryingsSF {
		float4 positionCS : SV_POSITION;
		float2 uv     : TEXCOORD0;
		float2 sunPos : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	VaryingsSF VertSF(AttributesSimple input) {
		VaryingsSF output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = input.positionOS;
		output.positionCS.y *= _ProjectionParams.x * _FlipY;
		output.uv = input.uv.xy;

		float4 clipPos = TransformWorldToHClip(_WorldSpaceCameraPos.xyz - _SunDir.xyz * 1000.0);
		float2 sunPos = float2(clipPos.x, clipPos.y * _ProjectionParams.x) * 0.5 / clipPos.w + 0.5;
		output.sunPos = sunPos;

		return output;
	}

	void rotate(inout float2 uv, float ang) {
		float2 sico;
		sincos(ang, sico.x, sico.y);
		float2 cosi = float2(sico.y, -sico.x);
		uv = float2(dot(cosi, uv), dot(sico, uv));
	}
	
   	float3 sunflare(VaryingsSF input) {

		// general params
		float2 uv = input.uv;
		float2 sunPos = input.sunPos;

		#if BEAUTIFY_SF_OCCLUSION_SIMPLE
   			float depth  = BEAUTIFY_GET_SCENE_DEPTH_01(sunPos);
			if (depth < 1.0) return 0;
			const float occlusion = 1.0;
		#elif BEAUTIFY_SF_OCCLUSION_SMOOTH
			float occlusion = SAMPLE_TEXTURE2D(_OcclusionTex, sampler_LinearRepeat, float2(0,0)).r;
			if (occlusion <= 0.02) return 0.0.xxx;
		#else
			const float occlusion = 1.0;
		#endif

   		float2 grd = uv - sunPos;
		float aspectRatio = _SunFlaresAspectRatio;
   		grd.y *= aspectRatio; 
   		float len = length(grd);

   		// sun disk
   		float s0 = pow( 1.0 + saturate(_SunData.y - len), 75) - 1.0;
        
   		// corona rays
		float gang = _SunPos.w; //atan2(0.5 - sunPos.y, sunPos.x - 0.5);
   		float ang = atan2(grd.y, grd.x) + gang;
   		float ray1 = _SunCoronaRays1.z + abs(_SunCoronaRays1.x * cos(_SunCoronaRays1.w + ang * _SunCoronaRays1.y));	// design
   		ray1 *= pow( 1.0 + len, 1.0/_SunCoronaRays1.x);	
   		s0 += 1.0 / ray1;

   		float ray2 = _SunCoronaRays2.z + abs(_SunCoronaRays2.x * sin(_SunCoronaRays2.w + ang * _SunCoronaRays2.y));	// design
   		ray2 *= pow( 1.0 + len, 1.0/_SunCoronaRays2.x);	
   		s0 += 1.0 / ray2;
   		
   		s0 *= _SunData.x;
   		
   		float3 flare = s0.xxx;
		
		#if BEAUTIFY_SF_USE_GHOSTS // defined(UNITY_SINGLE_PASS_STEREO) && !defined(UNITY_STEREO_INSTANCING_ENABLED) && !defined(UNITY_STEREO_MULTIVIEW_ENABLED)
   		// ghosts circular (not compatible with XR due to how projection works)

   		float2 ghost1Pos  = 1.0 - sunPos;
   		grd = uv - ghost1Pos + (ghost1Pos - 0.5) * _SunGhosts1.z;
		grd.y *= aspectRatio;

		float g0 = saturate(_SunGhosts1.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare += g0 * _SunGhosts1.w / len;

   		float2 ghost2Pos  = 1.0 - sunPos;
   		grd = uv - ghost2Pos + (ghost2Pos - 0.5) * _SunGhosts2.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts2.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts2.w / len;

   		float2 ghost3Pos  = 1.0 - sunPos;
   		grd = uv - ghost3Pos + (ghost3Pos - 0.5) * _SunGhosts3.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts3.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts3.w / len;

   		float2 ghost4Pos  = 1.0 - sunPos;
   		grd = uv - ghost4Pos + (ghost4Pos - 0.5) * _SunGhosts4.z;
		grd.y *= aspectRatio;
		g0 = saturate(_SunGhosts4.y / length(grd)); 
		g0 = pow(g0, 12);
   		flare +=  g0 * _SunGhosts4.w / len;

   		#endif

		// light rays
		float2 uv2 = uv - sunPos;
		float clen = length(uv2);
		rotate(uv2, gang);
		uv2.x *= aspectRatio;
		uv2.x *= 0.1;
		uv2 /= len;
		float lr = saturate(SAMPLE_TEXTURE2D(_FlareTex, sampler_LinearRepeat, uv2 + _SunPos.zz).r - _SunData.w);
		float3 rays = lr * sin(float3(len, len + 0.1, len + 0.2) * 3.1415927);
		float atten = pow(1.0 + clen, 13.0);
		rays *= _SunData.z / atten;
		flare += rays;

		// halo
		float hlen = clamp( (len - _SunHalo.x) * _SunHalo.y, 0, 3.1415927);
		float3 halo = pow(sin(float3(hlen, hlen + 0.1, hlen + 0.2)), 12.0.xxx);
		halo *= _SunHalo.z / atten;
		flare += halo; 
		
		return max(0, flare * SUN_TINT_COLOR * occlusion);
   	}  
   	
  	float4 FragSF (VaryingsSF i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        //i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

   		return float4(sunflare(i), 1.0);
   	}  

  	float4 FragSFAdditive (VaryingsSF i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 stereoUV = UnityStereoTransformScreenSpaceTex(i.uv);

  		float4 p = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, stereoUV);
   		return p + float4(sunflare(i), 1.0);
   	}  

    float GetDepth(float2 sunPos, float2 offset) {
        float2 pos = saturate(sunPos + offset * _MainTex_TexelSize.xy);
        return BEAUTIFY_GET_SCENE_DEPTH_01(pos);
    }

   	float OcclusionTest(float2 sunPos) {

		float  depth1 = GetDepth(sunPos, float2(-1, -1));
        float  depth2 = GetDepth(sunPos, float2( 1,  1));
        float  depth3 = GetDepth(sunPos, float2( 3,  3));
        float  depth4 = GetDepth(sunPos, float2(-3, -3));
		float occlusion = (depth1 + depth2 + depth3 + depth4) * 0.25;
		occlusion *= step(OCCLUSION_THRESHOLD, occlusion);
        return occlusion;
    }

  	float4 FragSFOcclusion (VaryingsSF input) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float occlusion = OcclusionTest(input.sunPos);
        #if BEAUTIFY_SF_OCCLUSION_INIT
            return float4(occlusion, occlusion, occlusion, 1.0);
        #else
            return float4(occlusion, occlusion, occlusion, unity_DeltaTime.x * OCCLUSION_SPEED);
        #endif
   	}

#endif