// Author: Blastom
// 使用虚拟光效之后的处理方法都放这个地方
#ifndef SimLightSpecular_Include
	#define SimLightSpecular_Include
	#include "Gles2Helper.cginc"
	#define GLOBAL_LIGHT_INFORMATION \
		uniform float4 _DirectionalLight; \
		uniform float4 _SpecularLight; \
		uniform float4 _DirectionalLightColor; \
		uniform float _DirectionalLightAtten;
		
	#define SIM_LIGHT_COLOR _DirectionalLightColor.rgb * _DirectionalLightAtten
#endif