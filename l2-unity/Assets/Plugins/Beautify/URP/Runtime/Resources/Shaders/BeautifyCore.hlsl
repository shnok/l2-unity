#ifndef BEAUTIFY_CORE_FX
#define BEAUTIFY_CORE_FX

#pragma warning (disable : 3571)

    TEXTURE2D_X(_MainTex);
    SAMPLER(sampler_MainTex);
    float4 _MainTex_TexelSize;

    #include "BeautifyACESFitted.hlsl"
    #include "BeautifyAGX.hlsl"
    #include "BeautifyColorTemp.hlsl"
    #include "BeautifyDistortion.hlsl"
    #include "BeautifyCommon.hlsl"

	// Copyright 2020-2021 Ramiro Oliva (Kronnect) - All Rights Reserved.
    float4 _CompareParams;
    TEXTURE2D_X(_CompareTex);
    TEXTURE2D_X(_BloomTex);
    TEXTURE2D_X(_ScreenLum);
    TEXTURE2D(_OverlayTex);
    TEXTURE2D(_LUTTex);
    TEXTURE3D(_LUT3DTex);
    TEXTURE2D_X(_EAHist);
    TEXTURE2D_X(_EALumSrc);
    TEXTURE2D(_BlueNoise);
    float4 _BlueNoise_TexelSize;
    TEXTURE2D_X(_BlurTex);
    TEXTURE2D(_BlurMask);

    TEXTURE2D_X(_MiniViewTex);
    float4 _MiniViewRect;
    float4 _MiniViewBlend;

	float4 _Params;
	float4 _Sharpen;
	float4 _Bloom;
    float4 _Dirt;    // x = brightness based, y = intensity, z = threshold, w = bloom contribution    
    float4 _FXColor;
    float4 _ColorBoost;
    float4 _TintColor;
    float4 _Purkinje;
    float4 _EyeAdaptation;
    float4 _BokehData;
    float4 _BokehData2;
    float3 _BokehData3;

    float4 _Outline;
	#define OUTLINE_EDGE_THRESHOLD _Outline.a

	float4 _OutlineData;
	#define OUTLINE_INTENSITY_MULTIPLIER _OutlineData.x
	#define OUTLINE_DISTANCE_FADE _OutlineData.y
	#define OUTLINE_MIN_DEPTH_THRESHOLD _OutlineData.z
    #define OUTLINE_MIN_SATURATION_THRESHOLD _OutlineData.w

    float3 _ColorTemp;
    float4 _NightVision;
    float2 _NightVisionDepth;
    float4 _LUTTex_TexelSize;
    float2 _LUT3DParams;

    #if BEAUTIFY_VIGNETTING || BEAUTIFY_VIGNETTING_MASK
        float4 _Vignetting;
        float4 _VignettingData;
        float  _VignettingData2;
        #define VIGNETTE_ASPECT_RATIO_Y _VignettingData.z
        #define VIGNETTE_CENTER _VignettingData.xy
        #define VIGNETTE_OUTER_RING _VignettingData.w
        #define VIGNETTE_FADE_AND_BLINK _Purkinje.z
        #define VIGNETTE_INNER_RING _Purkinje.w
        #define VIGNETTE_ASPECT_RATIO_X _VignettingData2
        TEXTURE2D_X(_VignettingMask);
    #endif

    #if BEAUTIFY_FRAME || BEAUTIFY_FRAME_MASK
        TEXTURE2D_X(_FrameMask);
        float4 _Frame;
        float4 _FrameData;
    #endif

    #if BEAUTIFY_DOF_TRANSPARENT || BEAUTIFY_DEPTH_OF_FIELD
        TEXTURE2D_X(_DoFTex);
        float4 _DoFTex_TexelSize;
    #endif
    #if BEAUTIFY_DOF_TRANSPARENT
        TEXTURE2D_X(_DoFTransparentDepth);
        //TEXTURE2D_X(_DofExclusionTexture;
    #endif

	#if defined(BEAUTIFY_EDGE_AA)
		float4 _AntialiasData;
		#define ANTIALIAS_STRENGTH _AntialiasData.x
		#define ANTIALIAS_THRESHOLD _AntialiasData.y
		#define ANTIALIAS_DEPTH_ATTEN _AntialiasData.z
		#define ANTIALIAS_CLAMP _AntialiasData.w
	#endif



 	struct VaryingsBeautify {
    	float4 positionCS : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsBeautify VertBeautify(AttributesSimple input) {
	    VaryingsBeautify output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = input.positionOS;
        output.positionCS.y *= _ProjectionParams.x * _FlipY;
        output.uv = input.uv.xy;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(output)
    	return output;
	}


	float getRandom(float2 uv) {
		return frac(sin(_Time.y + dot(uv, float2(12.9898, 78.233)))* 43758.5453);
	}


    float getCoc(VaryingsBeautify i) {
    #if BEAUTIFY_DOF_TRANSPARENT
        float depthTex = BEAUTIFY_GET_CUSTOM_DEPTH_01(_DoFTransparentDepth, i.uv);
        //float exclusionDepth = DecodeFloatRGBA(SAMPLE_TEXTURE2D_LOD(_DofExclusionTexture, sampler_DoFExclusionTexture, i.uv, 0));
        float depth  = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
        depth = min(depth, depthTex);
        //if (exclusionDepth < depth) return 0;
        depth *= _ProjectionParams.z;
    #else
        float depth  = BEAUTIFY_GET_SCENE_DEPTH_EYE(i.uv);
    #endif
        float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
        return min(_BokehData3.z, 0.5 * _BokehData.y * xd/depth);   // radius of CoC
    }

   	float3 tmoFilmicACES(float3 x) {
		const float A = 2.51;
		const float B = 0.03;
		const float C = 2.43;
		const float D = 0.59;
		const float E = 0.14;
		return (x * (A * x + B) ) / (x * (C * x + D) + E);
	}

	void beautifyPass(VaryingsBeautify i, inout float3 rgbM) {
        
        #if BEAUTIFY_ACES_TONEMAP || BEAUTIFY_AGX_TONEMAP || BEAUTIFY_ACES_FITTED_TONEMAP
             rgbM = clamp(rgbM, 0, _FXColor.z);
        #endif

        float2 uv = i.uv;
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float  depthS     = BEAUTIFY_GET_SCENE_DEPTH_01(uvS);
		float  depthW     = BEAUTIFY_GET_SCENE_DEPTH_01(uvW);
		float  depthE     = BEAUTIFY_GET_SCENE_DEPTH_01(uvE);		
		float  depthN     = BEAUTIFY_GET_SCENE_DEPTH_01(uvN);

		// neighbour depth analysis
		float  maxDepth   = max(depthN, depthS);
		       maxDepth   = max(maxDepth, depthW);
		       maxDepth   = max(maxDepth, depthE);
		float  minDepth   = min(depthN, depthS);
		       minDepth   = min(minDepth, depthW);
		       minDepth   = min(minDepth, depthE);
		float  dDepth     = maxDepth - minDepth + 0.000001;

        // luma analysis
		float3 rgbS       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvS).rgb;
	    float3 rgbW       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvW).rgb;
	    float3 rgbE       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvE).rgb;
	    float3 rgbN       = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uvN).rgb;
	    
    	float  lumaN      = getLuma(rgbN);
    	float  lumaE      = getLuma(rgbE);
    	float  lumaW      = getLuma(rgbW);
    	float  lumaS      = getLuma(rgbS);
		
    	float  maxLuma    = max(lumaN,lumaS);
    	       maxLuma    = max(maxLuma, lumaW);
