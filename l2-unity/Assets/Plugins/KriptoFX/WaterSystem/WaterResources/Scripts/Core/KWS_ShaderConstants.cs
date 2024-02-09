using UnityEngine;

namespace KWS
{
    public static class KWS_ShaderConstants
    {
        #region Shader names

        public static class ShaderNames
        {
            public static readonly string WaterShaderName           = "KriptoFX/KWS/Water";
            public static readonly string WaterTesselatedShaderName = "KriptoFX/KWS/WaterTesselated";
            public static readonly string MaskDepthNormalShaderName = "Hidden/KriptoFX/KWS/MaskDepthNormal";
            public static readonly string VolumetricLightingShaderName = "Hidden/KriptoFX/KWS/VolumetricLighting";
            public static readonly string BlurBilateralName = "Hidden/KriptoFX/KWS/BlurBilateral";
            public static readonly string ReflectionFiltering = "Hidden/KriptoFX/KWS/AnisotropicFiltering";
            public static readonly string CausticComputeShaderName = "Hidden/KriptoFX/KWS/Caustic_Pass";
            public static readonly string CausticDecalShaderName = "Hidden/KriptoFX/KWS/CausticDecal";
            public static readonly string FlowMapShaderName = "Hidden/KriptoFX/KWS/FlowMapEdit";
            public static readonly string ShorelineFoamParticles_shaderName = "Hidden/KriptoFX/KWS/FoamParticles";
            public static readonly string ShorelineFoamParticlesShadow_shaderName = "Hidden/KriptoFX/KWS/FoamParticlesShadow";
            public static readonly string UnderwaterShaderName = "Hidden/KriptoFX/KWS/Underwater";
            public static readonly string CopyColorShaderName = "Hidden/KriptoFX/KWS/CopyColorTexture";
            public static readonly string DrawToDepthShaderName = "Hidden/KriptoFX/KWS/DrawToDepth";
            public static readonly string CopyDepthShaderName = "Hidden/KriptoFX/KWS/CopyDepthTexture";
        }

        #endregion

        #region ShaderID

        public static class WaterID
        {
            public static readonly int KW_Time = Shader.PropertyToID("KW_Time");

            public static readonly int KW_Transparent = Shader.PropertyToID("KW_Transparent");
            public static readonly int KW_Turbidity = Shader.PropertyToID("KW_Turbidity");
            public static readonly int KW_TurbidityColor = Shader.PropertyToID("KW_TurbidityColor");
            public static readonly int KW_WaterColor = Shader.PropertyToID("KW_WaterColor");

            public static readonly int _TesselationMaxDisplace = Shader.PropertyToID("_TesselationMaxDisplace");
            public static readonly int _TesselationFactor = Shader.PropertyToID("_TesselationFactor");
            public static readonly int _TesselationMaxDistance = Shader.PropertyToID("_TesselationMaxDistance");

            public static readonly int KW_NormalScattering_Lod = Shader.PropertyToID("KW_NormalScattering_Lod");
            public static readonly int KW_WaterPosition        = Shader.PropertyToID("KW_WaterPosition");
            public static readonly int KW_ViewToWorld          = Shader.PropertyToID("KW_ViewToWorld");
            public static readonly int KW_ProjToView           = Shader.PropertyToID("KW_ProjToView");
            public static readonly int KW_CameraMatrix         = Shader.PropertyToID("KW_CameraMatrix");
            public static readonly int KWS_CameraProjectionMatrix         = Shader.PropertyToID("KWS_CameraProjectionMatrix");

            public static readonly int KW_WaterFarDistance = Shader.PropertyToID("KW_WaterFarDistance");
            public static readonly int KW_FFT_Size_Normalized = Shader.PropertyToID("KW_FFT_Size_Normalized");
            public static readonly int KW_FlowMapSize = Shader.PropertyToID("KW_FlowMapSize");
            public static readonly int KW_FlowMapOffset = Shader.PropertyToID("KW_FlowMapOffset");
            public static readonly int KW_FlowMapSpeed = Shader.PropertyToID("KW_FlowMapSpeed");
            public static readonly int KW_FlowMapFluidsStrength = Shader.PropertyToID("KW_FlowMapFluidsStrength");
            public static readonly int KW_WindSpeed = Shader.PropertyToID("KW_WindSpeed");
            public static readonly int KW_ReflectionClipOffset = Shader.PropertyToID("KWS_ReflectionClipOffset");
            public static readonly int KWS_SunCloudiness = Shader.PropertyToID("KWS_SunCloudiness");
            public static readonly int KWS_SunStrength = Shader.PropertyToID("KWS_SunStrength");
            public static readonly int KWS_SunMaxValue = Shader.PropertyToID("KWS_SunMaxValue");
            public static readonly int KWS_RefractionAproximatedDepth = Shader.PropertyToID("KWS_RefractionAproximatedDepth");
            public static readonly int KWS_RefractionSimpleStrength = Shader.PropertyToID("KWS_RefractionSimpleStrength");
            public static readonly int KWS_RefractionDispersionStrength = Shader.PropertyToID("KWS_RefractionDispersionStrength");

