Shader "Hidden/KriptoFX/KWS/DrawToDepth"
{

    SubShader
    {

        Cull Off ZWrite On ZTest Always

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "../KWS_WaterVariables.cginc"
            #include "../KWS_WaterPassHelpers.cginc"
            #include "../../PlatformSpecific/KWS_PlatformSpecificHelpers.cginc"

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

            texture2D _SourceRT;
            float4 _SourceRTHandleScale;

            float4 frag (v2f i, out float depth : SV_Depth) : SV_Target
            {
                half mask = GetWaterMaskScatterNormals(i.uv.xy).x;
                if (mask > 0.7)
                {
                    depth = 0.99;
                    return 0.001;
                }

                float sceneDepth = _SourceRT.SampleLevel(sampler_linear_clamp, i.uv * _SourceRTHandleScale.xy, 0).x;
                float waterDepth = GetWaterDepth(i.uv).x;
                depth = max(waterDepth, sceneDepth);
                return depth;
            }
            ENDHLSL
        }
    }
}
