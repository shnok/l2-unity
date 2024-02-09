#ifndef KWS_WATER_VARIABLES
#define KWS_WATER_VARIABLES

#define MIN_THRESHOLD 0.00001

SamplerState sampler_linear_repeat;
SamplerState sampler_linear_clamp;
SamplerState sampler_trilinear_clamp;
SamplerState sampler_trilinear_repeat;

half4						KW_WaterColor;
half4						KW_TurbidityColor;
half4						_MainColor;
half4						_SeaColor;
half4						_BottomColor;
half4						_BottomColorDeep;
half4						_SSSColor;
half4						_DiffColor;
half4						_BubblesColor;
half4						_IndirectDiffColor;
half4						_IndirectSpecColor;
half						_Metalic;
half						_Roughness;

//sampler2D KW_PointLightAttenuation;
//sampler2D _BRDFTex;
//sampler2D _MainTex;
//sampler2D					_FoamTex;
//sampler2D					_BubblesTex;
texture2D					KW_FlowMapTex;
texture2D					KW_DispTex;
texture2D					KW_DispTex_LOD1;
texture2D					KW_DispTex_LOD2;
texture2D					KW_NormTex;
SamplerState sampler_KW_NormTex;
texture2D					KW_NormTex_LOD1;
texture2D					KW_NormTex_LOD2;

texture2D KW_ShorelineDepthTex;
float KW_ShorelineDepthOrthoSize;
float3 KW_ShorelineDepth_Near_Far_Dist;
float3 KW_ShorelineAreaPos;

texture2D KW_FluidsDepthTex;
float KW_FluidsDepthOrthoSize;
float3 KW_FluidsDepth_Near_Far_Dist;
float3 KW_FluidsDepthPos;

sampler2D KW_InteractiveWavesTex;
float4 KW_InteractiveWavesTex_TexelSize;
half KW_InteractiveWavesAreaSize;

sampler2D					KW_RipplesTexture;
sampler2D					KW_RipplesTexturePrev;
sampler2D					KW_RipplesNormalTexture;
sampler2D					KW_RipplesNormalTexturePrev;
sampler2D					_ReflectionTex;

sampler2D KW_WaterOpaqueTexture;
//sampler2D					_ShadowMapTexture;

sampler2D KW_WaterMaskDepth;

float4						KW_DispTex_TexelSize;
float4						KW_DispTexDetail_TexelSize;
float4 KW_NormTex_TexelSize;
float4 KW_NormTex_LOD1_TexelSize;
float4 KW_NormTex_LOD2_TexelSize;
float4 KW_DispTex_LOD1_TexelSize;
float4 KW_DispTex_LOD2_TexelSize;



float						_Distortion;
float _test;
float4 _test2;
float _test3;

half _Turbidity;
half _WaterTimeScale;
half						KW_FFTDomainSize;
half						KW_FFTDomainSize_LOD1;
half						KW_FFTDomainSize_LOD2;
half						KW_FFTDomainSize_Detail;

half						KW_NormalLod;
half						KW_NormalLod_LOD1;
half						KW_NormalLod_LOD2;

half						KW_NormMipCount;
half						KW_NormMipCount_LOD1;
half						KW_NormMipCount_LOD2;
half						KW_NormMipCount_Detail;

float2						KW_RipplesUVOffset;
half						KW_RipplesScale;
half						KW_FlowMapSize;
float3						KW_FlowMapOffset;
half KW_FlowMapSpeed;
half KW_FlowMapFluidsStrength;
half						KW_WindSpeed;
half						KW_WindSpeed_LOD1;
half						KW_WindSpeed_LOD2;
half						KW_DistortScale;
half						KW_Time;
half _TesselationFactor;
half KW_WaterFarDistance;
half KW_NormalScattering_Lod;

sampler2D KW_DitherTexture;
sampler2D KW_DistanceFieldDepthIntersection;
sampler2D	KW_DistanceField;
sampler2D KW_TimeRemap;
sampler2D KW_ShoreWaveTex;
sampler2D					_TestTex;
sampler2D _NoiseTex;
sampler2D KW_UpDepth;
float4 KW_DistanceFieldPos;
float4 KW_UpDepthPos;
float3				KW_WaterPosition;
sampler2D KW_BeachWavesTex;
float4 KW_BeachWavesPos;

uniform float4 _GAmplitude;
uniform float4 _GFrequency;
uniform float4 _GSteepness;
uniform float4 _GSpeed;
uniform float4 _GDirectionAB;
uniform float4 _GDirectionCD;


sampler2D _CameraDepthTextureBeforeWaterZWrite;
sampler2D _CameraDepthTextureBeforeWaterZWrite_Blured;
float4 _CameraDepthTextureBeforeWaterZWrite_TexelSize;
float KW_Transparent;
float KW_Turbidity;
half _DistanceBetweenBeachWaves;
half _MinimalDepthForBeachWaves;

float3 KW_DirLightForward;
float3 KW_DirLightColor;

int KW_PointLightCount;
float4 KW_PointLightPositions[100];
float4 KW_PointLightColors[100];

float4x4 KW_ViewToWorld;
float4x4 KW_ProjToView;
float4x4 KWS_CameraProjectionMatrix;

float3 KW_DynamicWavesWorldPos;
float3 KW_InteractCameraOffset_Last;
sampler2D KW_InteractiveWavesNormalTex;
sampler2D KW_InteractiveWavesNormalTexPrev;
sampler2D KW_ShorelineTex;
sampler2D KW_ShorelineNormalTex;
float4 KW_ShorelineTex_TexelSize;

sampler2D KW_ShorelineTexMap;
float KW_ShorelineSize;
float3 KW_ShorelineOffset;

float3 KW_DistanceFieldDepthPos;
float KW_DistanceFieldDepthArea;
float KW_DistanceFieldDepthFar;

sampler2D _TestTexture;
sampler2D _TestDispTexture;
sampler2D _TestNormalTexture;
float4 FoamAnimUV;

sampler2D KW_BakedWaves1_UV_Angle_Alpha;
sampler2D KW_BakedWaves2_UV_Angle_Alpha;

sampler2D KW_BakedWaves1_TimeOffset_Scale;
sampler2D KW_BakedWaves2_TimeOffset_Scale;

sampler2D KW_ReflectionTex;



sampler2D KW_ShorelineWaveNormal;
sampler2D KW_ShorelineWaveDisplacement;



float KW_GlobalTimeOffsetMultiplier;
float KW_GlobalTimeSpeedMultiplier;
float KW_DepthOrthographicSize;
float4 Test4;
float KW_FFT_Size_Normalized;


float3 KW_FluidsMapWorldPosition_lod0;
float3 KW_FluidsMapWorldPosition_lod1;
float KW_FluidsMapAreaSize_lod0;
float KW_FluidsMapAreaSize_lod1;
sampler2D KW_Fluids_Lod0;
sampler2D KW_FluidsFoam_Lod0;
sampler2D KW_Fluids_Lod1;
sampler2D KW_FluidsFoam_Lod1;
sampler2D KW_FluidsFoamTex;
sampler2D KW_FluidsFoamTexBubbles;
float KW_FluidsVelocityAreaScale;
texture2D KW_DynamicWaves;
texture2D KW_DynamicWavesNormal;

float KW_WavesMapSize;

float KW_DynamicWavesAreaSize;
float KWS_RefractionAproximatedDepth;
float KWS_RefractionSimpleStrength;
float KWS_RefractionDispersionStrength;

float KWS_SunCloudiness;
float KWS_SunStrength;
float KWS_SunMaxValue;

float srpBatcherFix;

#endif