#if !BEAUTIFY_TURBO
    	       maxLuma    = max(maxLuma, lumaE);
#endif
	    float  minLuma    = min(lumaN,lumaS);
	           minLuma    = min(minLuma, lumaW);
#if !BEAUTIFY_TURBO
	           minLuma    = min(minLuma, lumaE);
#endif
               minLuma   -= 0.000001;

#if BEAUTIFY_EDGE_AA
   if (dDepth > ANTIALIAS_THRESHOLD) {
	        float2 n = float2(lumaN - lumaS, lumaW - lumaE);
			n *= ANTIALIAS_STRENGTH;
			n = clamp(n, -ANTIALIAS_CLAMP, ANTIALIAS_CLAMP);
			float antialiasDepthAtten = 1.0 - saturate(depthN * ANTIALIAS_DEPTH_ATTEN);
            n *= _MainTex_TexelSize.xy * antialiasDepthAtten;
            float3 rgbA = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - n * 0.166667).rgb + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + n * 0.166667).rgb;
            float3 rgbB = 0.25 * (rgbA + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - n * 0.5).rgb + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + n * 0.5).rgb);
            float lumaB = getLuma(rgbB);
	        if (lumaB < minLuma || lumaB > maxLuma) rgbB = rgbA * 0.5;
            rgbM = rgbB;
   }
