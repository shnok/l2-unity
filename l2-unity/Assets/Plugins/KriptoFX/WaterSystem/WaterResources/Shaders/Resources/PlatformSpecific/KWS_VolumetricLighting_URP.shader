Shader "Hidden/KriptoFX/KWS/VolumetricLighting_URP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma enable_d3d11_debug_symbols
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ USE_CAUSTIC
            #pragma multi_compile _ USE_LOD1 USE_LOD2 USE_LOD3

            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS 
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE 
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
            #pragma multi_compile _ _SHADOWS_SOFT 
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ KW_POINT_SHADOWS_SUPPORTED
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

             #include "../Common/KWS_WaterVariables.cginc"
            #include "../Common/KWS_WaterPassHelpers.cginc"
            #include "KWS_PlatformSpecificHelpers.cginc"

                 static const float ditherPattern[8][8] = {
                { 0.012f, 0.753f, 0.200f, 0.937f, 0.059f, 0.800f, 0.243f, 0.984f},
                { 0.506f, 0.259f, 0.690f, 0.443f, 0.553f, 0.306f, 0.737f, 0.490f},
                { 0.137f, 0.875f, 0.075f, 0.812f, 0.184f, 0.922f, 0.122f, 0.859f},
                { 0.627f, 0.384f, 0.569f, 0.322f, 0.675f, 0.427f, 0.612f, 0.369f},
                { 0.043f, 0.784f, 0.227f, 0.969f, 0.027f, 0.769f, 0.212f, 0.953f},
                { 0.537f, 0.290f, 0.722f, 0.475f, 0.522f, 0.275f, 0.706f, 0.459f},
                { 0.169f, 0.906f, 0.106f, 0.843f, 0.153f, 0.890f, 0.090f, 0.827f},
                { 0.659f, 0.412f, 0.600f, 0.353f, 0.643f, 0.400f, 0.584f, 0.337f},
                };

                uint KW_lightsCount;

                half MaxDistance;
                half KWS_RayMarchSteps;
                half KWS_VolumeLightMaxDistance;
                half KWS_VolumeDepthFade;
                half4 KWS_LightAnisotropy;

                float2 KWS_VolumeTexSceenSize;

                float4 KWS_NearPlaneWorldPos[3];
                half KWS_Transparent;

                texture2D KW_CausticLod0;
                texture2D KW_CausticLod1;
                texture2D KW_CausticLod2;
                texture2D KW_CausticLod3;
                float4 KW_CausticLodSettings;
                float3 KW_CausticLodOffset;
                float3 KW_CausticLodPosition;
                float KW_DecalScale;
                float KWS_VolumeLightBlurRadius;

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 nearWorldPos : TEXCOORD1;
                };

                v2f vert(uint vertexID : SV_VertexID)
                {
                    v2f o;
                    o.vertex = GetTriangleVertexPosition(vertexID);
                    o.uv = GetTriangleUV(vertexID);
                    o.nearWorldPos = KWS_NearPlaneWorldPos[vertexID].xyz;
                    return o;
                }

                inline half MieScattering(float cosAngle)
                {
                    //return KWS_LightAnisotropy.w * (KWS_LightAnisotropy.x / (pow(KWS_LightAnisotropy.y - KWS_LightAnisotropy.z * cosAngle, 1.5)));
                    return KWS_LightAnisotropy.w * (KWS_LightAnisotropy.x / (KWS_LightAnisotropy.y - KWS_LightAnisotropy.z * cosAngle));
                }

                half GetCausticLod(float3 lightForward, float3 currentPos, float offsetLength, float lodDist, texture2D tex, half lastLodCausticColor)
                {
                    float2 uv = ((currentPos.xz - KW_CausticLodPosition.xz) - offsetLength * lightForward.xz) / lodDist + 0.5 - KW_CausticLodOffset.xz;
                    half caustic = tex.SampleLevel(sampler_linear_repeat, uv, 2.5).r;
                    uv = 1 - min(1, abs(uv * 2 - 1));
                    float lerpLod = uv.x * uv.y;
                    lerpLod = min(1, lerpLod * 3);
                    return lerp(lastLodCausticColor, caustic, lerpLod);
                }

                half ComputeCaustic(float3 rayStart, float3 currentPos, float3 lightForward)
                {
                    float angle = dot(float3(0, -0.999, 0), lightForward);
                    float offsetLength = (rayStart.y - currentPos.y) / angle;

                    half caustic = 0.1;
#if defined(USE_LOD3)
                    caustic = GetCausticLod(lightForward, currentPos, offsetLength, KW_CausticLodSettings.w, KW_CausticLod3, caustic);
#endif
#if defined(USE_LOD2) || defined(USE_LOD3)
                    caustic = GetCausticLod(lightForward, currentPos, offsetLength, KW_CausticLodSettings.z, KW_CausticLod2, caustic);
#endif
#if defined(USE_LOD1) || defined(USE_LOD2) || defined(USE_LOD3)
                    caustic = GetCausticLod(lightForward, currentPos, offsetLength, KW_CausticLodSettings.y, KW_CausticLod1, caustic);
#endif
                    caustic = GetCausticLod(lightForward, currentPos, offsetLength, KW_CausticLodSettings.x, KW_CausticLod0, caustic);

                    float distToCamera = length(currentPos - _WorldSpaceCameraPos);
                    float distFade = saturate(distToCamera / KW_DecalScale * 2);
                    caustic = lerp(caustic, 0, distFade);
                    return caustic * 10 - 1;
                }

                inline float3 RayMarch(float2 uv, float3 rayStart, float3 rayDir, float rayLength, half isUnderwater)
                {
                    float2 ditherScreenPos = uv * KWS_VolumeTexSceenSize;
                    ditherScreenPos = ditherScreenPos % 8;
                    float offset = ditherPattern[ditherScreenPos.y][ditherScreenPos.x];

                    float stepSize = rayLength / KWS_RayMarchSteps;
                    float3 step = rayDir * stepSize;
                    float3 currentPos = rayStart + step * offset;

                    float3 result = 0;
                    float cosAngle = 0;
                    float shadowDistance = saturate(distance(rayStart, _WorldSpaceCameraPos) - KWS_Transparent);
                    float depthFade = 1 - exp(-((_WorldSpaceCameraPos.y - KW_WaterPosition.y) + KWS_Transparent));


                    Light mainLight = GetMainLight();
                    [loop]
                    for (int i = 0; i < KWS_RayMarchSteps; ++i)
                    {
                        float atten = MainLightRealtimeShadow(TransformWorldToShadowCoord(currentPos));
                        float3 scattering = stepSize;
#if defined (USE_CAUSTIC)
                        float underwaterStrength = lerp(saturate((KWS_Transparent - 1) / 5) * 0.5, 1, isUnderwater);
                        scattering += scattering * ComputeCaustic(rayStart, currentPos, mainLight.direction) * underwaterStrength;

#endif
                        float3 light = atten * scattering * mainLight.color;
                        result.rgb += light;
                        currentPos += step;
                    }
                    cosAngle = dot(mainLight.direction.xyz, -rayDir);
                    result *= MieScattering(cosAngle);



#ifdef _ADDITIONAL_LIGHTS
                    //  uint pixelLightCount = GetAdditionalLightsCount(); //bug, unity does not update light count after removal 
                      uint pixelLightCount = _AdditionalLightsCount.x;
                      //uint pixelLightCount = KW_lightsCount; 
                      for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                      {
                          currentPos = rayStart + step * offset;
                          [loop]
                          for (int i = 0; i < KWS_RayMarchSteps; ++i)
                          {

                              Light addLight = GetAdditionalPerObjectLight(lightIndex, currentPos);
  #if KW_POINT_SHADOWS_SUPPORTED
                              float atten = AdditionalLightRealtimeShadow(lightIndex, currentPos, addLight.direction);
  #else 
                              float atten = AdditionalLightRealtimeShadow(lightIndex, currentPos);
  #endif

                              float3 scattering = stepSize * addLight.color.rgb * 5;
                              float3 light = atten * scattering * addLight.distanceAttenuation;

                              cosAngle = dot(-rayDir, normalize(currentPos - addLight.direction.xyz));
                              light *= MieScattering(cosAngle);

                              result.rgb += light;
                              currentPos += step;

                          }

                      }
  #endif               

                      result /= KWS_Transparent;
                      result *= KWS_VolumeDepthFade;
                      //result *= 4;

                      return max(0, result);
                  }


                  float4 frag(v2f i) : SV_Target
                  {
                      half mask = GetWaterMaskScatterNormalsBlured(i.uv).x;

                      UNITY_BRANCH
                      if (mask < 0.25) return 0;

                      float depthTop = GetWaterDepth(i.uv);
                      float depthBot = GetSceneDepth(i.uv);


                      bool isUnderwater = mask > 0.72;

                      UNITY_BRANCH
                      if (depthBot > depthTop && !isUnderwater) return 0;

                      float3 topPos = ComputeWorldSpacePosition(i.uv, depthTop, UNITY_MATRIX_I_VP);
                      float3 botPos = ComputeWorldSpacePosition(i.uv, depthBot, UNITY_MATRIX_I_VP);

                      float3 rayDir = botPos - topPos;
                      rayDir = normalize(rayDir);
                      float rayLength = KWS_VolumeLightMaxDistance;

                      float3 rayStart;

                      UNITY_BRANCH
                    if (isUnderwater)
                    {
                        rayStart = i.nearWorldPos;
                        rayDir = normalize(botPos - _WorldSpaceCameraPos);
                        rayLength = min(length(_WorldSpaceCameraPos - botPos), rayLength);
                        rayLength = min(length(_WorldSpaceCameraPos - topPos), rayLength);
                    }
                    else
                    {
                        rayLength = min(length(topPos - botPos), rayLength);
                        rayStart = topPos;
                    }

                      half4 finalColor;
                      finalColor.rgb = RayMarch(i.uv, rayStart, rayDir, rayLength, isUnderwater);
                      finalColor.a = MainLightRealtimeShadow(TransformWorldToShadowCoord(topPos));
                      finalColor.rgb += MIN_THRESHOLD;
                      return finalColor;
                  }
             ENDHLSL
         }
    }
}