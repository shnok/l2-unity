//Copyright Awesome Technologies https://www.awesometech.no/

void InjectSetup_float(float3 A, out float3 Out)
{
	Out = A;
}

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

struct IndirectShaderData
{
	float4x4 PositionMatrix;
	float4x4 InversePositionMatrix;
	float4 ControlData;
};
#if defined(SHADER_API_GLCORE) || defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE)
uniform StructuredBuffer<IndirectShaderData> VisibleShaderDataBuffer;
#endif

#endif

void setupVSPro()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif

#ifdef unity_WorldToObject
#undef unity_WorldToObject
#endif

	unity_ObjectToWorld = VisibleShaderDataBuffer[unity_InstanceID].PositionMatrix;
	unity_WorldToObject = VisibleShaderDataBuffer[unity_InstanceID].InversePositionMatrix;
#endif
}