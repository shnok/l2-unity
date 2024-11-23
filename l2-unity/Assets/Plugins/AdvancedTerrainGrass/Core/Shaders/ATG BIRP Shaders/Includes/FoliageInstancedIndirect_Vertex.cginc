
//	Vertex Functions

	float2 _WindMultiplier;
	half2 _MinMaxScales;
		fixed4 _HealthyColor;
		fixed4 _DryColor;
		half _NormalBend;
		half _WindLOD;

	CBUFFER_START(AtgGrass)
		float4 _AtgWindDirSize;
		float4 _AtgWindStrengthMultipliers;
		float2 _AtgSinTime;
		float4 _AtgGrassFadeProps;
		float4 _AtgGrassShadowFadeProps;

		float3 _AtgSurfaceCameraPosition;
	CBUFFER_END
	sampler2D _AtgWindRT;
	float4 _AtgTerrainShiftSurface;



#if !defined(DEPTHNORMAL)
	struct Input {
		float2 uv_MainTex;
		float facingSign : VFACE;
		fixed3 color;
		float3 worldNormal;
		INTERNAL_DATA
	};
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float4x4> GrassMatrixBuffer;
	#endif
#endif

//	Simple random function
	inline float nrand(float2 pos) {
		//return frac(sin(dot(pos, half2(12.9898f, 78.233f))) * 43758.5453f);
		return frac((dot(pos, half2(12.9898f, 78.233f))) );
	}

