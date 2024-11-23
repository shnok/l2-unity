// Global Inputs
CBUFFER_START(AtgGrass)
    float4 _AtgTerrainShiftSurface;
    float4 _AtgWindDirSize;
    float4 _AtgWindStrengthMultipliers;
    float4 _AtgSinTime;
    float4 _AtgGrassFadeProps;
    float2 _AtgVertexFadeProps;
    float4 _AtgGrassShadowFadeProps;
    float3 _AtgSurfaceCameraPosition;
    float _AtgBaseWind;

    float _AtgBendFrequency;
    float _AtgBendFrequencyLastFrame;
CBUFFER_END

// Wind
TEXTURE2D(_AtgWindRT); SAMPLER(sampler_AtgWindRT);
TEXTURE2D(_AtgWindRTPrevious); SAMPLER(sampler_AtgWindRTPrevious);

// Displacement
TEXTURE2D(_Lux_DisplacementRT); SAMPLER(sampler_Lux_DisplacementRT);
float4 _Lux_DisplacementPosition;



//	Simple random function
inline float nrand(float2 pos) {
	return frac(sin(dot(pos, half2(12.9898f, 78.233f))) * 43758.5453f);
}

// Uses float2 only!
float2 SmoothCurve( float2 x ) {   
	return x * x * ( 3.0f - 2.0f * x );   
}
float2 TriangleWave( float2 x ) {   
    return abs( frac( x + 0.5f ) * 2.0f - 1.0f );   
}
float2 SmoothTriangleWave( float2 x ) {   
    return SmoothCurve( TriangleWave( x ) );   
}


