// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace TheVegetationEngine
{
    public class TVEMaterialManager : EditorWindow
    {
        const int GUI_SMALL_WIDTH = 50;
        const int GUI_HEIGHT = 18;
        const int GUI_SELECTION_HEIGHT = 24;
        float GUI_HALF_EDITOR_WIDTH = 200;

        string[] materialOptions = new string[]
        {
        "All", "Render Settings", "Global Settings", "Main Settings", "Detail Settings", "Occlusion Settings", "Gradient Settings", "Noise Settings", "Subsurface Settings", "Rim Light Settings", "Emissive Settings", "Perspective Settings", "Size Fade Settings", "Motion Settings",
        };

        string[] savingOptions = new string[]
        {
        "Save All Settings", "Save Current Settings",
        };

        List<TVEPropertyData> propertyDataSet = new List<TVEPropertyData>
        {

        };

        List<TVEPropertyData> savingDataSet = new List<TVEPropertyData>
        {

        };

        List<TVEPropertyData> renderData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryRender", "Render Settings"),

            new TVEPropertyData("_RenderMode", "Render Mode", -9000, "Opaque 0 Transparent 1", false),
            new TVEPropertyData("_RenderCull", "Render Faces", -9000, "Both 0 Back 1 Front 2", false),
            new TVEPropertyData("_RenderNormals", "Render Normals", -9000, "Flip 0 Mirror 1 Same 2", false),
            new TVEPropertyData("_RenderSpecular", "Render Specular", -9000, "Off 0 On 1", false),
            new TVEPropertyData("_RenderDecals", "Render Decals", -9000, "Off 0 On 1", false),
            new TVEPropertyData("_RenderSSR", "Render SSR", -9000, "Off 0 On 1", false),

            new TVEPropertyData("_RenderDirect", "Render Lighting", -9000, 0, 1, false, true),
            new TVEPropertyData("_RenderAmbient", "Render Ambient", -9000, 0, 1, false, false),
            new TVEPropertyData("_RenderShadow", "Render Shadow", -9000, 0, 1, false, false),

            new TVEPropertyData("_RenderClip", "Alpha Clipping", -9000, "Off 0 On 1", true),
            new TVEPropertyData("_RenderCoverage", "Alpha To Mask", -9000, "Off 0 On 1", false),
            new TVEPropertyData("_AlphaClipValue", "Alpha Treshold", -9000, 0, 1, false, false),
            new TVEPropertyData("_AlphaFeatherValue", "Alpha Feather", -9000, 0, 2, false, false),

            new TVEPropertyData("_SpaceRenderFade"),

            new TVEPropertyData("_FadeConstantValue", "Fade Constant", -9000, 0, 1, false, false),
            new TVEPropertyData("_FadeVerticalValue", "Fade Vertical", -9000, 0, 1, false, false),
            new TVEPropertyData("_FadeHorizontalValue", "Fade Horizontal", -9000, 0, 1, false, false),
            new TVEPropertyData("_FadeGlancingValue", "Fade Glancing", -9000, 0, 1, false, false),
            new TVEPropertyData("_FadeCameraValue", "Fade Proximity", -9000, 0, 1, false, false),

            new TVEPropertyData("_AI_Parallax", "Impostor Parallax", -9000, 0, 1, false, true),
            new TVEPropertyData("_AI_ShadowView", "Impostor Shadow View", -9000, 0, 1, false, false),
            new TVEPropertyData("_AI_ShadowBias", "Impostor Shadow Bias", -9000, 0, 1, false, false),
            new TVEPropertyData("_AI_TextureBias", "Impostor Texture Bias", -9000, false, false),
            new TVEPropertyData("_AI_Clip", "Impostor Alpha Treshold", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> globalData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryGlobals", "Global Settings"),

            new TVEPropertyData("_MessageGlobalsVariation", "Procedural Variation in use. The Variation might not work as expected when switching from one LOD to another.", 0, 10, MessageType.Info),

            new TVEPropertyData("_LayerColorsValue", "Layer Colors", -9000, "TVEColorsLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 8 Layer_8 8", false),
            new TVEPropertyData("_LayerExtrasValue", "Layer Extras", -9000, "TVEExtrasLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 8 Layer_8 8", false),
            new TVEPropertyData("_LayerMotionValue", "Layer Motion", -9000, "TVEMotionLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 8 Layer_8 8", false),
            new TVEPropertyData("_LayerVertexValue", "Layer Vertex", -9000, "TVEVertexLayers", "Default 0 Layer_1 1 Layer_2 2 Layer_3 3 Layer_4 4 Layer_5 5 Layer_6 6 Layer_7 8 Layer_8 8", false),

            new TVEPropertyData("_SpaceGlobalLayers"),

            new TVEPropertyData("_GlobalColors", "Global Color", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalAlpha", "Global Alpha", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalOverlay", "Global Overlay", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalWetness", "Global Wetness", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalEmissive", "Global Emissive", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalSize", "Global Size Fade", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalOrientation", "Global Orientation", -9000, 0, 1, false, false),
            new TVEPropertyData("_GlobalConform", "Global Conform", -9000, 0, 1, false, false),

            new TVEPropertyData("_SpaceGlobalsLocals"),

            new TVEPropertyData("_ColorsIntensityValue", "Color Intensity", -9000, 0, 2, false, false),
            new TVEPropertyData("_ColorsVariationValue", "Color Variation", -9000, 0, 1, false, false),
            new TVEPropertyData("_ColorsMaskValue", "Color Use Mask", -9000, 0, 1, false, false),
            new TVEPropertyData("_AlphaVariationValue", "Alpha Variation", -9000, 0, 1, false, false),
            new TVEPropertyData("_OverlayVariationValue", "Overlay Variation", -9000, 0, 1, false, false),
            new TVEPropertyData("_OverlayProjectionValue", "Overlay Projection", -9000, 0, 1, false, false),
            new TVEPropertyData("_ConformOffsetValue", "Conform Offset", -9000, -1, 1, false, false),

            new TVEPropertyData("_ConformOffsetMode", "Lock Position with Conform", -9000, true),

            new TVEPropertyData("_SpaceGlobalOptions"),

            new TVEPropertyData("_ColorsPositionMode", "Use Pivot Position for Colors", -9000, false),
            new TVEPropertyData("_ExtrasPositionMode", "Use Pivot Position for Extras", -9000, false),
            new TVEPropertyData("_VertexPositionMode", "Use Pivot Position for Vertex", -9000, false),
        };

        List<TVEPropertyData> mainData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryMain", "Main Settings"),

            new TVEPropertyData("_MessageMainMask", "Use the Main Mask remap sliders to control the mask for Global Color, Main Colors, Gradient Tinting and Subsurface Effect. The mask is stored in Main Mask Blue channel.", 0, 10, MessageType.Info),

            new TVEPropertyData("_MainUVs", "Main UVs", new Vector4(-9000, 0,0,0), true),

            new TVEPropertyData("_MainColorMode", "Main Color", -9000, "Constant 0 Dual_Colors 1", true),
            new TVEPropertyData("_MainColor", "Main Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_MainColorTwo", "Main ColorB", new Color(-9000, 0,0,0), true, false),
            //new TVEPropertyData("_MainAlphaValue", "Main Alpha", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainAlbedoValue", "Main Albedo", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainNormalValue", "Main Normal", -9000, -8, 8, false, false),
            new TVEPropertyData("_MainMetallicValue", "Main Metallic", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainOcclusionValue", "Main Occlusion", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainSmoothnessValue", "Main Smoothness", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainMaskMinValue", "Main Mask Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_MainMaskMaxValue", "Main Mask Max", -9000, 0, 1, false, false),

            new TVEPropertyData("_ImpostorColor", "Impostor Color", new Color(-9000, 0,0,0), true, true),
            new TVEPropertyData("_ImpostorSmoothnessValue", "Impostor Smoothness", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> secondData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryDetail", "Detail Settings"),

            new TVEPropertyData("_MessageSecondMask", "Use the Detail Mask remap sliders to control the mask for Global Color, Detail Colors, Gradient Tinting and Subsurface Effect. The mask is stored in Detail Mask Blue channel.", 0, 10, MessageType.Info),

            new TVEPropertyData("_DetailMode", "Detail Mode", -9000, "Off 0 On 1", false),
            new TVEPropertyData("_DetailBlendMode", "Detail Blend", -9000, "Overlay 0 Replace 1", false),
            new TVEPropertyData("_DetailAlphaMode", "Detail Alpha", -9000, "Overlay 0 Replace 1", false),

            new TVEPropertyData("_DetailFadeMode", "Use Global Alpha and Fade", -9000, true),

            new TVEPropertyData("_SecondUVsMode", "Detail UVs", -9000, "Main_UVs 0 Detail_UVs 1 Planar_UVs 2", true),
            new TVEPropertyData("_SecondUVs", "Detail UVs", new Vector4(-9000, 0,0,0), false),

            new TVEPropertyData("_SecondUVsScaleMode", "Use Inverted Tilling Mode", -9000, true),

            new TVEPropertyData("_SecondColorMode", "Detail Color", -9000, "Constant 0 Dual_Colors 1", true),
            new TVEPropertyData("_SecondColor", "Detail Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_SecondColorTwo", "Detail ColorB", new Color(-9000, 0,0,0), true, false),
            //new TVEPropertyData("_SecondAlphaValue", "Detail Alpha", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondAlbedoValue", "Detail Albedo", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondNormalValue", "Detail Normal", -9000, -8, 8, false, false),
            new TVEPropertyData("_SecondMetallicValue", "Detail Metallic", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondOcclusionValue", "Detail Occlusion", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondSmoothnessValue", "Detail Smoothness", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondMaskMinValue", "Detail Mask Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_SecondMaskMaxValue", "Detail Mask Max", -9000, 0, 1, false, false),

            new TVEPropertyData("_DetailValue", "Detail Blend Intensity", -9000, 0, 1, false, true),
            new TVEPropertyData("_DetailNormalValue", "Detail Blend Normals", -9000, 0, 1, false, false),
            new TVEPropertyData("_DetailBlendMinValue", "Detail Blend Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_DetailBlendMaxValue", "Detail Blend Max", -9000, 0, 1, false, false),

            new TVEPropertyData("_DetailMeshMode", "Detail Surface Mask", -9000, "Vertex_Blue 0 Projection 1", true),
            new TVEPropertyData("_DetailMeshMinValue", "Detail Surface Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_DetailMeshMaxValue", "Detail Surface Max", -9000, 0, 1, false, false),
            new TVEPropertyData("_DetailMaskMode", "Detail Texture Mask", -9000, "Main_Mask 0 Detail_Mask 1", false),
            new TVEPropertyData("_DetailMaskMinValue", "Detail Texture Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_DetailMaskMaxValue", "Detail Texture Max", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> occlusionData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryOcclusion", "Occlusion Settings"),

            new TVEPropertyData("_MessageOcclusion", "Use the Occlusion Color for tinting and the Occlusion Alpha as Global Color and Global Overlay mask control. The mask is stored in Vertex Green channel.", 0, 10, MessageType.Info),

            new TVEPropertyData("_VertexOcclusionColor", "Occlusion Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_VertexOcclusionMinValue", "Occlusion Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_VertexOcclusionMaxValue", "Occlusion Max", -9000, 0, 1, false, false),

            new TVEPropertyData("_VertexOcclusionColorsMode", "Use Inverted Mask for Colors", -9000, true),
            new TVEPropertyData("_VertexOcclusionOverlayMode", "Use Inverted Mask for Overlay", -9000, false),
        };

        List<TVEPropertyData> gradientData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryGradient", "Gradient Settings"),

            new TVEPropertyData("_GradientColorOne", "Gradient ColorA", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_GradientColorTwo", "Gradient ColorB", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_GradientMinValue", "Gradient Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_GradientMaxValue", "Gradient Max", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> noiseData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryNoise", "Noise Settings"),

            new TVEPropertyData("_NoiseColorOne", "Noise ColorA", new Color(-9000, 0,0,0), true, true),
            new TVEPropertyData("_NoiseColorTwo", "Noise ColorB", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_NoiseMinValue", "Noise Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_NoiseMaxValue", "Noise Max", -9000, 0, 1, false, false),
            new TVEPropertyData("_NoiseScaleValue", "Noise Scale", -9000, 0, 10, false, false),

            new TVEPropertyData("_NoisePositionMode", "Use Pivot Position for Noise", -9000, true),
        };

        List<TVEPropertyData> subsurfaceData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategorySubsurface", "Subsurface Settings"),

            new TVEPropertyData("_MessageSubsurface", "In HDRP, the Subsurface Color and Power are fake effects used for artistic control. For physically correct subsurface scattering the Power slider need to be set to 0.", 0, 10, MessageType.Info),

            new TVEPropertyData("_SubsurfaceValue", "Subsurface Intensity", -9000, 0, 1, false, false),
            new TVEPropertyData("_SubsurfaceColor", "Subsurface Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_SubsurfaceScatteringValue", "Subsurface Power", -9000, 0, 16, false, false),
            new TVEPropertyData("_SubsurfaceAngleValue", "Subsurface Angle", -9000, 1, 16, false, false),
            new TVEPropertyData("_SubsurfaceNormalValue", "Subsurface Normal", -9000, 0, 1, false, false),
            new TVEPropertyData("_SubsurfaceDirectValue", "Subsurface Direct", -9000, 0, 1, false, false),
            new TVEPropertyData("_SubsurfaceAmbientValue", "Subsurface Ambient", -9000, 0, 1, false, false),
            new TVEPropertyData("_SubsurfaceShadowValue", "Subsurface Shadow", -9000, 0, 1, false, false),
            new TVEPropertyData("_SubsurfaceMaskValue", "Subsurface Use Mask", -9000, 0, 1, false, false),

            new TVEPropertyData("_SubsurfaceThicknessValue", "Thickness Amplitude", -9000, 0, 16, false, true),
            new TVEPropertyData("_SubsurfaceThicknessMaskValue", "Thickness Use Mask", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> rimLightData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryRimLight", "Rim Light Settings"),

            new TVEPropertyData("_RimLightColor", "Rim Light Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_RimLightMinValue", "Rim Light Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_RimLightMaxValue", "Rim Light Max", -9000, 0, 1, false, false),
            new TVEPropertyData("_RimLightNormalValue", "Rim Light Normal", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> emissiveData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryEmissive", "Emissive Settings"),

            new TVEPropertyData("_EmissiveMode", "Emissive Mode", -9000, "Off 0 On 1", true),
            new TVEPropertyData("_EmissiveFlagMode", "Emissive GI", -9000, "None 0 Any 1 Baked 2 Realtime 3", false),

            new TVEPropertyData("_EmissiveUVs", "Emissive UVs", new Vector4(-9000, 0,0,0), true),

            new TVEPropertyData("_EmissiveColor", "Emissive Color", new Color(-9000, 0,0,0), true, true),
            new TVEPropertyData("_EmissiveIntensityMode", "Emissive Value", -9000, "Nits 0 EV100 1", false),
            new TVEPropertyData("_EmissiveIntensityValue", "Emissive Power", -9000, false, false),
            new TVEPropertyData("_EmissivePhaseValue", "Emissive Phase", -9000, 0, 1, false, false),
            new TVEPropertyData("_EmissiveExposureValue", "Emissive Weight", -9000, 0, 1, false, false),
            new TVEPropertyData("_EmissiveTexMinValue", "Emissive Tex Min", -9000, 0, 1, false, false),
            new TVEPropertyData("_EmissiveTexMaxValue", "Emissive Tex Max", -9000, 0, 1, false, false),
            new TVEPropertyData("_EmissiveAlphaValue", "Emissive Dissolve", -9000, 0, 1, false, false),
            //new TVEMaterialData("_EmissiveDarkValue", "Emissive Darkening", -9000, 0, 1, false, false),
        };

        List<TVEPropertyData> perspectiveSettings = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryPerspective", "Perspective Settings"),

            new TVEPropertyData("_PerspectivePushValue", "Perspective Push", -9000, 0, 4, false, false),
            new TVEPropertyData("_PerspectiveNoiseValue", "Perspective Noise", -9000, 0, 4, false, false),
            new TVEPropertyData("_PerspectiveAngleValue", "Perspective Angle", -9000, 0, 8, false, false),
        };

        List<TVEPropertyData> sizeFadeData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategorySizeFade", "Size Fade Settings"),

            new TVEPropertyData("_SizeFadeStartValue", "Size Fade Start", -9000, 0, 1000, false, false),
            new TVEPropertyData("_SizeFadeEndValue", "Size Fade End", -9000, 0, 1000, false, false),
        };

        List<TVEPropertyData> motionData = new List<TVEPropertyData>
        {
            new TVEPropertyData("_CategoryMotion", "Motion Settings"),

            new TVEPropertyData("_MessageMotionVariation", "Procedural variation in use. Use the Scale settings if the Variation is splitting the mesh.", 0, 10, MessageType.Info),

            new TVEPropertyData("_MotionHighlightColor", "Wind Highlight Color", new Color(-9000, 0,0,0), true, false),
            new TVEPropertyData("_MotionFacingValue", "Wind Direction Mask", -9000, 0, 1, false, false),

            new TVEPropertyData("_MotionAmplitude_10", "Motion Bending", -9000, 0, 2, false, true),
            new TVEPropertyData("_MotionPosition_10", "Motion Rigidity", -9000, 0, 1, false, false),
            new TVEPropertyData("_MotionSpeed_10", "Motion Speed", -9000, 0, 40, true, false),
            new TVEPropertyData("_MotionScale_10", "Motion Scale", -9000, 0, 20, false, false),
            new TVEPropertyData("_MotionVariation_10", "Motion Variation", -9000, 0, 20, false, false),

            new TVEPropertyData("_MotionAmplitude_20", "Motion Squash", -9000, 0, 2, false, true),
            new TVEPropertyData("_MotionAmplitude_22", "Motion Rolling", -9000, 0, 2, false, false),
            new TVEPropertyData("_MotionSpeed_20", "Motion Speed", -9000, 0, 40, true, false),
            new TVEPropertyData("_MotionScale_20", "Motion Scale", -9000, 0, 20, false, false),
            new TVEPropertyData("_MotionVariation_20", "Motion Variation", -9000, 0, 20, false, false),

            new TVEPropertyData("_MotionAmplitude_32", "Motion Flutter", -9000, 0, 2, false, true),
            new TVEPropertyData("_MotionSpeed_32", "Motion Speed", -9000, 0, 40, true, false),
            new TVEPropertyData("_MotionScale_32", "Motion Scale", -9000, 0, 20, false, false),
            new TVEPropertyData("_MotionVariation_32", "Motion Variation", -9000, 0, 20, false, false),

            new TVEPropertyData("_InteractionAmplitude", "Interaction Amplitude", -9000, 0, 2, false, true),
            new TVEPropertyData("_InteractionMaskValue", "Interaction Use Mask", -9000, 0, 1, false, false),

            //new TVEMaterialManagerData("_SpaceMotionLocals"),

            //new TVEMaterialManagerData("_MotionValue_20", "Use Branch Motion Settings", -9000, false),
            //new TVEMaterialManagerData("_MotionValue_30", "Use Flutter Motion Settings", -9000, false),
        };

        List<GameObject> selectedObjects = new List<GameObject>();
        List<Material> selectedMaterials = new List<Material>();

        string[] allPresetPaths;
        List<string> presetPaths;
        List<string> presetLines;
        string[] PresetsEnum;

        int presetIndex;
        int settingsIndex;
        int settingsIndexOld = -1;
        int savingIndex = 1;
        string savePath = "";

        bool isValid = true;
        bool showSelection = true;

        bool useLine;
        List<bool> useLines;

        float windPower = 0.5f;

        string userFolder = "Assets/BOXOPHOBIC/User";

        GUIStyle stylePopup;
        GUIStyle styleCenteredHelpBox;

        Color bannerColor;
        string bannerText;
        static TVEMaterialManager window;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Material Manager", false, 2006)]
        public static void ShowWindow()
        {
            window = GetWindow<TVEMaterialManager>(false, "Material Manager", true);
            window.minSize = new Vector2(400, 300);
        }

        void OnEnable()
        {
            bannerColor = new Color(0.890f, 0.745f, 0.309f);
            bannerText = "Material Manager";

            if (TVEManager.Instance == null)
            {
                isValid = false;
            }

            propertyDataSet = new List<TVEPropertyData>();
            propertyDataSet.AddRange(renderData);
            propertyDataSet.AddRange(globalData);
            propertyDataSet.AddRange(mainData);
            propertyDataSet.AddRange(secondData);
            propertyDataSet.AddRange(occlusionData);
            propertyDataSet.AddRange(gradientData);
            propertyDataSet.AddRange(noiseData);
            propertyDataSet.AddRange(subsurfaceData);
            propertyDataSet.AddRange(rimLightData);
            propertyDataSet.AddRange(emissiveData);
            propertyDataSet.AddRange(perspectiveSettings);
            propertyDataSet.AddRange(sizeFadeData);
            propertyDataSet.AddRange(motionData);

            GetPresets();

            userFolder = TVEUtils.GetUserFolder();

            settingsIndex = Convert.ToInt16(SettingsUtils.LoadSettingsData(userFolder + "/Material Settings.asset", "13"));
        }

        void OnSelectionChange()
        {
            Initialize();
            Repaint();
        }

        void OnFocus()
        {
            Initialize();
            Repaint();
        }

        void OnLostFocus()
        {
            ResetEditorWind();
        }

        void OnDisable()
        {
            ResetEditorWind();

            Shader.SetGlobalInt("TVE_ShowIcons", 0);
        }

        void OnDestroy()
        {
            ResetEditorWind();

            Shader.SetGlobalInt("TVE_ShowIcons", 0);
        }

        void OnGUI()
        {
            SetGUIStyles();

            Shader.SetGlobalInt("TVE_ShowIcons", 1);

            GUI_HALF_EDITOR_WIDTH = this.position.width / 2.0f - 24;

            StyledGUI.DrawWindowBanner(bannerColor, bannerText);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            if (isValid && selectedMaterials.Count > 0)
            {
                EditorGUILayout.HelpBox("The Material Manager tool allows to set the same values to all selected material. Please note that Undo is not supported for the Material Manager window!", MessageType.Info, true);
            }
            else
            {
                if (TVEManager.Instance == null)
                {
                    GUILayout.Button("\n<size=14>The Vegetation Engine manager is missing from your scene!</size>\n", styleCenteredHelpBox);

                    GUILayout.Space(10);

                    if (GUILayout.Button("Create Scene Manager", GUILayout.Height(24)))
                    {
                        GameObject manager = new GameObject();
                        manager.AddComponent<TVEManager>();

                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                        isValid = true;
                    }
                }
                else if (selectedMaterials.Count == 0)
                {
                    GUILayout.Button("\n<size=14>Select one or multiple gameobjects or materials to get started!</size>\n", styleCenteredHelpBox);
                }
            }

            if (isValid)
            {
                if (selectedMaterials.Count == 0)
                {
                    GUI.enabled = false;
                }

                DrawWindPower();
                SetEditorWind();

                if (selectedMaterials.Count > 0)
                {
                    GUILayout.Space(5);
                }

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 180));

                DrawMaterials();

                GUILayout.Space(10);

                DrawSettings();
                DrawProperties();

                GUILayout.Space(20);

                DrawSaving();

                SetMaterialProperties();

                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();

            GUILayout.Space(13);
            GUILayout.EndHorizontal();
        }

        void SetGUIStyles()
        {
            stylePopup = new GUIStyle(EditorStyles.popup)
            {
                alignment = TextAnchor.MiddleCenter
            };

            styleCenteredHelpBox = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        void DrawWindPower()
        {
            GUIStyle styleMid = new GUIStyle();
            styleMid.alignment = TextAnchor.MiddleCenter;
            styleMid.normal.textColor = Color.gray;
            styleMid.fontSize = 7;

            GUILayout.Space(10);

            //GUILayout.Label("Wind Power (for testing only)");

            GUILayout.BeginHorizontal();
            GUILayout.Space(4);
            windPower = GUILayout.HorizontalSlider(windPower, 0.0f, 1.0f);

            GUILayout.Space(4);
            GUILayout.EndHorizontal();

            int maxWidth = 20;

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label("None", styleMid, GUILayout.Width(maxWidth));
            GUILayout.Label("", styleMid);
            GUILayout.Label("Windy", styleMid, GUILayout.Width(maxWidth));
            GUILayout.Label("", styleMid);
            GUILayout.Label("Strong", styleMid, GUILayout.Width(maxWidth));
            GUILayout.Space(7);
            GUILayout.EndHorizontal();
        }

        void DrawMaterials()
        {
            if (selectedMaterials.Count > 0)
            {
                GUILayout.Space(10);
            }

            if (showSelection)
            {
                if (StyledButton("Hide Material Selection"))
                    showSelection = !showSelection;
            }
            else
            {
                if (StyledButton("Show Material Selection"))
                    showSelection = !showSelection;
            }
            if (showSelection)
            {
                for (int i = 0; i < selectedMaterials.Count; i++)
                {
                    if (selectedMaterials[i] != null)
                    {
                        StyledMaterial(selectedMaterials[i]);
                    }
                }
            }
        }

        void DrawSettings()
        {
            presetIndex = StyledPopup("Material Preset", presetIndex, PresetsEnum);

            if (presetIndex > 0)
            {
                GetPresetLines();

                for (int i = 0; i < selectedMaterials.Count; i++)
                {
                    var material = selectedMaterials[i];

                    GetMaterialConversionFromPreset(material);
                    TVEUtils.SetMaterialSettings(material);
                }

                propertyDataSet = new List<TVEPropertyData>();
                propertyDataSet.AddRange(renderData);
                propertyDataSet.AddRange(globalData);
                propertyDataSet.AddRange(mainData);
                propertyDataSet.AddRange(secondData);
                propertyDataSet.AddRange(occlusionData);
                propertyDataSet.AddRange(gradientData);
                propertyDataSet.AddRange(noiseData);
                propertyDataSet.AddRange(subsurfaceData);
                propertyDataSet.AddRange(rimLightData);
                propertyDataSet.AddRange(emissiveData);
                propertyDataSet.AddRange(perspectiveSettings);
                propertyDataSet.AddRange(sizeFadeData);
                propertyDataSet.AddRange(motionData);

                GetMaterialProperties();

                presetIndex = 0;
                settingsIndexOld = -1;

                Debug.Log("<b>[The Vegetation Engine]</b> " + "The selected preset has been applied!");
            }

            EditorGUI.BeginChangeCheck();

            settingsIndex = StyledPopup("Material Settings", settingsIndex, materialOptions);

            if (EditorGUI.EndChangeCheck())
            {
#if !THE_VEGETATION_ENGINE_DEVELOPMENT

                SettingsUtils.SaveSettingsData(userFolder + "/Material Settings.asset", settingsIndex);
#endif
            }

            if (settingsIndexOld != settingsIndex)
            {
                propertyDataSet = new List<TVEPropertyData>();

                if (settingsIndex == 0)
                {
                    propertyDataSet.AddRange(renderData);
                    propertyDataSet.AddRange(globalData);
                    propertyDataSet.AddRange(mainData);
                    propertyDataSet.AddRange(secondData);
                    propertyDataSet.AddRange(occlusionData);
                    propertyDataSet.AddRange(gradientData);
                    propertyDataSet.AddRange(noiseData);
                    propertyDataSet.AddRange(subsurfaceData);
                    propertyDataSet.AddRange(rimLightData);
                    propertyDataSet.AddRange(emissiveData);
                    propertyDataSet.AddRange(perspectiveSettings);
                    propertyDataSet.AddRange(sizeFadeData);
                    propertyDataSet.AddRange(motionData);
                }
                else if (settingsIndex == 1)
                {
                    propertyDataSet = renderData;
                }
                else if (settingsIndex == 2)
                {
                    propertyDataSet = globalData;
                }
                else if (settingsIndex == 3)
                {
                    propertyDataSet = mainData;
                }
                else if (settingsIndex == 4)
                {
                    propertyDataSet = secondData;
                }
                else if (settingsIndex == 5)
                {
                    propertyDataSet = occlusionData;
                }
                else if (settingsIndex == 6)
                {
                    propertyDataSet = gradientData;
                }
                else if (settingsIndex == 7)
                {
                    propertyDataSet = noiseData;
                }
                else if (settingsIndex == 8)
                {
                    propertyDataSet = subsurfaceData;
                }
                else if (settingsIndex == 9)
                {
                    propertyDataSet = rimLightData;
                }
                else if (settingsIndex == 10)
                {
                    propertyDataSet = emissiveData;
                }
                else if (settingsIndex == 11)
                {
                    propertyDataSet = perspectiveSettings;
                }
                else if (settingsIndex == 12)
                {
                    propertyDataSet = sizeFadeData;
                }
                else if (settingsIndex == 13)
                {
                    propertyDataSet = motionData;
                }

                settingsIndexOld = settingsIndex;
            }
        }

        void DrawProperties()
        {
            for (int i = 0; i < propertyDataSet.Count; i++)
            {
                var propertyData = propertyDataSet[i];

                if (propertyData.isVisible == 0)
                {
                    continue;
                }

                if (propertyData.space)
                {
                    GUILayout.Space(10);
                }

                if (propertyData.type == TVEPropertyData.PropertyType.Value)
                {
                    propertyData.value = StyledValue(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Range)
                {
                    propertyData.value = StyledSlider(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Vector)
                {
                    propertyData.vector = StyledVector(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Color)
                {
                    propertyData.vector = StyledColor(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Enum)
                {
                    propertyData.value = StyledEnum(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Toggle)
                {
                    propertyData.value = StyledToggle(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Space)
                {
                    StyledSpace(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Category)
                {
                    StyledCategory(propertyData);
                }
                else if (propertyData.type == TVEPropertyData.PropertyType.Message)
                {
                    StyledMessage(propertyData);
                }

                if (propertyData.snap)
                {
                    propertyData.value = Mathf.Round(propertyData.value);
                }
                else
                {
                    propertyData.value = Mathf.Round(propertyData.value * 1000f) / 1000f;
                }
            }
        }

        void StyledMaterial(Material material)
        {
            string color;
            bool useExternalSettings = true;

            if (EditorGUIUtility.isProSkin)
            {
                color = "<color=#87b8ff>";
            }
            else
            {
                color = "<color=#0b448b>";
            }

            if (material.GetTag("UseExternalSettings", false) == "False")
            {
                color = "<color=#808080>";
            }

            GUILayout.Label("<size=10><b>" + color + material.name.Replace(" (TVE Material)", "") + "</color></b></size>", styleCenteredHelpBox, GUILayout.Height(GUI_SELECTION_HEIGHT));

            var lastRect = GUILayoutUtility.GetLastRect();

            var buttonRect = new Rect(lastRect.x, lastRect.y, lastRect.width - 24, lastRect.height);

            if (GUI.Button(buttonRect, "", GUIStyle.none))
            {
                EditorGUIUtility.PingObject(material);
            }

            var toogleRect = new Rect(lastRect.width - 16, lastRect.y + 6, 12, 12);

            if (material.GetTag("UseExternalSettings", false) == "False")
            {
                useExternalSettings = false;
            }

            EditorGUI.BeginChangeCheck();

            useExternalSettings = EditorGUI.Toggle(toogleRect, useExternalSettings);
            GUI.Label(toogleRect, new GUIContent("", "Should the Prefab Settings tool affect the material?"));

            if (useExternalSettings)
            {
                material.SetOverrideTag("UseExternalSettings", "True");
            }
            else
            {
                material.SetOverrideTag("UseExternalSettings", "False");
            }

            if (EditorGUI.EndChangeCheck())
            {
                ResetMaterialData();
                InitCustomMaterials();
                GetMaterialProperties();
            }
        }

        int StyledPopup(string name, int index, string[] options)
        {
            if (index > options.Length)
            {
                index = 0;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
            index = EditorGUILayout.Popup(index, options, stylePopup, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            GUILayout.EndHorizontal();

            return index;
        }

        float StyledValue(TVEPropertyData propertyData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;

                EditorGUILayout.FloatField(0, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }
            else
            {
                if (propertyData.value == -8000)
                {
                    EditorGUI.showMixedValue = true;
                }

                propertyData.value = EditorGUILayout.FloatField(propertyData.value, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.value;
        }

        float StyledSlider(TVEPropertyData propertyData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;

                GUILayout.HorizontalSlider(propertyData.min, propertyData.min, propertyData.max, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 58));
                EditorGUILayout.FloatField(propertyData.min, GUILayout.MaxWidth(GUI_SMALL_WIDTH));
            }
            else
            {
                float equalValue = propertyData.value;
                float mixedValue = 0;

                if (propertyData.value == -8000)
                {
                    EditorGUI.showMixedValue = true;

                    mixedValue = GUILayout.HorizontalSlider(mixedValue, propertyData.min, propertyData.max, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 58));

                    if (mixedValue != 0)
                    {
                        propertyData.value = mixedValue;
                    }

                    float floatVal = EditorGUILayout.FloatField(propertyData.value, GUILayout.MaxWidth(GUI_SMALL_WIDTH));

                    if (propertyData.value != floatVal)
                    {
                        propertyData.value = Mathf.Clamp(floatVal, propertyData.min, propertyData.max);
                    }
                }
                else
                {
                    equalValue = GUILayout.HorizontalSlider(equalValue, propertyData.min, propertyData.max, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 58));

                    propertyData.value = equalValue;

                    float floatVal = EditorGUILayout.FloatField(propertyData.value, GUILayout.MaxWidth(GUI_SMALL_WIDTH));

                    if (propertyData.value != floatVal)
                    {
                        propertyData.value = Mathf.Clamp(floatVal, propertyData.min, propertyData.max);
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.value;
        }

        Vector4 StyledVector(TVEPropertyData propertyData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;

                EditorGUILayout.Vector4Field(new GUIContent(""), Vector4.zero, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }
            else
            {
                if (propertyData.vector.x == -8000)
                {
                    EditorGUI.showMixedValue = true;
                }

                propertyData.vector = EditorGUILayout.Vector4Field(new GUIContent(""), propertyData.vector, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.vector;
        }

        Color StyledColor(TVEPropertyData propertyData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;

                EditorGUILayout.ColorField(new GUIContent(""), Color.gray, true, true, propertyData.hdr, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }
            else
            {
                if (propertyData.vector.x == -8000)
                {
                    EditorGUI.showMixedValue = true;
                }

                propertyData.vector = EditorGUILayout.ColorField(new GUIContent(""), propertyData.vector, true, true, propertyData.hdr, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.vector;
        }

        float StyledEnum(TVEPropertyData propertyData)
        {
            if (Resources.Load<TextAsset>(propertyData.file) != null)
            {
                var layersPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>(propertyData.file));

                StreamReader reader = new StreamReader(layersPath);

                propertyData.options = reader.ReadLine();

                reader.Close();
            }

            string[] enumSplit = propertyData.options.Split(char.Parse(" "));
            List<string> enumOptions = new List<string>(enumSplit.Length / 2);
            List<int> enumIndices = new List<int>(enumSplit.Length / 2);

            for (int i = 0; i < enumSplit.Length; i++)
            {
                if (i % 2 == 0)
                {
                    enumOptions.Add(enumSplit[i].Replace("_", " "));
                }
                else
                {
                    enumIndices.Add(int.Parse(enumSplit[i]));
                }
            }

            int index = (int)propertyData.value;
            int realIndex = enumIndices[0];

            for (int i = 0; i < enumIndices.Count; i++)
            {
                if (enumIndices[i] == index)
                {
                    realIndex = i;
                }
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;

                propertyData.value = EditorGUILayout.Popup(-8000, enumOptions.ToArray(), GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
            }
            else
            {
                if (propertyData.value == -8000)
                {
                    EditorGUI.showMixedValue = true;

                    propertyData.value = EditorGUILayout.Popup(-8000, enumOptions.ToArray(), GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
                }
                else
                {
                    propertyData.value = EditorGUILayout.Popup(realIndex, enumOptions.ToArray(), GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH - 5));
                }
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.value;
        }

        float StyledToggle(TVEPropertyData propertyData)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(propertyData.name, GUILayout.MaxWidth(GUI_HALF_EDITOR_WIDTH));

            if (propertyData.isVisible == propertyData.isLocked)
            {
                GUI.enabled = false;
                EditorGUI.showMixedValue = true;

                EditorGUILayout.Toggle(false);
            }
            else
            {
                bool boolValue = false;

                if (propertyData.value > 0.5f)
                {
                    boolValue = true;
                }

                if (propertyData.value == -8000)
                {
                    EditorGUI.showMixedValue = true;

                    EditorGUI.BeginChangeCheck();

                    boolValue = EditorGUILayout.Toggle(boolValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        if (boolValue)
                        {
                            propertyData.value = 1;
                        }
                        else
                        {
                            propertyData.value = 0;
                        }
                    }

                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    boolValue = EditorGUILayout.Toggle(boolValue);

                    if (boolValue)
                    {
                        propertyData.value = 1;
                    }
                    else
                    {
                        propertyData.value = 0;
                    }
                }
            }

            GUILayout.EndHorizontal();

            GUI.enabled = true;
            EditorGUI.showMixedValue = false;

            return propertyData.value;
        }

        void StyledSpace(TVEPropertyData propertyData)
        {
            GUILayout.Space(10);
        }

        void StyledCategory(TVEPropertyData propertyData)
        {
            GUILayout.Space(10);
            StyledGUI.DrawWindowCategory(propertyData.category);
            GUILayout.Space(10);
        }

        void StyledMessage(TVEPropertyData propertyData)
        {
            GUILayout.Space(propertyData.spaceTop);
            EditorGUILayout.HelpBox(propertyData.message, propertyData.messageType, true);
            GUILayout.Space(propertyData.spaceDown);
        }

        bool StyledButton(string text)
        {
            bool value = GUILayout.Button("<b>" + text + "</b>", styleCenteredHelpBox, GUILayout.Height(GUI_SELECTION_HEIGHT));

            return value;
        }

        void Initialize()
        {
            ResetMaterialData();
            GetSelectedObjects();
            GetPrefabMaterials();
            InitCustomMaterials();
            GetMaterialProperties();
        }

        void ResetMaterialData()
        {
            for (int d = 0; d < propertyDataSet.Count; d++)
            {
                var propertyData = propertyDataSet[d];

                if (propertyData.setter == TVEPropertyData.PropertySetter.Display)
                {
                    propertyData.value = -9000;
                    propertyData.isVisible = 0;
                    propertyData.isLocked = 0;
                }
                else if (propertyData.setter == TVEPropertyData.PropertySetter.Value)
                {
                    propertyData.value = -9000;
                    propertyData.isVisible = 0;
                    propertyData.isLocked = 0;
                }
                else if (propertyData.setter == TVEPropertyData.PropertySetter.Vector)
                {
                    propertyData.vector.x = -9000;
                    propertyData.isVisible = 0;
                    propertyData.isLocked = 0;
                }
            }
        }

        void GetSelectedObjects()
        {
            selectedObjects = new List<GameObject>();
            selectedMaterials = new List<Material>();

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                var selection = Selection.objects[i];

                if (selection.GetType() == typeof(GameObject))
                {
                    selectedObjects.Add((GameObject)selection);
                }

                if (selection.GetType() == typeof(Material))
                {
                    selectedMaterials.Add((Material)selection);
                }
            }
        }

        void GetPrefabMaterials()
        {
            var gameObjects = new List<GameObject>();
            var meshRenderers = new List<MeshRenderer>();

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                gameObjects.Add(selectedObjects[i]);
                GetChildRecursive(selectedObjects[i], gameObjects);
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].GetComponent<MeshRenderer>() != null)
                {
                    meshRenderers.Add(gameObjects[i].GetComponent<MeshRenderer>());
                }
            }

            for (int i = 0; i < meshRenderers.Count; i++)
            {
                if (meshRenderers[i].sharedMaterials != null)
                {
                    for (int j = 0; j < meshRenderers[i].sharedMaterials.Length; j++)
                    {
                        var material = meshRenderers[i].sharedMaterials[j];

                        if (material != null)
                        {
                            if (!selectedMaterials.Contains(material))
                            {
                                selectedMaterials.Add(material);
                            }
                        }
                    }
                }
            }
        }

        void GetChildRecursive(GameObject go, List<GameObject> gameObjects)
        {
            foreach (Transform child in go.transform)
            {
                if (child == null)
                    continue;

                gameObjects.Add(child.gameObject);
                GetChildRecursive(child.gameObject, gameObjects);
            }
        }

        void InitCustomMaterials()
        {
            Material TVEMaterial = null;

            for (int i = 0; i < selectedMaterials.Count; i++)
            {
                var material = selectedMaterials[i];

                if (material.HasProperty("_IsTVEShader"))
                {
                    TVEMaterial = material;
                    break;
                }
            }

            if (TVEMaterial != null)
            {
                for (int i = 0; i < selectedMaterials.Count; i++)
                {
                    var material = selectedMaterials[i];

                    if (material.HasProperty("_IsInitialized"))
                    {
                        if (material.GetInt("_IsInitialized") == 0)
                        {
                            TVEUtils.CopyMaterialProperties(TVEMaterial, material);
                            material.SetInt("_IsInitialized", 1);
                        }
                    }
                }
            }
        }

        void GetMaterialProperties()
        {
            for (int i = 0; i < selectedMaterials.Count; i++)
            {
                var material = selectedMaterials[i];

                for (int d = 0; d < propertyDataSet.Count; d++)
                {
                    var propertyData = propertyDataSet[d];

                    if (material.HasProperty(propertyData.prop))
                    {
                        if (!TVEUtils.GetPropertyVisibility(material, propertyData.prop))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    if (material.GetTag("UseExternalSettings", false) == "False")
                    {
                        continue;
                    }

                    propertyData.isVisible++;

                    if (propertyData.setter == TVEPropertyData.PropertySetter.Display)
                    {
                        propertyData.value = 1;
                    }
                    else if (propertyData.setter == TVEPropertyData.PropertySetter.Value)
                    {
#if UNITY_2022_1_OR_NEWER
                        if (material.IsPropertyLocked(propertyData.prop))
                        {
                            propertyData.isLocked++;
                        }
                        else
#endif
                        {
                            var value = material.GetFloat(propertyData.prop);

                            if (propertyData.value != -9000 && propertyData.value != value)
                            {
                                propertyData.value = -8000;
                            }
                            else
                            {
                                propertyData.value = value;
                            }
                        }
                    }
                    else if (propertyData.setter == TVEPropertyData.PropertySetter.Vector)
                    {
#if UNITY_2022_1_OR_NEWER
                        if (material.IsPropertyLocked(propertyData.prop))
                        {
                            propertyData.isLocked++;
                        }
                        else
#endif
                        {
                            var vector = material.GetVector(propertyData.prop);

                            if (propertyData.vector.x != -9000 && propertyData.vector != vector)
                            {
                                propertyData.vector = new Color(-8000f, 0, 0, 0);
                            }
                            else
                            {
                                propertyData.vector = vector;
                            }
                        }
                    }
                }
            }
        }

        void SetMaterialProperties()
        {
            for (int i = 0; i < selectedMaterials.Count; i++)
            {
                var material = selectedMaterials[i];

                // Maybe a better check for unfocus on Converter Convert button pressed
                if (material != null)
                {
                    TVEUtils.SetMaterialSettings(material);

                    if (material.GetTag("UseExternalSettings", false, "True") == "False")
                    {
                        continue;
                    }

                    for (int d = 0; d < propertyDataSet.Count; d++)
                    {
                        var propertyData = propertyDataSet[d];

                        if (propertyData.setter == TVEPropertyData.PropertySetter.Value)
                        {
                            SetFloatProperty(propertyData, material);
                        }
                        else if (propertyData.setter == TVEPropertyData.PropertySetter.Vector)
                        {
                            SetVectorProperty(propertyData, material);
                        }
                    }
                }
            }
        }

        void SetFloatProperty(TVEPropertyData propertyData, Material material)
        {
            bool isValid = true;

            if (!material.HasProperty(propertyData.prop))
            {
                isValid = false;
            }

#if UNITY_2022_1_OR_NEWER
            if (material.IsPropertyLocked(propertyData.prop))
            {
                isValid = false;
            }
#endif

            if (propertyData.value < -99)
            {
                isValid = false;
            }

            if (isValid)
            {
                material.SetFloat(propertyData.prop, propertyData.value);
            }
        }

        void SetVectorProperty(TVEPropertyData propertyData, Material material)
        {
            bool isValid = true;

            if (!material.HasProperty(propertyData.prop))
            {
                isValid = false;
            }

#if UNITY_2022_1_OR_NEWER
            if (material.IsPropertyLocked(propertyData.prop))
            {
                isValid = false;
            }
#endif

            if (propertyData.vector.x < -99)
            {
                isValid = false;
            }

            if (isValid)
            {
                material.SetVector(propertyData.prop, propertyData.vector);
            }
        }

        void GetPresets()
        {
            presetPaths = new List<string>();
            presetPaths.Add("");

            allPresetPaths = TVEUtils.FindAssets("*.tvepreset", false);

            for (int i = 0; i < allPresetPaths.Length; i++)
            {
                string assetPath = allPresetPaths[i];

                if (assetPath.Contains("[SETTINGS]") == true)
                {
                    presetPaths.Add(assetPath);
                }
            }

            PresetsEnum = new string[presetPaths.Count];
            PresetsEnum[0] = "Choose a preset";

            for (int i = 1; i < presetPaths.Count; i++)
            {
                PresetsEnum[i] = Path.GetFileNameWithoutExtension(presetPaths[i]);
                //PresetsEnum[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(presetPaths[i]).name;
                PresetsEnum[i] = PresetsEnum[i].Replace("[SETTINGS] ", "");
                PresetsEnum[i] = PresetsEnum[i].Replace(" - ", "/");
            }
        }

        void GetPresetLines()
        {
            StreamReader reader = new StreamReader(presetPaths[presetIndex]);

            presetLines = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().TrimStart();

                presetLines.Add(line);
            }

            reader.Close();

            //for (int i = 0; i < presetLines.Count; i++)
            //{
            //    Debug.Log(presetLines[i]);
            //}
        }

        void GetMaterialConversionFromPreset(Material material)
        {
            InitConditionFromLine();

            for (int i = 0; i < presetLines.Count; i++)
            {
                useLine = GetConditionFromLine(presetLines[i], material);

                if (useLine)
                {
                    if (presetLines[i].StartsWith("Material"))
                    {
                        string[] splitLine = presetLines[i].Split(char.Parse(" "));

                        var type = "";
                        var src = "";
                        var dst = "";
                        var set = "";

                        var x = "0";
                        var y = "0";
                        var z = "0";
                        var w = "0";

                        if (splitLine.Length > 1)
                        {
                            type = splitLine[1];
                        }

                        if (splitLine.Length > 2)
                        {
                            src = splitLine[2];
                            set = splitLine[2];
                        }

                        if (splitLine.Length > 3)
                        {
                            dst = splitLine[3];
                            x = splitLine[3];
                        }

                        if (splitLine.Length > 4)
                        {
                            y = splitLine[4];
                        }

                        if (splitLine.Length > 5)
                        {
                            z = splitLine[5];
                        }

                        if (splitLine.Length > 6)
                        {
                            w = splitLine[6];
                        }

                        if (type == "SET_FLOAT")
                        {
                            material.SetFloat(set, float.Parse(x, CultureInfo.InvariantCulture));
                        }
                        else if (type == "SET_COLOR")
                        {
                            material.SetColor(set, new Color(float.Parse(x, CultureInfo.InvariantCulture), float.Parse(y, CultureInfo.InvariantCulture), float.Parse(z, CultureInfo.InvariantCulture), float.Parse(w, CultureInfo.InvariantCulture)));
                        }
                        else if (type == "SET_VECTOR")
                        {
                            material.SetVector(set, new Vector4(float.Parse(x, CultureInfo.InvariantCulture), float.Parse(y, CultureInfo.InvariantCulture), float.Parse(z, CultureInfo.InvariantCulture), float.Parse(w, CultureInfo.InvariantCulture)));
                        }
                        else if (type == "ENABLE_INSTANCING")
                        {
                            material.enableInstancing = true;
                        }
                        else if (type == "DISABLE_INSTANCING")
                        {
                            material.enableInstancing = false;
                        }
                        else if (type == "SET_SHADER_BY_NAME")
                        {
                            var shader = presetLines[i].Replace("Material SET_SHADER_BY_NAME ", "");

                            if (Shader.Find(shader) != null)
                            {
                                material.shader = Shader.Find(shader);
                            }
                        }
                        else if (type == "SET_SHADER_BY_LIGHTING")
                        {
                            var lighting = presetLines[i].Replace("Material SET_SHADER_BY_LIGHTING ", "");

                            var newShaderName = material.shader.name;
                            newShaderName = newShaderName.Replace("Vertex", "XXX");
                            newShaderName = newShaderName.Replace("Simple", "XXX");
                            newShaderName = newShaderName.Replace("Standard", "XXX");
                            newShaderName = newShaderName.Replace("Subsurface", "XXX");
                            newShaderName = newShaderName.Replace("XXX", lighting);

                            if (Shader.Find(newShaderName) != null)
                            {
                                material.shader = Shader.Find(newShaderName);
                            }
                        }
                        else if (type == "SET_SHADER_BY_REPLACE")
                        {
                            var shader = material.shader.name.Replace(src, dst);

                            if (Shader.Find(shader) != null)
                            {
                                material.shader = Shader.Find(shader);
                            }
                        }
                    }
                }
            }
        }

        void InitConditionFromLine()
        {
            useLines = new List<bool>();
            useLines.Add(true);
        }

        bool GetConditionFromLine(string line, Material material)
        {
            var valid = true;

            if (line.StartsWith("if"))
            {
                valid = false;

                string[] splitLine = line.Split(char.Parse(" "));

                var type = "";
                var check = "";
                var val = splitLine[splitLine.Length - 1];

                if (splitLine.Length > 1)
                {
                    type = splitLine[1];
                }

                if (splitLine.Length > 2)
                {
                    for (int i = 2; i < splitLine.Length; i++)
                    {
                        if (!float.TryParse(splitLine[i], out _))
                        {
                            check = check + splitLine[i] + " ";
                        }
                    }

                    check = check.TrimEnd();
                }

                if (type.Contains("SHADER_PATH_CONTAINS"))
                {
                    var path = AssetDatabase.GetAssetPath(material.shader).ToUpperInvariant();

                    if (path.Contains(check.ToUpperInvariant()))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("SHADER_NAME_CONTAINS"))
                {
                    var name = material.shader.name.ToUpperInvariant();

                    if (name.Contains(check.ToUpperInvariant()))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_NAME_CONTAINS"))
                {
                    var name = material.name.ToUpperInvariant();

                    if (name.Contains(check.ToUpperInvariant()))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_PATH_CONTAINS"))
                {
                    var path = AssetDatabase.GetAssetPath(material).ToUpperInvariant();

                    if (path.Contains(check.ToUpperInvariant()))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_NAME_CONTAINS"))
                {
                    if (material.name.Contains(check))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_PIPELINE_IS_STANDARD"))
                {
                    if (material.GetTag("RenderPipeline", false) == "")
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_PIPELINE_IS_UNIVERSAL"))
                {
                    if (material.GetTag("RenderPipeline", false) == "UniversalPipeline")
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_PIPELINE_IS_HD"))
                {
                    if (material.GetTag("RenderPipeline", false) == "HDRenderPipeline")
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_RENDERTYPE_TAG_CONTAINS"))
                {
                    if (material.GetTag("RenderType", false).Contains(check))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_HAS_PROP"))
                {
                    if (material.HasProperty(check))
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_FLOAT_EQUALS"))
                {
                    var min = float.Parse(val, CultureInfo.InvariantCulture) - 0.1f;
                    var max = float.Parse(val, CultureInfo.InvariantCulture) + 0.1f;

                    if (material.HasProperty(check) && material.GetFloat(check) > min && material.GetFloat(check) < max)
                    {
                        valid = true;
                    }
                }
                else if (type.Contains("MATERIAL_KEYWORD_ENABLED"))
                {
                    if (material.IsKeywordEnabled(check))
                    {
                        valid = true;
                    }
                }

                useLines.Add(valid);
            }

            if (line.StartsWith("if") && line.Contains("!"))
            {
                valid = !valid;
                useLines[useLines.Count - 1] = valid;
            }

            if (line.StartsWith("endif") || line.StartsWith("}"))
            {
                useLines.RemoveAt(useLines.Count - 1);
            }

            var useLine = true;

            for (int i = 1; i < useLines.Count; i++)
            {
                if (useLines[i] == false)
                {
                    useLine = false;
                    break;
                }
            }

            return useLine;
        }

        void DrawSaving()
        {
            GUILayout.BeginHorizontal();

            savingIndex = EditorGUILayout.Popup(savingIndex, savingOptions, stylePopup, GUILayout.Width(GUI_HALF_EDITOR_WIDTH - 2), GUILayout.Height(GUI_HEIGHT));

            if (GUILayout.Button("Save Preset", GUILayout.Height(GUI_HEIGHT)))
            {
                savePath = EditorUtility.SaveFilePanelInProject("Save Preset", "Custom - My Preset", "tvepreset", "Use the ' - ' simbol to create categories!");

                if (savePath != "")
                {
                    savePath = savePath.Replace("[SETTINGS] ", "");
                    savePath = savePath.Replace(Path.GetFileName(savePath), "[SETTINGS] " + Path.GetFileName(savePath));

                    StreamWriter writer = new StreamWriter(savePath);
                    savingDataSet = new List<TVEPropertyData>();

                    if (savingIndex == 0)
                    {
                        savingDataSet.AddRange(renderData);
                        savingDataSet.AddRange(globalData);
                        savingDataSet.AddRange(mainData);
                        savingDataSet.AddRange(secondData);
                        savingDataSet.AddRange(occlusionData);
                        savingDataSet.AddRange(gradientData);
                        savingDataSet.AddRange(noiseData);
                        savingDataSet.AddRange(subsurfaceData);
                        savingDataSet.AddRange(rimLightData);
                        savingDataSet.AddRange(emissiveData);
                        savingDataSet.AddRange(perspectiveSettings);
                        savingDataSet.AddRange(sizeFadeData);
                        savingDataSet.AddRange(motionData);
                    }
                    else if (savingIndex == 1)
                    {
                        savingDataSet = propertyDataSet;
                    }

                    for (int i = 0; i < savingDataSet.Count; i++)
                    {
                        if (savingDataSet[i].space == true)
                        {
                            writer.WriteLine("");
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Space)
                        {
                            writer.WriteLine("");
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Category)
                        {
                            writer.WriteLine("");
                            writer.WriteLine("// " + savingDataSet[i].category);
                            writer.WriteLine("");
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Value)
                        {
                            if (savingDataSet[i].value > -99)
                            {
                                writer.WriteLine("Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Range)
                        {
                            if (savingDataSet[i].value > -99)
                            {
                                writer.WriteLine("Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Vector)
                        {
                            if (savingDataSet[i].vector.x > -99)
                            {
                                writer.WriteLine("Material SET_VECTOR " + savingDataSet[i].prop + " " + savingDataSet[i].vector.x.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.y.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.z.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.w.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_VECTOR " + savingDataSet[i].prop + " " + savingDataSet[i].vector.x.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.y.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.z.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.w.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Color)
                        {
                            if (savingDataSet[i].vector.x > -99)
                            {
                                writer.WriteLine("Material SET_VECTOR " + savingDataSet[i].prop + " " + savingDataSet[i].vector.x.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.y.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.z.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.w.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_VECTOR " + savingDataSet[i].prop + " " + savingDataSet[i].vector.x.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.y.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.z.ToString(CultureInfo.InvariantCulture) + " " + savingDataSet[i].vector.w.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Enum)
                        {
                            if (savingDataSet[i].value > -99)
                            {
                                writer.WriteLine("Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        if (savingDataSet[i].type == TVEPropertyData.PropertyType.Toggle)
                        {
                            if (savingDataSet[i].value > -99)
                            {
                                writer.WriteLine("Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteLine("//Material SET_FLOAT " + savingDataSet[i].prop + " " + savingDataSet[i].value.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                    }

                    writer.Close();

                    AssetDatabase.Refresh();

                    GetPresets();

                    // Reset materialData
                    settingsIndexOld = -1;

                    Debug.Log("<b>[The Vegetation Engine]</b> " + "Material preset saved!");

                    GUIUtility.ExitGUI();
                }
            }

            GUILayout.EndHorizontal();
        }

        void ResetEditorWind()
        {
            Shader.SetGlobalVector("TVE_WindEditor", new Vector4(0, 0, windPower, 0));
        }

        void SetEditorWind()
        {
            Shader.SetGlobalVector("TVE_WindEditor", new Vector4(0, 0, windPower, 1));
        }
    }
}
