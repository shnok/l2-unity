Shader "Hidden/AdvancedTerrainGrass/Hi-Z Buffer"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vertex
            #pragma fragment blit
            #include "HiZBuffer.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vertex
            #pragma fragment reduce
            #include "HiZBuffer.cginc"
            ENDCG
        }
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma target 4.6
            #pragma vertex vertex
            #pragma fragment blit
            #include "HiZBuffer.cginc"
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma target 4.6
            #pragma vertex vertex
            #pragma fragment reduce
            #include "HiZBuffer.cginc"
            ENDCG
        }
    }
}
