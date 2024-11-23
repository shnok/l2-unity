
//	Vertex Functions

float2 _WindMultiplier;
half2 _MinMaxScales;
	fixed4 _HealthyColor;
	fixed4 _DryColor;
	half _NormalBend;
	half _WindLOD;

CBUFFER_START(AtgGrass)
	sampler2D _AtgWindRT;
	float4 _AtgWindDirSize;
	float4 _AtgWindStrengthMultipliers;
	float2 _AtgSinTime;
	float4 _AtgGrassFadeProps;
	float4 _AtgGrassShadowFadeProps;
CBUFFER_END

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

// VS culling distances
float _CullFarStart;
float _CullFarDistance;

// VS touch react
#ifdef TOUCH_BEND_ON
		sampler2D	_TouchReact_Buffer;
		float4		_TouchReact_Pos;
		float2		_ForceRadius;
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

		#define transformPosition float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w)
		#define distanceToCamera length(transformPosition - _WorldSpaceCameraPos.xyz)

		#if !defined(_IGNOREGRASSFADE)
			float cull = 1.0 - saturate((distanceToCamera - _CullFarStart) / _CullFarDistance);
			if (cull == 0) {
				v.vertex = 0.0f;
				return;
			}
		#endif

	// 	Scale contains some perlin noise – so we use it to add any perlin noise based variation
		float3 unitvec = mul( (float3x3 )unity_ObjectToWorld, float3(1,0,0)); // float4 would be 0,1,0, 0 !!!!!
		float scale = length( unitvec );
		//float scale = InstanceScale;

		float3 pivot = transformPosition; // float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);

	// 	Instance Color
		#if !defined(DEPTHNORMAL)
			fixed4 instanceColor = lerp(_HealthyColor, _DryColor, ((scale - _MinMaxScales.x) * _MinMaxScales.y).xxxx); 		// PS4 lerp
			o.color.rgb = instanceColor.rgb;
		#endif
	
	//	Wind
		float originalLength = length(v.vertex.xyz);

	//	VS touch bending
		#ifdef TOUCH_BEND_ON
				float3 pos = mul(unity_ObjectToWorld, v.vertex);
				float2 tbPos = saturate((float2(pivot.x, -pivot.z) - _TouchReact_Pos.xz) / _TouchReact_Pos.w);
				float2 touchBend = tex2Dlod(_TouchReact_Buffer, float4(tbPos, 0, 0));
				touchBend.y *= 1.0 - length(tbPos - 0.5) * 2; // clip texture "clamp" bugs
				if (touchBend.y > 0.01) {
					float influence = saturate(1.0 - abs((touchBend.x * 10000.0f) - pivot.y) * _ForceRadius.y /*radius or falloff*/);
					v.vertex.xyz += float3(1, -0.5, 1) * _ForceRadius.x * influence * v.color.a;
				}
		#endif

		float3 windDir = UnityWorldToObjectDir(_AtgWindDirSize.xyz);
		float4 wind = tex2Dlod(_AtgWindRT, float4(
			(
				#if defined(_METALLICGLOSSMAP)
			   		pivot.xz
			   	#else
					#ifdef TOUCH_BEND_ON
						pos.xz
					#else
			   			mul(unity_ObjectToWorld, v.vertex).xz
					#endif
			   	#endif
			 - instanceColor.a * windDir.xz
			)
			* _AtgWindDirSize.w + scale * 0.025,
			0, _WindLOD )									 // _WindLOD lets us smooth the sampling
		);

		wind.r = wind.r * (wind.g * 2.0f - 0.24376f  /* not a "real" normal as we want to keep the base direction*/  );
		wind.r *= _AtgWindStrengthMultipliers.y * instanceColor.a;

	// Reversed! direction for VS as we deal with a proper WorldToObject matrix here.
		#if defined (UNITY_PROCEDURAL_INSTANCING_ENABLED)
			wind.r *= -1;
		#endif

		const float fDetailAmp = 0.1;
		const float fBranchAmp = 0.3;

		float3 variations = abs( frac( pivot.xyz * 3) - 0.5 );
		float fObjPhase = dot(variations, float3(1,1,1) );

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
		bend.y = v.color.b * fBranchAmp;
		bend *= vWavesSum.y * wind.r * _WindMultiplier.y;
		offset += bend;

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
		float rand = nrand(pivot.xz);
#if !defined(DEPTHNORMAL)	
		v.vertex.xz += lerp(_AtgSinTime.x, _AtgSinTime.y, /*scale*/ rand) * 0.25 * v.color.b * _WindMultiplier.y * saturate(wind.r);
#else
		v.vertex.xz -= lerp(_AtgSinTime.x, _AtgSinTime.y, /*scale*/ rand) * 0.25 * v.color.b * _WindMultiplier.y * saturate(wind.r);
#endif
	
	//	Preserve length
		v.vertex.xyz = normalize(v.vertex.xyz) * originalLength;

	//	Fade out
		#if !defined(_IGNOREGRASSFADE)
				v.vertex.xyz *= cull;
		#endif
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