#endif

		float  lumaM      = getLuma(rgbM);

		// daltonize
        #if BEAUTIFY_COLOR_TWEAKS
		float3 rgb0       = 1.0.xxx - saturate(rgbM.rgb);
		       rgbM.r    *= 1.0 + rgbM.r * rgb0.g * rgb0.b * _Params.y;
			   rgbM.g    *= 1.0 + rgbM.g * rgb0.r * rgb0.b * _Params.y;
			   rgbM.b    *= 1.0 + rgbM.b * rgb0.r * rgb0.g * _Params.y;	
			   rgbM      *= lumaM / (getLuma(rgbM) + 0.0001);
        #endif

        // sharpen
#if BEAUTIFY_TURBO
        const float  lumaDepth  = 1.0;
#else
		float  lumaDepth  = saturate( _Sharpen.y / dDepth);
#endif

	    float  lumaPower  = 2.0 * lumaM - minLuma - maxLuma;
		float  lumaAtten  = saturate(_Sharpen.w / (maxLuma - minLuma));
#if BEAUTIFY_TURBO
        const float depthClamp = 1.0;
#else
		float  depthClamp = abs(depthW - _Params.z) < _Params.w;
#endif

#if BEAUTIFY_SHARPEN
		       rgbM      *= 1.0 + clamp(lumaPower * lumaAtten * lumaDepth * _Sharpen.x, -_Sharpen.z, _Sharpen.z) * depthClamp;
