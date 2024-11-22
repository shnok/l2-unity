#ifndef BEAUTIFY_PPSLUM_FX
#define BEAUTIFY_PPSLUM_FX

	// Copyright 2020-2021 Kronnect - All Rights Reserved.
    #include "BeautifyCommon.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_BloomTex);
	float4 	  _BloomTex_TexelSize;
    TEXTURE2D_X(_CombineTex);
	TEXTURE2D_X(_BloomTex1);
	TEXTURE2D_X(_BloomTex2);
	TEXTURE2D_X(_BloomTex3);
	TEXTURE2D_X(_BloomTex4);
	float4    _MainTex_TexelSize;
	float4    _MainTex_ST;
    float4 	  _Bloom;
	float4 	  _BloomWeights;
	float4 	  _BloomWeights2;
	float4    _BloomTint;
	float3    _BloomTint0, _BloomTint1, _BloomTint2, _BloomTint3, _BloomTint4, _BloomTint5;
	float     _BloomSpread;
    float4 	  _AFTint;
	float     _BlurScale;
    float4    _AFData;

	#if defined(USE_AF_THRESHOLD)
		#define MAX_BRIGHTNESS _AFData.w
	#else
		#define MAX_BRIGHTNESS _BloomWeights2.z

	#endif
	#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
	    float      _BloomDepthThreshold;
	    float      _BloomNearThreshold;
        float      _AFDepthThreshold;
		float	   _AFNearThreshold;
	#endif

	TEXTURE2D_X(_BloomSourceDepth);
	TEXTURE2D_X(_AFSourceDepth);

	struct VaryingsCross {
	    float4 positionCS : SV_POSITION;
	    float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsLum {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
	};

	struct VaryingsCrossLum {
		float4 positionCS : SV_POSITION;
		float2 uv: TEXCOORD0;
        BEAUTIFY_VERTEX_CROSS_UV_DATA
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsLum VertLum(AttributesSimple input) {

	    VaryingsLum output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = input.positionOS;
		output.positionCS.y *= _ProjectionParams.x * _FlipY;
        output.uv = input.uv;

        return output;
	}

	half Brightness(half3 c) {
		return max(c.r, max(c.g, c.b));
	}

	half Included(half4 c) {
		#if BEAUTIFY_BLOOM_USE_LAYER_INCLUSION || BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER_INCLUSION
			return Brightness(c.rgb) >= 0.01;
		#else
			return Brightness(c.rgb) < 0.01;
		#endif
	}

	half3 ColorAboveThreshold(half3 c, half brightness) {
		#if defined(USE_AF_THRESHOLD)
			half threshold = _AFData.y;
		#else
			half threshold = _Bloom.w;
		#endif

		#if BEAUTIFY_ANAMORPHIC_PROP_THRESHOLDING || BEAUTIFY_BLOOM_PROP_THRESHOLDING
	        half cs = clamp(brightness - 0.5 * threshold, 0.0, threshold);
			cs = 0.5 * cs * cs / (threshold + 0.0001);
			c *= max(brightness - threshold, cs) / max(brightness, 0.0001);
		#else
			c = max(c - threshold, 0);
		#endif

		return c;
	}

	half4 FragLum (VaryingsLum i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

	    half4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
		c = clamp(c, 0.0.xxxx, MAX_BRIGHTNESS.xxxx);

   		#if UNITY_COLORSPACE_GAMMA
		    c.rgb = GAMMA_TO_LINEAR(c.rgb);
		#endif

		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		    float depth01 = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
		#endif

		#if BEAUTIFY_BLOOM_USE_DEPTH
		    c.rgb *= max(0, 1.0 - depth01 * _BloomDepthThreshold);
			c.rgb *= min(1.0, depth01 / _BloomNearThreshold);
        #endif

		#if BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
            c.rgb *= max(0, 1.0 - depth01 * _AFDepthThreshold);
			c.rgb *= min(1.0, depth01 / _AFNearThreshold);
		#endif
        
		#if BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_BLOOM_USE_LAYER_INCLUSION
			half included = Included(SAMPLE_TEXTURE2D_X(_BloomSourceDepth, sampler_LinearClamp, i.uv));
			c.rgb *= included;
		#endif

		#if BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER_INCLUSION
			half included = Included(SAMPLE_TEXTURE2D_X(_AFSourceDepth, sampler_LinearClamp, i.uv));
			c.rgb *= included;
		#endif

		c.a = Brightness(c.rgb);
		c.rgb = ColorAboveThreshold(c.rgb, c.a);
   		return c;
   	}

   	VaryingsCross VertCross(AttributesSimple v) {
    	VaryingsCross o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x * _FlipY;
        o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)

		return o;
	}

	VaryingsCrossLum VertCrossLum(AttributesSimple v) {
		VaryingsCrossLum o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = v.positionOS;
		o.positionCS.y *= _ProjectionParams.x * _FlipY;
        o.uv = v.uv;
        BEAUTIFY_VERTEX_OUTPUT_CROSS_UV(o)

		return o;
	}

   	half4 FragLumAntiflicker(VaryingsCrossLum i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		half4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		half4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		half4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		half4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);

		c1 = clamp(c1, 0.0.xxxx, MAX_BRIGHTNESS.xxxx);
		c2 = clamp(c2, 0.0.xxxx, MAX_BRIGHTNESS.xxxx);
		c3 = clamp(c3, 0.0.xxxx, MAX_BRIGHTNESS.xxxx);
		c4 = clamp(c4, 0.0.xxxx, MAX_BRIGHTNESS.xxxx);

		#if BEAUTIFY_BLOOM_USE_DEPTH || BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
		    float depth01 = BEAUTIFY_GET_SCENE_DEPTH_01(i.uv);
		#endif
        
		#if BEAUTIFY_BLOOM_USE_DEPTH
            float depthAtten = max(0, 1.0 - depth01 * _BloomDepthThreshold);
		    c1.rgb *= depthAtten;
		    c2.rgb *= depthAtten;
		    c3.rgb *= depthAtten;
		    c4.rgb *= depthAtten;
			float nearAtten = min(1.0, depth01 /  _BloomNearThreshold);
			c1.rgb *= nearAtten;
			c2.rgb *= nearAtten;
			c3.rgb *= nearAtten;
			c4.rgb *= nearAtten;
        #endif

		#if BEAUTIFY_ANAMORPHIC_FLARES_USE_DEPTH
            float depthAtten = max(0, 1.0 - depth01 * _AFDepthThreshold);
            c1.rgb *= depthAtten;
            c2.rgb *= depthAtten;
            c3.rgb *= depthAtten;
            c4.rgb *= depthAtten;
			float nearAtten = min(1.0, depth01 / _AFNearThreshold);
			c1.rgb *= nearAtten;
			c2.rgb *= nearAtten;
			c3.rgb *= nearAtten;
			c4.rgb *= nearAtten;
        #endif
                        
		#if BEAUTIFY_BLOOM_USE_LAYER || BEAUTIFY_BLOOM_USE_LAYER_INCLUSION
			half included = Included(SAMPLE_TEXTURE2D_X(_BloomSourceDepth, sampler_LinearClamp, i.uv));
			c1.rgb *= included;
			c2.rgb *= included;
			c3.rgb *= included;
			c4.rgb *= included;
		#endif

		#if BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER || BEAUTIFY_ANAMORPHIC_FLARES_USE_LAYER_INCLUSION
			half included = Included(SAMPLE_TEXTURE2D_X(_AFSourceDepth, sampler_LinearClamp, i.uv));
			c1.rgb *= included;
			c2.rgb *= included;
			c3.rgb *= included;
			c4.rgb *= included;
		#endif
		
		c1.a = Brightness(c1.rgb);
		c2.a = Brightness(c2.rgb);
		c3.a = Brightness(c3.rgb);
		c4.a = Brightness(c4.rgb);
	    
	    half w1 = 1.0 / (c1.a + 1.0);
	    half w2 = 1.0 / (c2.a + 1.0);
	    half w3 = 1.0 / (c3.a + 1.0);
	    half w4 = 1.0 / (c4.a + 1.0);

	    half dd  = 1.0 / (w1 + w2 + w3 + w4);
	    c1 = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        
   		#if UNITY_COLORSPACE_GAMMA
		    c1.rgb = GAMMA_TO_LINEAR(c1.rgb);
		#endif

		c1.rgb = ColorAboveThreshold(c1.rgb, c1.a);

   		return c1;
	}

	float4 FragBloomCompose (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float4 b0 = SAMPLE_TEXTURE2D_X( _BloomTex  , sampler_LinearClamp, i.uv );
		float4 b1 = SAMPLE_TEXTURE2D_X( _BloomTex1 , sampler_LinearClamp, i.uv );
		float4 b2 = SAMPLE_TEXTURE2D_X( _BloomTex2 , sampler_LinearClamp, i.uv );
		float4 b3 = SAMPLE_TEXTURE2D_X( _BloomTex3 , sampler_LinearClamp, i.uv );
		float4 b4 = SAMPLE_TEXTURE2D_X( _BloomTex4 , sampler_LinearClamp, i.uv );
		float4 b5 = SAMPLE_TEXTURE2D_X( _MainTex   , sampler_LinearClamp, i.uv );
		b0.rgb *= _BloomTint0;
		b1.rgb *= _BloomTint1;
		b2.rgb *= _BloomTint2;
		b3.rgb *= _BloomTint3;
		b4.rgb *= _BloomTint4;
		b5.rgb *= _BloomTint5;
		float4 pixel = b0 * _BloomWeights.x + b1 * _BloomWeights.y + b2 * _BloomWeights.z + b3 * _BloomWeights.w + b4 * _BloomWeights2.x + b5 * _BloomWeights2.y;
		pixel.rgb = lerp(pixel.rgb, Brightness(pixel.rgb) * _BloomTint.rgb, _BloomTint.a);
		return pixel;
	}

    inline float4 Lerp3(float4 a, float4 b, float4 c, float t) {
        if (t <= 0.5) {
            return lerp(a, b, t * 2.0);
        } else {
            return lerp(b, c, t * 2.0 - 1.0);
        }
    }

	float4 FragResample(VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    float4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
        #if defined(COMBINE_BLOOM)
            float4 o = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv);
            v = Lerp3(o, o+v, v, _BloomSpread);
        #endif
        return v;
	}

	float4 FragResampleAF(VaryingsCross i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_CROSS_UV(i)

		float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1);
		float4 c2 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2);
		float4 c3 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3);
		float4 c4 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4);
			    
	    float w1 = 1.0 / (c1.a + 1.0);
	    float w2 = 1.0 / (c2.a + 1.0);
	    float w3 = 1.0 / (c3.a + 1.0);
	    float w4 = 1.0 / (c4.a + 1.0);
	    
	    float dd  = 1.0 / (w1 + w2 + w3 + w4);
	    float4 v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * dd;
	    v.rgb = lerp(v.rgb, Brightness(c1.rgb) * _AFTint.rgb, _AFTint.a);
	    v.rgb *= _AFData.xxx;
        
        #if defined(COMBINE_BLOOM)
            float4 o = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv);
            v += o;
        #endif
        
	    return v;
	}

	
    float4 FragCombine(VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

        float4 c1 = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        float4 c2 = SAMPLE_TEXTURE2D_X(_CombineTex, sampler_LinearClamp, i.uv);
        return c1 + c2;
    }
    
    
	float4 FragDebugBloom (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		return SAMPLE_TEXTURE2D_X(_BloomTex, sampler_LinearClamp, i.uv) * _Bloom.xxxx;
	}
	
    
	float4 FragDebugBloomExclusionLayer (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float depth = BEAUTIFY_GET_CUSTOM_DEPTH_01(_BloomSourceDepth, i.uv);
		return float4(depth.xxx, 1.0);
	}
   
	float4 FragDebugAnamorphicFlaresExclusionLayer (VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float depth = BEAUTIFY_GET_CUSTOM_DEPTH_01(_AFSourceDepth, i.uv);
		return float4(depth.xxx, 1.0);
	}

	float4 FragResampleFastAF(VaryingsSimple i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);

		float4 c = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
	    c.rgb = lerp(c.rgb, Brightness(c.rgb) * _AFTint.rgb, _AFTint.a);
	    c.rgb *= _AFData.xxx;
	    return c;
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
	
	float4 FragBlur (VaryingsCross i): SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = UnityStereoTransformScreenSpaceTex(i.uv);
        BEAUTIFY_FRAG_SETUP_GAUSSIAN_UV(i)

		float4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv) * 0.2270270270
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv1) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv2)) * 0.3162162162
					+ (SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv3) + SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, uv4)) * 0.0702702703;
   		return pixel;
	}	

#endif