            public static readonly int KWS_AmbientColor = Shader.PropertyToID("KWS_AmbientColor");
        }

        public static class MaskPassID
        {
            public static readonly int KW_WaterMaskScatterNormals_ID = Shader.PropertyToID("KW_WaterMaskScatterNormals");
            public static readonly int KW_WaterDepth_ID = Shader.PropertyToID("KW_WaterDepth");
            public static readonly int KW_WaterMaskScatterNormals_Blured_ID = Shader.PropertyToID("KW_WaterMaskScatterNormals_Blured");
            public static readonly int KWS_WaterMask_RTHandleScale = Shader.PropertyToID("KWS_WaterMask_RTHandleScale");
        }

        public static class ReflectionsID
        {
            public static readonly int KWS_AnisoReflectionsScale = Shader.PropertyToID("KWS_AnisoReflectionsScale");
            public static readonly int KWS_NormalizedWind        = Shader.PropertyToID("KWS_NormalizedWind");
            public static readonly int KWS_PlanarReflectionRT = Shader.PropertyToID("KWS_PlanarReflectionRT");
            public static readonly int KWS_CubemapReflectionRT = Shader.PropertyToID("KWS_CubemapReflectionRT");
        }

        public static class RefractionID
        {
           
        }

        public static class SSR_ID
        {
            public static readonly int _RTSize = Shader.PropertyToID("_RTSize");
            public static readonly int _HorizontalPlaneHeightWS = Shader.PropertyToID("_HorizontalPlaneHeightWS");
            public static readonly int _DepthHolesFillDistance = Shader.PropertyToID("_DepthHolesFillDistance");
            public static readonly int KW_MATRIX_VP = Shader.PropertyToID("KW_MATRIX_VP");
            public static readonly int KW_MATRIX_I_VP = Shader.PropertyToID("KW_MATRIX_I_VP");

            public static readonly int KW_PREV_MATRIX_VP = Shader.PropertyToID("KW_PREV_MATRIX_VP");
            public static readonly int KW_PREV_MATRIX_I_VP = Shader.PropertyToID("KW_PREV_MATRIX_I_VP");
            public static readonly int _PrevReflectionRT = Shader.PropertyToID("_PrevReflectionRT");
            public static readonly int _PrevReflectionRTHandleSize = Shader.PropertyToID("_PrevReflectionRTHandleSize");

            public static readonly int HashRT = Shader.PropertyToID("HashRT");
            public static readonly int ColorRT = Shader.PropertyToID("ColorRT");
            public static readonly int PosWSyRT = Shader.PropertyToID("PosWSyRT");
            public static readonly int PackedDataRT = Shader.PropertyToID("PackedDataRT");
            public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
            public static readonly int KWS_ScreenSpaceReflectionRT = Shader.PropertyToID("KWS_ScreenSpaceReflectionRT");
            public static readonly int KWS_ScreenSpaceReflection_RTHandleScale = Shader.PropertyToID("KWS_ScreenSpaceReflection_RTHandleScale");
        }

        public static class VolumetricLightConstantsID
        {
            public static readonly int KWS_CameraDepthTextureLowRes = Shader.PropertyToID("KWS_CameraDepthTextureLowRes");
            public static readonly int KWS_VolumetricLightRT = Shader.PropertyToID("KWS_VolumetricLightRT");

