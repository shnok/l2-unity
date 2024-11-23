#ifndef AFS_PBS_LIGHTING_INCLUDED
#define AFS_PBS_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"

#include "Includes/AtgStandardBRDF.cginc"


//-------------------------------------------------------------------------------------
// Compatibilty settings
// Uncomment either "#define USEALLOY" or "#define USEUBER" to enable deferred lighting support for the given shader package.
// Leave them commented in case you are using the Lux Foliage deferred lighting shader.
// More infos in the docs.

// #define USEALLOY
// #define USEUBER

//-------------------------------------------------------------------------------------
// Debug shader lighting

struct SurfaceOutputATGUnlit
{
	fixed3 Albedo;
	fixed3 Normal;
	half3 Emission;
	half Smoothness;
	half Occlusion;
	fixed Alpha;	
};

inline half4 LightingATGUnlit (SurfaceOutputATGUnlit s, half3 viewDir, UnityGI gi)
{
	half4 c;
	c.rgb = s.Albedo;
	c.a = s.Alpha;
	return c;
}
inline void LightingATGUnlit_GI (
	SurfaceOutputATGUnlit s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

//-------------------------------------------------------------------------------------

#if defined (VERTEXLIT)
	half _AfsVertexLitHorizonFade;
#else
	half _HorizonFade;
#endif

//-------------------------------------------------------------------------------------
// Surface shader output structure to be used with physically
// based shading model.
struct SurfaceOutputATGSpecular
{
	fixed3 Albedo;		// diffuse color
	fixed3 Specular;	// specular color
	fixed3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;
	fixed Alpha;
	fixed Translucency;
	half TranslucencyPower;
	fixed Lighting;

	#if defined (TANGETFREELIGHTING)
		fixed3 WorldNormal;
	#endif

	fixed3 VertexNormal;
};



//-------------------------------------------------------------------------------------
float3x3 GetCotangentFrame( float3 N, float3 p, float2 uv )
{
    // get edge vectors of the pixel triangle
    float3 dp1 = ddx( p );
    float3 dp2 = ddy( p );
    float2 duv1 = ddx( uv );
    float2 duv2 = ddy( uv );

    // solve the linear system
    float3 dp2perp = cross( dp2, N );
    float3 dp1perp = cross( N, dp1 );
    float3 T = -(dp2perp * duv1.x + dp1perp * duv2.x);

    float3 B = -(dp2perp * duv1.y + dp1perp * duv2.y); // * unity_WorldTransformParams.w;
 
    // construct a scale-invariant frame 
    float invmax = rsqrt( max( dot(T,T), dot(B,B) ) );
    return float3x3( T * invmax, B * invmax, N );
}


// https://forum.libcinder.org/topic/calculating-normals-after-displacing-vertices-in-shader
float3x3 GetCotangentFrameNew( float3 N, float3 p, float2 uv )
{
	// calculate tangent and bitangent
	float3 P1 = ddx( p );
	float3 P2 = ddy( p );
	float2 Q1 = ddx( uv );
	float2 Q2 = ddy( uv );
	 
	float3 T = normalize(  P1 * Q2.y - P2 * Q1.y );
	float3 B = normalize(  P2 * Q1.x - P1 * Q2.x );

	// construct tangent space matrix and perturb normal
	return float3x3( T, B, N );
}


// Horizon Occlusion for Normal Mapped Reflections: http://marmosetco.tumblr.com/post/81245981087
float GetHorizonOcclusion(float3 V, float3 normalWS, float3 vertexNormal, float horizonFade)
{
    float3 R = reflect(-V, normalWS);
    float specularOcclusion = saturate(1.0 + horizonFade * dot(R, vertexNormal));
    // smooth it
    return specularOcclusion * specularOcclusion;
}


//-------------------------------------------------------------------------------------
//	This is more or less like the "LightingStandardSpecular" but with translucency added on top ond wrapped around diffuse

inline half4 LightingATGSpecular (SurfaceOutputATGSpecular s, half3 viewDir, UnityGI gi)
{
	#if defined (TANGETFREELIGHTING)
		s.Normal = normalize(s.WorldNormal);
	#else
		s.Normal = normalize(s.Normal);
	#endif

	// energy conservation
	half oneMinusReflectivity;


//	As Grass has very strange spec values...
	#if defined (ISGRASS)
		//half3 skipIt = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);
		oneMinusReflectivity = 1 - unity_ColorSpaceDielectricSpec.r;
	#else
		s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular.rrr, oneMinusReflectivity);
	#endif

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, outputAlpha);

	// energy conserving wrapped around diffuse lighting
	// http://blog.stevemcauley.com/2011/12/03/energy-conserving-wrapped-diffuse/
	//half wrap1 = 0.4;
	//half NdotLDirect = saturate( ( dot(s.Normal, gi.light.dir) + wrap1 ) / ( (1 + wrap1) * (1 + wrap1) ) );
	half NdotLDirect = saturate(dot(s.Normal, gi.light.dir));

	#if defined(ISTREE)
		#if !defined(DIRECTIONAL) && !defined(DIRECTIONAL_COOKIE)
		//	Fade out point and spot lights
			gi.light.color *= s.Lighting;
		#endif
	#endif

