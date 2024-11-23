	
//	Optimized vertex input structur
	struct appdata_grassinstanced {
		float4 vertex : POSITION;
		//float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
		//float4 texcoord1 : TEXCOORD1;
		//float4 texcoord2 : TEXCOORD2; // GI
		//float4 texcoord3 : TEXCOORD3;
		fixed4 color : COLOR;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

//	Generalized custom CBUFFER
	CBUFFER_START(AtgGrass)
		float4 _AtgWindDirSize;
		float4 _AtgWindStrengthMultipliers;
		float4 _AtgSinTime;
		float4 _AtgGrassFadeProps;
		float4 _AtgGrassShadowFadeProps;

		float3 _AtgSurfaceCameraPosition;
	CBUFFER_END

//	This should not be part of the cbuffer due to ps4
	sampler2D _AtgWindRT;
	float4 _AtgTerrainShiftSurface;

//	StructuredBuffers needed by IndirectInstancing
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		StructuredBuffer<float4x4> GrassMatrixBuffer;
	#endif

//	Vertex to Fragment struct
	#if defined(ISGRASS)
		struct Input {
			#if defined(GRASSUSESTEXTUREARRAYS)
				float2 uv_MainTexArray;
			#else
				float2 uv_MainTex;
			#endif
			//float fade;
			float scale;
			float occ;
			float layer;
			fixed4 color; // : COLOR0;
		};
	#endif

//	Setup function for IndirectInstancing
	#if !defined(DONOTUSE_ATGSETUP)
		void setup() {
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				float4x4 data = GrassMatrixBuffer[unity_InstanceID];
				unity_ObjectToWorld = data;

			//	Handle Floating Origin
				// randPivot does not work out properly when using compute and floating origin for some reason :( - so we use size instead.
				// float usesCompute = 1.0 - _AtgTerrainShiftSurface.w; // 1 if usesCompute, else 0
				// randPivot = float2(unity_ObjectToWorld[0].w + _AtgTerrainShiftSurface.x * usesCompute, unity_ObjectToWorld[2].w + _AtgTerrainShiftSurface.z * usesCompute);
				
				float3 shift = _AtgTerrainShiftSurface.xyz * _AtgTerrainShiftSurface.w; // w = 0 when compute / 1 when no compute
				unity_ObjectToWorld[0].w -= shift.x;
				unity_ObjectToWorld[1].w -= shift.y;
				unity_ObjectToWorld[2].w -= shift.z;

			//	Restore matrix as it could contain layer data here!
				InstanceScale = frac(unity_ObjectToWorld[3].w);
				TextureLayer = unity_ObjectToWorld[3].w - InstanceScale;
				InstanceScale *= 100.0f;
				#if defined(_NORMAL)
					terrainNormal = unity_ObjectToWorld[3].xyz;
				#endif
				unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);

			//	Bullshit!
			//	unity_WorldToObject = unity_ObjectToWorld;
			//	unity_WorldToObject._14_24_34 *= -1;
			//	unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
			
			// 	Not correct but good enough to get the wind direction in object space
				unity_WorldToObject = unity_ObjectToWorld;
				unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
				unity_WorldToObject._11_22_33 *= -1;
			//	Seems to be rather cheap - on: 34 / off 36fps
				//unity_WorldToObject = inverseMat(unity_ObjectToWorld); //inverspositionBuffer[unity_InstanceID];
			#endif
		}
	#endif