            public static readonly int KWS_Transparent = Shader.PropertyToID("KWS_Transparent");
            public static readonly int KWS_LightAnisotropy = Shader.PropertyToID("KWS_LightAnisotropy");
            public static readonly int KWS_VolumeDepthFade = Shader.PropertyToID("KWS_VolumeDepthFade");
            public static readonly int KWS_VolumeLightMaxDistance = Shader.PropertyToID("KWS_VolumeLightMaxDistance");
            public static readonly int KWS_RayMarchSteps = Shader.PropertyToID("KWS_RayMarchSteps");
            public static readonly int KWS_VolumeLightBlurRadius = Shader.PropertyToID("KWS_VolumeLightBlurRadius");
            public static readonly int KWS_VolumeTexSceenSize = Shader.PropertyToID("KWS_VolumeTexSceenSize");
            public static readonly int KWS_NearPlaneWorldPos = Shader.PropertyToID("KWS_NearPlaneWorldPos");
            public static readonly int KWS_VolumetricLight_RTHandleScale = Shader.PropertyToID("KWS_VolumetricLight_RTHandleScale");
        }
        public static class CausticID
        {
            public static readonly int KW_CaustisStrength = Shader.PropertyToID("KW_CaustisStrength");
            public static readonly int KW_CausticDispersionStrength = Shader.PropertyToID("KW_CausticDispersionStrength");
            public static readonly int KW_MeshScale = Shader.PropertyToID("KW_MeshScale");
            public static readonly int KW_CausticDepthScale = Shader.PropertyToID("KW_CausticDepthScale");
            public static readonly int KW_CausticCameraOffset = Shader.PropertyToID("KW_CausticCameraOffset");
            public static readonly int KW_CausticLod0 = Shader.PropertyToID("KW_CausticLod0");
            public static readonly int KW_CausticLod1 = Shader.PropertyToID("KW_CausticLod1");
            public static readonly int KW_CausticLod2 = Shader.PropertyToID("KW_CausticLod2");
            public static readonly int KW_CausticLod3 = Shader.PropertyToID("KW_CausticLod3");
            public static readonly int KW_CausticLodSettings = Shader.PropertyToID("KW_CausticLodSettings");
            public static readonly int KW_CausticLodOffset = Shader.PropertyToID("KW_CausticLodOffset");
            public static readonly int KW_CausticLodPosition = Shader.PropertyToID("KW_CausticLodPosition");
            public static readonly int KW_DecalScale = Shader.PropertyToID("KW_DecalScale");


            public static readonly int KW_CausticDepthTex = Shader.PropertyToID("KW_CausticDepthTex");
            public static readonly int KW_CausticDepthOrthoSize = Shader.PropertyToID("KW_CausticDepthOrthoSize");
            public static readonly int KW_CausticDepth_Near_Far_Dist = Shader.PropertyToID("KW_CausticDepth_Near_Far_Dist");
            public static readonly int KW_CausticDepthPos = Shader.PropertyToID("KW_CausticDepthPos");

        }


        public static class FlowmapID
        {
            public static readonly int KW_FlowMapTex = Shader.PropertyToID("KW_FlowMapTex");
        }

        public static class DynamicWaves
        {
            public static readonly int KW_DynamicWavesWorldPos = Shader.PropertyToID("KW_DynamicWavesWorldPos");
            public static readonly int KW_DynamicWavesAreaSize = Shader.PropertyToID("KW_DynamicWavesAreaSize");
            public static readonly int KW_DynamicObjectsMap = Shader.PropertyToID("KW_DynamicObjectsMap");
            public static readonly int KW_InteractiveWavesPixelSpeed = Shader.PropertyToID("KW_InteractiveWavesPixelSpeed");
            public static readonly int _PrevTex = Shader.PropertyToID("_PrevTex");
            public static readonly int KW_AreaOffset = Shader.PropertyToID("KW_AreaOffset");
            public static readonly int KW_LastAreaOffset = Shader.PropertyToID("KW_LastAreaOffset");
            public static readonly int KW_InteractiveWavesAreaSize = Shader.PropertyToID("KW_InteractiveWavesAreaSize");
            public static readonly int KW_DynamicWaves = Shader.PropertyToID("KW_DynamicWaves");
            public static readonly int KW_DynamicWavesPrev = Shader.PropertyToID("KW_DynamicWavesPrev");
            public static readonly int KW_DynamicWavesNormal = Shader.PropertyToID("KW_DynamicWavesNormal");
            public static readonly int KW_DynamicWavesNormalPrev = Shader.PropertyToID("KW_DynamicWavesNormalPrev");

        }