#endif

        #if BEAUTIFY_DEPTH_OF_FIELD || BEAUTIFY_DOF_TRANSPARENT
            float4 dofPix     = SAMPLE_TEXTURE2D_X(_DoFTex, sampler_LinearClamp, i.uv);
            #if UNITY_COLORSPACE_GAMMA
               dofPix.rgb = LINEAR_TO_GAMMA(dofPix.rgb);
            #endif
            if (_DoFTex_TexelSize.z < _MainTex_TexelSize.z) {
                float  CoC = getCoc(i) / COC_BASE;
                dofPix.a   = lerp(CoC, dofPix.a, _DoFTex_TexelSize.z / _MainTex_TexelSize.z);
            }
            rgbM = lerp(rgbM, dofPix.rgb, saturate(dofPix.a * COC_BASE));
        #endif

        #if BEAUTIFY_NIGHT_VISION || (BEAUTIFY_OUTLINE && !BEAUTIFY_OUTLINE_SOBEL)
		    float3 uvNormalDisp    = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
                 float depth       = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
                 float3 normalNW   = getNormal(depth, depthN, depthW, uvNormalDisp.zy, float2(-uvNormalDisp.x, -uvNormalDisp.z));
        #endif

        #if BEAUTIFY_OUTLINE
            #if !BEAUTIFY_OUTLINE_SOBEL
                float3 normalSE   = getNormal(depth, depthS, depthE, -uvNormalDisp.zy,  uvNormalDisp.xz);
                float  dnorm      = dot(normalNW, normalSE);
                float  satN       = getSaturation(rgbN);
                float  satS       = getSaturation(rgbS);
                float  satW       = getSaturation(rgbW);
                float  satE       = getSaturation(rgbE);
                float  maxSat     = max( max(satN, satS), max(satW, satE) );
                float  minSat     = min( min(satN, satS), min(satW, satE) );
                float  dSat       = maxSat - minSat;
                rgbM              = lerp(rgbM, _Outline.rgb, (float)(dnorm  < OUTLINE_EDGE_THRESHOLD && dDepth > OUTLINE_MIN_DEPTH_THRESHOLD && dSat > OUTLINE_MIN_SATURATION_THRESHOLD));
            #else
                float3 uvInc = float3(_MainTex_TexelSize.x, _MainTex_TexelSize.y, 0);
                float3 rgbSW = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv - uvInc.xy).rgb;    // was tex2Dlod
                float3 rgbNE = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + uvInc.xy).rgb;
                float3 rgbSE = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + float2( uvInc.x, -uvInc.y)).rgb;
                float3 rgbNW = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv + float2(-uvInc.x,  uvInc.y)).rgb;
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
                float olColor = (length(gx * gx + gy * gy) - _Outline.a) > 0.0;
                rgbM = lerp(rgbM, _Outline.rgb, olColor); 
            #endif
        #endif

		#if UNITY_COLORSPACE_GAMMA && (BEAUTIFY_BLOOM || BEAUTIFY_NIGHT_VISION || BEAUTIFY_THERMAL_VISION || BEAUTIFY_DIRT || BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE || BEAUTIFY_ACES_TONEMAP || BEAUTIFY_AGX_TONEMAP || BEAUTIFY_ACES_FITTED_TONEMAP)
	    	rgbM = GAMMA_TO_LINEAR(rgbM);
		#endif

		#if BEAUTIFY_BLOOM
    		rgbM += SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv).rgb * _Bloom.xxx;
		#endif

        #if BEAUTIFY_DIRT
            float3 scrLum = SAMPLE_TEXTURE2D_X(_ScreenLum, sampler_LinearClamp, i.uv).rgb;
            #if BEAUTIFY_BLOOM
                scrLum *= _Dirt.www;
            #endif

            #if defined(UNITY_SINGLE_PASS_STEREO)
                float2 dirtUV = float2(uv.x * 2.0, uv.y); // TODO: ?? should ignore Single Pass
            #else
                float2 dirtUV = uv;
            #endif
            float4 dirt = SAMPLE_TEXTURE2D(_OverlayTex, sampler_LinearRepeat, dirtUV);
            rgbM       += saturate(0.5.xxx - _Dirt.zzz + scrLum) * dirt.rgb * _Dirt.y;
        #endif

        #if BEAUTIFY_NIGHT_VISION
               rgbM      *= saturate( (_NightVisionDepth.x - BEAUTIFY_GET_SCENE_DEPTH_EYE(uv)) * _NightVisionDepth.y ); 
   	 	       lumaM      = getLuma(rgbM);	// updates luma
   		float  nvbase     = saturate(normalNW.z - 0.8); // minimum ambient self radiance (useful for pitch black)
   			   nvbase    += lumaM;						// adds current lighting
   			   nvbase    *= nvbase * (0.5 + nvbase);	// increase contrast
   			   rgbM	      = nvbase * _NightVision.rgb;
   			   rgbM      *= frac(floor(uv.y * _MainTex_TexelSize.w)*0.25)>0.4;	// scan lines
   			   rgbM	     *= 1.0 + getRandom(uv) * 0.3 - 0.15;				// noise
	 	#endif

   	 	#if BEAUTIFY_THERMAL_VISION
   	 	       lumaM      = getLuma(rgbM);	// updates luma
    	float3 tv0 	      = lerp(float3(0.0,0.0,1.0), float3(1.0,1.0,0.0), lumaM * 2.0);
    	float3 tv1  	  = lerp(float3(1.0,1.0,0.0), float3(1.0,0.0,0.0), lumaM * 2.0 - 1.0);
    		   rgbM       = lerp(tv0, tv1, (float)(lumaM >= 0.5));
   			   rgbM      *= 0.2 + frac(floor(uv.y * _MainTex_TexelSize.w)*0.25)>_NightVision.a;	// scan lines
   			   rgbM		 *= 1.0 + getRandom(uv) * 0.2 - 0.1;					// noise
	 	#endif

        #if BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE
            float4 avgLum = SAMPLE_TEXTURE2D_X(_EAHist, sampler_LinearClamp, 0.5.xx);
        #endif

        #if BEAUTIFY_EYE_ADAPTATION
            float srcLum  = SAMPLE_TEXTURE2D_X(_EALumSrc, sampler_LinearClamp, 0.5.xx).r;
            float  diff   = srcLum / (avgLum.r + 0.0001); // diff > 1 scene becomes brighter
            // uncomment to debug
