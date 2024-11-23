void Transmission_float (
	float3 PositionWS,
	half3 NormalWS,
	float4 ScreenPosRaw,
	bool IsFrontFace,
	bool EnableVFace,

	half TransmissionPower,
	half TransmissionDistortion,
	half TransmissionStrength,
	half TransmissionShadowStrength,

	bool AdditionalLights,

	half3 Albedo,
	half TransmissionMask,

	out half3 o_Emission,
	out	half3 o_NormalWS
)
{
	o_Emission = 0;
	o_NormalWS = NormalWS;

	#if !defined(SHADERGRAPH_PREVIEW)

	//	In case we use the normal buffer and normals from the terrain VFace does not make any sense
		#if !defined(_NORMAL)
			if (EnableVFace)
			{
				NormalWS = IsFrontFace ? NormalWS : NormalWS * half3(-1, -1, -1);	
			}
		#endif
		
		o_NormalWS = NormalWS;

		half trans = 8 * TransmissionStrength * TransmissionMask;

		if (trans > 0)
		{

		//	Since URP 14 we need GetMeshRenderingLayer() // Mind the 6 digits here!
			#if UNITY_VERSION >= 202210
				uint meshRenderingLayers = GetMeshRenderingLayer();
				#if defined(_FORWARD_PLUS)
					InputData inputData = (InputData)0;
					inputData.normalizedScreenSpaceUV = ScreenPosRaw.xy * rcp(ScreenPosRaw.w);
					inputData.positionWS = PositionWS;
				#endif
			#else
				uint meshRenderingLayers = GetMeshRenderingLightLayer();
			#endif

			half3 viewDirectionWS = GetWorldSpaceNormalizeViewDir(PositionWS); 

			half4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
		   	Light mainLight = GetMainLight(shadowCoord);

		   	if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    		{

		   		half3 transLight = mainLight.color * lerp(1, mainLight.shadowAttenuation, TransmissionShadowStrength);

		        half3 transLightDir = mainLight.direction + NormalWS * TransmissionDistortion;
		        half transDot = dot( transLightDir, -viewDirectionWS );
		        transDot = exp2(saturate(transDot) * TransmissionPower - TransmissionPower);

		        half NdotL = dot(transLightDir, NormalWS);

				o_Emission = trans * transDot * (transLight * Albedo); //* (1.0 - NdotL)

			}

			#if defined(_ADDITIONAL_LIGHTS)

				if(AdditionalLights)
				{
	    			uint pixelLightCount = GetAdditionalLightsCount();

	    			LIGHT_LOOP_BEGIN(pixelLightCount)
				        Light light = GetAdditionalLight(lightIndex, PositionWS);

				        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
				        {
				            half3 transLight = light.color * light.distanceAttenuation * lerp(1, light.shadowAttenuation, TransmissionShadowStrength);
				            half3 transLightDir = light.direction + NormalWS * TransmissionDistortion;
		        			half transDot = dot( transLightDir, -viewDirectionWS );
		        			transDot = exp2(saturate(transDot) * TransmissionPower - TransmissionPower);

		        			o_Emission += trans * transDot * (transLight * Albedo);
				        }
				    LIGHT_LOOP_END
				}
			#endif

		}
	#endif
}