        public static class ShorelineID
        {
            public static readonly int KW_BakedWaves1_UV_Angle_Alpha = Shader.PropertyToID("KW_BakedWaves1_UV_Angle_Alpha");
            public static readonly int KW_BakedWaves2_UV_Angle_Alpha = Shader.PropertyToID("KW_BakedWaves2_UV_Angle_Alpha");
            public static readonly int KW_BakedWaves1_TimeOffset_Scale = Shader.PropertyToID("KW_BakedWaves1_TimeOffset_Scale");
            public static readonly int KW_BakedWaves2_TimeOffset_Scale = Shader.PropertyToID("KW_BakedWaves2_TimeOffset_Scale");
            public static readonly int KW_GlobalTimeOffsetMultiplier = Shader.PropertyToID("KW_GlobalTimeOffsetMultiplier");
            public static readonly int KW_GlobalTimeSpeedMultiplier = Shader.PropertyToID("KW_GlobalTimeSpeedMultiplier");

        }

        public static class UnderwaterID
        {
            public static readonly int KWS_TargetResolutionMultiplier = Shader.PropertyToID("KWS_TargetResolutionMultiplier");
            public static readonly int KWS_UnderwaterRT_Blured = Shader.PropertyToID("KWS_UnderwaterRT_Blured");
            public static readonly int KWS_Underwater_RTHandleScale = Shader.PropertyToID("KWS_Underwater_RTHandleScale");
        }

        public static class DrawToDepthID
        {

        }

        #endregion

        #region Shader Keywords

        public static class WaterKeywords
        {
            public static readonly string USE_MULTIPLE_SIMULATIONS = "USE_MULTIPLE_SIMULATIONS";
            public static readonly string KW_FLOW_MAP_EDIT_MODE = "KW_FLOW_MAP_EDIT_MODE";
            public static readonly string KW_FLOW_MAP = "KW_FLOW_MAP";
            public static readonly string KW_FLOW_MAP_FLUIDS = "KW_FLOW_MAP_FLUIDS";
            public static readonly string KW_DYNAMIC_WAVES = "KW_DYNAMIC_WAVES";
            public static readonly string USE_CAUSTIC = "USE_CAUSTIC";
            public static readonly string REFLECT_SUN = "REFLECT_SUN";
            public static readonly string USE_VOLUMETRIC_LIGHT = "USE_VOLUMETRIC_LIGHT";
            public static readonly string USE_FILTERING = "USE_FILTERING";
            public static readonly string USE_SHORELINE = "USE_SHORELINE";
            public static readonly string PLANAR_REFLECTION = "PLANAR_REFLECTION";
            public static readonly string SSPR_REFLECTION = "SSPR_REFLECTION";
            public static readonly string USE_REFRACTION_DISPERSION = "USE_REFRACTION_DISPERSION";
            public static readonly string USE_REFRACTION_IOR = "USE_REFRACTION_IOR";
        }

        public static class CausticKeywords
        {
            public static readonly string USE_DEPTH_SCALE = "USE_DEPTH_SCALE";
            public static readonly string USE_DISPERSION = "USE_DISPERSION";
            public static readonly string USE_LOD1 = "USE_LOD1";
            public static readonly string USE_LOD2 = "USE_LOD2";
            public static readonly string USE_LOD3 = "USE_LOD3";
            public static readonly string USE_CAUSTIC_FILTERING = "USE_CAUSTIC_FILTERING";
        }

        public static class ShorelineKeywords
        {
            public static readonly string FOAM_RECEIVE_SHADOWS = "FOAM_RECEIVE_SHADOWS";
            public static readonly string FOAM_COMPUTE_WATER_OFFSET = "FOAM_COMPUTE_WATER_OFFSET";
        }

        #endregion

        #region Compute kernels

        public static class SSR_Kernels
        {
            public static readonly string Clear_kernel = "Clear_kernel";
            public static readonly string RenderHash_kernel = "RenderHash_kernel";
            public static readonly string RenderColorFromHash_kernel = "RenderColorFromHash_kernel";
            public static readonly string RenderSinglePassSSR_kernel = "RenderSinglePassSSR_kernel";
            public static readonly string FillHoles_kernel = "FillHoles_kernel";
            public static readonly string TemporalFiltering_kernel = "TemporalFiltering_kernel";
        }

        #endregion
    }
}