/*
            if (i.uv.y< 0.1 && i.uv.y>0.05) {
                rgbM = i.uv.x * 2.0 < avgLum.r ? 1: 0;
                if (avgLum.r > 1) {
                    rgbM.gb = 0;
                }
                return;
            }
            if (i.uv.y< 0.05) {
                rgbM = i.uv.x * 2.0 < srcLum ? 1: 0;
                if (srcLum > 1) {
                    rgbM.gb = 0;
                }
                return;
            }
*/
            float eaLuma = getLuma(rgbM);
            float pixLum  = max(0,log(1.0 + eaLuma));
            diff   = pow(pixLum / (avgLum.r + 0.0001), abs(diff-1.0));
            diff   = clamp(diff, _EyeAdaptation.x, _EyeAdaptation.y);
            rgbM   = rgbM * diff;
        #endif

        #if BEAUTIFY_AGX_TONEMAP
             rgbM *= _FXColor.r;
             rgbM  = AGXFitted(rgbM); // already applies gamma correction
             rgbM *= _FXColor.g;
             rgbM  = saturate(rgbM);
        #else
            #if BEAUTIFY_ACES_FITTED_TONEMAP
                rgbM *= _FXColor.r;
                rgbM  = ACESFitted(rgbM);
                rgbM *= _FXColor.g;
            #endif
            #if BEAUTIFY_ACES_TONEMAP
                 rgbM *= _FXColor.r;
                 rgbM  = tmoFilmicACES(rgbM);
                 rgbM *= _FXColor.g;
            #endif
    		#if UNITY_COLORSPACE_GAMMA && (BEAUTIFY_BLOOM || BEAUTIFY_NIGHT_VISION || BEAUTIFY_THERMAL_VISION || BEAUTIFY_DIRT || BEAUTIFY_EYE_ADAPTATION || BEAUTIFY_PURKINJE || BEAUTIFY_ACES_FITTED_TONEMAP || BEAUTIFY_ACES_TONEMAP)
        		rgbM    = LINEAR_TO_GAMMA(rgbM);
		    #endif
        #endif

        #if BEAUTIFY_LUT || BEAUTIFY_3DLUT
            #if !UNITY_COLORSPACE_GAMMA
                rgbM = LINEAR_TO_GAMMA(rgbM);
            #endif
            #if BEAUTIFY_3DLUT
				float3 xyz = rgbM * _LUT3DParams.y + _LUT3DParams.x;
				float3 lut = SAMPLE_TEXTURE3D(_LUT3DTex, sampler_LinearClamp, xyz).rgb;
            #else
	            float3 lutST = float3(_LUTTex_TexelSize.x, _LUTTex_TexelSize.y, _LUTTex_TexelSize.w - 1);
                float3 lookUp = saturate(rgbM) * lutST.zzz;
                lookUp.xy = lutST.xy * (lookUp.xy + 0.5);
                float slice = floor(lookUp.z);
                lookUp.x += slice * lutST.y;
                float2 lookUpNextSlice = float2(lookUp.x + lutST.y, lookUp.y);
                float3 lut = lerp(SAMPLE_TEXTURE2D(_LUTTex, sampler_LinearClamp, lookUp.xy).rgb, SAMPLE_TEXTURE2D(_LUTTex, sampler_LinearClamp, lookUpNextSlice).rgb, lookUp.z - slice);
            #endif
            rgbM = lerp(rgbM, lut, _FXColor.a);
            
            #if !UNITY_COLORSPACE_GAMMA
                rgbM = GAMMA_TO_LINEAR(rgbM);
            #endif
        #endif


 		// sepia
        #if BEAUTIFY_COLOR_TWEAKS
		float3 sepia      = float3(
   		            	   			dot(rgbM, float3(0.393, 0.769, 0.189)),
               						dot(rgbM, float3(0.349, 0.686, 0.168)),
               						dot(rgbM, float3(0.272, 0.534, 0.131))
               					  );
        rgbM      = lerp(rgbM, sepia, _Params.x);
        #endif

        // saturate
        float maxComponent = max(rgbM.r, max(rgbM.g, rgbM.b));
        float minComponent = min(rgbM.r, min(rgbM.g, rgbM.b));
        float sat = saturate(maxComponent - minComponent);
        rgbM *= 1.0 + _ColorBoost.z * (1.0 - sat) * (rgbM - getLuma(rgbM));
        rgbM = lerp(rgbM, rgbM * _TintColor.rgb, _TintColor.a);
        rgbM = (rgbM - 0.5.xxx) * _ColorBoost.y + 0.5.xxx;
        rgbM *= _ColorBoost.x;

        #if BEAUTIFY_COLOR_TWEAKS
            lumaM    = getLuma(rgbM);
            float3 kelvin = KelvinToRGB(_ColorTemp.x);
            rgbM = lerp(rgbM, rgbM * kelvin, _ColorTemp.y);
            //rgbM *= getLuma(rgbM) / lumaM; // energy preservation (optional)
        #endif

        #if BEAUTIFY_PURKINJE
              lumaM    = getLuma(rgbM);
        float3 shifted  = saturate(float3(lumaM / (1.0 + _Purkinje.x * 1.14), lumaM, lumaM * (1.0 + _Purkinje.x * 2.99)));
              rgbM     = lerp(shifted, rgbM, saturate(exp(avgLum.g) - _Purkinje.y));
        #endif

