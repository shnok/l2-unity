
using UnityEditor;

namespace AtmosphericHeightFog
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HeightFogOverride))]
    public class HeightFogOverrideInspector : Editor
    {
        readonly string[] scriptMode = { "m_Script", "presetMaterial", "presetDay", "presetNight", "timeOfDay" };
        readonly string[] presetMode = { "m_Script", "presetDay", "presetNight", "timeOfDay", "categoryFog", "fogIntensity", "fogAxisMode", "fogLayersMode", "fogColorStart", "fogColorEnd", "fogColorDuo", "fogDistanceStart", "fogDistanceEnd", "fogDistanceFalloff", "fogHeightStart", "fogHeightEnd", "fogHeightFalloff", "farDistanceHeight", "farDistanceOffset", "categorySkybox", "skyboxFogIntensity", "skyboxFogHeight", "skyboxFogFalloff", "skyboxFogOffset", "skyboxFogBottom", "skyboxFogFill", "categoryDirectional", "directionalIntensity", "directionalFalloff", "directionalColor", "categoryNoise", "noiseIntensity", "noiseMin", "noiseMax", "noiseScale", "noiseSpeed", "noiseDistanceEnd", "categoryAdvanced", "jitterIntensity" };
        readonly string[] timeOfDayMode = { "m_Script", "presetMaterial", "categoryFog", "fogIntensity", "fogAxisMode", "fogLayersMode", "fogColorStart", "fogColorEnd", "fogColorDuo", "fogDistanceStart", "fogDistanceEnd", "fogDistanceFalloff", "fogHeightStart", "fogHeightEnd", "fogHeightFalloff", "farDistanceHeight", "farDistanceOffset", "categorySkybox", "skyboxFogIntensity", "skyboxFogHeight", "skyboxFogFalloff", "skyboxFogOffset", "skyboxFogBottom", "skyboxFogFill", "categoryDirectional", "directionalIntensity", "directionalFalloff", "directionalColor", "categoryNoise", "noiseIntensity", "noiseMin", "noiseMax", "noiseScale", "noiseSpeed", "noiseDistanceEnd", "categoryAdvanced", "jitterIntensity" };
        HeightFogOverride targetScript;

        void OnEnable()
        {
            targetScript = (HeightFogOverride)target;
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
