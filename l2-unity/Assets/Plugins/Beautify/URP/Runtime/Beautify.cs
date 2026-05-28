using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Beautify.Universal {

    [ExecuteInEditMode, VolumeComponentMenu("Kronnect/Beautify")]
    public class Beautify : VolumeComponent, IPostProcessComponent {


        [AttributeUsage(AttributeTargets.Field)]
        public class SectionGroup : Attribute {
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class SettingsGroup : Attribute {

            bool? expanded;

            public bool IsExpanded {
                get {
#if UNITY_EDITOR
                    if (!expanded.HasValue) {
                        expanded = UnityEditor.EditorPrefs.GetBool("BeautifyURP" + GetType().ToString(), false);
                    }
                    return expanded.Value;
#else
                    return false;
#endif
                }
                set {
#if UNITY_EDITOR
                    if (expanded.Value != value) {
                        expanded = value;
                        UnityEditor.EditorPrefs.SetBool("BeautifyURP" + GetType().ToString(), value);
                    }
#endif
                }
            }

        }

        public class GeneralSettings : SectionGroup { }
        public class ImageEnhancement : SectionGroup { }
        public class TonemappingAndColorGrading : SectionGroup { }
        public class LensAndLightingEffects : SectionGroup { }
        public class ArtisticChoices : SectionGroup { }

        public class OptimizeBeautifyBuild : SettingsGroup { }
        public class OptimizeUnityPostProcessingBuild : SettingsGroup { }
        public class Performance : SettingsGroup { }
        public class Dither : SettingsGroup { }
        public class Sharpen : SettingsGroup { }
        public class EdgeAntialiasing : SettingsGroup { }
        public class TonemapSettings : SettingsGroup { }
        public class WhiteBalance : SettingsGroup { }
        public class LUT : SettingsGroup { }
        public class Bloom : SettingsGroup { }
        public class AnamorphicFlares : SettingsGroup { }
        public class SunFlares : SettingsGroup { }
        public class LensDirt : SettingsGroup { }
        public class ChromaticAberration : SettingsGroup { }
        public class DepthOfField : SettingsGroup { }
        public class EyeAdaptation : SettingsGroup { }
        public class PurkinjeShift : SettingsGroup { }

        public class Vignette : SettingsGroup { }
        public class Outline : SettingsGroup { }
        public class NightVision : SettingsGroup { }
        public class ThermalVision : SettingsGroup { }
        public class FinalBlur : SettingsGroup { }
        public class Frame : SettingsGroup { }


        [AttributeUsage(AttributeTargets.Field)]
        public class DisplayName : Attribute {
            public string name;

            public DisplayName(string name) {
                this.name = name;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class DisplayConditionEnum : Attribute {
            public string field;
            public int enumValueIndex;
            public bool isEqual;

            public DisplayConditionEnum(string field, int enumValueIndex, bool isEqual = true) {
                this.field = field;
                this.enumValueIndex = enumValueIndex;
                this.isEqual = isEqual;
            }
        }


        [AttributeUsage(AttributeTargets.Field)]
        public class DisplayConditionBool : Attribute {
            public string field;
            public bool value;
            public string field2;
            public bool value2;

            public DisplayConditionBool(string field, bool value = true, string field2 = null, bool value2 = true) {
                this.field = field;
                this.value = value;
                this.field2 = field2;
                this.value2 = value2;
            }
        }


        [AttributeUsage(AttributeTargets.Field)]
        public class ToggleAllFields : Attribute {
        }


        [AttributeUsage(AttributeTargets.Field)]
        public class GlobalOverride : Attribute {
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class BuildToggle : Attribute {
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class ShowStrippedLabel : Attribute {
        }

        public enum CompareStyle {
            FreeAngle,
            VerticalLine,
            SameSide
        }

        public enum TonemapOperator {
            Linear = 0,
            ACESFitted = 1,
            ACES = 2,
            AGX = 3
        }

        public enum DoFFocusMode {
            FixedDistance,
            AutoFocus,
            FollowTarget
        }

        public enum DoFTargetFallback {
            KeepCurrentFocalDistance,
            SwitchToAutofocus,
            FixedDistanceFocus
        }

        public enum DoFBokehComposition {
            Integrated,
            Separated
        }

        public enum DoFCameraSettings {
            Classic,
            Real
        }

        public enum VignetteFitMode {
            FitToWidth,
            FitToHeight
        }

        public enum DebugOutput {
            Nothing,
            BloomAndFlares = 10,
            BloomExclusionPass = 11,
            AnamorphicFlaresExclusionPass = 12,
            DepthOfFieldCoC = 20,
            DepthOfFieldTransparentPass = 21
        }

        public enum BlinkStyle {
            Cutscene,
            Human
        }

        public enum OutlineStage {
            BeforeBloom = 0,
            AfterBloom = 10
        }

        public enum OutlineTechnique {
            Depth = 0,
            PerObjectId = 1
        }

        public enum DownsamplingMode {
            BeautifyEffectsOnly = 0,
            FullFrame = 10
        }

        public enum SunFlaresDepthOcclusionMode {
            None,
            Simple,
            Smooth
        }

        public enum BloomLayersFilterMethod {
            SelectedLayersOnly,
            ExcludeSelectedLayers
        }

        [Serializable]
        public sealed class BeautifyCompareStyleParameter : VolumeParameter<CompareStyle> { }

        [Serializable]
        public sealed class BeautifyDownsamplingModeParameter : VolumeParameter<DownsamplingMode> { }

        [Serializable]
        public sealed class BeautifyTonemapOperatorParameter : VolumeParameter<TonemapOperator> { }

        [Serializable]
        public sealed class BeautifyDoFFocusModeParameter : VolumeParameter<DoFFocusMode> { }

        [Serializable]
        public sealed class DoFTargetFallbackParameter : VolumeParameter<DoFTargetFallback> { }

        [Serializable]
        public sealed class BeautifyDoFFilterModeParameter : VolumeParameter<FilterMode> { }

        [Serializable]
        public sealed class BeautifyDoFBokehCompositionParameter : VolumeParameter<DoFBokehComposition> { }

        [Serializable]
        public sealed class BeautifyDoFCameraSettingsParameter : VolumeParameter<DoFCameraSettings> { }

        [Serializable]
        public sealed class BeautifyLayerMaskParameter : VolumeParameter<LayerMask> { }

        [Serializable]
        public sealed class BeautifyDebugOutputParameter : VolumeParameter<DebugOutput> {
            public BeautifyDebugOutputParameter(DebugOutput debugOutput = DebugOutput.Nothing, bool overrideState = false) : base(debugOutput, overrideState) { }
        }

        [Serializable]
        public sealed class BeautifyBlinkStyleParameter : VolumeParameter<BlinkStyle> { }

        [Serializable]
        public sealed class BeautifyVignetteFitMode : VolumeParameter<VignetteFitMode> { }

        [Serializable]
        public sealed class BeautifySunFlaresDepthOcclusionMode : VolumeParameter<SunFlaresDepthOcclusionMode> { }


        [Serializable]
        public sealed class MinMaxFloatParameter : VolumeParameter<Vector2> {
            public float min;
            public float max;

            public MinMaxFloatParameter(Vector2 value, float min, float max, bool overrideState = false)
                : base(value, overrideState) {
                this.min = min;
                this.max = max;
            }
        }


        [Serializable]
        public sealed class BeautifyBloomLayersFilterMethodParameter : VolumeParameter<BloomLayersFilterMethod> { }


        #region General settings

        [GeneralSettings, DisplayName("Disable Beautify Effects"), GlobalOverride, Tooltip("Ignore all Beautify effects. This option overrides any existing profile.")]
        public BoolParameter disabled = new BoolParameter(false, overrideState: true);

        [GeneralSettings, DisplayName("Enable Compare Mode"), ToggleAllFields, GlobalOverride]
        public BoolParameter compareMode = new BoolParameter(false, overrideState: true);

        [GeneralSettings, DisplayName("Style"), DisplayConditionBool("compareMode")]
        public BeautifyCompareStyleParameter compareStyle = new BeautifyCompareStyleParameter();

        [GeneralSettings, DisplayName("Panning"), DisplayConditionEnum("compareStyle", (int)CompareStyle.FreeAngle, false)]
        public ClampedFloatParameter comparePanning = new ClampedFloatParameter(0.25f, 0, 0.5f);

        [GeneralSettings, DisplayName("Angle"), DisplayConditionEnum("compareStyle", (int)CompareStyle.FreeAngle, true)]
        public FloatParameter compareLineAngle = new ClampedFloatParameter(1.4f, -Mathf.PI, Mathf.PI);

        [GeneralSettings, DisplayName("Line Width"), DisplayConditionBool("compareMode")]
        public FloatParameter compareLineWidth = new ClampedFloatParameter(0.002f, 0.0001f, 0.05f);

        [GeneralSettings, DisplayName("Flip Vertically"), GlobalOverride, Tooltip("Inverts vertical orientation of image when blitting. This option can be used to overcome an issue in certain versions of URP.")]
        public BoolParameter flipY = new BoolParameter(false, overrideState: true);

        [GeneralSettings, DisplayName("Hide In SceneView"), GlobalOverride]
        public BoolParameter hideInSceneView = new BoolParameter(false, overrideState: true);

        [GeneralSettings, DisplayName("Debug Output"), GlobalOverride, DisplayConditionBool("compareMode", false)]
        public BeautifyDebugOutputParameter debugOutput = new BeautifyDebugOutputParameter();

        [GeneralSettings, Performance, DisplayName("Prioritize Shader Performance"), GlobalOverride, Tooltip("Sharpen, bloom and anamorphic flares will reduce quality a bit to improve performance. This option can be useful on less powerful platforms or devices.")]
        public BoolParameter turboMode = new BoolParameter(false, overrideState: true);

        [GeneralSettings, Performance, DisplayName("Direct Write To Camera"), GlobalOverride, Tooltip("Writes result directly to camera target saving intermediate blits. This option will overwrite any previous post-processing effects so make sure there's no other effects being executed besides Beautify.")]
        public BoolParameter directWrite = new BoolParameter(false, overrideState: true);

        [GeneralSettings, Performance, DisplayName("Ignore Depth Texture"), GlobalOverride, Tooltip("Doesn't request depth texture - effects or options that rely on depth will be disabled.")]
        public BoolParameter ignoreDepthTexture = new BoolParameter(false, overrideState: true);

        [GeneralSettings, Performance, DisplayName("Downsampling"), GlobalOverride, Tooltip("Reduces camera target before applying Beautify effects This option can contribute to compensate render scale if it's set to greater than 1 or to improve performance.")]
        public BoolParameter downsampling = new BoolParameter(false, overrideState: true);

        [GeneralSettings, Performance, DisplayName("Mode"), GlobalOverride, Tooltip("How downsampling is applied."), DisplayConditionBool("downsampling")]
        public BeautifyDownsamplingModeParameter downsamplingMode = new BeautifyDownsamplingModeParameter { value = DownsamplingMode.BeautifyEffectsOnly };

        [GeneralSettings, Performance, DisplayName("Multiplier"), GlobalOverride, Tooltip("Downsampling multiplier."), DisplayConditionBool("downsampling")]
        public ClampedFloatParameter downsamplingMultiplier = new ClampedFloatParameter(1, 1, 64f);

        [GeneralSettings, Performance, DisplayName("Bilinear Filtering"), GlobalOverride, Tooltip("Enables bilinear filtering when using downsampling."), DisplayConditionBool("downsampling")]
        public BoolParameter downsamplingBilinear = new BoolParameter(false);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Automatically Strip Unused Features"), GlobalOverride, BuildToggle, Tooltip("Do not compile any shader features not active in the inspector, reducing build time.")]
        public BoolParameter optimizeBuildBeautifyAuto = new BoolParameter(true, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip ACES Tonemapping"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile ACES tonemapping shader feature, reducing build time.")]
        public BoolParameter stripBeautifyTonemapping = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip ACES Fitted Tonemapping"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile ACES Fitted tonemapping shader feature, reducing build time.")]
        public BoolParameter stripBeautifyTonemappingACESFitted = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip AGX Tonemapping"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile AGX tonemapping shader feature, reducing build time.")]
        public BoolParameter stripBeautifyTonemappingAGX = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Sharpen"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile sharpen shader feature, reducing build time.")]
        public BoolParameter stripBeautifySharpen = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Dithering"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile dithering shader feature, reducing build time.")]
        public BoolParameter stripBeautifyDithering = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Edge Antialiasing"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile edge antialiasing shader feature, reducing build time.")]
        public BoolParameter stripBeautifyEdgeAA = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip LUT"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile LUT shader feature, reducing build time.")]
        public BoolParameter stripBeautifyLUT = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip LUT 3D"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile LUT 3D shader feature, reducing build time.")]
        public BoolParameter stripBeautifyLUT3D = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Sepia, Daltonize & White Balance"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile White Balance shader feature, reducing build time.")]
        public BoolParameter stripBeautifyColorTweaks = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Bloom, Anamorphic & Sun Flares"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Bloom, Anamorphic & Sun Flares shader features, reducing build time.")]
        public BoolParameter stripBeautifyBloom = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Lens Dirt"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Lens Dirt shader feature, reducing build time.")]
        public BoolParameter stripBeautifyLensDirt = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Chromatic Aberration"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Chromatic Aberration shader feature, reducing build time.")]
        public BoolParameter stripBeautifyChromaticAberration = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Depth of Field"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Depth Of Field shader feature, reducing build time.")]
        public BoolParameter stripBeautifyDoF = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Depth of Field Transparent Support"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Depth Of Field transparency support shader feature, reducing build time.")]
        public BoolParameter stripBeautifyDoFTransparentSupport = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Eye Adaptation"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Purkinje Shift shader feature, reducing build time.")]
        public BoolParameter stripBeautifyEyeAdaptation = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Purkinje Shift"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Purkinje Shift shader feature, reducing build time.")]
        public BoolParameter stripBeautifyPurkinje = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Vignetting"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Vignetting shader features, reducing build time.")]
        public BoolParameter stripBeautifyVignetting = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Outline"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Outline shader feature, reducing build time.")]
        public BoolParameter stripBeautifyOutline = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Night Vision"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Night Vision shader feature, reducing build time.")]
        public BoolParameter stripBeautifyNightVision = new BoolParameter(true, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Thermal Vision"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Thermal Vision shader feature, reducing build time.")]
        public BoolParameter stripBeautifyThermalVision = new BoolParameter(true, overrideState: true);

        [GeneralSettings, OptimizeBeautifyBuild, DisplayName("Strip Frame"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildBeautifyAuto", false), Tooltip("Do not compile Frame shader features, reducing build time.")]
        public BoolParameter stripBeautifyFrame = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip All"), GlobalOverride, BuildToggle, Tooltip("Do not compile Unity Post Processing shader features, reducing build time.")]
        public BoolParameter optimizeBuildUnityPPSAuto = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Film Grain"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Film Grain shader feature, reducing build time.")]
        public BoolParameter stripUnityFilmGrain = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Dithering"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Dithering shader feature, reducing build time.")]
        public BoolParameter stripUnityDithering = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Tonemapping"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Tonemapping shader feature, reducing build time.")]
        public BoolParameter stripUnityTonemapping = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Bloom"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Bloom shader feature, reducing build time.")]
        public BoolParameter stripUnityBloom = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Chromatic Aberration"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Chromatic Aberration shader feature, reducing build time.")]
        public BoolParameter stripUnityChromaticAberration = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Distortion"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's Screen Distortion features, reducing build time.")]
        public BoolParameter stripUnityDistortion = new BoolParameter(false, overrideState: true);

        [GeneralSettings, OptimizeUnityPostProcessingBuild, DisplayName("Strip Debug Variants"), GlobalOverride, BuildToggle, DisplayConditionBool("optimizeBuildUnityPPSAuto", false), Tooltip("Do not compile Unity Post Processing's debug variants, reducing build time.")]
        public BoolParameter stripUnityDebugVariants = new BoolParameter(false, overrideState: true);

        #endregion

        #region Sharpen settings

        [ImageEnhancement, Sharpen, DisplayName("Intensity"), DisplayConditionBool("stripBeautifySharpen", false), ShowStrippedLabel]
        public ClampedFloatParameter sharpenIntensity = new ClampedFloatParameter(0f, 0f, 25f);

        [ImageEnhancement, Sharpen, DisplayName("Depth Threshold"), DisplayConditionBool("turboMode", false, "ignoreDepthTexture", false), Tooltip("By default, sharpen ignores edges to avoid aliasing. Increase this property to also include edges. Edge detection is based on scene depth.")]
        public ClampedFloatParameter sharpenDepthThreshold = new ClampedFloatParameter(0.035f, 0f, 0.05f);

        [ImageEnhancement, Sharpen, DisplayName("Depth Range"), DisplayConditionBool("turboMode", false, "ignoreDepthTexture", false), Tooltip("Restricts sharpen to a scene depth range.")]
        public MinMaxFloatParameter sharpenMinMaxDepth = new MinMaxFloatParameter(new Vector2(0, 0.999f), 0, 1.1f);

        [ImageEnhancement, Sharpen, DisplayName("Depth Range FallOff"), DisplayConditionBool("turboMode", false, "ignoreDepthTexture", false)]
        public ClampedFloatParameter sharpenMinMaxDepthFallOff = new ClampedFloatParameter(0f, 0f, 1f);

        [ImageEnhancement, Sharpen, DisplayName("Relaxation"), Tooltip("Reduces sharpen intensity based on area brightness.")]
        public ClampedFloatParameter sharpenRelaxation = new ClampedFloatParameter(0.08f, 0, 0.2f);

        [ImageEnhancement, Sharpen, DisplayName("Clamp"), Tooltip("Reduces final sharpen modifier.")]
        public ClampedFloatParameter sharpenClamp = new ClampedFloatParameter(0.45f, 0, 1f);

        [ImageEnhancement, Sharpen, DisplayName("Motion Sensibility"), Tooltip("Reduces sharpen gracefully when camera moves or rotates. This setting reduces flickering while contributes to a motion blur sense.")]
        public ClampedFloatParameter sharpenMotionSensibility = new ClampedFloatParameter(0.5f, 0, 1f);

        [ImageEnhancement, Sharpen, DisplayName("Motion Restore Speed"), Tooltip("The speed at which the sharpen intensity restores when camera stops moving.")]
        public ClampedFloatParameter sharpenMotionRestoreSpeed = new ClampedFloatParameter(0.5f, 0.01f, 5f);

        #endregion

        #region Edge AA

        [ImageEnhancement, EdgeAntialiasing, DisplayName("Strength"), DisplayConditionBool("stripBeautifyEdgeAA", false, "ignoreDepthTexture", false), Tooltip("Strength of the integrated edge antialiasing. A value of 0 disables this feature."), ShowStrippedLabel]
        public ClampedFloatParameter antialiasStrength = new ClampedFloatParameter(0, 0, 20);

        [ImageEnhancement, EdgeAntialiasing, DisplayName("Edge Threshold"), Tooltip("Minimum difference in depth between neighbour pixels to determine if edge antialiasing should be applied.")]
        public ClampedFloatParameter antialiasDepthThreshold = new ClampedFloatParameter(0.000001f, 0, 0.001f);

        [ImageEnhancement, EdgeAntialiasing, DisplayName("Max Spread"), Tooltip("The maximum extent of antialiasing.")]
        public ClampedFloatParameter antialiasSpread = new ClampedFloatParameter(3f, 0.1f, 8f);

        [ImageEnhancement, EdgeAntialiasing, DisplayName("Depth Attenuation"), Tooltip("Reduces antialias effect on the distance.")]
        public FloatParameter antialiasDepthAttenuation = new FloatParameter(0);

        #endregion

        #region Tonemapping and Color Grading

        [TonemappingAndColorGrading, DisplayConditionBool("stripBeautifyTonemapping", false), ShowStrippedLabel]
        public BeautifyTonemapOperatorParameter tonemap = new BeautifyTonemapOperatorParameter { value = TonemapOperator.Linear };

        [TonemappingAndColorGrading, DisplayName("Gamma"), Tooltip("Gamma applied to the AGX tonemapper."), DisplayConditionEnum("tonemap", (int)TonemapOperator.AGX, true)]
        public FloatParameter tonemapAGXGamma = new ClampedFloatParameter(2.5f, 0, 5f);

        [TonemappingAndColorGrading, Min(0), DisplayName("Max Input Brightness"), Tooltip("Clamps input image brightness to avoid artifacts due to NaN or out of range pixel values."), DisplayConditionEnum("tonemap", (int)TonemapOperator.Linear, false)]
        public FloatParameter tonemapMaxInputBrightness = new FloatParameter(1000f);

        [TonemappingAndColorGrading, Min(0), DisplayName("Pre Exposure"), Tooltip("Brightness multiplier before applying tonemap operator."), DisplayConditionEnum("tonemap", (int)TonemapOperator.Linear, false)]
        public FloatParameter tonemapExposurePre = new FloatParameter(1f);

        [TonemappingAndColorGrading, Min(0), DisplayName("Post Brightness"), Tooltip("Brightness multiplier after applying tonemap operator."), DisplayConditionEnum("tonemap", (int)TonemapOperator.Linear, false)]
        public FloatParameter tonemapBrightnessPost = new FloatParameter(1f);

        [TonemappingAndColorGrading]
        public ClampedFloatParameter saturate = new ClampedFloatParameter(1f, -2f, 3f);

        [TonemappingAndColorGrading]
        public ClampedFloatParameter brightness = new ClampedFloatParameter(1.0f, 0f, 2f);

        [TonemappingAndColorGrading]
        public ClampedFloatParameter contrast = new ClampedFloatParameter(1.0f, 0.5f, 1.5f);

        [TonemappingAndColorGrading, DisplayConditionBool("stripBeautifyColorTweaks", false), ShowStrippedLabel]
        public ClampedFloatParameter daltonize = new ClampedFloatParameter(0f, 0f, 2f);

        [TonemappingAndColorGrading, DisplayConditionBool("stripBeautifyColorTweaks", false), ShowStrippedLabel]
        public ClampedFloatParameter sepia = new ClampedFloatParameter(0f, 0f, 1f);

        [TonemappingAndColorGrading]
        public ColorParameter tintColor = new ColorParameter(new Color(1, 1, 1, 0));

        [TonemappingAndColorGrading, WhiteBalance, DisplayName("Temperature"), DisplayConditionBool("stripBeautifyColorTweaks", false), ShowStrippedLabel]
        public ClampedFloatParameter colorTemp = new ClampedFloatParameter(6550f, 1000f, 40000f);

        [TonemappingAndColorGrading, WhiteBalance, DisplayName("Blend"), DisplayConditionBool("stripBeautifyColorTweaks", false)]
        public ClampedFloatParameter colorTempBlend = new ClampedFloatParameter(0f, 0f, 1f);


        #endregion

        #region LUT

        [TonemappingAndColorGrading, LUT, DisplayName("Enable LUT"), ToggleAllFields, DisplayConditionBool("stripBeautifyLUT", false), ShowStrippedLabel]
        public BoolParameter lut = new BoolParameter(false);

        [TonemappingAndColorGrading, LUT, DisplayName("Intensity")]
        public ClampedFloatParameter lutIntensity = new ClampedFloatParameter(0f, 0, 1f);

        [TonemappingAndColorGrading, LUT, DisplayName("LUT Texture")]
        public TextureParameter lutTexture = new TextureParameter(null);

        #endregion

        #region Bloom

        [LensAndLightingEffects, Bloom, DisplayName("Intensity"), DisplayConditionBool("stripBeautifyBloom", false), ShowStrippedLabel]
        public FloatParameter bloomIntensity = new FloatParameter(0);

        [LensAndLightingEffects, Bloom, DisplayName("Threshold")]
        public FloatParameter bloomThreshold = new FloatParameter(0.75f);

        [LensAndLightingEffects, Bloom, DisplayName("Conservative Threshold"), Tooltip("A convervative threshold keeps the ratio of the rgb values after applying the thresholding")]
        public BoolParameter bloomConservativeThreshold = new BoolParameter(false);

        [LensAndLightingEffects, Bloom, DisplayName("Spread")]
        public ClampedFloatParameter bloomSpread = new ClampedFloatParameter(0.5f, 0, 1);

        [LensAndLightingEffects, Bloom, DisplayName("Max Brightness")]
        public FloatParameter bloomMaxBrightness = new FloatParameter(1000f);

        [LensAndLightingEffects, Bloom, DisplayName("Tint Color"), Tooltip("Use Alpha channel to blend original bloom color with the tinted color.")]
        public ColorParameter bloomTint = new ColorParameter(new Color(0.5f, 0.5f, 1f, 0f));

        [LensAndLightingEffects, Bloom, DisplayName("Depth Attenuation"), DisplayConditionBool("ignoreDepthTexture", false)]
        public FloatParameter bloomDepthAtten = new FloatParameter(0f);

        [LensAndLightingEffects, Bloom, DisplayName("Near Attenuation"), DisplayConditionBool("ignoreDepthTexture", false), Min(0)]
        public FloatParameter bloomNearAtten = new FloatParameter(0f);

        [LensAndLightingEffects, Bloom, DisplayName("Use Layers"), DisplayConditionBool("ignoreDepthTexture", false)]
        public BoolParameter bloomExcludeLayers = new BoolParameter(false);

        [LensAndLightingEffects, Bloom, DisplayName("Include/Exclude Layers"), DisplayConditionBool("bloomExcludeLayers", true, "ignoreDepthTexture", false), Tooltip("Choose if bloom should be applied only to specified layers or if bloom should be excluded from the specified layers.")]
        public BeautifyBloomLayersFilterMethodParameter bloomLayersFilterMethod = new BeautifyBloomLayersFilterMethodParameter { value = BloomLayersFilterMethod.ExcludeSelectedLayers };

        [LensAndLightingEffects, Bloom, DisplayName("Layers"), DisplayConditionBool("bloomExcludeLayers", true, "ignoreDepthTexture", false)]
        public BeautifyLayerMaskParameter bloomExclusionLayerMask = new BeautifyLayerMaskParameter { value = 0 };

        [LensAndLightingEffects, Bloom, DisplayName("Antiflicker")]
        public BoolParameter bloomAntiflicker = new BoolParameter(false);

        [LensAndLightingEffects, Bloom, DisplayName("Resolution")]
        public ClampedIntParameter bloomResolution = new ClampedIntParameter(1, 1, 10);

        [LensAndLightingEffects, Bloom, DisplayName("Quicker Blur")]
        public BoolParameter bloomQuickerBlur = new BoolParameter(false);

        [LensAndLightingEffects, Bloom, DisplayName("Customize"), ToggleAllFields]
        public BoolParameter bloomCustomize = new BoolParameter(false);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 1 Weight")]
        public ClampedFloatParameter bloomWeight0 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 1 Boost")]
        public ClampedFloatParameter bloomBoost0 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 1 Tint Color")]
        public ColorParameter bloomTint0 = new ColorParameter(Color.white);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 2 Weight")]
        public ClampedFloatParameter bloomWeight1 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 2 Boost")]
        public ClampedFloatParameter bloomBoost1 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 2 Tint Color")]
        public ColorParameter bloomTint1 = new ColorParameter(Color.white);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 3 Weight")]
        public ClampedFloatParameter bloomWeight2 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 3 Boost")]
        public ClampedFloatParameter bloomBoost2 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 3 Tint Color")]
        public ColorParameter bloomTint2 = new ColorParameter(Color.white);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 4 Weight")]
        public ClampedFloatParameter bloomWeight3 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 4 Boost")]
        public ClampedFloatParameter bloomBoost3 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 4 Tint Color")]
        public ColorParameter bloomTint3 = new ColorParameter(Color.white);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 5 Weight")]
        public ClampedFloatParameter bloomWeight4 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 5 Boost")]
        public ClampedFloatParameter bloomBoost4 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 5 Tint Color")]
        public ColorParameter bloomTint4 = new ColorParameter(Color.white);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 6 Weight")]
        public ClampedFloatParameter bloomWeight5 = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 6 Boost")]
        public ClampedFloatParameter bloomBoost5 = new ClampedFloatParameter(0f, 0, 3f);

        [LensAndLightingEffects, Bloom, DisplayName("Layer 6 Tint Color")]
        public ColorParameter bloomTint5 = new ColorParameter(Color.white);


        #endregion

        #region Anamorphic flares

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Intensity"), DisplayConditionBool("stripBeautifyBloom", false), ShowStrippedLabel]
        public FloatParameter anamorphicFlaresIntensity = new FloatParameter(0f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Threshold")]
        public FloatParameter anamorphicFlaresThreshold = new FloatParameter(0.75f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Conservative Threshold"), Tooltip("A convervative threshold keeps the ratio of the rgb values after applying the thresholding")]
        public BoolParameter anamorphicFlaresConservativeThreshold = new BoolParameter(false);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Tint Color"), Tooltip("Ignore all Beautify effects. This option overrides any existing profile.")]
        public ColorParameter anamorphicFlaresTint = new ColorParameter(new Color(0.5f, 0.5f, 1f, 0f));

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Vertical")]
        public BoolParameter anamorphicFlaresVertical = new BoolParameter(false);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Spread")]
        public ClampedFloatParameter anamorphicFlaresSpread = new ClampedFloatParameter(1f, 0.1f, 2f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Max Brightness")]
        public FloatParameter anamorphicFlaresMaxBrightness = new FloatParameter(1000f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Depth Attenuation"), DisplayConditionBool("ignoreDepthTexture", false)]
        public FloatParameter anamorphicFlaresDepthAtten = new FloatParameter(0f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Near Attenuation"), DisplayConditionBool("ignoreDepthTexture", false), Min(0)]
        public FloatParameter anamorphicFlaresNearAtten = new FloatParameter(0f);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Use Layers"), DisplayConditionBool("ignoreDepthTexture", false)]
        public BoolParameter anamorphicFlaresExcludeLayers = new BoolParameter(false);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Include/Exclude Layers"), DisplayConditionBool("anamorphicFlaresExcludeLayers", true, "ignoreDepthTexture", false), Tooltip("Choose if the effect should be applied only on the specified layers or if it should be excluded instead.")]
        public BeautifyBloomLayersFilterMethodParameter anamorphicFlaresLayersFilterMethod = new BeautifyBloomLayersFilterMethodParameter { value = BloomLayersFilterMethod.ExcludeSelectedLayers };

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Layers"), DisplayConditionBool("anamorphicFlaresExcludeLayers", true, "ignoreDepthTexture", false)]
        public BeautifyLayerMaskParameter anamorphicFlaresExclusionLayerMask = new BeautifyLayerMaskParameter { value = 0 };

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Antiflicker")]
        public BoolParameter anamorphicFlaresAntiflicker = new BoolParameter(false);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Resolution")]
        public ClampedIntParameter anamorphicFlaresResolution = new ClampedIntParameter(1, 1, 10);

        [LensAndLightingEffects, AnamorphicFlares, DisplayName("Quicker Blur")]
        public BoolParameter anamorphicFlaresQuickerBlur = new BoolParameter(false);

        #endregion

        #region Sun Flares

        [LensAndLightingEffects, SunFlares, DisplayName("Global Intensity"), DisplayConditionBool("stripBeautifyBloom", false), ShowStrippedLabel]
        public ClampedFloatParameter sunFlaresIntensity = new ClampedFloatParameter(0.0f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Tint Color")]
        public ColorParameter sunFlaresTint = new ColorParameter(new Color(1, 1, 1));

        [LensAndLightingEffects, SunFlares, DisplayName("Solar Wind Speed")]
        public ClampedFloatParameter sunFlaresSolarWindSpeed = new ClampedFloatParameter(0.01f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Rotation Dead Zone")]
        public BoolParameter sunFlaresRotationDeadZone = new BoolParameter(false);

        [LensAndLightingEffects, SunFlares, DisplayName("Downsampling")]
        public ClampedIntParameter sunFlaresDownsampling = new ClampedIntParameter(1, 1, 5);

        [LensAndLightingEffects, SunFlares, DisplayName("Depth Occlusion Mode"), DisplayConditionBool("ignoreDepthTexture", false), Tooltip("None = no depth buffer checking. Simple = sample depth buffer at Sun position. Smooth = sample 4 positions around Sun position and interpolate value across frames.")]
        public BeautifySunFlaresDepthOcclusionMode sunFlaresDepthOcclusionMode = new BeautifySunFlaresDepthOcclusionMode { value = SunFlaresDepthOcclusionMode.Simple };

        [LensAndLightingEffects, SunFlares, DisplayName("Occlusion Threshold"), DisplayConditionEnum("sunFlaresDepthOcclusionMode", (int)SunFlaresDepthOcclusionMode.Smooth)]
        public ClampedFloatParameter sunFlaresDepthOcclusionThreshold = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Use Layer Mask"), Tooltip("Specifies if occluding objects will be detected by raycasting.")]
        public BoolParameter sunFlaresUseLayerMask = new BoolParameter(false);

        [LensAndLightingEffects, SunFlares, DisplayName("Occluding Layer Mask"), DisplayConditionBool("sunFlaresUseLayerMask")]
        public BeautifyLayerMaskParameter sunFlaresLayerMask = new BeautifyLayerMaskParameter { value = -1 };

        [LensAndLightingEffects, SunFlares, DisplayName("Occlusion Speed"), DisplayConditionBool("sunFlaresUseLayerMask"), DisplayConditionEnum("sunFlaresDepthOcclusionMode", (int)SunFlaresDepthOcclusionMode.Smooth)]
        public FloatParameter sunFlaresAttenSpeed = new FloatParameter(30f);

        [Header("Sun"), LensAndLightingEffects, SunFlares, DisplayName("Intensity")]
        public ClampedFloatParameter sunFlaresSunIntensity = new ClampedFloatParameter(0.1f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Disk Size")]
        public ClampedFloatParameter sunFlaresSunDiskSize = new ClampedFloatParameter(0.05f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Diffraction Intensity")]
        public ClampedFloatParameter sunFlaresSunRayDiffractionIntensity = new ClampedFloatParameter(3.5f, 0, 25f);

        [LensAndLightingEffects, SunFlares, DisplayName("Diffraction Threshold")]
        public ClampedFloatParameter sunFlaresSunRayDiffractionThreshold = new ClampedFloatParameter(0.13f, 0, 1f);

        [Header("Corona Rays Group 1"), LensAndLightingEffects, SunFlares, DisplayName("Length")]
        public ClampedFloatParameter sunFlaresCoronaRays1Length = new ClampedFloatParameter(0.02f, 0, 0.2f);

        [LensAndLightingEffects, SunFlares, DisplayName("Streaks")]
        public ClampedIntParameter sunFlaresCoronaRays1Streaks = new ClampedIntParameter(12, 2, 30);

        [LensAndLightingEffects, SunFlares, DisplayName("Spread")]
        public ClampedFloatParameter sunFlaresCoronaRays1Spread = new ClampedFloatParameter(0.001f, 0, 0.1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Angle Offset")]
        public ClampedFloatParameter sunFlaresCoronaRays1AngleOffset = new ClampedFloatParameter(0f, 0, 2f * Mathf.PI);

        [Header("Corona Rays Group 2"), LensAndLightingEffects, SunFlares, DisplayName("Length")]
        public ClampedFloatParameter sunFlaresCoronaRays2Length = new ClampedFloatParameter(0.05f, 0, 0.2f);

        [LensAndLightingEffects, SunFlares, DisplayName("Streaks")]
        public ClampedIntParameter sunFlaresCoronaRays2Streaks = new ClampedIntParameter(12, 2, 30);

        [LensAndLightingEffects, SunFlares, DisplayName("Spread")]
        public ClampedFloatParameter sunFlaresCoronaRays2Spread = new ClampedFloatParameter(0.1f, 0, 0.1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Angle Offset")]
        public ClampedFloatParameter sunFlaresCoronaRays2AngleOffset = new ClampedFloatParameter(0f, 0, 2f * Mathf.PI);

        [Header("Ghost 1"), LensAndLightingEffects, SunFlares, DisplayName("Size")]
        public ClampedFloatParameter sunFlaresGhosts1Size = new ClampedFloatParameter(0.03f, 0f, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Offset")]
        public ClampedFloatParameter sunFlaresGhosts1Offset = new ClampedFloatParameter(1.04f, -3f, 3f);

        [LensAndLightingEffects, SunFlares, DisplayName("Brightness")]
        public ClampedFloatParameter sunFlaresGhosts1Brightness = new ClampedFloatParameter(0.037f, 0f, 1f);

        [Header("Ghost 2"), LensAndLightingEffects, SunFlares, DisplayName("Size")]
        public ClampedFloatParameter sunFlaresGhosts2Size = new ClampedFloatParameter(0.1f, 0f, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Offset")]
        public ClampedFloatParameter sunFlaresGhosts2Offset = new ClampedFloatParameter(0.71f, -3f, 3f);

        [LensAndLightingEffects, SunFlares, DisplayName("Brightness")]
        public ClampedFloatParameter sunFlaresGhosts2Brightness = new ClampedFloatParameter(0.03f, 0f, 1f);

        [Header("Ghost 3"), LensAndLightingEffects, SunFlares, DisplayName("Size")]
        public ClampedFloatParameter sunFlaresGhosts3Size = new ClampedFloatParameter(0.24f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Offset")]
        public ClampedFloatParameter sunFlaresGhosts3Offset = new ClampedFloatParameter(0.31f, -3f, 3f);

        [LensAndLightingEffects, SunFlares, DisplayName("Brightness")]
        public ClampedFloatParameter sunFlaresGhosts3Brightness = new ClampedFloatParameter(0.025f, 0f, 1f);

        [Header("Ghost 4"), LensAndLightingEffects, SunFlares, DisplayName("Size")]
        public ClampedFloatParameter sunFlaresGhosts4Size = new ClampedFloatParameter(0.016f, 0f, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Offset")]
        public ClampedFloatParameter sunFlaresGhosts4Offset = new ClampedFloatParameter(0f, -3f, 3f);

        [LensAndLightingEffects, SunFlares, DisplayName("Brightness")]
        public ClampedFloatParameter sunFlaresGhosts4Brightness = new ClampedFloatParameter(0.017f, 0, 1f);

        [Header("Halo"), LensAndLightingEffects, SunFlares, DisplayName("Offset")]
        public ClampedFloatParameter sunFlaresHaloOffset = new ClampedFloatParameter(0.22f, 0, 1f);

        [LensAndLightingEffects, SunFlares, DisplayName("Amplitude")]
        public ClampedFloatParameter sunFlaresHaloAmplitude = new ClampedFloatParameter(15.1415f, 0, 50f);

        [LensAndLightingEffects, SunFlares, DisplayName("Intensity")]
        public ClampedFloatParameter sunFlaresHaloIntensity = new ClampedFloatParameter(0.01f, 0, 1f);

        #endregion

        #region Lens Dirt

        [LensAndLightingEffects, LensDirt, DisplayName("Intensity"), Min(0), DisplayConditionBool("stripBeautifyLensDirt", false), ShowStrippedLabel]
        public FloatParameter lensDirtIntensity = new FloatParameter(0f);

        [LensAndLightingEffects, LensDirt, DisplayName("Threshold")]
        public ClampedFloatParameter lensDirtThreshold = new ClampedFloatParameter(0.5f, 0, 1f);

        [LensAndLightingEffects, LensDirt, DisplayName("Dirt Texture")]
        public TextureParameter lensDirtTexture = new TextureParameter(null);

        [LensAndLightingEffects, LensDirt, DisplayName("Spread")]
        public ClampedIntParameter lensDirtSpread = new ClampedIntParameter(3, 3, 5);

        #endregion

        #region Chromatic Aberration

        [LensAndLightingEffects, ChromaticAberration, DisplayName("Intensity"), Min(0), DisplayConditionBool("stripBeautifyChromaticAberration", false), ShowStrippedLabel]
        public FloatParameter chromaticAberrationIntensity = new ClampedFloatParameter(0f, 0f, 0.1f);

        [LensAndLightingEffects, ChromaticAberration, DisplayName("Hue Shift"), Min(0), DisplayConditionBool("turboMode", false)]
        public ClampedFloatParameter chromaticAberrationShift = new ClampedFloatParameter(0, -1, 1);

        [LensAndLightingEffects, ChromaticAberration, DisplayName("Smoothing"), DisplayConditionBool("turboMode", false)]
        public FloatParameter chromaticAberrationSmoothing = new ClampedFloatParameter(0f, 0f, 32f);

        [LensAndLightingEffects, ChromaticAberration, DisplayName("Separate Pass")]
        public BoolParameter chromaticAberrationSeparatePass = new BoolParameter(false);

        #endregion

        #region Depth of Field

        [LensAndLightingEffects, DepthOfField, DisplayName("Enable"), ToggleAllFields, DisplayConditionBool("stripBeautifyDoF", false, "ignoreDepthTexture", false), ShowStrippedLabel]
        public BoolParameter depthOfField = new BoolParameter(false);

        [Header("Focus")]
        [LensAndLightingEffects, DepthOfField, DisplayName("Focus Mode")]
        public BeautifyDoFFocusModeParameter depthOfFieldFocusMode = new BeautifyDoFFocusModeParameter { value = DoFFocusMode.FixedDistance };

        [LensAndLightingEffects, DepthOfField, DisplayName("Min Distance"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.FixedDistance, isEqual: false)]
        public FloatParameter depthOfFieldAutofocusMinDistance = new FloatParameter(0);

        [LensAndLightingEffects, DepthOfField, DisplayName("Max Distance"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.FixedDistance, isEqual: false)]
        public FloatParameter depthOfFieldAutofocusMaxDistance = new FloatParameter(10000);

        [LensAndLightingEffects, DepthOfField, DisplayName("Fallback"), Tooltip("Focus behaviour if object is not visible on the screen"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.FollowTarget)]
        public DoFTargetFallbackParameter depthOfFieldTargetFallback = new DoFTargetFallbackParameter { value = DoFTargetFallback.KeepCurrentFocalDistance };

        [LensAndLightingEffects, DepthOfField, DisplayName("Distance"), DisplayConditionEnum("depthOfFieldTargetFallback", (int)DoFTargetFallback.FixedDistanceFocus)]
        public FloatParameter depthOfFieldTargetFallbackFixedDistance = new FloatParameter(1000f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Viewport Point"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.AutoFocus)]
        public Vector2Parameter depthOfFieldAutofocusViewportPoint = new Vector2Parameter(new Vector2(0.5f, 0.5f));

        [LensAndLightingEffects, DepthOfField, DisplayName("Distance Shift"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.AutoFocus), Tooltip("Custom distance adjustment (positive or negative)")]
        public FloatParameter depthOfFieldAutofocusDistanceShift = new FloatParameter(0);

        [LensAndLightingEffects, DepthOfField, DisplayName("Layer Mask"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.AutoFocus)]
        public BeautifyLayerMaskParameter depthOfFieldAutofocusLayerMask = new BeautifyLayerMaskParameter { value = -1 };

        //public ClampedFloatParameter depthOfFieldTransparencySupportDownsampling = new ClampedFloatParameter(1f, 1f, 4f);
        //public ClampedFloatParameter depthOfFieldExclusionBias = new ClampedFloatParameter(0.99f, 0.9f, 1f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Distance"), DisplayConditionEnum("depthOfFieldFocusMode", (int)DoFFocusMode.FixedDistance), Min(0)]
        public FloatParameter depthOfFieldDistance = new FloatParameter(10f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Camera Lens Settings")]
        public BeautifyDoFCameraSettingsParameter depthOfFieldCameraSettings = new BeautifyDoFCameraSettingsParameter { value = DoFCameraSettings.Classic };

        [LensAndLightingEffects, DepthOfField, DisplayName("Focal Length"), DisplayConditionEnum("depthOfFieldCameraSettings", 0)]
        public ClampedFloatParameter depthOfFieldFocalLength = new ClampedFloatParameter(0.050f, 0.005f, 0.5f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Aperture"), Min(0), DisplayConditionEnum("depthOfFieldCameraSettings", 0)]
        public FloatParameter depthOfFieldAperture = new FloatParameter(2.8f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Focal Length"), DisplayConditionEnum("depthOfFieldCameraSettings", 1)]
        [Tooltip("The distance between the lens center and the camera's sensor in mm.")]
        public ClampedFloatParameter depthOfFieldFocalLengthReal = new ClampedFloatParameter(50, 1, 300);

        [LensAndLightingEffects, DepthOfField, DisplayName("F-Stop"), DisplayConditionEnum("depthOfFieldCameraSettings", 1)]
        [Tooltip("The F-stop or F-number is the relation between the focal length and the diameter of the aperture")]
        public ClampedFloatParameter depthOfFieldFStop = new ClampedFloatParameter(2, 1, 32);

        [LensAndLightingEffects, DepthOfField, DisplayName("Image Sensor Height"), DisplayConditionEnum("depthOfFieldCameraSettings", 1)]
        [Tooltip("Represents the height of the virtual image sensor. By default, it uses a 24mm image sensor of a classic full-frame camera")]
        public ClampedFloatParameter depthOfFieldImageSensorHeight = new ClampedFloatParameter(24, 1, 48);

        [LensAndLightingEffects, DepthOfField, DisplayName("Focus Speed")]
        public ClampedFloatParameter depthOfFieldFocusSpeed = new ClampedFloatParameter(1f, 0.001f, 5f);

        [Header("Extra Features")]

        [LensAndLightingEffects, DepthOfField, DisplayName("Transparency Support"), DisplayConditionBool("stripBeautifyDoFTransparentSupport", false), ShowStrippedLabel]
        public BoolParameter depthOfFieldTransparentSupport = new BoolParameter(false);

        [LensAndLightingEffects, DepthOfField, DisplayName("Layer Mask"), DisplayConditionBool("depthOfFieldTransparentSupport")]
        public BeautifyLayerMaskParameter depthOfFieldTransparentLayerMask = new BeautifyLayerMaskParameter { value = 1 };

        [LensAndLightingEffects, DepthOfField, DisplayName("Double Sided"), DisplayConditionBool("depthOfFieldTransparentSupport")]
        [Tooltip("When enabled, transparent geometry is rendered with cull off.")]
        public BoolParameter depthOfFieldTransparentDoubleSided = new BoolParameter(false);

        [LensAndLightingEffects, DepthOfField, DisplayName("Transparency Alpha Test Support"), DisplayConditionBool("stripBeautifyDoFTransparentSupport", false), ShowStrippedLabel]
        [Tooltip("When enabled, transparent materials using alpha clipping will be included in the transparent mask")]
        public BoolParameter depthOfFieldAlphaTestSupport = new BoolParameter(false);

        [LensAndLightingEffects, DepthOfField, DisplayName("Layer Mask"), DisplayConditionBool("depthOfFieldAlphaTestSupport")]
        public BeautifyLayerMaskParameter depthOfFieldAlphaTestLayerMask = new BeautifyLayerMaskParameter { value = 1 };

        [LensAndLightingEffects, DepthOfField, DisplayName("Double Sided"), DisplayConditionBool("depthOfFieldAlphaTestSupport")]
        [Tooltip("When enabled, transparent geometry is rendered with cull off.")]
        public BoolParameter depthOfFieldAlphaTestDoubleSided = new BoolParameter(false);

        [LensAndLightingEffects, DepthOfField, DisplayName("Foreground Blur")]
        public BoolParameter depthOfFieldForegroundBlur = new BoolParameter(true);

        [LensAndLightingEffects, DepthOfField, DisplayName("Blur HQ"), DisplayConditionBool("depthOfFieldForegroundBlur")]
        public BoolParameter depthOfFieldForegroundBlurHQ = new BoolParameter(false);

        [LensAndLightingEffects, DepthOfField, DisplayName("Blur Spread"), DisplayConditionBool("depthOfFieldForegroundBlurHQ")]
        public ClampedFloatParameter depthOfFieldForegroundBlurHQSpread = new ClampedFloatParameter(4, 0, 32);

        [LensAndLightingEffects, DepthOfField, DisplayName("Distance"), DisplayConditionBool("depthOfFieldForegroundBlur")]
        public FloatParameter depthOfFieldForegroundDistance = new FloatParameter(0.25f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Bokeh Effect")]
        public BoolParameter depthOfFieldBokeh = new BoolParameter(true);

        [LensAndLightingEffects, DepthOfField, DisplayName("Composition"), DisplayConditionBool("depthOfFieldBokeh")]
        [Tooltip("Specifies if the pass to compute bokeh is integrated (faster) or separated (stronger)")]
        public BeautifyDoFBokehCompositionParameter depthOfFieldBokehComposition = new BeautifyDoFBokehCompositionParameter { value = DoFBokehComposition.Integrated };

        [LensAndLightingEffects, DepthOfField, DisplayName("Threshold"), DisplayConditionBool("depthOfFieldBokeh")]
        public ClampedFloatParameter depthOfFieldBokehThreshold = new ClampedFloatParameter(1f, 0f, 3f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Intensity"), DisplayConditionBool("depthOfFieldBokeh")]
        public ClampedFloatParameter depthOfFieldBokehIntensity = new ClampedFloatParameter(2f, 0, 8f);

        [Header("Quality")]

        [LensAndLightingEffects, DepthOfField, DisplayName("Downsampling")]
        public ClampedIntParameter depthOfFieldDownsampling = new ClampedIntParameter(2, 1, 5);

        [LensAndLightingEffects, DepthOfField, DisplayName("Sample Count")]
        public ClampedIntParameter depthOfFieldMaxSamples = new ClampedIntParameter(6, 2, 16);

        [LensAndLightingEffects, DepthOfField, DisplayName("Max Brightness")]
        public FloatParameter depthOfFieldMaxBrightness = new FloatParameter(1000f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Max Blur Radius")]
        public FloatParameter depthOfFieldMaxBlurRadius = new FloatParameter(1000f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Resolution Invariant"), Tooltip("Adjusts circle of confusion radius based on screen resolution.")]
        public BoolParameter depthOfFieldResolutionInvariant = new BoolParameter(false);
        
        [LensAndLightingEffects, DepthOfField, DisplayName("Max Depth")]
        public ClampedFloatParameter depthOfFieldMaxDistance = new ClampedFloatParameter(1f, 0, 1f);

        [LensAndLightingEffects, DepthOfField, DisplayName("Filter Mode")]
        public BeautifyDoFFilterModeParameter depthOfFieldFilterMode = new BeautifyDoFFilterModeParameter { value = FilterMode.Bilinear };

        #endregion

        #region Eye Adaptation

        [LensAndLightingEffects, EyeAdaptation, DisplayName("Enable"), ToggleAllFields, DisplayConditionBool("stripBeautifyEyeAdaptation", false), ShowStrippedLabel]
        public BoolParameter eyeAdaptation = new BoolParameter(false);

        [LensAndLightingEffects, EyeAdaptation, DisplayName("Min Exposure")]
        public ClampedFloatParameter eyeAdaptationMinExposure = new ClampedFloatParameter(0.2f, 0, 1);

        [LensAndLightingEffects, EyeAdaptation, DisplayName("Max Exposure")]
        public ClampedFloatParameter eyeAdaptationMaxExposure = new ClampedFloatParameter(5f, 1f, 100f);

        [LensAndLightingEffects, EyeAdaptation, DisplayName("Light Adapt Speed")]
        public ClampedFloatParameter eyeAdaptationSpeedToLight = new ClampedFloatParameter(0.4f, 0f, 1f);

        [LensAndLightingEffects, EyeAdaptation, DisplayName("Dark Adapt Speed")]
        public ClampedFloatParameter eyeAdaptationSpeedToDark = new ClampedFloatParameter(0.2f, 0f, 1f);

        #endregion

        #region Purkinje effect

        [LensAndLightingEffects, PurkinjeShift, DisplayName("Enable"), ToggleAllFields, DisplayConditionBool("stripBeautifyPurkinje", false), ShowStrippedLabel]
        public BoolParameter purkinje = new BoolParameter(false);

        [LensAndLightingEffects, PurkinjeShift, DisplayName("Shift Amount")]
        public ClampedFloatParameter purkinjeAmount = new ClampedFloatParameter(1f, 0f, 5f);

        [LensAndLightingEffects, PurkinjeShift, DisplayName("Threshold")]
        public ClampedFloatParameter purkinjeLuminanceThreshold = new ClampedFloatParameter(0.15f, 0f, 1f);

        #endregion

        #region Vignetting

        [ArtisticChoices, Vignette, DisplayName("Outer Ring"), DisplayConditionBool("stripBeautifyVignetting", false), ShowStrippedLabel]
        public ClampedFloatParameter vignettingOuterRing = new ClampedFloatParameter(0f, -2f, 1f);

        [ArtisticChoices, Vignette, DisplayName("Inner Ring")]
        public ClampedFloatParameter vignettingInnerRing = new ClampedFloatParameter(0, 0, 1f);

        [ArtisticChoices, Vignette, DisplayName("Fade")]
        public ClampedFloatParameter vignettingFade = new ClampedFloatParameter(0, 0, 1f);

        [ArtisticChoices, Vignette, DisplayName("Circular Shape")]
        public BoolParameter vignettingCircularShape = new BoolParameter(false);

        [ArtisticChoices, Vignette, DisplayName("Fit Mode"), DisplayConditionBool("vignettingCircularShape")]
        public BeautifyVignetteFitMode vignettingCircularShapeFitMode = new BeautifyVignetteFitMode { value = VignetteFitMode.FitToWidth };

        [ArtisticChoices, Vignette, DisplayName("Aspect Ratio"), DisplayConditionBool("vignettingCircularShape", false)]
        public ClampedFloatParameter vignettingAspectRatio = new ClampedFloatParameter(1f, 0, 1f);

        [ArtisticChoices, Vignette, DisplayName("Blink")]
        public ClampedFloatParameter vignettingBlink = new ClampedFloatParameter(0f, 0, 1);

        [ArtisticChoices, Vignette, DisplayName("Blink Style")]
        public BeautifyBlinkStyleParameter vignettingBlinkStyle = new BeautifyBlinkStyleParameter { value = BlinkStyle.Cutscene };

        [ArtisticChoices, Vignette, DisplayName("Center")]
        public Vector2Parameter vignettingCenter = new Vector2Parameter(new Vector2(0.5f, 0.5f));

        [ArtisticChoices, Vignette, DisplayName("Tint Color")]
        public ColorParameter vignettingColor = new ColorParameter(new Color(0f, 0f, 0f, 1f));

        [ArtisticChoices, Vignette, DisplayName("Texture Mask")]
        public TextureParameter vignettingMask = new TextureParameter(null);

        #endregion

        #region Outline

        [Serializable]
        public sealed class BeautifyOutlineStageParameter : VolumeParameter<OutlineStage> { }

        [Serializable]
        public sealed class BeautifyOutlineTechniqueParameter : VolumeParameter<OutlineTechnique> { }

        [ArtisticChoices, Outline, DisplayConditionBool("stripBeautifyOutline", false, "ignoreDepthTexture", false), ShowStrippedLabel]
        public BoolParameter outline = new BoolParameter(false);

        [ArtisticChoices, Outline, DisplayName("Color")]
        public ColorParameter outlineColor = new ColorParameter(value: new Color(0, 0, 0, 0.8f), hdr: true, showAlpha: false, showEyeDropper: true);

        [ArtisticChoices, Outline, DisplayName("Technique"), Tooltip("Depth: uses scene depth to find edges. Per Object Id: performs a custom pre-pass which renders meshes id to identify independent objects.")]
        public BeautifyOutlineTechniqueParameter outlineTechnique = new BeautifyOutlineTechniqueParameter { value = OutlineTechnique.Depth };

        [ArtisticChoices, Outline, DisplayName("Minimum Separation"), DisplayConditionEnum("outlineTechnique", (int)OutlineTechnique.PerObjectId)]
        public ClampedIntParameter outlineMinSeparation = new ClampedIntParameter(1, 1, 5);

        [ArtisticChoices, Outline, DisplayName("Edge Threshold"), DisplayConditionEnum("outlineTechnique", (int)OutlineTechnique.Depth)]
        public ClampedFloatParameter outlineThreshold = new ClampedFloatParameter(0.2f, 0f, 0.5f);

        [ArtisticChoices, Outline, DisplayName("Depth Diff Threshold"), DisplayConditionEnum("outlineTechnique", (int)OutlineTechnique.Depth)]
        public ClampedFloatParameter outlineMinDepthThreshold = new ClampedFloatParameter(0.0005f, 0f, 0.01f);

        [ArtisticChoices, Outline, DisplayName("Saturation Diff Threshold"), DisplayConditionEnum("outlineTechnique", (int)OutlineTechnique.Depth)]
        public ClampedFloatParameter outlineSaturationDiffThreshold = new ClampedFloatParameter(0.015f, 0f, 0.5f);

        [ArtisticChoices, Outline, DisplayName("Customize")]
        public BoolParameter outlineCustomize = new BoolParameter(false);

        [ArtisticChoices, Outline, DisplayName("Layer Mask"), DisplayConditionBool("outlineCustomize"), Tooltip("Optionally specify which objects should receive the outline effect")]
        public LayerMaskParameter outlineLayerMask = new LayerMaskParameter(-1);

        [ArtisticChoices, Outline, DisplayName("Render Stage"), DisplayConditionBool("outlineCustomize")]
        public BeautifyOutlineStageParameter outlineStageParameter = new BeautifyOutlineStageParameter { value = OutlineStage.BeforeBloom };

        [ArtisticChoices, Outline, DisplayName("Spread"), DisplayConditionBool("outlineCustomize")]
        public ClampedFloatParameter outlineSpread = new ClampedFloatParameter(1f, 0, 1.3f);

        [ArtisticChoices, Outline, DisplayName("Blur Count"), DisplayConditionBool("outlineCustomize")]
        public ClampedIntParameter outlineBlurPassCount = new ClampedIntParameter(1, 1, 5);

        [ArtisticChoices, Outline, DisplayName("Downscale Blur"), DisplayConditionBool("outlineCustomize")]
        public BoolParameter outlineBlurDownscale = new BoolParameter(false);

        [ArtisticChoices, Outline, DisplayName("Intensity Multiplier"), DisplayConditionBool("outlineCustomize")]
        public ClampedFloatParameter outlineIntensityMultiplier = new ClampedFloatParameter(1, 0, 8);

        [Tooltip("Maximum distance in meters from the camera")]
        [ArtisticChoices, Outline, DisplayName("Distance Fade"), DisplayConditionBool("outlineCustomize")]
        public FloatParameter outlineDistanceFade = new FloatParameter(0);

        [ArtisticChoices, Outline, DisplayName("Use Optimized Shader"), DisplayConditionBool("outlineCustomize"), Tooltip("Uses a depth-only custom shader which should be faster. This option is only used when outline technique is set to Depth. If the scene uses shaders that transform the vertices coordinates, you may want to disable this option.")]
        public BoolParameter outlineUsesOptimizedShader = new BoolParameter(false);

#if UNITY_2022_3_OR_NEWER
        [ArtisticChoices, Outline, DisplayName("Cut Off"), DisplayConditionBool("outlineUsesOptimizedShader"), Tooltip("Optionally set the alpha test/cut off. Only applies if optimized shader is used.")]
        public ClampedFloatParameter outlineLayerCutOff = new ClampedFloatParameter(0f, 0, 1);
#endif


        #endregion

        #region Night Vision

        [ArtisticChoices, NightVision, DisplayName("Enable"), DisplayConditionBool("stripBeautifyNightVision", false), ShowStrippedLabel]
        public BoolParameter nightVision = new BoolParameter(false);

        [ArtisticChoices, NightVision, DisplayName("Scan Lines Color"), DisplayConditionBool("stripBeautifyNightVision", false)]
        public ColorParameter nightVisionColor = new ColorParameter(new Color(0.5f, 1f, 0.5f, 0.5f));

        [ArtisticChoices, NightVision, DisplayName("Max Visible Depth"), DisplayConditionBool("stripBeautifyNightVision", false)]
        public FloatParameter nightVisionDepth = new FloatParameter(100000);

        [ArtisticChoices, NightVision, DisplayName("Max Visible Depth Fall Off"), DisplayConditionBool("stripBeautifyNightVision", false)]
        public ClampedFloatParameter nightVisionDepthFallOff = new ClampedFloatParameter(1, 0.01f, 10);

        #endregion

        #region Thermal Vision

        [ArtisticChoices, ThermalVision, DisplayName("Enable"), DisplayConditionBool("stripBeautifyThermalVision", false), ShowStrippedLabel]
        public BoolParameter thermalVision = new BoolParameter(false);

        [ArtisticChoices, ThermalVision, DisplayName("Scan Lines"), DisplayConditionBool("stripBeautifyThermalVision", false)]
        public BoolParameter thermalVisionScanLines = new BoolParameter(true);

        [ArtisticChoices, ThermalVision, DisplayName("Distortion Amount"), DisplayConditionBool("stripBeautifyThermalVision", false)]
        public ClampedFloatParameter thermalVisionDistortionAmount = new ClampedFloatParameter(9f, 0, 100f);

        #endregion

        #region RGB Dither

        [ImageEnhancement, Dither, DisplayName("Intensity"), DisplayConditionBool("stripBeautifyDithering", false), ShowStrippedLabel]
        public ClampedFloatParameter ditherIntensity = new ClampedFloatParameter(0.0f, 0, 0.02f);

        #endregion

        #region Frame

        public enum FrameStyle {
            Border,
            CinematicBands
        }

        [Serializable]
        public sealed class BeautifyFrameStyleParameter : VolumeParameter<FrameStyle> { }

        [ArtisticChoices, Frame, DisplayName("Enable"), DisplayConditionBool("stripBeautifyFrame", false)]
        public BoolParameter frame = new BoolParameter(false);

        [ArtisticChoices, Frame, DisplayName("Style")]
        public BeautifyFrameStyleParameter frameStyle = new BeautifyFrameStyleParameter { value = FrameStyle.Border };

        [ArtisticChoices, Frame, DisplayName("Horizontal Bands Size"), DisplayConditionEnum("frameStyle", (int)FrameStyle.CinematicBands)]
        public ClampedFloatParameter frameBandHorizontalSize = new ClampedFloatParameter(0, 0, 0.5f);

        [ArtisticChoices, Frame, DisplayName("Horizontal Bands Smoothness"), DisplayConditionEnum("frameStyle", (int)FrameStyle.CinematicBands)]
        public ClampedFloatParameter frameBandHorizontalSmoothness = new ClampedFloatParameter(0, 0, 1);

        [ArtisticChoices, Frame, DisplayName("Vertical Bands Size"), DisplayConditionEnum("frameStyle", (int)FrameStyle.CinematicBands)]
        public ClampedFloatParameter frameBandVerticalSize = new ClampedFloatParameter(0.1f, 0, 0.5f);

        [ArtisticChoices, Frame, DisplayName("Vertical Bands Smoothness"), DisplayConditionEnum("frameStyle", (int)FrameStyle.CinematicBands)]
        public ClampedFloatParameter frameBandVerticalSmoothness = new ClampedFloatParameter(0, 0, 1);

        [ArtisticChoices, Frame, DisplayName("Color"), DisplayConditionEnum("frameStyle", (int)FrameStyle.Border)]
        public ColorParameter frameColor = new ColorParameter(new Color(1, 1, 1, 0.047f));

        [ArtisticChoices, Frame, DisplayName("Texture Mask")]
        public TextureParameter frameMask = new TextureParameter(null);



        #endregion

        #region Final Blur

        [ArtisticChoices, FinalBlur, DisplayName("Intensity")]
        public ClampedFloatParameter blurIntensity = new ClampedFloatParameter(0f, 0f, 64f);

        [ArtisticChoices, FinalBlur, DisplayName("Mask")]
        public TextureParameter blurMask = new TextureParameter(null);

        [ArtisticChoices, FinalBlur, DisplayName("Keep Source On Top")]
        public BoolParameter blurKeepSourceOnTop = new BoolParameter(false);

        [ArtisticChoices, FinalBlur, DisplayName("Rect"), DisplayConditionBool("blurKeepSourceOnTop")]
        public Vector4Parameter blurSourceRect = new Vector4Parameter(new Vector4(0.1f, 0.1f, 0.8f, 0.8f));

        [ArtisticChoices, FinalBlur, DisplayName("Edge Blending Width"), DisplayConditionBool("blurKeepSourceOnTop")]
        public ClampedFloatParameter blurSourceEdgeBlendWidth = new ClampedFloatParameter(0, 0, 1);

        [ArtisticChoices, FinalBlur, DisplayName("Edge Blending Strength"), DisplayConditionBool("blurKeepSourceOnTop")]
        public FloatParameter blurSourceEdgeBlendStrength = new FloatParameter(20);

        #endregion

        public bool IsActive() => !disabled.value;

        public bool IsTileCompatible() => true;

        public bool RequiresDepthTexture() {
            return sharpenIntensity.value > 0 || antialiasStrength.value > 0 || depthOfField.value || bloomDepthAtten.value > 0 || bloomNearAtten.value > 0 || anamorphicFlaresDepthAtten.value > 0 || anamorphicFlaresNearAtten.value > 0 || sunFlaresIntensity.value > 0 || outline.value;
        }

        int downsamplingUsed;


        void OnValidate() {
            bloomIntensity.value = Mathf.Max(bloomIntensity.value, 0);
            bloomDepthAtten.value = Mathf.Max(bloomDepthAtten.value, 0);
            bloomNearAtten.value = Mathf.Max(bloomNearAtten.value, 0);
            bloomThreshold.value = Mathf.Max(bloomThreshold.value, 0);
            anamorphicFlaresIntensity.value = Mathf.Max(anamorphicFlaresIntensity.value, 0);
            anamorphicFlaresDepthAtten.value = Mathf.Max(anamorphicFlaresDepthAtten.value, 0);
            anamorphicFlaresNearAtten.value = Mathf.Max(anamorphicFlaresNearAtten.value, 0);
            anamorphicFlaresThreshold.value = Mathf.Max(anamorphicFlaresThreshold.value, 0);
            depthOfFieldDistance.value = Mathf.Max(depthOfFieldDistance.value, depthOfFieldFocalLength.value);
            outlineDistanceFade.value = Mathf.Max(outlineDistanceFade.value, 0);
            antialiasDepthAttenuation.value = Mathf.Max(antialiasDepthAttenuation.value, 0);
            nightVisionDepth.value = Mathf.Max(0, nightVisionDepth.value);
            depthOfFieldMaxBlurRadius.value = Mathf.Max(0, depthOfFieldMaxBlurRadius.value);
            tonemapMaxInputBrightness.value = Mathf.Max(0, tonemapMaxInputBrightness.value);

            if (!downsampling.value && downsamplingUsed == 1) {
                // deactivate downsampling on the pipeline
                UniversalRenderPipelineAsset pipe = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                pipe.renderScale = 1f;
            }
            downsamplingUsed = downsampling.value ? 1 : -1;
            blurSourceEdgeBlendStrength.value = Mathf.Max(blurSourceEdgeBlendStrength.value, 0);

        }


    }
}
