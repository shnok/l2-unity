
struct Attributes
{
    float4 positionOS : POSITION;
    #if DEPTH_PREPASS_ALPHA_TEST
        float2 uv : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;

    #if DEPTH_PREPASS_ALPHA_TEST
        float2 uv : TEXCOORD0;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings UnlitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    output.positionCS = vertexInput.positionCS;
    #if DEPTH_PREPASS_ALPHA_TEST
        output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    #endif

    return output;
}

void UnlitPassFragment(
    Varyings input
    , out half4 outColor : SV_Target0
)
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

     #if DEPTH_PREPASS_ALPHA_TEST
         half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
          clip(color.a - _Cutoff);
     #endif

         outColor = 0;

}

   