#if BEAUTIFY_VIGNETTING
            float2 vd = float2((uv.x - VIGNETTE_CENTER.x) * VIGNETTE_ASPECT_RATIO_X, (uv.y - VIGNETTE_CENTER.y) * VIGNETTE_ASPECT_RATIO_Y);
            float3 vcolor = lerp(_Vignetting.rgb, rgbM, saturate( saturate((dot(vd, vd) - VIGNETTE_OUTER_RING) / (VIGNETTE_INNER_RING - VIGNETTE_OUTER_RING) ) - VIGNETTE_FADE_AND_BLINK)) ;
            rgbM = lerp(rgbM, vcolor, _Vignetting.a);
#elif BEAUTIFY_VIGNETTING_MASK
            float2 vd = float2((uv.x - VIGNETTE_CENTER.x) * VIGNETTE_ASPECT_RATIO_X, (uv.y - VIGNETTE_CENTER.y) * VIGNETTE_ASPECT_RATIO_Y);
            float  vmask = SAMPLE_TEXTURE2D_X(_VignettingMask, sampler_LinearClamp, uv).a;
            rgbM = lerp(rgbM, lumaM * _Vignetting.rgb, saturate(VIGNETTE_FADE_AND_BLINK + vmask * _Vignetting.a * saturate(VIGNETTE_OUTER_RING * 4.0 * dot(vd, vd))));
#endif

  		#if BEAUTIFY_FRAME
			float2 frameIntensity = saturate( (abs(i.uv.xy - 0.5) - _FrameData.xz) * _FrameData.yw);
			rgbM       = lerp(rgbM, _Frame.rgb, max(frameIntensity.x, frameIntensity.y));
	    #elif BEAUTIFY_FRAME_MASK
			float4 frameMask  = SAMPLE_TEXTURE2D_X(_FrameMask, sampler_LinearClamp, uv);
			rgbM = lerp(rgbM, frameMask.rgb * _Frame.rgb, frameMask.a * _Frame.a);
   		#endif

