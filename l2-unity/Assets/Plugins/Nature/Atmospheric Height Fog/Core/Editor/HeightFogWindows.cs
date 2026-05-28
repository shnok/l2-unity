using UnityEditor;
using UnityEngine;

namespace AtmosphericHeightFog
{
    public static class HeightFogWindows
    {

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Publisher Page", false, 8000)]
        public static void MoreAssets()
        {
            Application.OpenURL("https://assetstore.unity.com/publishers/20529");
        }

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Discord Server", false, 8001)]
        public static void Discord()
        {
            Application.OpenURL("https://discord.com/invite/znxuXET");
        }

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Documentation", false, 8002)]
        public static void Documentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#");
        }

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Changelog", false, 8003)]
        public static void Chnagelog()
        {
            Application.OpenURL("https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.1rbujejuzjce");
        }

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Write A Review", false, 9999)]
        public static void WriteAReview()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/atmospheric-height-fog-optimized-fog-shaders-for-consoles-mobile-143825#reviews");
        }
    }
}


