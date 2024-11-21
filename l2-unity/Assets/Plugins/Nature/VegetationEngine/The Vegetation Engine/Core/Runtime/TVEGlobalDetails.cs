// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;

namespace TheVegetationEngine
{
    [HelpURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.pq49kopnu8z2")]
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Global Details")]
    public class TVEGlobalDetails : StyledMonoBehaviour
    {
        [StyledBanner(0.890f, 0.745f, 0.309f, "Global Details")]
        public bool styledBanner;

        [StyledMessage("Info", "The Detail settings are used to control the terrain grass and details for the Terrain Detail Module!")]
        public bool styledMessage = true;

//#if !THE_VEGETATION_ENGINE_DETAILS
        //[StyledInteractive()]
        //public bool inspectorEnabled = false;
//#endif

        [StyledCategory("Global Settings")]
        public bool globalCat;

        [StyledEnum("TVEColorsLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8", 0, 0)]
        public int layerColors = 0;
        [StyledEnum("TVEExtrasLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8", 0, 0)]
        public int layerExtras = 0;
        [StyledEnum("TVEMotionLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 7 Layer_8 8", 0, 0)]
        public int layerMotion = 0;

        [Space(10)]
        [Range(0, 1)]
        public float globalColor = 1;
        [Range(0, 1)]
        public float globalAlpha = 1;
        [Range(0, 1)]
        public float globalOverlay = 1;
        [Range(0, 1)]
        public float globalWetness = 1;

        [Space(10)]
        [Range(0, 1)]
        public float colorMaskMin = 0.4f;
        [Range(0, 1)]
        public float colorMaskMax = 0.6f;
        [Range(0, 1)]
        public float overlayMaskMin = 0.4f;
        [Range(0, 1)]
        public float overlayMaskMax = 0.6f;

        [Space(10)]
        [Range(0, 1)]
        public float alphaTreshold = 0.5f;

        [StyledCategory("Perspective Settings")]
        public bool perspectiveCat;

        [Range(0, 4)]
        public float perspectivePush = 0;
        [Range(0, 4)]
        public float perspectiveNoise = 0;
        [Range(0, 8)]
        public float perspectiveAngle = 1;

        [StyledCategory("Motion Settings")]
        public bool motionCat;

        [ColorUsage(false, true)]
        public Color motionHighlight = new Color(1, 1, 1, 1);

        [Space(10)]
        [Range(0, 2)]
        public float bendingAmplitude = 1f;
        [Range(0, 40)]
        public int bendingSpeed = 6;
        [Range(0, 20)]
        public float bendingScale = 2;

        [Space(10)]
        [Range(0, 2)]
        public float flutterAmplitude = 0.1f;
        [Range(0, 40)]
        public int flutterSpeed = 20;
        [Range(0, 20)]
        public float flutterScale = 10;

        [Space(10)]
        [Range(0, 2)]
        public float interactionAmplitude = 1;

        [StyledInteractive()]
        public bool inspectorReset = true;

        [StyledSpace(5)]
        public bool styledSpace0;

        void Start()
        {
            gameObject.name = "Global Details";
            gameObject.transform.SetSiblingIndex(1);

            SetGlobalShaderProperties();
        }

#if UNITY_EDITOR
        void Update()
        {
            SetGlobalShaderProperties();
        }
#endif

        void SetGlobalShaderProperties()
        {
//#if THE_VEGETATION_ENGINE_DETAILS
            Shader.SetGlobalFloat("TVE_DetailLayerColors", layerColors);
            Shader.SetGlobalFloat("TVE_DetailLayerExtras", layerExtras);
            Shader.SetGlobalFloat("TVE_DetailLayerMotion", layerMotion);

            Shader.SetGlobalFloat("TVE_DetailGlobalColors", globalColor);
            Shader.SetGlobalFloat("TVE_DetailGlobalOverlay", globalOverlay);
            Shader.SetGlobalFloat("TVE_DetailGlobalWetness", globalWetness);
            Shader.SetGlobalFloat("TVE_DetailGlobalAlpha", globalAlpha);

            Shader.SetGlobalFloat("TVE_DetailColorsMaskMin", colorMaskMin);
            Shader.SetGlobalFloat("TVE_DetailColorsMaskMax", colorMaskMax);
            Shader.SetGlobalFloat("TVE_DetailOverlayMaskMin", overlayMaskMin);
            Shader.SetGlobalFloat("TVE_DetailOverlayMaskMax", overlayMaskMax);

            Shader.SetGlobalFloat("TVE_DetailCutoff", alphaTreshold - 0.5f);

            Shader.SetGlobalFloat("TVE_DetailPerspectivePush", perspectivePush);
            Shader.SetGlobalFloat("TVE_DetailPerspectiveNoise", perspectiveNoise);
            Shader.SetGlobalFloat("TVE_DetailPerspectiveAngle", perspectiveAngle);

            Shader.SetGlobalColor("TVE_DetailMotionHighlightColor", motionHighlight);
            Shader.SetGlobalFloat("TVE_DetailMotionAmplitude_10", bendingAmplitude);
            Shader.SetGlobalFloat("TVE_DetailMotionSpeed_10", bendingSpeed);
            Shader.SetGlobalFloat("TVE_DetailMotionScale_10", bendingScale);
            Shader.SetGlobalFloat("TVE_DetailMotionAmplitude_32", flutterAmplitude);
            Shader.SetGlobalFloat("TVE_DetailMotionSpeed_32", flutterSpeed);
            Shader.SetGlobalFloat("TVE_DetailMotionScale_32", flutterScale);
            Shader.SetGlobalFloat("TVE_DetailInteractionAmplitude", interactionAmplitude);
//#endif
        }
    }
}
