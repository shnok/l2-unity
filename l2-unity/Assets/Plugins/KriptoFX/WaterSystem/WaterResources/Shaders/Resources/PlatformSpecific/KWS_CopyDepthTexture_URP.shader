Shader "Hidden/KriptoFX/KWS/CopyDepthTexture_URP"
{
    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "KWS_PlatformSpecificHelpers.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(uint vertexID : SV_VertexID)
            {
                v2f o;
                o.vertex = GetTriangleVertexPosition(vertexID);
                o.uv = GetTriangleUV(vertexID);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float depth = GetSceneDepth(i.uv);
                return float4(depth, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}
