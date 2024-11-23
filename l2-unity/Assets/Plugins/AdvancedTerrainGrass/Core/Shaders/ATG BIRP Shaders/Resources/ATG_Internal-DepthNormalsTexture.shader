// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/ATG-Internal-DepthNormalsTexture" {
Properties {
	_MainTex ("", 2D) = "white" {}
	_MainTexArray ("", 2DArray) = "white" {}
	_Cutoff ("", Float) = 0.5
	_Color ("", Color) = (1,1,1,1)

//	Additional inputs

	_SampleNormal 	("Use NormalBuffer", Float) = 0
	_NormalBend		("Bend Normal", Range(0,1)) = 0.5

	_WindMultiplier ("Strength Main (X) Jitter (Y)", Vector) = (1, 0.5, 0, 0)
	_SamplePivot 	("Sample Wind at Pivot", Float) = 0
	_WindLOD 		("Wind LOD (int)", Float) = 0

	_ScaleMode      ("Scale Mode", Float) = 0
}


SubShader {
	Tags { "RenderType"="ATGrass" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.5
// Wind sample mode
#pragma multi_compile _ _METALLICGLOSSMAP
// NormalBuffer
#pragma multi_compile _ _NORMAL
#define UNITY_ASSUME_UNIFORM_SCALING
#include "UnityCG.cginc"
#define DEPTHNORMAL

float _Clip;
float _WindLOD;
float _NormalBend;
half2 _MinMaxScales;
fixed4 _HealthyColor;
fixed4 _DryColor;
float _ScaleMode;

//	Generalized custom CBUFFER
CBUFFER_START(AtgGrass)
	sampler2D _AtgWindRT;
	float4 _AtgWindDirSize;
	float4 _AtgWindStrengthMultipliers;
	float4 _AtgSinTime;
	float4 _AtgGrassFadeProps;
	float4 _AtgGrassShadowFadeProps;
CBUFFER_END
#include "../Includes/GrassInstancedIndirect_Vertex.cginc"

#if SHADER_TARGET >= 45
	StructuredBuffer<float4x4> GrassMatrixBuffer;
#endif

struct v2f {
    float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert( appdata_full v, uint instanceID : SV_InstanceID ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	#if SHADER_TARGET >= 45
        float4x4 data = GrassMatrixBuffer[instanceID];
    #else
        float4x4 data = 0;
    #endif
    unity_ObjectToWorld = data;
//	Restore matrix as it could contain layer data here!
	float InstanceScale = frac(unity_ObjectToWorld[3].w);
	InstanceScale *= 100.0f;
	float3 terrainNormal = 0;
	#if defined(_NORMAL)
		terrainNormal = unity_ObjectToWorld[3].xyz;
	#endif
	unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);
	unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
	unity_WorldToObject._11_22_33 *= -1;

	vertgrass(v, terrainNormal, InstanceScale);

    o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
    
	v.normal = UnityObjectToWorldNormal(v.normal);
  	v.normal = mul(UNITY_MATRIX_V, float4( v.normal, 0) ).xyz;   
    o.nz.xyz = normalize(v.normal); //COMPUTE_VIEW_NORMAL;
    
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}

uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D(_MainTex, i.uv);
	clip( texcol.a - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="ATGrassArray"}
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.5
// Wind sample mode
#pragma multi_compile _ _METALLICGLOSSMAP
// NormalBuffer
#pragma multi_compile _ _NORMAL
#define UNITY_ASSUME_UNIFORM_SCALING
#include "UnityCG.cginc"
#define DEPTHNORMAL

float _Clip;
float _WindLOD;
float _NormalBend;
half2 _MinMaxScales;
fixed4 _HealthyColor;
fixed4 _DryColor;
float _ScaleMode;

//	Generalized custom CBUFFER

CBUFFER_START(AtgGrass)
	sampler2D _AtgWindRT;
	float4 _AtgWindDirSize;
	float4 _AtgWindStrengthMultipliers;
	float4 _AtgSinTime;
	float4 _AtgGrassFadeProps;
	float4 _AtgGrassShadowFadeProps;
CBUFFER_END
#include "../Includes/GrassInstancedIndirect_Vertex.cginc"

#if SHADER_TARGET >= 45
	StructuredBuffer<float4x4> GrassMatrixBuffer;
	StructuredBuffer<float3> GrassNormalBuffer;
#endif

struct v2f {
    float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    float  layer : TEXCOORD2;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert( appdata_full v, uint instanceID : SV_InstanceID ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	#if SHADER_TARGET >= 45
        float4x4 data = GrassMatrixBuffer[instanceID];
    #else
        float4x4 data = 0;
    #endif
    unity_ObjectToWorld = data;
//	Restore matrix as it could contain layer data here!
	float InstanceScale = frac(unity_ObjectToWorld[3].w);
	o.layer = unity_ObjectToWorld[3].w - InstanceScale;
	InstanceScale *= 100.0f;
	float3 terrainNormal = 0;
	#if defined(_NORMAL)
		terrainNormal = unity_ObjectToWorld[3].xyz;
	#endif
	unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);

	unity_WorldToObject = unity_ObjectToWorld;
	unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
	unity_WorldToObject._11_22_33 *= -1;

	vertgrass(v, terrainNormal, InstanceScale);

    o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
 
 	v.normal = UnityObjectToWorldNormal(v.normal);
  	v.normal = mul(UNITY_MATRIX_V, float4( v.normal, 0) ).xyz;   
    o.nz.xyz = normalize(v.normal); //COMPUTE_VIEW_NORMAL;
    

    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
UNITY_DECLARE_TEX2DARRAY(_MainTexArray);
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = UNITY_SAMPLE_TEX2DARRAY(_MainTexArray, float3(i.uv, i.layer ));
	clip( texcol.a - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="ATGFoliage" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.5
// Wind sample mode
#pragma multi_compile _ _METALLICGLOSSMAP
#define UNITY_ASSUME_UNIFORM_SCALING
#include "UnityCG.cginc"
#define DEPTHNORMAL

#include "TerrainEngine.cginc"
#include "../Includes/FoliageInstancedIndirect_Vertex.cginc"

#if SHADER_TARGET >= 45
	StructuredBuffer<float4x4> GrassMatrixBuffer;
#endif

struct v2f {
    float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert( appdata_full v, uint instanceID : SV_InstanceID ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	#if SHADER_TARGET >= 45
        float4x4 data = GrassMatrixBuffer[instanceID];
    #else
        float4x4 data = 0;
    #endif
    unity_ObjectToWorld = data;
//	Restore matrix as it could contain layer data here!
	float InstanceScale = frac(unity_ObjectToWorld[3].w);
	InstanceScale *= 100.0f;
	unity_ObjectToWorld[3] = float4(0, 0, 0, 1.0f);
	
	unity_WorldToObject = unity_ObjectToWorld;
	unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
	unity_WorldToObject._11_22_33 *= -1;

	vertfoliage(v, InstanceScale);

    o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
    
	v.normal = UnityObjectToWorldNormal(v.normal);
  	v.normal = mul(UNITY_MATRIX_V, float4( v.normal, 0) ).xyz;   
    o.nz.xyz = normalize(v.normal); //COMPUTE_VIEW_NORMAL;
    
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}

uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D(_MainTex, i.uv);
	clip( texcol.a - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="ATGVertexLit" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.5
// Wind sample mode
#pragma multi_compile _ _METALLICGLOSSMAP
#define UNITY_ASSUME_UNIFORM_SCALING
#include "UnityCG.cginc"
#define DEPTHNORMAL

#if SHADER_TARGET >= 45
	StructuredBuffer<float4x4> GrassMatrixBuffer;
#endif

struct v2f {
    float4 pos : SV_POSITION;
    float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert( appdata_full v, uint instanceID : SV_InstanceID ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	#if SHADER_TARGET >= 45
        float4x4 data = GrassMatrixBuffer[instanceID];
    #else
        float4x4 data = 0;
    #endif
    unity_ObjectToWorld = data;
//	Restore matrix as it could contain layer data here!
	unity_ObjectToWorld[3].w = 1.0f;
	unity_WorldToObject = unity_ObjectToWorld;
	unity_WorldToObject._14_24_34 = 1.0f / unity_WorldToObject._14_24_34;
	unity_WorldToObject._11_22_33 *= -1;

    o.pos = UnityObjectToClipPos(v.vertex);
    
	v.normal = UnityObjectToWorldNormal(v.normal);
  	v.normal = mul(UNITY_MATRIX_V, float4( v.normal, 0) ).xyz;   
    o.nz.xyz = normalize(v.normal); //COMPUTE_VIEW_NORMAL;
    
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}

fixed4 frag(v2f i) : SV_Target {
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}


// original unity shaders ----------------------------

SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float4 nz : TEXCOORD0;
	UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_base v ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag(v2f i) : SV_Target {
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TransparentCutout" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
uniform float4 _MainTex_ST;
v2f vert( appdata_base v ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	clip( texcol.a*_Color.a - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeBark" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityBuiltin3xTreeLibrary.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_full v ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TreeVertBark(v);
	
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag( v2f i ) : SV_Target {
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeLeaf" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityBuiltin3xTreeLibrary.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_full v ) {
    v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TreeVertLeaf(v);
	
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag( v2f i ) : SV_Target {
	half alpha = tex2D(_MainTex, i.uv).a;

	clip (alpha - _Cutoff);
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeOpaque" "DisableBatching"="True" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : SV_POSITION;
	float4 nz : TEXCOORD0;
	UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	TerrainAnimateTree(v.vertex, v.color.w);
	o.pos = UnityObjectToClipPos(v.vertex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
fixed4 frag(v2f i) : SV_Target {
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
} 

SubShader {
	Tags { "RenderType"="TreeTransparentCutout" "DisableBatching"="True" }
	Pass {
		Cull Back
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float4 texcoord : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	TerrainAnimateTree(v.vertex, v.color.w);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
	half alpha = tex2D(_MainTex, i.uv).a;

	clip (alpha - _Cutoff);
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
	Pass {
		Cull Front
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float4 texcoord : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	TerrainAnimateTree(v.vertex, v.color.w);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord.xy;
    o.nz.xyz = -COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	clip( texcol.a - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}

}

SubShader {
	Tags { "RenderType"="TreeBillboard" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert (appdata_tree_billboard v) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv.x = v.texcoord.x;
	o.uv.y = v.texcoord.y > 0;
    o.nz.xyz = float3(0,0,1);
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
uniform sampler2D _MainTex;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	clip( texcol.a - 0.001 );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="GrassBillboard" }
	Pass {
		Cull Off		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	fixed4 color : COLOR;
	float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert (appdata_full v) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	WavingGrassBillboardVert (v);
	o.color = v.color;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	fixed alpha = texcol.a * i.color.a;
	clip( alpha - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="Grass" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : SV_POSITION;
	fixed4 color : COLOR;
	float2 uv : TEXCOORD0;
	float4 nz : TEXCOORD1;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert (appdata_full v) {
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	WavingGrassVert (v);
	o.color = v.color;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	fixed alpha = texcol.a * i.color.a;
	clip( alpha - _Cutoff );
	return EncodeDepthNormal (i.nz.w, i.nz.xyz);
}
ENDCG
	}
}
Fallback Off
}
