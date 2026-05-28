// This file "BeautifyACESFitted.hlsl" is covered by MIT license as follows:
//
//=================================================================================================
//
//  ACES Fitted, an alternate ACES tonemap operator by MJP and David Neubelt
//  http://mynameismjp.wordpress.com/
//
//  Licensed under the MIT license
//
//=================================================================================================
// The code in this file was originally written by Stephen Hill (@self_shadow), who deserves all
// credit for coming up with this fit and implementing it. Buy him a beer next time you see him. :)

#ifndef BEAUTIFY_ACES_FITTED
#define BEAUTIFY_ACES_FITTED

// sRGB => XYZ => D65_2_D60 => AP1 => RRT_SAT
static const float3x3 ACESInputMat =
{
    {0.59719, 0.35458, 0.04823},
    {0.07600, 0.90834, 0.01566},
    {0.02840, 0.13383, 0.83777}
};

// ODT_SAT => XYZ => D60_2_D65 => sRGB
static const float3x3 ACESOutputMat =
{
    { 1.60475, -0.53108, -0.07367},
    {-0.10208,  1.10813, -0.00605},
    {-0.00327, -0.07276,  1.07602}
};

float3 RRTAndODTFit(float3 v)
{
    float3 a = v * (v + 0.0245786f) - 0.000090537f;
    float3 b = v * (0.983729f * v + 0.4329510f) + 0.238081f;
    return a / b;
}

float3 ACESFitted(float3 val)
{
    val = mul(ACESInputMat, val);

    // Apply RRT and ODT
    val = RRTAndODTFit(val);

    val = mul(ACESOutputMat, val);

    // Clamp to [0, 1]
    //val = saturate(val);

    return val;
}

#endif // BEAUTIFY_ACES_FITTED