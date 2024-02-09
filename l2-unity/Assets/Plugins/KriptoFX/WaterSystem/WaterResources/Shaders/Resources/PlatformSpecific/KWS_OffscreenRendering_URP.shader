Shader "Hidden/KriptoFX/KWS/OffscreenRendering_URP"
{
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "../Common/KWS_WaterVariables.cginc"
            #include "KWS_PlatformSpecificHelpers.cginc"
            #include "../Common/CommandPass/KWS_OffscreenRendering_Common.cginc"

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(uint vertexID : SV_VertexID)
            {
                v2f o;
                o.vertex = GetTriangleVertexPosition(vertexID);
                o.uv = GetTriangleUV(vertexID);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 finalColor = GetOffscreenColor(i.uv);
                return half4(finalColor, 1);
            }

            ENDHLSL
        }
    }
}
