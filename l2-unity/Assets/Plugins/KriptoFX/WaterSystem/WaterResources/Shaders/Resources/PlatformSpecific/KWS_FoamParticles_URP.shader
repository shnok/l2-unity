Shader "Hidden/KriptoFX/KWS/FoamParticles_URP"
{
    Properties
    {   _Color("Color", Color) = (0.9, 0.9, 0.9, 0.2)
        _MainTex("Texture", 2D) = "white" {}
        KW_VAT_Position("Position texture", 2D) = "white" {}
        KW_VAT_Alpha("Alpha texture", 2D) = "white" {}
        KW_VAT_Offset("Height Offset", 2D) = "black" {}
        KW_VAT_RangeLookup("Range Lookup texture", 2D) = "white" {}
        _FPS("FPS", Float) = 6.66666
        _Size("Size", Float) = 0.09
        _Scale("AABB Scale", Vector) = (26.3, 4.8, 30.5)
        _NoiseOffset("Noise Offset", Vector) = (0, 0, 0)
        _Offset("Offset", Vector) = (-9.35, -2.025, -15.6, 0)
        _Test("Test", Float) = 0.1
    }
        SubShader
        {
            Tags {  "RenderType" = "Geometry" "Queue" = "Transparent+1"}
            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                Cull Off

                HLSLPROGRAM

                #define FORWARD_BASE_PASS
                //#define KWS_USE_SOFT_SHADOWS
                #define KWS_DISABLE_POINT_SPOT_SHADOWS

                #pragma vertex vert_foam
                #pragma fragment frag_foam
             
                #pragma multi_compile_fog

                #pragma multi_compile _ FOAM_RECEIVE_SHADOWS
                #pragma multi_compile _ FOAM_COMPUTE_WATER_OFFSET

                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS 
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE 
                //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
                //#pragma multi_compile _ _SHADOWS_SOFT 
                //#pragma multi_compile _ _ADDITIONAL_LIGHTS

                #pragma multi_compile _ USE_MULTIPLE_SIMULATIONS

                //#include "UnityCG.cginc"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                #include "../Common/KWS_WaterVariables.cginc"
                #include "../Common/KWS_WaterPassHelpers.cginc"
                #include "../Common/KWS_CommonHelpers.cginc"
                #include "KWS_PlatformSpecificHelpers.cginc"

                #include "../Common/Shoreline/KWS_Shoreline_Common.cginc"
                #include "../Common/KWS_WaterVertPass.cginc"
                #include "../Common/Shoreline/KWS_FoamParticles_Core.cginc"


                inline half3 GetLight(float3 worldPos)
                {
                  
                   
                    half3 lightColor = 0;
                    half atten = 1.0;

                    Light mainLight = GetMainLight();
    #if defined(FOAM_RECEIVE_SHADOWS)
                    atten = MainLightRealtimeShadow(TransformWorldToShadowCoord(worldPos));
    #endif
                    lightColor.rgb = saturate(GetAmbientColor()) * 0.2 + saturate(mainLight.color.rgb) * lerp(0.1, 1, atten);

                    return clamp(lightColor, 0, 0.95);
                }

                inline half3 GetStandardMainLight(float3 worldPos)
                {
                    return clamp(GetMainLightColor() + GetAmbientColor(), 0, 0.95);
                }


             /*   inline half3 ComputeStandardFog(half3 color, float3 worldPos)
                {
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    float viewDist = GetWorldToCameraDistance(worldPos);
                    UNITY_CALC_FOG_FACTOR_RAW(viewDist);
                    return lerp(unity_FogColor, color, saturate(unityFogFactor));
#else 
                    return color;
#endif
                }

                inline half3 ComputeWaterFog(half3 sourceColor, float3 worldPos, float2 uv, float3 waterZ)
                {
#if defined(ENVIRO_FOG)
                    sourceColor = TransparentFog(half4(sourceColor, 0), worldPos.xyz, uv, waterZ);
#elif defined(AZURE_FOG)
                    sourceColor = ApplyAzureFog(half4(sourceColor, 1.), worldPos).xyz;
#else
                    sourceColor = ComputeStandardFog(sourceColor, worldPos);
#endif
                    return sourceColor;
                }
*/

                struct v2f_foam
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float alpha : TEXCOORD1;
                    float3 worldPos : TEXCOORD2;
                };

                v2f_foam vert_foam(appdata_foam v)
                {
                    v2f_foam o;

                    float particleID = v.uv.z;
                    float4 particleData = DecodeParticleData(particleID); //xyz - position, w - alpha
                    float depth;
                    float3 localPos = ParticleDataToLocalPosition(particleData.xyz, 0.0, depth);
                    v.vertex.xyz = CreateBillboardParticle(0.65, v.uv.xy, localPos.xyz);
                    o.alpha = particleData.a;
                    o.uv = GetParticleUV(v.uv.xy);
                    o.worldPos = LocalToWorldPos(v.vertex.xyz);
                    o.pos = ObjectToClipPos(v.vertex);
                    return o;
                }

                half4 frag_foam(v2f_foam i) : SV_Target
                {
                    half4 result;
                    result.a = GetParticleAlpha(i.alpha, _Color.a, i.uv);
                    UNITY_BRANCH if (result.a < 0.002) return 0;

                    result.rgb = GetLight(i.worldPos.xyz);
                    //result.rgb += GetCustomAdditionalLights(i.worldPos.xyz);
                    //result.rgb = ComputeWaterFog(result.rgb, i.worldPos.xyz, i.uv, i.pos.z);
                
                    return result;
                }

                ENDHLSL
            }
        }
}
