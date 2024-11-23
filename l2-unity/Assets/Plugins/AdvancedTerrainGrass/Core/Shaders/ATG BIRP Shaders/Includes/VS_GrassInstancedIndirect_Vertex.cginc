#if defined(_BENDINPUT_ATG)
	#define AG_PHASE_SHIFT (1.0f - v.color.r) + frac(pivot.x + pivot.z)
	#define AG_BEND_FORCE  v.color.a
	#define AG_AO_MULTIPLIER v.color.a
#else
	#define AG_PHASE_SHIFT v.color.g + frac(pivot.x + pivot.z)
	#define AG_BEND_FORCE  v.color.r
	#define AG_AO_MULTIPLIER v.color.r
#endif

// VS culling distances
float _CullFarStart;
float _CullFarDistance;

// VS touch react
#ifdef TOUCH_BEND_ON
	sampler2D	_TouchReact_Buffer;
	float4		_TouchReact_Pos;
#endif

//	Simple random function
inline float simplenrand(float2 pos) {
	return frac(sin(dot(pos, half2(12.9898f, 78.233f))) * 43758.5453f);
	//return frac((dot(pos, half2(12.9898f, 78.233f))));
}

void atg_vs_vertgrass(inout appdata_grassinstanced v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input, o);

	#define transformPosition float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w)
	#define distanceToCamera length(transformPosition - _WorldSpaceCameraPos.xyz)

//	VS Pro	
	#if !defined(_IGNOREGRASSFADE)
		float cull = 1.0 - saturate((distanceToCamera - _CullFarStart) / _CullFarDistance);
		if (cull == 0) {
			v.vertex = 0.0f;
			return;
		}
	#endif

	float3 pivot = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
	v.normal = half3(0, 1, 0);

//	Get some random value per instance
	fixed random = simplenrand(pivot.xz);
	
//	Set color variation
	fixed4 color = 0;
	color = lerp(_HealthyColor, _DryColor, random.xxxx); 		// PS4 lerp
	
//	Set random bend strength
	float mainBending = AG_BEND_FORCE * color.a;

// 	Scale contains some perlin noise – so we use it to add any perlin noise based variation
	float3 unitvec = mul( (float3x3)unity_ObjectToWorld, float3(1,0,0)); // float4 would be 0,1,0, 0 !!!!!
	float scale = length( unitvec );
	//float scale = InstanceScale;
	
	#ifdef TOUCH_BEND_ON
		float3 pos = mul(unity_ObjectToWorld, v.vertex);
		float2 tbPos = saturate((float2(pos.x, -pos.z) - _TouchReact_Pos.xz) / _TouchReact_Pos.w);
		float2 touchBend = tex2Dlod(_TouchReact_Buffer, float4(tbPos, 0, 0));
		touchBend.y *= 1.0 - length(tbPos - 0.5) * 2; // clip texture "clamp" bugs
		if (touchBend.y > 0.01) {
			pos.y = min(pos.y, touchBend.x * 10000); // touchBend.x is in worldpos...
			v.vertex.xyz = mul(unity_WorldToObject, float4(pos, 1)).xyz;
		}
	#endif

//	Wind
	#if defined(_METALLICGLOSSMAP)
	//	Read wind at pivot
		float4 wind = tex2Dlod(_AtgWindRT, float4(pivot.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, _WindLOD));
	#else
	//	Read wind texture at vertex world position
		float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		float4 wind = tex2Dlod(_AtgWindRT, float4(wPos.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, _WindLOD));
	#endif
	wind.r = wind.r * (wind.g * 2.0f - 0.24376f); //  Not a "real" normal as we want to keep the base direction.

	// if not procedural instanced drawn swap direction
	//#if !defined (UNITY_PROCEDURAL_INSTANCING_ENABLED)
	//	wind.r *= -1;
	//#endif

//	Add bending from wind
	float windStrength = wind.r * _AtgWindStrengthMultipliers.x * _WindMultiplier.x			* mainBending;
	float3 bend = UnityWorldToObjectDir(_AtgWindDirSize.xyz) * windStrength;

//	Reversed for VS as we deal with a real WorldToObject matrix here.		
	v.vertex.xz += bend.xz;

//	Add none directional "jitter" – this helps to hide the quantized wind from the texture lookup.
	float2 jitter = lerp(float2 (_AtgSinTime.x, 0), _AtgSinTime.yz, float2(random, windStrength));
	v.vertex.xz +=
		(jitter.x + jitter.y * _WindMultiplier.y)
		* (0.075 + _AtgSinTime.w) * saturate(windStrength)
	;

//	Bend normal //	Reversed for VS as we deal with a real WorldToObject matrix here.
	v.normal.xz += (bend.xz * UNITY_PI) * _NormalBend;

	#if !defined(_IGNOREGRASSFADE)
		v.vertex.xyz *= cull;
	#endif

//	Store smoothness and color variation
	scale = lerp(0.6, 1.0, random);
	o.scale = scale;
	color.a = v.color.a;
	o.color = color;

}