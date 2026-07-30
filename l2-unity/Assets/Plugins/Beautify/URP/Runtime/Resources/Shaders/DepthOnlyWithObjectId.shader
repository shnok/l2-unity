Shader "Hidden/Beautify2/DepthOnlyWithObjectId"
{
    Properties
    {
        [MainTexture] _BaseMap ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        ZWrite On
        Cull [_Cull]

        Pass
        {
            Name "Beautify DepthOnly With ObjectId Pass"
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #pragma multi_compile_local _ DEPTH_PREPASS_ALPHA_TEST
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            #if DEPTH_PREPASS_ALPHA_TEST
                CBUFFER_START(UnityPerMaterial)
                half _Cutoff;
                float4 _BaseMap_ST;
                CBUFFER_END

                #ifdef UNITY_DOTS_INSTANCING_ENABLED
                    UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                        UNITY_DOTS_INSTANCED_PROP(float, _Cutoff)
                        UNITY_DOTS_INSTANCED_PROP(float4, _BaseMap_ST)
                    UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
                    #define _Cutoff UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _Cutoff)
                    #define _BaseMap_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BaseMap_ST)
                #endif

            #endif

            #include "DepthOnlyWithObjectId_Include.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Beautify DepthOnly With ObjectId Pass"
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #pragma multi_compile_local _ DEPTH_PREPASS_ALPHA_TEST
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            #if DEPTH_PREPASS_ALPHA_TEST
                CBUFFER_START(UnityPerMaterial)
                half _Cutoff;
                float4 _BaseMap_ST;
                CBUFFER_END
            #endif

            #include "DepthOnlyWithObjectId_Include.hlsl"

            ENDHLSL
        }
    }
}
