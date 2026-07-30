using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Beautify.Universal {

    public delegate float OnBeforeFocusEvent(float currentFocusDistance);


    [ExecuteInEditMode]
    public class BeautifySettings : MonoBehaviour {

        [Header("Scene Settings")]
        public Transform sun;
        public Transform depthOfFieldTarget;

        public OnBeforeFocusEvent OnBeforeFocus;

        public static float depthOfFieldCurrentFocalPointDistance;

        public static int bloomExcludeMask;

        public static int anamorphicFlaresExcludeMask;

        public static bool dofTransparentSupport;

        public static int dofTransparentLayerMask;

        public static bool dofTransparentDoubleSided;

        public static bool dofAlphaTestSupport;

        public static int dofAlphaTestLayerMask;

        public static bool dofAlphaTestDoubleSided;

        public static bool outlineDepthPrepass;
        public static bool outlineUseObjectId;
        public static int outlineLayerMask;
        public static float outlineLayerCutOff;

        public static bool outlineDepthPrepassUseOptimizedShader;

        internal static bool _refreshAlphaClipRenderers;

        static BeautifySettings _instance;
        static Volume _beautifyVolume;
        static Beautify _beautify;

        /// <summary>
        /// Forces a reset of the internal cached settings of Beautify. Call this method if Beautify settings are not resetted when switching scenes.
        /// </summary>
        public static void UnloadBeautify() {
            _instance = null;
            _beautifyVolume = null;
            _beautify = null;
        }

        /// <summary>
        /// Returns a reference to the Beautify Settings component attached to the Post Processing Layer or camera
        /// </summary>
        /// <value>The instance.</value>
        public static BeautifySettings instance {
            get {
                if (_instance == null) {
#if UNITY_2023_1_OR_NEWER
                    _instance = FindAnyObjectByType<BeautifySettings>();
#else
                    _instance = FindObjectOfType<BeautifySettings>();
#endif
                    if (_instance == null) {
                        // Check if there's a single volume component, then add BeautifySettings singleton to that gameobject
                        _beautifyVolume = FindBeautifyVolume();
                        if (_beautifyVolume == null) {
                            return null;
                        }
                        GameObject go = _beautifyVolume.gameObject;
                        _instance = go.GetComponent<BeautifySettings>();
                        if (_instance == null) {
                            _instance = go.AddComponent<BeautifySettings>();
                        }
                    }
                }
                return _instance;
            }
        }


        static Volume FindBeautifyVolume() {
#if UNITY_2023_1_OR_NEWER
            Volume[] vols = FindObjectsByType<Volume>(FindObjectsSortMode.None);
#else
            Volume[] vols = FindObjectsOfType<Volume>();
#endif
            foreach (Volume volume in vols) {
                if (volume.sharedProfile != null && volume.sharedProfile.Has<Beautify>()) {
                    _beautifyVolume = volume;
                    return volume;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a reference to the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The shared settings.</value>
        public static Beautify sharedSettings {
            get {
                if (_beautify != null) return _beautify;
                if (_beautifyVolume == null) FindBeautifyVolume();
                if (_beautifyVolume == null) return null;

                bool foundEffectSettings = _beautifyVolume.sharedProfile.TryGet(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }

        /// <summary>
        /// Returns the profile of the volume in the scene where Beautify has been found
        /// </summary>
        public static VolumeProfile currentProfile {
            get {
                if (_beautifyVolume == null) {
                    FindBeautifyVolume();
                }
                if (_beautifyVolume == null) return null;
                return _beautifyVolume.sharedProfile;
            }
        }

        /// <summary>
        /// Returns a copy of the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The settings.</value>
        public static Beautify settings {
            get {
                if (_beautify != null) return _beautify;
                if (_beautifyVolume == null) FindBeautifyVolume();
                if (_beautifyVolume == null) return null;

                bool foundEffectSettings = _beautifyVolume.profile.TryGet(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }


        public static void Blink(float duration, float maxValue = 1) {
            if (duration <= 0)
                return;
            BeautifySettings i = instance;
            if (i == null) return;
            i.StartCoroutine(i.DoBlink(duration, maxValue));
        }

        IEnumerator DoBlink(float duration, float maxValue) {

            Beautify beautify = settings;
            if (beautify == null) yield break;
            float start = Time.time;
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            beautify.vignettingBlink.overrideState = true;
            float t;
            // Close
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeOut = t * (2f - t);
                beautify.vignettingBlink.value = easeOut * maxValue;
                yield return w;
            } while (t < 1f);

            // Open
            start = Time.time;
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeIn = t * t;
                beautify.vignettingBlink.value = (1f - easeIn) * maxValue;
                yield return w;
            } while (t < 1f);
            beautify.vignettingBlink.overrideState = false;
        }

        void OnEnable() {
#if UNITY_EDITOR
            ManageBuildOptimizationStatus(true);
#endif
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
            UnloadBeautify();
        }


        /// <summary>
        /// Updates the list of alpha clip renderers
        /// </summary>
        public static void UpdateAlphaClipRenderers() {
            _refreshAlphaClipRenderers = true;
        }


#if UNITY_EDITOR
        static bool wasBuildOptActive;
        public static void ManageBuildOptimizationStatus(bool force) {
            Beautify beautify = sharedSettings;
            if (beautify == null) return;

            if (!beautify.active && (wasBuildOptActive || force)) {
                StripBeautifyKeywords();
            } else if (beautify.active && (!wasBuildOptActive || force)) {
                SetStripShaderKeywords(beautify);
            }
            wasBuildOptActive = beautify.active;
        }

        const string PLAYER_PREF_KEYNAME = "BeautifyStripKeywordSet";


        public static void StripBeautifyKeywords() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(BeautifyRendererFeature.SKW_BLOOM);
            sb.Append(BeautifyRendererFeature.SKW_BLOOM_USE_DEPTH);
            sb.Append(BeautifyRendererFeature.SKW_DEPTH_OF_FIELD);
            sb.Append(BeautifyRendererFeature.SKW_DEPTH_OF_FIELD_TRANSPARENT);
            sb.Append(BeautifyRendererFeature.SKW_DIRT);
            sb.Append(BeautifyRendererFeature.SKW_LUT);
            sb.Append(BeautifyRendererFeature.SKW_LUT3D);
            sb.Append(BeautifyRendererFeature.SKW_OUTLINE);
            sb.Append(BeautifyRendererFeature.SKW_NIGHT_VISION);
            sb.Append(BeautifyRendererFeature.SKW_PURKINJE);
            sb.Append(BeautifyRendererFeature.SKW_TONEMAP_ACES);
            sb.Append(BeautifyRendererFeature.SKW_TONEMAP_ACES_FITTED);
            sb.Append(BeautifyRendererFeature.SKW_TONEMAP_AGX);
            sb.Append(BeautifyRendererFeature.SKW_VIGNETTING);
            sb.Append(BeautifyRendererFeature.SKW_VIGNETTING_MASK);
            sb.Append(BeautifyRendererFeature.SKW_COLOR_TWEAKS);
            sb.Append(BeautifyRendererFeature.SKW_TURBO);
            sb.Append(BeautifyRendererFeature.SKW_DITHER);
            sb.Append(BeautifyRendererFeature.SKW_SHARPEN);
            sb.Append(BeautifyRendererFeature.SKW_EYE_ADAPTATION);
            sb.Append(BeautifyRendererFeature.SKW_CHROMATIC_ABERRATION);
            PlayerPrefs.SetString(PLAYER_PREF_KEYNAME, sb.ToString());
        }

        public static void SetStripShaderKeywords(Beautify beautify) {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool auto = beautify.optimizeBuildBeautifyAuto.value;
            if (auto) {
                beautify.stripBeautifyBloom.value = false;
                beautify.stripBeautifyDoF.value = false;
                beautify.stripBeautifyDoFTransparentSupport.value = false;
                beautify.stripBeautifyLensDirt.value = false;
                beautify.stripBeautifyLUT.value = false;
                beautify.stripBeautifyLUT3D.value = false;
                beautify.stripBeautifyOutline.value = false;
                beautify.stripBeautifyNightVision.value = false;
                beautify.stripBeautifyThermalVision.value = false;
                beautify.stripBeautifyColorTweaks.value = false;
                beautify.stripBeautifyPurkinje.value = false;
                beautify.stripBeautifyTonemapping.value = false;
                beautify.stripBeautifyTonemappingACESFitted.value = false;
                beautify.stripBeautifyTonemappingAGX.value = false;
                beautify.stripBeautifyDithering.value = false;
                beautify.stripBeautifySharpen.value = false;
                beautify.stripBeautifyEyeAdaptation.value = false;
                beautify.stripBeautifyVignetting.value = false;
            }
            if (beautify.stripBeautifyEdgeAA.value || (auto && beautify.antialiasStrength.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_EDGE_ANTIALIASING);
            }
            if (beautify.stripBeautifyBloom.value || (auto && beautify.bloomIntensity.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_BLOOM);
                sb.Append(BeautifyRendererFeature.SKW_BLOOM_USE_DEPTH);
            }
            if (beautify.stripBeautifyDoF.value || (auto && !beautify.depthOfField.value)) {
                sb.Append(BeautifyRendererFeature.SKW_DEPTH_OF_FIELD);
            }
            if (beautify.stripBeautifyDoF.value || (auto && !(beautify.depthOfFieldTransparentSupport.value || beautify.depthOfFieldAlphaTestSupport.value) )) {
                sb.Append(BeautifyRendererFeature.SKW_DEPTH_OF_FIELD_TRANSPARENT);
            }
            if (beautify.stripBeautifyLensDirt.value || (auto && beautify.lensDirtIntensity.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_DIRT);
            }
            bool usesLUT = beautify.lut.value && beautify.lutTexture.value != null;
            bool usesLUT3D = usesLUT && beautify.lutTexture.value is Texture3D;
            if (beautify.stripBeautifyLUT3D.value || (auto && !usesLUT3D)) {
                sb.Append(BeautifyRendererFeature.SKW_LUT3D);
            }
            if (beautify.stripBeautifyLUT.value || (auto && !usesLUT)) {
                sb.Append(BeautifyRendererFeature.SKW_LUT);
            }
            if (beautify.stripBeautifyOutline.value || (auto && !beautify.outline.value)) {
                sb.Append(BeautifyRendererFeature.SKW_OUTLINE);
            }
            if (beautify.stripBeautifyNightVision.value || (auto && !beautify.nightVision.value)) {
                sb.Append(BeautifyRendererFeature.SKW_NIGHT_VISION);
            }
            if (beautify.stripBeautifyThermalVision.value || (auto && !beautify.nightVision.value)) {
                sb.Append(BeautifyRendererFeature.SKW_THERMAL_VISION);
            }
            bool usesColorTweaks = beautify.sepia.value > 0 || beautify.daltonize.value > 0 || beautify.colorTempBlend.value > 0;
            if (beautify.stripBeautifyColorTweaks.value || (auto && !usesColorTweaks)) {
                sb.Append(BeautifyRendererFeature.SKW_COLOR_TWEAKS);
            }
            if (beautify.stripBeautifyPurkinje.value || (auto && !beautify.purkinje.value)) {
                sb.Append(BeautifyRendererFeature.SKW_PURKINJE);
            }
            if (beautify.stripBeautifyTonemapping.value || (auto && beautify.tonemap.value != Beautify.TonemapOperator.ACES)) {
                sb.Append(BeautifyRendererFeature.SKW_TONEMAP_ACES);
            }
            if (beautify.stripBeautifyTonemappingACESFitted.value || (auto && beautify.tonemap.value != Beautify.TonemapOperator.ACESFitted)) {
                sb.Append(BeautifyRendererFeature.SKW_TONEMAP_ACES_FITTED);
            }
            if (beautify.stripBeautifyTonemappingAGX.value || (auto && beautify.tonemap.value != Beautify.TonemapOperator.AGX)) {
                sb.Append(BeautifyRendererFeature.SKW_TONEMAP_AGX);
            }
            if (beautify.stripBeautifyDithering.value || (auto && beautify.ditherIntensity.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_DITHER);
            }
            if (beautify.stripBeautifySharpen.value || (auto && beautify.sharpenIntensity.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_SHARPEN);
            }
            if (beautify.stripBeautifyEyeAdaptation.value || (auto && !beautify.eyeAdaptation.value)) {
                sb.Append(BeautifyRendererFeature.SKW_EYE_ADAPTATION);
            }
            if (beautify.stripBeautifyChromaticAberration.value || (auto && beautify.chromaticAberrationIntensity.value <= 0)) {
                sb.Append(BeautifyRendererFeature.SKW_CHROMATIC_ABERRATION);
            }

            float outerRing = 1f - beautify.vignettingOuterRing.value;
            float innerRing = 1f - beautify.vignettingInnerRing.value;
            bool vignettingEnabled = outerRing < 1 || innerRing < 1f || beautify.vignettingFade.value > 0 || beautify.vignettingBlink.value > 0;
            if (beautify.stripBeautifyVignetting.value || (auto && !vignettingEnabled)) {
                sb.Append(BeautifyRendererFeature.SKW_VIGNETTING);
                sb.Append(BeautifyRendererFeature.SKW_VIGNETTING_MASK);
            }

            bool stripUnityPPS = beautify.optimizeBuildUnityPPSAuto.value;
            if (beautify.stripUnityBloom.value || stripUnityPPS) {
                sb.Append("_BLOOM_LQ _BLOOM_HQ _BLOOM_LQ_DIRT _BLOOM_HQ_DIRT");
            }
            if (beautify.stripUnityChromaticAberration.value || stripUnityPPS) {
                sb.Append("_CHROMATIC_ABERRATION");
            }
            if (beautify.stripUnityDistortion.value || stripUnityPPS) {
                sb.Append("_DISTORTION");
            }
            if (beautify.stripUnityFilmGrain.value || stripUnityPPS) {
                sb.Append("_FILM_GRAIN");
            }
            if (beautify.stripUnityDithering.value || stripUnityPPS) {
                sb.Append("_DITHERING");
            }
            if (beautify.stripUnityTonemapping.value || stripUnityPPS) {
                sb.Append("_TONEMAP_ACES _TONEMAP_NEUTRAL");
            }
            if (beautify.stripUnityDebugVariants.value || stripUnityPPS) {
                sb.Append("DEBUG_DISPLAY");
            }
            PlayerPrefs.SetString(PLAYER_PREF_KEYNAME, sb.ToString());
        }
#endif
    }
}
