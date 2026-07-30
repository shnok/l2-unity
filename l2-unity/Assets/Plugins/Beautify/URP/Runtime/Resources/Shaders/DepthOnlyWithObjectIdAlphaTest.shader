Shader "Hidden/Beautify2/DepthOnlyWithObjectIdAlphaTest"
{
    Properties
    {
        [MainTexture] _BaseMap ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        ZWrite On
        Cull Back

        Pass
        {
            Name "Beautify DepthOnly With ObjectId Alpha Test Pass"
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            half _OutlineCutOff;

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            CBUFFER_END

            #ifdef UNITY_DOTS_INSTANCING_ENABLED
                UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
                    UNITY_DOTS_INSTANCED_PROP(float4, _BaseMap_ST)
                UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
                #define _BaseMap_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BaseMap_ST)
            #endif

            #include "DepthOnlyWithObjectIdAlphaTest_Include.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Beautify DepthOnly With ObjectId Alpha Test Pass"  // for older GPUs with no DOTs instancing
            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            half _OutlineCutOff;

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            CBUFFER_END

            #include "DepthOnlyWithObjectIdAlphaTest_Include.hlsl"

            ENDHLSL
        }
    }
}