//	Horizon Occlusion
	#if !defined(ISGRASS)
		#if defined (VERTEXLIT)
			gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _AfsVertexLitHorizonFade );
		#else
			gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _HorizonFade );
		#endif
	#endif

	#if !defined(ISGRASS)
		half specularIntensity = 1;
		s.TranslucencyPower *= 10;
	#else
		half specularIntensity = s.TranslucencyPower;
		s.TranslucencyPower = 6;
		s.Specular = unity_ColorSpaceDielectricSpec.rgb;
		gi.indirect.specular *= saturate(specularIntensity + 0.5);
	#endif
	
	half4 c = BRDF1_ATG_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, /*NdotLDirect, */s.Normal, viewDir, gi.light, gi.indirect, specularIntensity);
//	For gi lighting we simply use the built in BRDF
	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);

//	Add Translucency – needs light dir and intensity: so real time only
	#if !defined(LIGHTMAP_ON)
	//	Best for grass as the normal counts less
	//	//	https://colinbarrebrisebois.com/2012/04/09/approximating-translucency-revisited-with-simplified-spherical-gaussian/
		half3 transLightDir = gi.light.dir + s.Normal * 0.01;
		half transDot = dot( -transLightDir, viewDir ); // sign(minus) comes from eyeVec
		transDot = exp2(saturate(transDot) * s.TranslucencyPower - s.TranslucencyPower);
		half3 lightScattering = transDot * gi.light.color *
			#if !defined(ISGRASS)
				(1.0 - NdotLDirect)
			#else
				1.0
			#endif
		;
		c.rgb += s.Albedo * 4.0 * s.Translucency * lightScattering /* mask trans by spec */  * (1.0 - saturate(c.a));
	#endif
	c.a = outputAlpha;
	return c;
}




inline half4 LightingATGSpecular_Deferred (SurfaceOutputATGSpecular s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	// energy conservation
	half oneMinusReflectivity;


//	As Grass has very strange spec values...
	#if defined (ISGRASS)
		//half3 skipIt = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);
		oneMinusReflectivity = 1 - unity_ColorSpaceDielectricSpec.r;
	#else
		s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);
	#endif

	#if defined (TANGETFREELIGHTING)
		s.Normal = normalize(s.WorldNormal);
	#else
		s.Normal = normalize(s.Normal);
	#endif

//	Horizon Occlusion
	#if !defined(ISGRASS)
		#if defined (VERTEXLIT)
			gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _AfsVertexLitHorizonFade );
		#else
			gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _HorizonFade );
		#endif
	#endif

//	Indirect Lighting - needs proper specular in case deferred reflections are disabled
	half3 spec = s.Specular;
	#if defined(ISGRASS)
		spec = unity_ColorSpaceDielectricSpec.rgb;
		gi.indirect.specular *= saturate(s.TranslucencyPower + 0.5);
	#else
		spec = s.Specular.rrr;
	#endif
//	For indirect lighting we simply use the built in BRDF
	half4 c = UNITY_BRDF_PBS (s.Albedo, spec, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.Albedo, spec, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);

	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);
	
	half4 emission;
//	Alloy Support
	#if defined (USEALLOY)
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		outNormal = half4(s.Normal * 0.5 + 0.5, 1);
		emission = half4(s.Emission + c.rgb, s.Translucency);
//	UBER Support
	#elif defined (USEUBER)
		//outDiffuseOcclusion = half4(half3(1,0,0), s.Occlusion);
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		float translucency = floor(saturate(s.Translucency) * 15) * (-128);
		outNormal = half4(s.Normal * 0.5 + 0.5, 1);
		emission = half4(s.Emission + c.rgb, translucency);
// 	Lux Support
	#else
		outSpecSmoothness = half4(s.Specular.r, s.TranslucencyPower, s.Translucency, s.Smoothness);
		// Mark as translucent
		outNormal = half4(s.Normal * 0.5 + 0.5, 0.66);
		emission = half4(s.Emission + c.rgb, 1);
	#endif

	return emission;
}


inline void LightingATGSpecular_GI (
	SurfaceOutputATGSpecular s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

#endif // AFS_PBS_LIGHTING_INCLUDED