//	Our vertex function which handles wind and culling
#if !defined(DEPTHNORMAL)
	void vertfoliage(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
#else
	void vertfoliage(inout appdata_full v, in float InstanceScale) {
#endif

	// 	Scale contains some perlin noise – so we use it to add any perlin noise based variation
		//float3 unitvec = mul( (float3x3 )unity_ObjectToWorld, float3(1,0,0)); // float4 would be 0,1,0, 0 !!!!!
		//float scale = length( unitvec );
		float scale = InstanceScale;

		float3 pivot = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
		float3 dist = pivot
					  //+ scale.xxx * 4 /* lets break up the boring distance*/ 
					  #if defined(DONOTUSE_ATGSETUP)
							- _WorldSpaceCameraPos.xyz;			// vs shader version
					  #elif !defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
							- _WorldSpaceCameraPos.xyz;			// for wind setup
					  #else
							- _AtgSurfaceCameraPosition.xyz;	// atg original shader: we have to use a custom cam pos to make it match compute.
					  #endif
		float SqrDist = dot(dist, dist);

	//	Calculate far fade factor
		#if defined (UNITY_PASS_SHADOWCASTER)
			float fade = 1;
		//	Depth Pass
			if (unity_LightShadowBias.z == 0.0) {
				fade = saturate((_AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
			}
		//	Shadow Pass
			else {
				fade = saturate((_AtgGrassShadowFadeProps.z - SqrDist) * _AtgGrassShadowFadeProps.w);
			}
		#else
			float fade = saturate((_AtgGrassFadeProps.x - SqrDist) * _AtgGrassFadeProps.y);
		#endif
	//	Cull based on far culling distance
		if (fade == 0.0f) {
			v.vertex.xyz = 0;
			return;
		} 
	//	Apply fading
		v.vertex.xyz = lerp(v.vertex.xyz, float3(0,0,0), (1.0 - fade).xxx); // PS4 lerp

	// 	Instance Color
		fixed4 instanceColor = lerp(_HealthyColor, _DryColor, ((scale - _MinMaxScales.x) * _MinMaxScales.y).xxxx ); // PS4 lerp
		#if !defined(DEPTHNORMAL)
			o.color.rgb = instanceColor.rgb;
		#endif

	
	//	Wind

		float originalLength = length(v.vertex.xyz);

		float3 windDir = UnityWorldToObjectDir(_AtgWindDirSize.xyz);
		float4 wind = tex2Dlod(_AtgWindRT, float4(
			(
				#if defined(_METALLICGLOSSMAP)
			   		pivot.xz
			   	#else
			   		mul(unity_ObjectToWorld, v.vertex).xz
			   	#endif
			 - instanceColor.a * windDir.xz
			)
			* _AtgWindDirSize.w + scale * 0.025,
			0, _WindLOD )									// _WindLOD lets us smooth the sampling
		);

		wind.r = wind.r * (wind.g * 2.0f - 0.24376f); 		// Not a "real" normal as we want to keep the base direction
		wind.r *= _AtgWindStrengthMultipliers.y * instanceColor.a;

	//	If not procedural instanced drawn swap direction?
		#if !defined (UNITY_PROCEDURAL_INSTANCING_ENABLED)
			wind.r *= -1;
		#endif

		const float fDetailAmp = 0.1;
		const float fBranchAmp = 0.3;

		//float3 variations = abs( frac( pivot.xyz * 3) - 0.5 );
		float2 variations = abs(frac( float2(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].y)));
		float fObjPhase = dot(variations, float2(1,1) );

		float3 offset = 0;

	//	Primary bending
		offset = v.color.a * windDir * (wind.r * _WindMultiplier.x);

		float2 vWavesIn = _Time.yy + float2(0, fObjPhase  +  (v.color.r + instanceColor.a) );
		float4 vWaves = frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0;
		vWaves = SmoothTriangleWave( vWaves );
		float2 vWavesSum = vWaves.xz + vWaves.yw;
	//	Edge Flutter
		float3 bend = v.color.g * fDetailAmp * v.normal.xyz;
	//  Secondary bending
		//bend.y = v.color.b * fBranchAmp;
		//bend *= vWavesSum.y * wind.r * _WindMultiplier.y;
		//offset += bend;

		offset += ((vWavesSum.xyx * bend) + (v.color.b * fBranchAmp * windDir * vWavesSum.y * _WindMultiplier.y)) * wind.r;

	//	Apply Wind Animation
		#if !defined(DEPTHNORMAL)	
			v.vertex.xyz -= offset;
			//	Per pixel normalize is applied in lighting function
			v.normal.xz -= offset * _NormalBend * UNITY_PI;
		#else
			v.vertex.xyz += offset;
			v.normal.xz += offset * _NormalBend * UNITY_PI;
		#endif

	// 	Non directional "jitter"
		float rand = nrand(float2(scale, 1.0 - scale)); // pivot.xz);
		#if !defined(DEPTHNORMAL)	
			v.vertex.xz += lerp(_AtgSinTime.x, _AtgSinTime.y, /*scale*/ rand) * 0.25 * v.color.b * _WindMultiplier.y * saturate(wind.r);
		#else
			v.vertex.xz -= lerp(_AtgSinTime.x, _AtgSinTime.y, /*scale*/ rand) * 0.25 * v.color.b * _WindMultiplier.y * saturate(wind.r);
		#endif
	
	//	Preserve length
		v.vertex.xyz = normalize(v.vertex.xyz) * originalLength;
	}

//	----------------------------------------------
//	Currently unsused functions

	void rotate2D(inout float2 v, float r) {
		float s, c;
		sincos(r, s, c);
		v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
	}

	// http://answers.unity3d.com/questions/218333/shader-inversefloat4x4-function.html
	inline float4x4 inverseMat(float4x4 input) {
    	#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
		float4x4 cofactors = float4x4(
        	minor(_22_23_24, _32_33_34, _42_43_44), 
           -minor(_21_23_24, _31_33_34, _41_43_44),
        	minor(_21_22_24, _31_32_34, _41_42_44),
           -minor(_21_22_23, _31_32_33, _41_42_43),
         
           -minor(_12_13_14, _32_33_34, _42_43_44),
        	minor(_11_13_14, _31_33_34, _41_43_44),
           -minor(_11_12_14, _31_32_34, _41_42_44),
        	minor(_11_12_13, _31_32_33, _41_42_43),
         
        	minor(_12_13_14, _22_23_24, _42_43_44),
           -minor(_11_13_14, _21_23_24, _41_43_44),
        	minor(_11_12_14, _21_22_24, _41_42_44),
           -minor(_11_12_13, _21_22_23, _41_42_43),
         
           -minor(_12_13_14, _22_23_24, _32_33_34),
         	minor(_11_13_14, _21_23_24, _31_33_34),
           -minor(_11_12_14, _21_22_24, _31_32_34),
        	minor(_11_12_13, _21_22_23, _31_32_33)
     	);
     	#undef minor
     	return transpose(cofactors) / determinant(input);
    }