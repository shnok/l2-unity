// Cristian Pop - https://boxophobic.com/

using UnityEditor;

namespace AtmosphericHeightFog
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HeightFogGlobal))]
    public class HeightFogGlobalInspector : Editor
    {
        readonly string[] scriptMode = { "m_Script", "presetMaterial", "presetDay", "presetNight", "timeOfDay" };
        readonly string[] presetMode = { "m_Script", "presetDay", "presetNight", "timeOfDay", "categoryFog", "fogIntensity", "fogAxisMode", "fogLayersMode", "fogCameraMode", "fogColorStart", "fogColorEnd", "fogColorDuo", "fogDistanceStart", "fogDistanceEnd", "fogDistanceFalloff", "fogHeightStart", "fogHeightEnd", "fogHeightFalloff", "farDistanceHeight", "farDistanceOffset", "categorySkybox", "skyboxFogIntensity", "skyboxFogHeight", "skyboxFogFalloff", "skyboxFogOffset", "skyboxFogBottom", "skyboxFogFill", "categoryDirectional", "directionalIntensity", "directionalFalloff", "directionalColor", "categoryNoise", "noiseIntensity", "noiseMin", "noiseMax", "noiseScale", "noiseSpeed", "noiseDistanceEnd", "jitterIntensity" };
        readonly string[] timeOfDayMode = { "m_Script", "presetMaterial", "categoryFog", "fogIntensity", "fogAxisMode", "fogLayersMode", "fogCameraMode", "fogColorStart", "fogColorEnd", "fogColorDuo", "fogDistanceStart", "fogDistanceEnd", "fogDistanceFalloff", "fogHeightStart", "fogHeightEnd", "fogHeightFalloff", "farDistanceHeight", "farDistanceOffset", "categorySkybox", "skyboxFogIntensity", "skyboxFogHeight", "skyboxFogFalloff", "skyboxFogOffset", "skyboxFogBottom", "skyboxFogFill", "categoryDirectional", "directionalIntensity", "directionalFalloff", "directionalColor", "categoryNoise", "noiseIntensity", "noiseMin", "noiseMax", "noiseScale", "noiseSpeed" ,"noiseDistanceEnd", "jitterIntensity" };
        HeightFogGlobal targetScript;

        void OnEnable()
        {
            targetScript = (HeightFogGlobal)target;
        }

        public override void OnInspectorGUI()
        {
            DrawInspector();
        }

        void DrawInspector()
        {
            string[] exclude = scriptMode;

            if (targetScript.fogMode == FogMode.UsePresetSettings)
            {
                exclude = presetMode;
            }
            else if (targetScript.fogMode == FogMode.UseTimeOfDay)
            {
                exclude = timeOfDayMode;
            }

            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, exclude);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
