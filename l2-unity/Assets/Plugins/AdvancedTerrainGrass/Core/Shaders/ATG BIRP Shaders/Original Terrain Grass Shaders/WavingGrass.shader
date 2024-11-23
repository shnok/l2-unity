// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
	_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
	_Cutoff ("Cutoff", float) = 0.5
}

SubShader {
	Tags {
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="Grass"
		"DisableBatching"="True"
	}
	Cull Off
	LOD 200
		
CGPROGRAM
#pragma surface surf StandardSpecular vertex:WavingGrassVertATG addshadow
//exclude_path:deferred
#include "TerrainEngine.cginc"

sampler2D _MainTex;
fixed _Cutoff;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void WavingGrassVertATG(inout appdata_full v) {
	float waveAmount = v.color.a * _WaveAndDistance.z;
	//v.color.rgb = fixed3(1, 1, 1);
	v.color = TerrainWaveGrass(v.vertex, waveAmount, v.color);
}

void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// * IN.color;
	o.Albedo = c.rgb * IN.color.rgb;
	clip (c.a * IN.color.a - _Cutoff);
	o.Alpha = 1;
}
ENDCG
}
	Fallback Off
}