void GrassBending_float
(
	float Time,

	float3 PositionOS,
	half3 NormalOS,
	half3 TangentOS,
	half2 BendingParams,
	float2 UV,

	float WindSampleRadius,
	float WindLOD,
	float PerInstanceVariation,
	float PhaseOffset,

	float MainBending,
	float Turbulence,
	float TurbulenceFrequency,
	
	float NormalBend,

	float WindFadeDist,
	float WindFadeRange,

	float ScaleMode,

	float2 MinMaxScales,
	float4 HealthyColor,
	float4 DryColor,

	float Clip,
	float NormalToUpNormal,
	float UpperBound,

	out real3 outPositionOS,
	out real3 outNormalOS,
	out real3 outTangentOS,
	out real2 outVertexToFragmentInterpolator
)
{
	outPositionOS = PositionOS;
	outNormalOS = NormalOS;
	outTangentOS = TangentOS;
	outVertexToFragmentInterpolator = 0;

	#if !defined(SHADERGRAPH_PREVIEW)

	//  Wind animation
		// #if defined(_BENDINGMODE_BLUE)
	    //      #define vMainBending VertexColor.b
	    // #else
	    //     #define vMainBending VertexColor.a
	    // #endif

	    #define vMainBending BendingParams.x
	    #define vPhase BendingParams.y

	//	We leave this in the setup func as this will quiet the compiler.
		//float InstanceScale = 1.0f;
		//int TextureLayer = 0;
		// #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		// 	InstanceScale = frac(unity_ObjectToWorld[3].w);
		// 	TextureLayer = unity_ObjectToWorld[3].w - InstanceScale;
		// 	InstanceScale *= 100.0f;
		// 	//#if defined(_NORMAL)
		// 	//	terrainNormal = unity_ObjectToWorld[3].xyz;
		// 	//#endif
		// 	unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);
		// #endif

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			const float scale = InstanceScale;
		#else 
			const float scale = length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x));
		#endif
		const float3 pivot = UNITY_MATRIX_M._m03_m13_m23;
        const float3 dist = pivot - _WorldSpaceCameraPos.xyz;
        const float SqrDist = dot(dist, dist);
    
    //  Calculate far fade factor

    	#ifndef SHADERPASS
    		#error SHADERPASS must be defined
    	#endif

   	//	NOTE: Mind URP naming here, suckers! SHADERPASS_SHADOWCASTER
    	#if (SHADERPASS == SHADERPASS_SHADOWCASTER)
            // TODO: Check why i can't revert this as well? Clip?
            float fade = 1.0f - saturate((SqrDist - _AtgGrassShadowFadeProps.x) * _AtgGrassShadowFadeProps.y);
        // #elif (SHADERPASS == SHADERPASS_DEPTH_ONLY)
        //     float fade = saturate(( _AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
        #else
			float fade = saturate(( _AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
		#endif        

    //  Cull based on far culling distance
        if (fade == 0.0f) {
            outPositionOS /= fade;
            return;
        }

	//	Get some random value per instance
		float random = nrand(  float2(scale, 1.0 - scale) );

	//  Calculate near fade factor based on instance random / reversed!
        const float smallScaleClipping = saturate(( SqrDist - _AtgGrassFadeProps.z) * _AtgGrassFadeProps.w);
        float clip = (random < Clip)? 1 : 0;
        clip = 1.0f - smallScaleClipping * clip;
    //  Cull based on near culling distance
        if (clip == 0.0f) {
            outPositionOS /= clip;
            return;
        }

    //	Calculate Vertex Fade
    	const float vertexClipping = saturate(( SqrDist - _AtgVertexFadeProps.x) * _AtgVertexFadeProps.y);
    	float vclip = (UV.x < 0)? 1 : 0;
    	clip = saturate(clip - vertexClipping * vclip);
        if (clip == 0.0f) {
            outPositionOS /= clip;
            return;
        }

        fade *= clip;
    
    //  Apply fading
    	if(!ScaleMode) {
    		outPositionOS.xyz = lerp(outPositionOS.xyz, float3(0, 0, 0), (1.0 - fade).xxx);	
    	}
    	else {
    		outPositionOS.xz = lerp(outPositionOS.xz, float2(0, 0), (1.0 - fade).xx);
    	}

    //	Set normal to terrain normal
    	#if defined(_NORMAL)
			outNormalOS = TerrainNormal;
		#else
	//	Tweak normal to normal up
    		outNormalOS = lerp(outNormalOS, float3(0,1,0), NormalToUpNormal.xxx);
    	#endif

    //	Fade Wind
		float windFade = saturate(( WindFadeDist * WindFadeDist - SqrDist) * WindFadeRange);



    	if (windFade > 0)
    	{
	
    		//windFade = smoothstep(0,1,windFade*windFade);

		//	Time
			float localTime = _AtgBendFrequency;

		//	Handle motion vectors
			// bool renderPreviousPosition = false;
			// #if (SHADERPASS == SHADERPASS_MOTION_VECTORS)
			// //	Motion Vectors rely on _LastTime.xyz
			// 	if(Time != _Time.y) {
			// 		localTime = _AtgBendFrequencyLastFrame;
			// 		renderPreviousPosition = true;
			// 	}
			// //	We discard both passes! Otherwise motion vectors go nuts!
			// //	This would work if gbuffer depth prepass was disabled.
			// 	if(SqrDist > 100) {
			// 		//outPositionOS /= 0;
		    //     	//return;
			// 	}
			// #endif

		//	Calculate Wind
		    float3 positionWS = TransformObjectToWorld(outPositionOS.xyz);
		    float3 samplePosWS = TransformObjectToWorld(outPositionOS.xyz * WindSampleRadius);

		//	Handle motion vectors
			float2 windUV = samplePosWS.xz * _AtgWindDirSize.w + vPhase.xx * PhaseOffset - random.xx * PerInstanceVariation;
		    // #if (SHADERPASS == SHADERPASS_MOTION_VECTORS)
			// 	float4 wind;
			// 	if(renderPreviousPosition) {
			// 		wind = SAMPLE_TEXTURE2D_LOD(_AtgWindRTPrevious, sampler_AtgWindRT, windUV, WindLOD);
			// 	}
			// 	else {
			// 		wind = SAMPLE_TEXTURE2D_LOD(_AtgWindRT, sampler_AtgWindRT, windUV, WindLOD);
			// 	}
			// #else
				float4 wind = SAMPLE_TEXTURE2D_LOD(_AtgWindRT, sampler_AtgWindRT, windUV, WindLOD);
			//#endif

			// TODO: Motion Vectors
			float3 windDir = _AtgWindDirSize.xyz;
		    windDir = TransformWorldToObjectDir(windDir);

		    float windStrength = vMainBending * MainBending * _AtgWindStrengthMultipliers.x;
		    windStrength *= wind.r * wind.g; // (wind.g * 2.0f - 0.243f);
			windStrength *= windFade;

		//	Add turbulence
			float vWaveIn = TurbulenceFrequency * (random + (frac(positionWS.x + positionWS.y + positionWS.z) * 2 - 1) + localTime); 
			float2 vWaves = frac( vWaveIn * float2(1.975, 0.793) ) * 2.0 - 1.0;
			vWaves = SmoothTriangleWave( vWaves );
			float noiseSum = vWaves.x + vWaves.y;
			//Good enough?: No! noise here has a strange directionality.
			//float noiseSum = sin( 4.0f * 2.650f * TurbulenceFrequency * (positionWS.x + positionWS.y + positionWS.z + localTime));
			//float noiseSum = sin( 4.0f * 2.650f * TurbulenceFrequency * ( random + (frac(positionWS.x + positionWS.y + positionWS.z) * 2 - 1) + localTime));

			float turbulence = wind.g; // wind.g = 1 + gust
			float3 disp = NormalOS * float3(1, 0.35, 1) * noiseSum * turbulence * vMainBending * Turbulence * _AtgWindStrengthMultipliers.x;

			float3 mainDisp = 0;
		    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		    	mainDisp.xz -= windDir.xz * windStrength; // minus!
		    #else 
		    	mainDisp.xz += windDir.xz * windStrength; // plus!
		    #endif

		    outPositionOS += disp + mainDisp;

		//	Do something to y as well.
			//position.y *= 1 - sqrt( bend.x * bend.x + bend.z * bend.z);
			//Good enough?
			outPositionOS.y -= 0.375 * windStrength;

		//  Add small scale jitter (HZD) - replaced by code above, see turbulence
		    //float3 disp = sin( 4.0f * 2.650f * (positionWS.x + positionWS.y + positionWS.z + Time)) * NormalOS * float3(1.0f, 0.35f, 1.0f);
		    //outPositionOS += disp * windStrength * Turbulence;

		//  Do something to the normal. Sign looks fine. Normalize will be applied later.
		    outNormalOS = outNormalOS + mainDisp * PI * NormalBend;
		//	Here we apply normalize - will be stripped if we do not connect outTangentOS.
		    outTangentOS = Orthonormalize(outTangentOS, normalize(outNormalOS));
		}

	//  Displacement
        if(_EnableDisplacement)
        {
            #define bendAmount BendingParams.x

			float3 cachedPositionWS = TransformObjectToWorld(PositionOS);
            float2 samplePos = lerp(pivot.xz, cachedPositionWS.xz, _DisplacementSampleSize) - _Lux_DisplacementPosition.xy; // lower left corner
            samplePos = samplePos * _Lux_DisplacementPosition.z; // _Lux_DisplacementPosition.z = one OverSize

            if(saturate(samplePos.x) == samplePos.x) {
                if(saturate(samplePos.y) == samplePos.y) {

                    half4 displacementSample = SAMPLE_TEXTURE2D_LOD(_Lux_DisplacementRT, sampler_Lux_DisplacementRT, samplePos, 0);
                    
                    half3 bend = ( (displacementSample.rgb * 2 - 1));
				//	NOTE bend is totally swizzled here as it comes in as a tangent space normal
					bend.z = max(1.0e-16, sqrt(1.0 - saturate(dot(bend.xz, bend.xz)))); // Does not make sense but looks ok?
					//bend.z = max(1.0e-16, sqrt(1.0 - saturate(dot(bend.xy, bend.xy))));
				//	Swizzle bend to a normal in WS
					half3 bendDir = bend.xzy;
					bend = TransformWorldToObjectDir(bendDir, false) * bendAmount;

                //  Blue usually is close to 1 (speaking of a normal). So we use saturate to get only the negative part.
                	bend.y = (saturate(displacementSample.b * 2) - 1) * bendAmount;

                //  Radial fade out of the touch bending
                    half2 radialMask = (samplePos.xy * 2 - 1);
                    half finarm = 1 - dot(radialMask, radialMask);
                    finarm = smoothstep(0,0.5,finarm);
                    bend *= finarm;

                    half3 disp;
					disp.xz = bend.xz * _DisplacementStrength;
					disp.y = -(abs(bend.x) + abs(bend.z) - bend.y) * _DisplacementStrengthVertical;

                //  ATG and VSP: We have to invert direction!?
					#if !defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
						disp.xz = -disp.xz;
					#endif

                    // Normalizing length? Not really worth it...
                    // float vLength = length(cachedPositionWS - pivot);
					outPositionOS = lerp(outPositionOS, PositionOS + disp, saturate(dot(disp, disp) * 16).xxx );
                //  Do something to the normal. Sign looks fine.
                    outNormalOS = outNormalOS + disp * PI * _NormalDisplacement;
                }
            }
        }


	//  Set color variation
		float normalizedScale = (scale - _MinMaxScales.x) * _MinMaxScales.y;
	    normalizedScale = saturate(normalizedScale);
	    #if defined(GRASSUSESTEXTUREARRAYS) && defined(_MIXMODE_RANDOM)
	        //outInstanceColor = lerp(HealthyColor, DryColor, nrand(pivot.zx));
	        outVertexToFragmentInterpolator.x = nrand(pivot.zx);
	    #else
	        //outInstanceColor = lerp(HealthyColor, DryColor, normalizedScale);
	        outVertexToFragmentInterpolator.x = normalizedScale;
	    #endif
	    outVertexToFragmentInterpolator.y = smoothstep(0, UpperBound, vMainBending); // AO

	#endif
}