#if BEAUTIFY_DITHER
    #if BEAUTIFY_TURBO
		float3 dither     = dot(float2(171.0, 231.0), i.uv * _MainTex_TexelSize.zw).xxx;
		       dither     = frac(dither / float3(103.0, 71.0, 97.0)) - 0.5.xxx;
		       rgbM      += dither * _ColorBoost.w;
    #else
        float2 noiseUV = i.uv * _BlueNoise_TexelSize.xy * _MainTex_TexelSize.zw;
        float3 blueNoise = SAMPLE_TEXTURE2D(_BlueNoise, sampler_PointRepeat, noiseUV).rgb;
        rgbM += (blueNoise - 0.5.xxx) * _ColorBoost.w * saturate(1.0 - lumaM);
    #endif
        rgbM = max(0.0.xxx, rgbM);
#endif

	}

	float4 FragBeautify (VaryingsBeautify i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv     = UnityStereoTransformScreenSpaceTex(i.uv);

        #if BEAUTIFY_THERMAL_VISION
		    i.uv.x += _NightVision.z * sin(_Time.z + i.uv.y * 20.0) / (1.0 + 10.0 * dot(i.uv - 0.5.xx, i.uv.xy - 0.5.xx));	// wave animation
		#endif

        #if BEAUTIFY_CABERRATION
            float4 pixel = GetDistortedColor(i.uv);
        #else
       		float4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, i.uv);
        #endif

   		beautifyPass(i, pixel.rgb);

   		return pixel;
	}


    float4 FragCompare (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv     = UnityStereoTransformScreenSpaceTex(i.uv);

        // separator line + antialias
        float2 dd     = i.uv;
        if (_CompareParams.z < -15) {
            dd.x -= (_CompareParams.z - -20);
            _CompareParams.z = -10;
        } else {
            dd.x -= 0.5;
        }
        dd.y -= 0.5;

        float  co     = dot(_CompareParams.xy, dd);
        float  dist   = distance( _CompareParams.xy * co, dd );
        float4 aa     = saturate( (_CompareParams.w - dist) / abs(_MainTex_TexelSize.y) );

        float  sameSide = (_CompareParams.z > -5);
        float2 pixelUV = lerp(i.uv, float2(i.uv.x + _CompareParams.z, i.uv.y), sameSide);
        float2 pixelNiceUV = lerp(i.uv, float2(i.uv.x - 0.5 + _CompareParams.z, i.uv.y), sameSide);
        float4 pixel  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, pixelUV);
        float4 pixelNice = SAMPLE_TEXTURE2D_X(_CompareTex, sampler_MainTex, pixelNiceUV);
        
        // are we on the beautified side?
        float2 cp     = float2(_CompareParams.y, -_CompareParams.x);
        float t       = dot(dd, cp) > 0;
        pixel         = lerp(pixel, pixelNice, t);
        return pixel + aa;
    }


	half4 FragCopy (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv     = UnityStereoTransformScreenSpaceTex(i.uv);
        #if defined(USE_BILINEAR)
            return clamp(SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv), 0, 1e6);
        #else
		    return SAMPLE_TEXTURE2D_X(_MainTex, sampler_PointClamp, uv);
        #endif
	}


	half4 FragCopyWithMask (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv       = UnityStereoTransformScreenSpaceTex(i.uv);
        half4 mask      = SAMPLE_TEXTURE2D(_BlurMask, sampler_LinearClamp, uv);
        half4 srcPixel  = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv);
        half4 blurPixel = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_LinearClamp, uv);
        return lerp(srcPixel, blurPixel, mask.a);
	}

	half4 FragCopyWithMiniView (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        float2 uv     = UnityStereoTransformScreenSpaceTex(i.uv);
        float2 uvOrig = (uv - _MiniViewRect.xy) / _MiniViewRect.zw;
        half4 source  = SAMPLE_TEXTURE2D_X_LOD(_MiniViewTex, sampler_LinearClamp, uvOrig, 0);
        half4 final   = SAMPLE_TEXTURE2D_X_LOD(_MainTex, sampler_LinearClamp, uv, 0);
        if (any(floor(uvOrig) != 0)) source = final;
        float2 frameIntensity = saturate( (abs(uvOrig - 0.5) - _MiniViewBlend.xz) * _MiniViewBlend.yw);
        float frameBlend = max(frameIntensity.x, frameIntensity.y);
        return lerp(source, final, frameBlend);
	}


#endif // BEAUTIFY_CORE_FX