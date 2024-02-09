namespace KWS
{
    public static class KWS_Settings
    {
        public static class Water
        {
            public static readonly float UpdatePositionEveryMeters = 2.5f;
            public static readonly int MaxNormalsAnisoLevel = 4;
            public static readonly int MaxRefractionDispersion = 5;
        }

        public static class DataPaths
        {
            public static readonly string CausticFolder = "CausticMaps";
            public static readonly string CausticDepthTexture = "KW_CausticDepthTexture";
            public static readonly string CausticDepthData = "KW_CausticDepthData";

            public static readonly string FlowmapFolder = "FlowMaps";
            public static readonly string FlowmapTexture = "FlowMapTexture";
            public static readonly string FlowmapData = "FlowMapData";
        }

        public static class Caustic
        {
            public static readonly int CausticCameraDepth_Near = -1;
            public static readonly int CausticCameraDepth_Far = 50;
        }

        public static class Shoreline
        {
            public static readonly int ShadowParticlesDivider = 4;
        }
        public static class VolumetricLighting
        {
            public static readonly bool UseFastBilateralMode = false;
        }
        
        public static class Reflection
        {
            public static readonly float MaxSunStrength = 10;
        }
    }
}
