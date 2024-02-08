#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


namespace KWS
{
    [System.Serializable]
    [CustomEditor(typeof(WaterSystem))]
    public partial class WaterSystemInspector : Editor
    {
        private Texture2D ButtonTex;
        private GUIStyle buttonStyle;
        private WaterSystem waterSystem;

        private float floatMapCircleRadiusDefault = 2f;
        private bool leftKeyPressed;
        private bool keyPressed;
        private bool isFlowMapChanged;
        private bool isActive;

        private static float InteractiveWaveHelperEnableTime = 300000;
        private float interactiveWaveCurrentTime;
        private const int InteractMaxResolution = 2048;


        GUIStyle helpBoxStyle;
        GUIStyle notesLabelStyleFade;
        GUIStyle notesLabelStyle;
        private VideoTooltipWindow window;
        private string pathToHelpVideos;

        Camera _sceneCamera;

        void OnDestroy()
        {
            KW_Extensions.SafeDestroy(ButtonTex);
        }

        void InitializeStyles()
        {
            buttonStyle = new GUIStyle();
            buttonStyle.overflow.left = buttonStyle.overflow.right = 3;
            buttonStyle.overflow.top = 2;
            buttonStyle.overflow.bottom = 0;
            if (ButtonTex == null)
            {
                if (EditorGUIUtility.isProSkin) ButtonTex = CreateTex(32, 32, new Color(80 / 255f, 80 / 255f, 80 / 255f));
                else ButtonTex = CreateTex(32, 32, new Color(171 / 255f, 171 / 255f, 171 / 255f));
            }
            buttonStyle.normal.background = ButtonTex;

            helpBoxStyle = new GUIStyle("button");
            helpBoxStyle.alignment = TextAnchor.MiddleCenter;
            helpBoxStyle.stretchHeight = false;
            helpBoxStyle.stretchWidth = false;

            notesLabelStyleFade = new GUIStyle("label");
            notesLabelStyleFade.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.75f, 0.75f, 0.75f, 0.5f) : new Color(0.1f, 0.1f, 0.1f, 0.3f);

            notesLabelStyle = new GUIStyle("label");
            notesLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.85f, 0.25f, 0.25f, 0.95f) : new Color(0.7f, 0.1f, 0.1f, 0.95f);
        }

        public override void OnInspectorGUI()
        {
            waterSystem = (WaterSystem)target;
            if (waterSystem.enabled && waterSystem.gameObject.activeSelf && waterSystem.IsEditorAllowed())
            {
                isActive = true;
                GUI.enabled = true;
            }
            else
            {
                isActive = false;
                GUI.enabled = false;
            }
            UpdateWaterGUI();
        }

#if UNITY_2019_1_OR_NEWER

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUICustom;
    }


    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUICustom;
    }

     void OnSceneGUICustom(SceneView sceneView)
    {
      DrawWaterEditor();
    }
#else
        public void OnSceneGUI()
        {
            DrawWaterEditor();
        }

#endif

        void DrawWaterEditor()
        {
            if (!isActive) return;

            currentTimeBeforeShorelineUpdate += KW_Extensions.DeltaTime();
            currentTimeBeforeOrthoDepthUpdate += KW_Extensions.DeltaTime();

            if (waterSystem.FlowMapInEditMode) DrawFlowMapEditor();
            if (waterSystem.ShorelineInEditMode) DrawShorelineEditor();
            if (waterSystem.CausticDepthScaleInEditMode) DrawCausticEditor();
            //if (waterSystem.UseInteractiveWaves && interactiveWaveCurrentTime < InteractiveWaveHelperEnableTime)
            //{
            //    interactiveWaveCurrentTime += KW_Extensions.DeltaTime();
            //    DrawInteractiveWavesHelpers();
            //}
        }

        void UpdateWaterGUI()
        {
            if (buttonStyle == null) InitializeStyles();
#if KWS_DEBUG
            waterSystem.Test4 = EditorGUILayout.Vector4Field("Test4", waterSystem.Test4);
#endif
            //waterSystem.TestObj = (GameObject) EditorGUILayout.ObjectField(waterSystem.TestObj, typeof(GameObject), true);

#if UNITY_2020_2_OR_NEWER
        SceneView.lastActiveSceneView.sceneViewState.alwaysRefresh = true;
#else
            SceneView.lastActiveSceneView.sceneViewState.showMaterialUpdate = true;
#endif
            SceneView.lastActiveSceneView.sceneViewState.showSkybox = true;
            SceneView.lastActiveSceneView.sceneViewState.showImageEffects = true;

#if UNITY_2020_3_OR_NEWER
        SceneView.lastActiveSceneView.sceneLighting = true;
#else
            //SceneView.lastActiveSceneView.m_SceneLighting = true;
#endif

            EditorGUI.BeginChangeCheck();

            CheckVolumetricLightingWarnings();
            CheckSunReflectionWarnings();

            AddColorSetting();
            AddWaves();
            AddReflection();
            AddRefraction();
            AddFlowMap();
            AddDynamicWaves();
            AddShorelineMap();
            AddVolumeLight();
            AddCausticEffect();
            AddUnderwaterEffect();
            AddRendering();

            // if (EditorGUI.EndChangeCheck()) waterSystem.VariablesChanged();   

            EditorGUILayout.LabelField("Water GUID: " + waterSystem.WaterGUID, notesLabelStyleFade);

            Undo.RecordObject(target, "Changed water parameters");
        }

        void CheckVolumetricLightingWarnings()
        {
            //#if (!UNITY_PIPELINE_URP) && (!UNITY_PIPELINE_HDRP)
            //        if (waterSystem.UseVolumetricLight && KWS_WaterLights.Lights.Count == 0) EditorGUILayout.HelpBox("Volumetric lighting doesn't work because no lights has been added for water rendering! Add the script 'AddLightToWaterRendering' to your light.", MessageType.Error);
            //#endif
        }

        void CheckSunReflectionWarnings()
        {
            //#if (!UNITY_PIPELINE_URP) && (!UNITY_PIPELINE_HDRP)

            //        if (waterSystem.ReflectSun)
            //        {
            //            if (KWS_WaterLights.Lights.Count == 0 || KWS_WaterLights.Lights.Count(l => l.Light.type == LightType.Directional) == 0)
            //            {
            //                EditorGUILayout.HelpBox("Sun reflection doesn't work because no directional light has been added for water rendering! Add the script 'AddLightToWaterRendering' to your directional light!", MessageType.Error);
            //            }
            //        }
            //#endif
        }

        float Slider(string text, string description, float value, float leftValue, float rightValue, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.Slider(new GUIContent(text, description), value, leftValue, rightValue);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        int IntSlider(string text, string description, int value, int leftValue, int rightValue, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.IntSlider(new GUIContent(text, description), value, leftValue, rightValue);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        int IntField(string text, string description, int value, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.IntField(new GUIContent(text, description), value);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        float FloatField(string text, string description, float value, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.FloatField(new GUIContent(text, description), value);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        Vector2 Vector2Field(string text, string description, Vector2 value, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.Vector2Field(new GUIContent(text, description), value);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        Vector3 Vector3Field(string text, string description, Vector3 value, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.Vector3Field(new GUIContent(text, description), value);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        Color ColorField(string text, string description, Color value, bool shoeEyedropper, bool showAlpha, bool hdr, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.ColorField(new GUIContent(text, description), value, shoeEyedropper, showAlpha, hdr);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        Enum EnumPopup(string text, string description, Enum value, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.EnumPopup(new GUIContent(text, description), value);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        int MaskField(string text, string description, int mask, string[] layers, string helpVideoName = "")
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.MaskField(new GUIContent(text, description), mask, layers);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        bool Toggle(string text, string description, bool value, string helpVideoName = "", params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.Toggle(new GUIContent(text, description), value, options);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        //bool Toggle(bool value, string description, string helpVideoName = "", params GUILayoutOption[] options)
        //{

        //    var newValue = EditorGUILayout.Toggle(value, description, options);
        //    if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow(helpVideoName == string.Empty ? text : helpVideoName);

        //    return newValue;
        //}

        void OpenHelpVideoWindow(string filename)
        {
            if (window != null) window.Close();
            window = (VideoTooltipWindow)EditorWindow.GetWindow(typeof(VideoTooltipWindow));
            if (string.IsNullOrEmpty(pathToHelpVideos)) pathToHelpVideos = GetPathToHelpVideos();
            window.VideoClipFileURI = Path.Combine(pathToHelpVideos, filename + ".mp4");
            window.maxSize = new Vector2(854, 480);
            window.minSize = new Vector2(854, 480);
            window.Show();
        }


        public static string GetPathToHelpVideos()
        {
            var dirs = Directory.GetDirectories(Application.dataPath, "HelpVideos", SearchOption.AllDirectories);

            return dirs.Length != 0 ? dirs[0] : string.Empty;
        }

        void AddColorSetting()
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(4));
            waterSystem.ShowColorSettings = EditorGUILayout.Foldout(waterSystem.ShowColorSettings, new GUIContent("Color Settings"), true);

            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.LabelField(waterSystem._waterGUID);
            if (waterSystem.ShowColorSettings)
            {
                waterSystem.Transparent = Slider("Transparent", "Opacity in meters", waterSystem.Transparent, 0.1f, 50f);
                waterSystem.WaterColor = ColorField("Water Color", "This is the solution color of clean water without impurities", waterSystem.WaterColor, false, false, false);
                EditorGUILayout.Space();
                waterSystem.TurbidityColor = ColorField("Turbidity Color", "Color of suspended particles, such as algae or dirt", waterSystem.TurbidityColor, false, false, false);
                waterSystem.Turbidity = Slider("Turbidity", "Total suspended solids in water, water purity", waterSystem.Turbidity, 0.05f, 1f);
            }

            EditorGUILayout.EndVertical();

        }

        void AddWaves()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(4));
            waterSystem.ShowWaves = EditorGUILayout.Foldout(waterSystem.ShowWaves, new GUIContent("Waves"), true);

            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowWaves)
            {
                var fftSize = (int)waterSystem.FFT_SimulationSize;
                float currentRenderedPixels = fftSize * fftSize;
                if (waterSystem.UseMultipleSimulations)
                {
                    var fftSizeLod = (waterSystem.FFT_SimulationSize == FFT_GPU.SizeSetting.Size_512) ? 128 : 64;
                    currentRenderedPixels = currentRenderedPixels + fftSizeLod * fftSizeLod;
                }
                currentRenderedPixels = (currentRenderedPixels / 1000f);
                EditorGUILayout.LabelField("Simulation rendered pixels (less is better): " + currentRenderedPixels.ToString("0") + " thousands", notesLabelStyleFade);
                waterSystem.FFT_SimulationSize = (FFT_GPU.SizeSetting)EnumPopup("FFT Simulation Size", "Quality of wave simulation", waterSystem.FFT_SimulationSize);

                waterSystem.UseMultipleSimulations = Toggle("Use Multiple Simulations", "Use this option to avoid tiling, also allows you to use waves with strong wind more than 2 meters per second", waterSystem.UseMultipleSimulations);

                waterSystem.WindSpeed = Slider("Wind Speed", "Wind speed in meters", waterSystem.WindSpeed, 0.1f, 15.0f);
                if (waterSystem.WindSpeed > 2f && !waterSystem.UseMultipleSimulations) EditorGUILayout.LabelField("Enable 'Use Multiple Simulations' for strong wind!", notesLabelStyle);
                waterSystem.WindRotation = Slider("Wind Rotation", "Wind rotation in degrees", waterSystem.WindRotation, 0.0f, 360.0f);
                waterSystem.WindTurbulence = Slider("Wind Turbulence", "The power of the wind turbidity. Smaller turbulence = calmer water", waterSystem.WindTurbulence, 0.0f, 1.0f);
                waterSystem.TimeScale = Slider("Time Scale", "Time speed of wave simulation", waterSystem.TimeScale, 0.0f, 2.0f);

            }

            EditorGUILayout.EndVertical();
        }

        void AddReflection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(4));
            waterSystem.ShowReflectionSettings = EditorGUILayout.Foldout(waterSystem.ShowReflectionSettings, new GUIContent("Reflection"), true);

            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowReflectionSettings)
            {
                waterSystem.ReflectionMode = (WaterSystem.ReflectionModeEnum)EnumPopup("Reflection Mode",
                   "Cubemap(reflection probe) : Often used only for sky rendering and almost performance free because you can update it for example once per minute" +
                   Environment.NewLine + Environment.NewLine + "Screen space reflection: can reflect only what you see above the water, for example, " +
                   "can not reflect the bottom of a car, but this method almost free, use it if possible. " +
                   "This method compute reflection using one iteration unlike standard \"screen space reflection\" with multiple iterations" +
                   Environment.NewLine + Environment.NewLine + "Planar reflection: expensive because it render a scene twice, but can reflect accurate reflections with transparent geometry", waterSystem.ReflectionMode);


                if (waterSystem.ReflectionMode == WaterSystem.ReflectionModeEnum.PlanarReflection)
                {
                    waterSystem.PlanarReflectionResolutionQuality =
                        (WaterSystem.PlanarReflectionResolutionQualityEnum)EnumPopup("Planar Resolution Quality", "Reflection texture resolution, less resolution = faster rendering", waterSystem.PlanarReflectionResolutionQuality, "Reflection Resolution Quality");
                }

                if (waterSystem.ReflectionMode == WaterSystem.ReflectionModeEnum.ScreenSpaceReflection)
                {
                    waterSystem.ScreenSpaceReflectionResolutionQuality =
                        (WaterSystem.ScreenSpaceReflectionResolutionQualityEnum)EnumPopup("Screen Space Resolution Quality", "Reflection texture resolution, less resolution = faster rendering",
                                                                                           waterSystem.ScreenSpaceReflectionResolutionQuality, "Reflection Resolution Quality");
                }

                if (waterSystem.ReflectionMode != WaterSystem.ReflectionModeEnum.CubemapReflection)
                    waterSystem.ReflectionClipPlaneOffset = Slider("Clip Plane Offset", "Use it for avoid reflection artefacts near to the water edge", waterSystem.ReflectionClipPlaneOffset, 0, 0.07f);

                if (waterSystem.ReflectionMode == WaterSystem.ReflectionModeEnum.ScreenSpaceReflection)
                    waterSystem.ReflectioDepthHolesFillDistance = IntSlider("Depth Holes Fill Distance", "Filling pixels with lost reflection information", waterSystem.ReflectioDepthHolesFillDistance, 0, 25);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (waterSystem.ReflectionMode == WaterSystem.ReflectionModeEnum.CubemapReflection || waterSystem.ReflectionMode == WaterSystem.ReflectionModeEnum.ScreenSpaceReflection)
                {
                    var layerNames = new List<string>();
                    for (int i = 0; i <= 31; i++)
                    {
                        layerNames.Add(LayerMask.LayerToName(i));
                    }
                    EditorGUILayout.Space();
                    var mask = MaskField("Cubemap Culling Mask", "Culling mask for reflection camera rendering", waterSystem.CubemapCullingMask, layerNames.ToArray());
                    waterSystem.CubemapCullingMask = mask & ~(1 << 4);
                    waterSystem.CubemapUpdateInterval = Slider("Cubemap Update Delay", "Realtime cubemap rendering very expensive, so you can render cubemap once per few frames, not each frame" +
                                                                                       "For example you can render only sky layer once in 60 seconds. In this case it's performance free", waterSystem.CubemapUpdateInterval, 0, 60);
                    waterSystem.CubemapReflectionResolutionQuality = (WaterSystem.CubemapReflectionResolutionQualityEnum)EnumPopup("Cubemap Resolution Quality", "Cubemap texture resolution per side", waterSystem.CubemapReflectionResolutionQuality, "Cubemap Resolution Quality");
                }

                waterSystem.ReflectionClearFlag = (WaterSystem.ReflectionClearFlagEnum)EnumPopup("Clear Flag", "What to display in empty areas of this reflection's view." + Environment.NewLine +
                    "Choose skybox to display a skybox in empty areas. If you use reflections in caves (or other scenes where a skybox should be invisible) use the color cleag flag instead of the skybox flag", waterSystem.ReflectionClearFlag);
                if (waterSystem.ReflectionClearFlag == WaterSystem.ReflectionClearFlagEnum.Color) waterSystem.ReflectionClearColor = EditorGUILayout.ColorField("Clear Color", waterSystem.ReflectionClearColor);

                EditorGUILayout.Space();
                waterSystem.UseAnisotropicReflections = Toggle("Use Anisotropic Reflections", "", waterSystem.UseAnisotropicReflections);
                if (waterSystem.UseAnisotropicReflections)
                {
                    waterSystem.AnisotropicReflectionsScale = EditorGUILayout.Slider("Anisotropic Reflections Scale", waterSystem.AnisotropicReflectionsScale, 0.1f, 1.5f);
                    waterSystem.AnisotropicReflectionsHighQuality = Toggle("High Quality Anisotropic", "", waterSystem.AnisotropicReflectionsHighQuality);
                    EditorGUILayout.Space();
                }

                waterSystem.ReflectSun = Toggle("Reflect Sunlight", "Rendering of the solar path on the water", waterSystem.ReflectSun);
                if (waterSystem.ReflectSun)
                {
                    waterSystem.ReflectedSunCloudinessStrength = Slider("Sun Cloudiness", "", waterSystem.ReflectedSunCloudinessStrength, 0.03f, 0.25f);
                    waterSystem.ReflectedSunStrength = EditorGUILayout.Slider("Sun Strength", waterSystem.ReflectedSunStrength, 0f, 1f);
                }
                CheckSunReflectionWarnings();
            }

            EditorGUILayout.EndVertical();
        }

        void AddRefraction()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(4));
            waterSystem.ShowRefractionSettings = EditorGUILayout.Foldout(waterSystem.ShowRefractionSettings, new GUIContent("Color Refraction"), true);

            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowRefractionSettings)
            {
                waterSystem.RefractionMode = (WaterSystem.RefractionModeEnum)EnumPopup("Refraction Mode", "", waterSystem.RefractionMode);


                if (waterSystem.RefractionMode == WaterSystem.RefractionModeEnum.PhysicalAproximationIOR)
                {
                    waterSystem.RefractionAproximatedDepth = Slider("Aproximated Depth", "", waterSystem.RefractionAproximatedDepth, 0.25f, 5f);
                }

                if (waterSystem.RefractionMode == WaterSystem.RefractionModeEnum.Simple)
                {
                    waterSystem.RefractionSimpleStrength = Slider("Strength", "", waterSystem.RefractionSimpleStrength, 0.02f, 1);
                }

                waterSystem.UseRefractionDispersion = Toggle("Use Dispersion", "", waterSystem.UseRefractionDispersion, "Use Refraction Dispersion");
                if (waterSystem.UseRefractionDispersion)
                {
                    waterSystem.RefractionDispersionStrength = Slider("Dispersion Strength", "", waterSystem.RefractionDispersionStrength, 0.25f, 1, "Refraction Dispersion Strength");
                }
            }

            EditorGUILayout.EndVertical();
        }

        async void AddFlowMap()
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseFlowMap = EditorGUILayout.Toggle(waterSystem.UseFlowMap, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowFlowMap = EditorGUILayout.Foldout(waterSystem.ShowFlowMap, new GUIContent("Flowing", "Used to imitate water flow"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Flowing");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowFlowMap)
            {

                if (isActive) GUI.enabled = waterSystem.UseFlowMap;

                EditorGUILayout.HelpBox("A flow map stores directional information in a texture. " + Environment.NewLine +
                    "By painting on the water area, the brush will draw directions into the flow texture.", MessageType.Info);

                if (GUILayout.Toggle(waterSystem.FlowMapInEditMode, "Flowmap Painter", "Button"))
                {
                    if (!waterSystem.FlowMapInEditMode) SetEditorCameraPosition(waterSystem.FlowMapAreaPosition);
                    waterSystem.FlowMapInEditMode = true;
                    GUI.enabled = isActive;


                }
                else
                {
                    waterSystem.FlowMapInEditMode = false;
                    GUI.enabled = false;
                }

                if (waterSystem.FlowMapInEditMode)
                {
                    EditorGUILayout.HelpBox("\"Left Mouse Click\" for painting " + Environment.NewLine +
                        "Hold \"Ctrl Button\" for erase mode" + Environment.NewLine +
                        "Use \"Mouse Wheel\" for brush size", MessageType.Info);

                }

                EditorGUI.BeginChangeCheck();
                waterSystem.FlowMapAreaPosition = Vector3Field("FlowMap Area Position", "Flowmap area position (in world space).", waterSystem.FlowMapAreaPosition);
                waterSystem.FlowMapAreaPosition.y = waterSystem.transform.position.y;

                var newAreaSize = IntSlider("Flowmap Area Size", "Area size in meters for drawing on the flowmap texture, this parameter relative to the texture resolution", waterSystem.FlowMapAreaSize, 10, 2000);
                waterSystem.FlowMapTextureResolution = (WaterSystem.FlowmapTextureResolutionEnum)EditorGUILayout.EnumPopup(new GUIContent("Flowmap resolution", "The higher the resolution -> more details per meter"), waterSystem.FlowMapTextureResolution);
                waterSystem.FlowMapSpeed = Slider("Flow Speed", "Velocity of flow", waterSystem.FlowMapSpeed, 0.1f, 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    isFlowMapChanged = true;
                    waterSystem.RedrawFlowMap(newAreaSize);
                    waterSystem.FlowMapAreaSize = newAreaSize;

                }

                EditorGUILayout.Space();
                waterSystem.FlowMapBrushStrength = Slider("Brush Strength", "Higher parameter = faster flow velocity", waterSystem.FlowMapBrushStrength, 0.01f, 1);

                // if (waterSystem.FlowMapInEditMode)
                {
                    if (GUILayout.Button("Load Latest Saved"))
                    {
                        if (EditorUtility.DisplayDialog("Load Latest Saved?", "Are you sure you want to RELOAD latest flowmap texture?" + Environment.NewLine +
                            "All new changes will be deleted and the last saved texture will be loaded.", "Yes", "Cancel"))
                        {
                            waterSystem.ReadFlowMap();

                            Debug.Log("Load Latest Saved");
                            isFlowMapChanged = false;
                        }
                    }

                    if (GUILayout.Button("Delete All"))
                    {
                        if (EditorUtility.DisplayDialog("Delete All?", "Are you sure you want to DELETE flowmap texture?" + Environment.NewLine +
                            "All changes and flowmap texture will be deleted", "Yes", "Cancel"))
                        {
                            waterSystem.ClearFlowMap();

                            Debug.Log("Flowmap changes and texture deleted");
                            isFlowMapChanged = false;
                        }
                    }

                    GUI.enabled = isFlowMapChanged;

                    if (GUILayout.Button("Save All"))
                    {
                        waterSystem.SaveFlowMap();
                        waterSystem.ReadFlowMap();


                        Debug.Log("Flowmap texture saved");
                        isFlowMapChanged = false;
                    }

                }
                if (isActive) GUI.enabled = waterSystem.UseFlowMap;

                EditorGUILayout.Space();
                waterSystem.UseFluidsSimulation = Toggle("Use Fluids Simulation", "Used to simulate the flow of river and foam", waterSystem.UseFluidsSimulation);
                if (waterSystem.UseFluidsSimulation)
                {
                    EditorGUILayout.HelpBox("Fluids simulation calculate dynamic flow around static objects." + Environment.NewLine +
                   "Step 1: draw the flow direction on the current flowmap" + Environment.NewLine +
                   "Step 2: save flowmap" + Environment.NewLine +
                   "Step 3: press the button 'Bake Fluids Obstacles'", MessageType.Info);

                    var fluidsInfo = waterSystem.BakedFluidsSimPercentPassed > 0 ? string.Concat(" (", waterSystem.BakedFluidsSimPercentPassed, "%)") : string.Empty;
                    if (GUILayout.Button("Bake Fluids Obstacles" + fluidsInfo))
                    {
                        waterSystem.FlowMapInEditMode = false;
                        waterSystem.Editor_SaveFluidsDepth();
                        waterSystem.BakeFluidSimulation();
                    }
                    EditorGUILayout.Space();
                    float currentRenderedPixels = waterSystem.FluidsSimulationIterrations * waterSystem.FluidsTextureSize * waterSystem.FluidsTextureSize * 2f; //iterations * width * height * lodLevels
                    currentRenderedPixels = (currentRenderedPixels / 1000000f);
                    EditorGUILayout.LabelField("Current rendered pixels(less is better): " + currentRenderedPixels.ToString("0.0") + " millions", notesLabelStyleFade);
                    waterSystem.FluidsSimulationIterrations = IntSlider("Simulation iterations", "More iterations = more flow speed, but more perfomance cost", waterSystem.FluidsSimulationIterrations, 1, 4);
                    waterSystem.FluidsTextureSize = IntSlider("Fluids Texture Resolution", "Simulation detailing per meter", waterSystem.FluidsTextureSize, 368, 2048);
                    waterSystem.FluidsAreaSize = IntSlider("Fluids Area Size", "Area size in meters for fluid simulation", waterSystem.FluidsAreaSize, 20, 80);

                    var newSimulationFPS = IntSlider("Fluids Simulation FPS", "Simulation update interval. More simulation fps -> expensive rendering", waterSystem.FluidsSimulationFPS, 25, 60);
                    if (newSimulationFPS != waterSystem.FluidsSimulationFPS)
                    {
                        waterSystem.FluidsSimulationFPS = newSimulationFPS;
                    }
                    waterSystem.FluidsSpeed = Slider("Fluids Flow Speed", "Velocity of flow", waterSystem.FluidsSpeed, 0.25f, 1.0f);

                    waterSystem.FluidsFoamStrength = Slider("Fluids Foam Strength", "The amount of foam relative to the flow velocity", waterSystem.FluidsFoamStrength, 0.0f, 1.0f);

                }
            }

            if (isActive) GUI.enabled = true;
            if (!waterSystem.UseFlowMap || !isActive) waterSystem.FlowMapInEditMode = false;

            EditorGUILayout.EndVertical();
        }

        int MaxDynamicWavesTexSize = 2048;
        void AddDynamicWaves()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseDynamicWaves = EditorGUILayout.Toggle(waterSystem.UseDynamicWaves, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowDynamicWaves = EditorGUILayout.Foldout(waterSystem.ShowDynamicWaves, new GUIContent("Dynamic Waves", "Used to simulate waves from dynamic objects like a characters/bullets/etc"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Dynamic Waves");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowDynamicWaves)
            {
                if (isActive) GUI.enabled = waterSystem.UseDynamicWaves;
                EditorGUILayout.HelpBox("You must add the script 'KW_InteractWithWater' to your moving objects", MessageType.Warning);

                float currentRenderedPixels = waterSystem.DynamicWavesAreaSize * waterSystem.DynamicWavesResolutionPerMeter;
                currentRenderedPixels *= currentRenderedPixels;
                currentRenderedPixels = (currentRenderedPixels / 1000000f);
                EditorGUILayout.LabelField("Simulation rendered pixels (less is better): " + currentRenderedPixels.ToString("0.0") + " millions", notesLabelStyleFade);

                waterSystem.DynamicWavesAreaSize = IntSlider("Waves Area Size", "Area size in meters for dynamic waves simulation, this parameter relative to the texture resolution", waterSystem.DynamicWavesAreaSize, 10, 300);
                waterSystem.DynamicWavesResolutionPerMeter = waterSystem.DynamicWavesAreaSize * waterSystem.DynamicWavesResolutionPerMeter > MaxDynamicWavesTexSize
                    ? MaxDynamicWavesTexSize / waterSystem.DynamicWavesAreaSize
                    : waterSystem.DynamicWavesResolutionPerMeter;


                waterSystem.DynamicWavesResolutionPerMeter = IntSlider("Pixels per meter", " ", waterSystem.DynamicWavesResolutionPerMeter, 5, 50);
                waterSystem.DynamicWavesAreaSize = waterSystem.DynamicWavesAreaSize * waterSystem.DynamicWavesResolutionPerMeter > MaxDynamicWavesTexSize
                  ? MaxDynamicWavesTexSize / waterSystem.DynamicWavesResolutionPerMeter
                  : waterSystem.DynamicWavesAreaSize;

                waterSystem.DynamicWavesPropagationSpeed = Slider("Propagation speed", " ", waterSystem.DynamicWavesPropagationSpeed, 0.1f, 2);
                var newSimulationFPS = IntSlider("Simulation FPS", "Simulation update interval. More simulation fps -> expensive rendering", waterSystem.DynamicWavesSimulationFPS, 25, 60);
                if (newSimulationFPS != waterSystem.DynamicWavesSimulationFPS)
                {
                    waterSystem.DynamicWavesSimulationFPS = newSimulationFPS;
                }
                //waterSystem.DynamicWavesQuality = (WaterSystem.QualityEnum)EditorGUILayout.EnumPopup(new GUIContent("Quality", "Simulation detailing per meter"), waterSystem.DynamicWavesQuality);

                waterSystem.UseDynamicWavesRainEffect = Toggle("Rain Drops", "Rain waves simulation", waterSystem.UseDynamicWavesRainEffect);
                if (waterSystem.UseDynamicWavesRainEffect)
                {
                    waterSystem.DynamicWavesRainStrength = EditorGUILayout.Slider(new GUIContent("Rain Strength", "Rain drops count"), waterSystem.DynamicWavesRainStrength, 0.01f, 1);
                }

            }
            GUI.enabled = isActive;
            EditorGUILayout.EndVertical();
        }

        private List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData;

        async void AddShorelineMap()
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseShorelineRendering = EditorGUILayout.Toggle(waterSystem.UseShorelineRendering, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowShorelineMap = EditorGUILayout.Foldout(waterSystem.ShowShorelineMap, new GUIContent("Shoreline", "Used for rendering of coastal waves with foam"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Shoreline");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowShorelineMap)
            {
                if (isActive) GUI.enabled = waterSystem.UseShorelineRendering;
                VolumeLightMessages();

                waterSystem.FoamLodQuality = (WaterSystem.QualityEnum)EnumPopup("Foam Lod Quality", "Foam particles count", waterSystem.FoamLodQuality);
                waterSystem.FoamCastShadows = Toggle("Foam Cast Shadows", "", waterSystem.FoamCastShadows);
                waterSystem.FoamReceiveShadows = Toggle("Foam Receive Shadows", "", waterSystem.FoamReceiveShadows);
                if (waterSystem.UseShorelineRendering && waterSystem.FoamReceiveShadows && !waterSystem.UseVolumetricLight) EditorGUILayout.HelpBox("Enable volumetric lighting for receive foam shadows!", MessageType.Warning);
                if (waterSystem.UseShorelineRendering && waterSystem.FoamReceiveShadows) EditorGUILayout.HelpBox("Foam shadows receiving DRASTICALLY reduces FPS! It is not recommended using this setting without a high-end GPU!", MessageType.Warning);

                EditorGUI.BeginChangeCheck();
                waterSystem.ShorelineInEditMode = GUILayout.Toggle(waterSystem.ShorelineInEditMode, "Edit Mode", "Button");
                if (EditorGUI.EndChangeCheck() || (waterSystem.ShorelineInEditMode && wavesData == null))
                {
                    if (waterSystem.ShorelineInEditMode) SetEditorCameraPosition(waterSystem.ShorelineAreaPosition);
                    wavesData = await waterSystem.GetShorelineWavesData();
                    if (wavesData.Count == 0) waterSystem.ShorelineAreaPosition = GetSceneCameraPosition();
                    waterSystem.InitialiseShorelineEditorResources();
                    isRequiredUpdateShorelineParams = true;


                }


                if (isActive)
                {
                    GUI.enabled = isActive && waterSystem.ShorelineInEditMode;

                    if (waterSystem.ShorelineInEditMode)
                    {
                        EditorGUILayout.HelpBox("You must add shoreline waves only in the drawing area." + Environment.NewLine +
                            "Avoid crossing boxes of the same color." + Environment.NewLine +
                            "Use Insert/Delete buttons for Add/Delete waves at the current mouse position.", MessageType.Info);

                    }
                    waterSystem.ShorelineAreaPosition = Vector3Field("Drawing Area Position", "Shoreline area position (in world space). " +
                        "You must add shoreline waves only in this drawing area. " +
                        "Outside of this area, waves will not be displayed correctly.", waterSystem.ShorelineAreaPosition);
                    waterSystem.ShorelineAreaPosition.y = waterSystem.transform.position.y;
                    waterSystem.ShorelineAreaSize = IntSlider("Shoreline Area Size", "Area size in meters for baked waves, this parameter relative to the texture resolution", waterSystem.ShorelineAreaSize, 100, 8000);

                    waterSystem.ShorelineCurvedSurfacesQuality = (WaterSystem.QualityEnum)EnumPopup("Curved Surfaces Quality", "Quality of rendering foam around curved obstacles, for examples rocks", waterSystem.ShorelineCurvedSurfacesQuality);

                    if (GUILayout.Button(new GUIContent("Add Wave")))
                    {
                        wavesData = await waterSystem.GetShorelineWavesData();
                        AddWave(wavesData, GetCameraToWorldRay(), true);
                        waterSystem.SaveShorelineWavesParamsToDataFolder();
                    }

                    GUILayout.Space(10);
                    if (GUILayout.Button("Delete All Waves"))
                    {
                        if (EditorUtility.DisplayDialog("Delete Shoreline Waves?", "Are you sure you want to DELETE shoreline waves?", "Yes", "Cancel"))
                        {
                            waterSystem.ClearShorelineWavesWithFoam();
                            Debug.Log("Shoreline waves deleted");
                            isRequiredUpdateShorelineParams = true;
                        }
                    }

                    if (GUILayout.Button("Save Changes"))
                    {
                        // waterSystem.RenderOrthoDepth(waterSystem.ShorelineAreaSize);
                        // waterSystem.SaveOrthoDepth();
                        await waterSystem.BakeWavesToTexture();
                        waterSystem.SaveShorelineToDataFolder();
                        waterSystem.SaveShorelineDepth();
                        //Debug.Log("total save time: " + sw.ElapsedMilliseconds);
                        Debug.Log("Shoreline Saved");
                    }
                    GUI.enabled = !waterSystem.ShorelineInEditMode;
                }
                if (isActive) GUI.enabled = true;
            }

            if (!waterSystem.UseShorelineRendering || !isActive) waterSystem.ShorelineInEditMode = false;

            EditorGUILayout.EndVertical();
        }

        private void AddWave(List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData, Ray ray, bool interpolateNextPosition)
        {
            var newWave = new KW_ShorelineWaves.ShorelineWaveInfo();
            ComputeShorelineNextTransform(newWave, wavesData, ray, interpolateNextPosition);

            newWave.ID = (wavesData.Count == 0) ? 0 : wavesData.Last().ID + 1;
            newWave.TimeOffset = GetShorelineTimeOffset(wavesData);
            //Debug.Log("ID " + newWave.ID + "   time offset " + newWave.TimeOffset);

            wavesData.Add(newWave);
            waterSystem.ClearShorelineFoam();
            isRequiredUpdateShorelineParams = true;
        }

        void ComputeShorelineNextTransform(KW_ShorelineWaves.ShorelineWaveInfo newWave, List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData, Ray ray, bool interpolateNextPosition)
        {
            var intersectionPos = GetWaterRayIntersection(ray);
            newWave.PositionX = intersectionPos.x;
            newWave.PositionZ = intersectionPos.z;
            if (wavesData.Count > 0)
            {
                var lastIdx = wavesData.Count - 1;
                newWave.EulerRotation = wavesData[lastIdx].EulerRotation;
                newWave.ScaleX = wavesData[lastIdx].ScaleX;
                newWave.ScaleY = wavesData[lastIdx].ScaleY;
                newWave.ScaleZ = wavesData[lastIdx].ScaleZ;
            }

            if (interpolateNextPosition && wavesData.Count > 0)
            {
                if (wavesData.Count < 2) newWave.PositionZ += 10;
                else
                {
                    var currentIdx = wavesData.Count - 1;
                    var lastPos = new Vector2(wavesData[currentIdx].PositionX, wavesData[currentIdx].PositionZ);
                    var lastLastPos = new Vector2(wavesData[currentIdx - 1].PositionX, wavesData[currentIdx - 1].PositionZ);
                    var direction = (lastPos - lastLastPos).normalized;
                    var radius = new Vector2(wavesData[currentIdx].ScaleX, wavesData[currentIdx].ScaleZ).magnitude * 0.4f;
                    newWave.PositionX = lastPos.x + radius * direction.x;
                    newWave.PositionZ = lastPos.y + radius * direction.y;
                }
            }

            //var plane = new Plane(Vector3.down, waterSystem.transform.position.y);

            //var ray = useMousePositionAsStartPoint ? HandleUtility.GUIPointToWorldRay(Event.current.mousePosition) : new Ray(waterSystem.currentCamera.transform.position, waterSystem.currentCamera.transform.forward * 1000);

            //if (plane.Raycast(ray, out var distanceToPlane))
            //{
            //    var intersectionPos = ray.GetPoint(distanceToPlane);

            //    newWave.PositionX = intersectionPos.x;
            //    newWave.PositionZ = intersectionPos.z;
            //    if (wavesData.Count > 0)
            //    {
            //        var lastIdx = wavesData.Count - 1;
            //        newWave.EulerRotation = wavesData[lastIdx].EulerRotation;
            //        newWave.ScaleX = wavesData[lastIdx].ScaleX;
            //        newWave.ScaleY = wavesData[lastIdx].ScaleY;
            //        newWave.ScaleZ = wavesData[lastIdx].ScaleZ;
            //    }

            //    if (!useMousePositionAsStartPoint && wavesData.Count > 0)
            //    {
            //        if (wavesData.Count < 2) newWave.PositionZ += 10;
            //        else
            //        {
            //            var currentIdx = wavesData.Count - 1;
            //            var lastPos = new Vector2(wavesData[currentIdx].PositionX, wavesData[currentIdx].PositionZ);
            //            var lastLastPos = new Vector2(wavesData[currentIdx - 1].PositionX, wavesData[currentIdx - 1].PositionZ);
            //            var direction = (lastPos - lastLastPos).normalized;
            //            var radius = new Vector2(wavesData[currentIdx].ScaleX, wavesData[currentIdx].ScaleZ).magnitude * 0.4f;
            //            newWave.PositionX = lastPos.x + radius * direction.x;
            //            newWave.PositionZ = lastPos.y + radius * direction.y;
            //        }
            //    }
            //}
        }

        Ray GetCurrentMouseToWorldRay()
        {
            return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        }

        Camera GetSceneCamera()
        {
            if (SceneView.lastActiveSceneView != null) _sceneCamera = SceneView.lastActiveSceneView.camera;
            else
            {
                var camCurrent = Camera.current;
                if (camCurrent != null && camCurrent.cameraType == CameraType.SceneView) _sceneCamera = camCurrent;
            }
            return _sceneCamera;
        }


        public Vector3 GetSceneCameraPosition()
        {
            if (_sceneCamera == null) return Vector3.zero;
            return _sceneCamera.transform.position;
        }


        Ray GetCameraToWorldRay()
        {
            var sceneCamT = GetSceneCamera().transform;
            return new Ray(sceneCamT.position, sceneCamT.forward * 1000);
        }

        Vector3 GetWaterRayIntersection(Ray ray)
        {
            var plane = new Plane(Vector3.down, waterSystem.transform.position.y);

            if (plane.Raycast(ray, out var distanceToPlane))
            {
                return ray.GetPoint(distanceToPlane);
            }
            return Vector3.zero;
        }


        bool IsShorelineIntersectOther(int currentShorelineIdx, List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData)
        {
            if (currentShorelineIdx < 0 || currentShorelineIdx >= wavesData.Count) return false;
            var currentWave = wavesData[currentShorelineIdx];
            var currentWavePos = new Vector3(currentWave.PositionX, 0, currentWave.PositionZ);
            var currentWaveMaxScale = Mathf.Max(currentWave.ScaleX, currentWave.ScaleZ);
            int startIdx = currentShorelineIdx % 2 == 0 ? 0 : 1;
            for (var i = startIdx; i < wavesData.Count; i += 2)
            {
                if (currentShorelineIdx == i) continue;

                var distance = (currentWavePos - new Vector3(wavesData[i].PositionX, 0, wavesData[i].PositionZ)).magnitude;
                var maxScale = Mathf.Max(wavesData[i].ScaleX, wavesData[i].ScaleZ);
                if (distance < (currentWaveMaxScale + maxScale) * 0.5f)
                {
                    return true;
                }
            }

            return false;
        }

        float GetShorelineTimeOffset(List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData)
        {
            if (wavesData.Count == 0) return 0;
            // if (shorelineWaves.Count == 1) return Random.Range(0.25f, 0.35f);

            // return shorelineWaves[shorelineWaves.Count - 2].TimeOffset + Random.Range(0.25f, 0.35f);
            var timeOffset = wavesData[wavesData.Count - 1].TimeOffset + Random.Range(0.13f, 0.22f);
            return timeOffset % 1;
        }

        void AddVolumeLight()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseVolumetricLight = EditorGUILayout.Toggle(waterSystem.UseVolumetricLight, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowVolumetricLightSettings = EditorGUILayout.Foldout(waterSystem.ShowVolumetricLightSettings, new GUIContent("Volumetric Lighting", "Used to simulate volumetric lighting/shadows/caustic"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Volumetric Scattering");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowVolumetricLightSettings)
            {
                if (isActive) GUI.enabled = waterSystem.UseVolumetricLight;

                CheckVolumetricLightingWarnings();
                //EditorGUILayout.HelpBox("You must add the script 'KWS_AddLightToWaterRendering' to your lights", MessageType.Warning);
                VolumeLightMessages();

                //  var currentRenderedPixels = Screen.width * Screen.height * waterSystem.VolumetricLightResolutionScale * waterSystem.VolumetricLightIteration;
                var resDownsample = (int)waterSystem.VolumetricLightResolutionQuality / 100f;
                float currentRenderedPixels = (float)Screen.width * Screen.height * resDownsample;
                currentRenderedPixels = (currentRenderedPixels / 1000f);
                EditorGUILayout.LabelField("Current rendered pixels (less is better): " + currentRenderedPixels.ToString("0") + "k", notesLabelStyleFade);

                waterSystem.VolumetricLightResolutionQuality = (WaterSystem.VolumetricLightResolutionQualityEnum)EnumPopup("Resolution Quality", "Less resolution -> faster rendering, but the worst visual quality", waterSystem.VolumetricLightResolutionQuality, "Volumetric Light Resolution Quality");

                //waterSystem.VolumetricLightResolutionScale = EditorGUILayout.Slider(new GUIContent("Resolution Scale", "Texture resolution scaled relative to screen size"), waterSystem.VolumetricLightResolutionScale, 0.15f, 0.75f);
                waterSystem.VolumetricLightIteration = IntSlider("Iteration Count", "More iteration = better volume light/shadow quality", waterSystem.VolumetricLightIteration, 2, 8, "Volumetric Light Iteration Count");

                waterSystem.VolumetricLightFilter = (WaterSystem.VolumetricLightFilterEnum)EnumPopup("Filter Mode", "Bilateral filter preserves sharp edges but it is a bit more expensive then other filters."
                     + Environment.NewLine + Environment.NewLine + "Gaussian filter gives better light blurring, but also creates halo artifacts around objects."
                    , waterSystem.VolumetricLightFilter, "Volumetric Light Filter Mode");
                if (waterSystem.VolumetricLightFilter != WaterSystem.VolumetricLightFilterEnum.Bilateral)
                {
                    waterSystem.VolumetricLightBlurRadius = EditorGUILayout.Slider(new GUIContent("Blur Radius", "Blur artefacts of volume light"), waterSystem.VolumetricLightBlurRadius, 1, 4);
                }
            }
            GUI.enabled = isActive;
            EditorGUILayout.EndVertical();
        }

        void AddCausticEffect()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseCausticEffect = EditorGUILayout.Toggle(waterSystem.UseCausticEffect, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowCausticEffectSettings = EditorGUILayout.Foldout(waterSystem.ShowCausticEffectSettings, new GUIContent("Caustic Effect", "Used to simulate light rays on surfaces"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Caustic");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowCausticEffectSettings)
            {
                if (isActive) GUI.enabled = waterSystem.UseCausticEffect;

                float currentRenderedPixels = waterSystem.CausticTextureSize * waterSystem.CausticTextureSize * waterSystem.CausticActiveLods;
                currentRenderedPixels = (currentRenderedPixels / 1000000f);
                EditorGUILayout.LabelField("Simulation rendered pixels (less is better): " + currentRenderedPixels.ToString("0.0") + " millions", notesLabelStyleFade);

                waterSystem.UseCausticDispersion = Toggle("Use Dispersion", "Used to break up light into its constituent spectral colors", waterSystem.UseCausticDispersion);
                waterSystem.UseCausticBicubicInterpolation = Toggle("Use Bicubic Interpolation", "Improves the quality of caustic detailing by averaging the adjacent pixels.", waterSystem.UseCausticBicubicInterpolation);
                var texSize = IntSlider("Texture Size", "Caustic Texture resolution", waterSystem.CausticTextureSize, 256, 1024);
                texSize = Mathf.RoundToInt(texSize / 64f);
                waterSystem.CausticTextureSize = (int)texSize * 64;
                waterSystem.CausticMeshResolution = IntSlider("Mesh Resolution", "Caustic simulation mesh size. Less size = faster", waterSystem.CausticMeshResolution, 128, 384);
                waterSystem.CausticActiveLods = IntSlider("Active Lods", "Caustic cascades, works like the standard shadow cascades", waterSystem.CausticActiveLods, 1, 4);
                waterSystem.CausticStrength = Slider("Caustic Strength", "Caustic light intensity", waterSystem.CausticStrength, 0, 2);
                waterSystem.CausticDepthScale = EditorGUILayout.Slider(new GUIContent("Caustic Scale", "Strength of caustic distortion relative to the water depth"), waterSystem.CausticDepthScale, 0.1f, 5);
                waterSystem.UseDepthCausticScale = Toggle("Use Depth Scaling", "", waterSystem.UseDepthCausticScale);

                if (waterSystem.UseDepthCausticScale)
                {

                    EditorGUILayout.Space();
                    if (GUILayout.Toggle(waterSystem.CausticDepthScaleInEditMode, "Edit Mode", "Button"))
                    {
                        waterSystem.CausticDepthScaleInEditMode = true;
                        GUI.enabled = isActive;
                    }
                    else
                    {
                        waterSystem.CausticDepthScaleInEditMode = false;
                        GUI.enabled = false;
                    }

                    if (waterSystem.CausticOrthoDepthPosition.x == Vector3.positiveInfinity.x) waterSystem.CausticOrthoDepthPosition = GetSceneCameraPosition();
                    waterSystem.CausticOrthoDepthPosition = EditorGUILayout.Vector3Field(new GUIContent("Depth Area Position", "Area in the world position where the depth buffer will be saved "), waterSystem.CausticOrthoDepthPosition);
                    waterSystem.CausticOrthoDepthPosition.y = waterSystem.transform.position.y;

                    waterSystem.CausticOrthoDepthAreaSize = EditorGUILayout.IntSlider(new GUIContent("Depth Area Size", "Area size in meters where caustic will be rendered"), waterSystem.CausticOrthoDepthAreaSize, 10, 8000);
                    waterSystem.CausticOrthoDepthTextureResolution = EditorGUILayout.IntSlider(new GUIContent("Depth Texture Size", ""), waterSystem.CausticOrthoDepthTextureResolution, 128, 4096);
                    if (GUILayout.Button("Bake Caustic Depth"))
                    {
                        waterSystem.Editor_SaveCausticDepth();
                    }

                    GUI.enabled = isActive;
                }



            }
            GUI.enabled = isActive;
            EditorGUILayout.EndVertical();
        }

        void AddUnderwaterEffect()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            waterSystem.UseUnderwaterEffect = EditorGUILayout.Toggle(waterSystem.UseUnderwaterEffect, GUILayout.MaxWidth(14));
            GUILayout.Space(14);
            waterSystem.ShowUnderwaterEffectSettings = EditorGUILayout.Foldout(waterSystem.ShowUnderwaterEffectSettings, new GUIContent("Underwater Effect"), true);
            if (GUILayout.Button("?", helpBoxStyle, GUILayout.Width(16), GUILayout.Height(14))) OpenHelpVideoWindow("Underwater Effect");
            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowUnderwaterEffectSettings)
            {
                if (isActive) GUI.enabled = waterSystem.UseUnderwaterEffect;
                //waterSystem.UseHighQualityUnderwater = EditorGUILayout.Toggle( new GUIContent("High Quality", "Quality of underwater mask"), waterSystem.UseHighQualityUnderwater);
                waterSystem.UseUnderwaterBlur = Toggle("Use Blur Effect", "Blur underwater image", waterSystem.UseUnderwaterBlur, "");
                if (waterSystem.UseUnderwaterBlur)
                {
                    waterSystem.UnderwaterResolutionQuality = (WaterSystem.UnderwaterResolutionQualityEnum)EnumPopup("Resolution Quality", "Texture resolution relative to screen size", waterSystem.UnderwaterResolutionQuality, "Underwater Resolution Quality");
                    waterSystem.UnderwaterBlurRadius = EditorGUILayout.Slider(new GUIContent("Blur Radius", "Blur strength"), waterSystem.UnderwaterBlurRadius, 0.25f, 3);

                }

            }
            GUI.enabled = isActive;
            EditorGUILayout.EndVertical();
        }

        void AddRendering()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal(buttonStyle);

            EditorGUILayout.LabelField("", GUILayout.MaxWidth(4));
            waterSystem.ShowRendering = EditorGUILayout.Foldout(waterSystem.ShowRendering, new GUIContent("Rendering"), true);

            EditorGUILayout.EndHorizontal();

            if (waterSystem.ShowRendering)
            {
                waterSystem.UseFiltering = Toggle("Use Filtering", "Improves the quality of normal detailing by averaging the adjacent pixels. Allows you to overcome the effects of aliasing and adds smoothing normals.", waterSystem.UseFiltering);
                waterSystem.UseAnisotropicFiltering = Toggle("Use Anisotropic Normals", "", waterSystem.UseAnisotropicFiltering);

                // if (waterSystem.UseOffscreenRendering && waterSystem.UseShorelineRendering) EditorGUILayout.LabelField("Shoreline waves don't work properly with offscreen rendering!", notesLabelStyle);
                waterSystem.UseOffscreenRendering = Toggle("Offscreen Rendering", "Water rendering in screen space allows you to set the resolution of the rendering for water separately from the game resolution", waterSystem.UseOffscreenRendering);
                if (waterSystem.UseOffscreenRendering)
                {
                    waterSystem.OffscreenResolutionQuality = (WaterSystem.OffscreenResolutionQualityEnum)EnumPopup("Offscreen Resolution Quality", "Water rendering texture resolution scaled relative to screen size", waterSystem.OffscreenResolutionQuality);
                    waterSystem.OffscreenRenderingAntialiasing = (WaterSystem.AntialiasingEnum)EnumPopup("Offscreen Antialiasing", "", waterSystem.OffscreenRenderingAntialiasing);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
                waterSystem.DrawToPosteffectsDepth = Toggle("Draw To Depth", "Write water depth to the scene depth buffer after transparent geometry." + Environment.NewLine +
                                                                             "Required for correct rendering of \"Depth Of Field\" post effect", waterSystem.DrawToPosteffectsDepth);
                var currentWaterMeshType = (WaterSystem.WaterMeshTypeEnum)EnumPopup("Render Mode", "In the current version available only infinite ocean mesh. ", waterSystem.WaterMeshType);
                if (currentWaterMeshType == WaterSystem.WaterMeshTypeEnum.CustomMesh)
                {
                    waterSystem.CustomMesh = (Mesh)EditorGUILayout.ObjectField(waterSystem.CustomMesh, typeof(Mesh), true);
                    waterSystem.InitializeOrUpdateCustomMesh();
                }

                if (waterSystem.WaterMeshType != currentWaterMeshType)
                {
                    waterSystem.WaterMeshType = currentWaterMeshType;
                    waterSystem.InitializeOrUpdateMesh();
                }

                EditorGUI.BeginChangeCheck();
                if (waterSystem.WaterMeshType == WaterSystem.WaterMeshTypeEnum.Finite) waterSystem.MeshSize = EditorGUILayout.Vector3Field("Water Mesh Size", waterSystem.MeshSize);
                if (EditorGUI.EndChangeCheck()) waterSystem.InitializeOrUpdateMesh();

                EditorGUILayout.Space();
                if (waterSystem.WaterMesh != null)
                {
                    float currentRenderedTriangles = waterSystem.WaterMesh.triangles.Length / 3.0f;
                    if (waterSystem.UseTesselation)
                    {
                        var sqrtDist = Mathf.Sqrt(waterSystem.TesselationMaxDistance - 20) / 45;
                        sqrtDist = Mathf.Pow(sqrtDist, 0.7f);
                        currentRenderedTriangles = Mathf.Lerp(currentRenderedTriangles * waterSystem.TesselationFactor * 5, currentRenderedTriangles * waterSystem.TesselationFactor * 30, sqrtDist);
                    }
                    currentRenderedTriangles = (currentRenderedTriangles / 1000f);
                    EditorGUILayout.LabelField("Approximated triangles count: ~" + currentRenderedTriangles.ToString("0") + " k", notesLabelStyleFade);

                }
                EditorGUI.BeginChangeCheck();
                waterSystem.MeshQuality = IntSlider("Mesh Quality", "Water mesh detailing", waterSystem.MeshQuality, 1, 9);
                if (EditorGUI.EndChangeCheck()) waterSystem.InitializeOrUpdateMesh();



                EditorGUI.BeginChangeCheck();
                waterSystem.UseTesselation = Toggle("Use Tesselation", "Tessellation dynamically increases the detail of the water mesh", waterSystem.UseTesselation);
                if (EditorGUI.EndChangeCheck()) waterSystem.InitializeWaterMaterial(waterSystem.UseTesselation);
                if (waterSystem.UseTesselation)
                {
                    waterSystem.TesselationFactor = Slider("Tesselation Factor", "Detail factor", waterSystem.TesselationFactor, 0.1f, 1);
                    waterSystem.TesselationMaxDistance = Slider("Tesselation Max Distance", "Detail distance", waterSystem.TesselationMaxDistance, 20, 2000);
                }

            }

            EditorGUILayout.EndVertical();
        }

        private Vector3 flowMapLastPos = Vector3.positiveInfinity;
        void DrawFlowMapEditor()
        {
            if (Application.isPlaying) return;

            var e = Event.current;
            if (e.type == EventType.ScrollWheel)
            {
                floatMapCircleRadiusDefault -= (e.delta.y * floatMapCircleRadiusDefault) / 40f;
                floatMapCircleRadiusDefault = Mathf.Clamp(floatMapCircleRadiusDefault, 0.1f, waterSystem.FlowMapAreaSize);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            if (e.type == EventType.ScrollWheel) e.Use();

            var waterPos = waterSystem.transform.position;
            var waterHeight = waterSystem.transform.position.y;
            var flowmapWorldPos = GetMouseWorldPosProjectedToWater(waterHeight, e);
            if (float.IsInfinity(flowmapWorldPos.x)) return;
            var flowPosWithOffset = new Vector3(-waterSystem.FlowMapAreaPosition.x, 0, -waterSystem.FlowMapAreaPosition.z) + (Vector3)flowmapWorldPos;

            Handles.color = e.control ? new Color(1, 0, 0) : new Color(0, 0.8f, 1);
            Handles.CircleHandleCap(controlId, (Vector3)flowmapWorldPos, Quaternion.LookRotation(Vector3.up), floatMapCircleRadiusDefault, EventType.Repaint);

            Handles.color = e.control ? new Color(1, 0, 0, 0.2f) : new Color(0, 0.8f, 1, 0.25f);
            Handles.DrawSolidDisc((Vector3)flowmapWorldPos, Vector3.up, floatMapCircleRadiusDefault);



            // var flowMapAreaPos = new Vector3(waterPos.x + waterSystem.FlowMapOffset.x, waterPos.y, waterPos.z + waterSystem.FlowMapOffset.y);
            var flowMapAreaScale = new Vector3(waterSystem.FlowMapAreaSize, 0.5f, waterSystem.FlowMapAreaSize);
            Handles.matrix = Matrix4x4.TRS(waterSystem.FlowMapAreaPosition, Quaternion.identity, flowMapAreaScale);


            Handles.color = new Color(0, 0.75f, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);

            if (Event.current.button == 0)
            {
                if (e.type == EventType.MouseDown)
                {
                    leftKeyPressed = true;
                    //waterSystem.flowMap.LastDrawFlowMapPosition = flowPosWithOffset;
                }
                if (e.type == EventType.MouseUp)
                {
                    leftKeyPressed = false;
                    isFlowMapChanged = true;
                    flowMapLastPos = Vector3.positiveInfinity;

                    Repaint();
                }
            }

            if (leftKeyPressed)
            {
                if (float.IsPositiveInfinity(flowMapLastPos.x))
                {
                    flowMapLastPos = flowPosWithOffset;
                }
                else
                {
                    var brushDir = (flowPosWithOffset - flowMapLastPos);
                    flowMapLastPos = flowPosWithOffset;
                    waterSystem.DrawOnFlowMap(flowPosWithOffset, brushDir, floatMapCircleRadiusDefault, waterSystem.FlowMapBrushStrength, e.control);
                }
            }

        }

        private bool isRequiredUpdateShorelineParams;
        private const float updateShorelineParamsEverySeconds = 0.1f;
        private float currentTimeBeforeShorelineUpdate;

        private bool isRequiredUpdateOrthoDepth;
        private const float updateOrthoDepthEverySeconds = 0.5f;
        private float currentTimeBeforeOrthoDepthUpdate;

        int nearMouseSelectionWaveIDX = -1;
        private bool isMousePressed = false;


        async Task DrawShorelineEditor()
        {
            if (Application.isPlaying) return;
            Profiler.BeginSample("DrawShorelineEditor");

            Handles.lighting = false;
            float waterYPos = waterSystem.transform.position.y;
            var defaultMatrix = Handles.matrix;

            var e = Event.current;

            if (e.type == EventType.MouseDown) isMousePressed = true;
            else if (e.type == EventType.MouseUp) isMousePressed = false;

            if (!isMousePressed) nearMouseSelectionWaveIDX = GetNearestWave(wavesData);


            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Insert)
                {
                    wavesData = await waterSystem.GetShorelineWavesData();
                    AddWave(wavesData, GetCurrentMouseToWorldRay(), false);
                    waterSystem.SaveShorelineWavesParamsToDataFolder();
                }

                if (Event.current.keyCode == KeyCode.Delete)
                {
                    wavesData.RemoveAt(nearMouseSelectionWaveIDX);
                    waterSystem.SaveShorelineToDataFolder();
                    waterSystem.Editor_RenderShorelineWavesWithFoam();
                    isRequiredUpdateShorelineParams = true;
                    Event.current.Use();
                }
            }


            for (var i = 0; i < wavesData.Count; i++)
            {
                var wave = wavesData[i];
                var wavePos = new Vector3(wave.PositionX, waterYPos, wave.PositionZ);

                Handles.matrix = defaultMatrix;
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

                if (nearMouseSelectionWaveIDX == i)
                {
                    switch (Tools.current)
                    {
                        case Tool.Move:
                            var newWavePos = Handles.DoPositionHandle(wavePos, Quaternion.identity);
                            if (wavePos != newWavePos) isRequiredUpdateShorelineParams = true;

                            wave.PositionX = newWavePos.x;
                            wave.PositionZ = newWavePos.z;

                            break;
                        case Tool.Rotate:
                            {
                                var currentRotation = Quaternion.Euler(0, wave.EulerRotation, 0);
                                var newRotation = Handles.DoRotationHandle(currentRotation, wavePos);
                                if (currentRotation != newRotation) isRequiredUpdateShorelineParams = true;
                                wave.EulerRotation = newRotation.eulerAngles.y;
                                break;
                            }
                        case Tool.Scale:
                            {
                                var distToCamera = Vector3.Distance(GetSceneCameraPosition(), wavePos);
                                var handleScaleToCamera = Mathf.Lerp(1, 50, Mathf.Clamp01(distToCamera / 500));

                                var currentScale = new Vector3(wave.ScaleX, wave.ScaleY, wave.ScaleZ);
                                var newScale = Handles.DoScaleHandle(new Vector3(wave.ScaleX, wave.ScaleY, wave.ScaleZ), wavePos, Quaternion.Euler(0, wave.EulerRotation, 0), handleScaleToCamera);
                                if (currentScale != newScale)
                                {
                                    isRequiredUpdateShorelineParams = true;
                                    var maxNewScale = Mathf.Min(wave.ScaleX, wave.ScaleZ);
                                    var maxDefaultScale = Mathf.Min(wave.DefaultScaleX, wave.DefaultScaleZ);

                                    wave.ScaleX = newScale.x;
                                    wave.ScaleZ = newScale.z;
                                    wave.ScaleY = wave.DefaultScaleY * (maxNewScale / maxDefaultScale);

                                }



                                break;
                            }
                    }
                }

                var waveColor = i % 2 == 0 ? new Color(0, 0.75f, 1, 0.95f) : new Color(0.75f, 1, 0, 0.95f);
                var selectionColor = new Color(Mathf.Clamp01(waveColor.r * 1.5f), Mathf.Clamp01(waveColor.g * 1.5f), Mathf.Clamp01(waveColor.b * 1.5f), 0.95f);
                if (IsShorelineIntersectOther(nearMouseSelectionWaveIDX, wavesData)) selectionColor = new Color(1, 0, 0, 0.9f);

                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.color = nearMouseSelectionWaveIDX == i ? selectionColor : waveColor;
                Handles.matrix = Matrix4x4.TRS(wavePos, Quaternion.Euler(0, wave.EulerRotation, 0), new Vector3(wave.ScaleX, wave.ScaleY, wave.ScaleZ));
                Handles.DrawWireCube(Vector3.zero, Vector3.one);

                Handles.color = nearMouseSelectionWaveIDX == i ? selectionColor * 0.3f : waveColor * 0.2f;
                Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);


                //Handles.matrix = defaultMatrix;
                //if (i % 2 == 1 && i >= 3)
                //{
                //    Handles.color = new Color(1f, 1f, 0f, 0.99f);
                //    Handles.DrawLine(new Vector3(wavesData[i - 2].PositionX, waterYPos, wavesData[i - 2].PositionZ), wavePos);
                //}
                //if (i % 2 == 0 && i >= 2)
                //{
                //    Handles.color = new Color(0f, 1f, 1f, 0.99f);
                //    Handles.DrawLine(new Vector3(wavesData[i - 2].PositionX, waterYPos, wavesData[i - 2].PositionZ), wavePos);
                //}

                //Handles.matrix = Matrix4x4.TRS(wavePos, Quaternion.Euler(0, wave.EulerRotation, 0), new Vector3(0.5f, 0.5f, wave.ScaleZ));
                //Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);


            }

            if (IsCanUpdateShoreline())
            {
                //waterSystem.RenderOrthoDepth();
                await waterSystem.BakeWavesToTexture();

                waterSystem.Editor_RenderShorelineWavesWithFoam();
            }

            Handles.matrix = Matrix4x4.TRS(waterSystem.ShorelineAreaPosition - Vector3.up, Quaternion.identity, new Vector3(waterSystem.ShorelineAreaSize, 3.0f, waterSystem.ShorelineAreaSize));


            //Handles.color = new Color(0, 0.75f, 1, 0.05f);
            //Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.075f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);

            Handles.matrix = defaultMatrix;
            Profiler.EndSample();
        }

        void DrawCausticEditor()
        {
            Handles.lighting = false;
            var causticAreaScale = new Vector3(waterSystem.CausticOrthoDepthAreaSize, waterSystem.Transparent, waterSystem.CausticOrthoDepthAreaSize);
            var causticAreaPos = waterSystem.CausticOrthoDepthPosition - Vector3.up * waterSystem.Transparent * 0.5f;
            Handles.matrix = Matrix4x4.TRS(causticAreaPos, Quaternion.identity, causticAreaScale);


            Handles.color = new Color(0, 0.75f, 1, 0.15f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }

        private int GetNearestWave(List<KW_ShorelineWaves.ShorelineWaveInfo> wavesData)
        {
            var mouseWorldPos = GetMouseWorldPosProjectedToWater(waterSystem.transform.position.y, Event.current);
            float minDistance = float.PositiveInfinity;
            int minIdx = 0;
            if (!float.IsInfinity(mouseWorldPos.x))
            {
                for (var i = 0; i < wavesData.Count; i++)
                {
                    var wave = wavesData[i];
                    var distToMouse = new Vector2(wave.PositionX - mouseWorldPos.x, wave.PositionZ - mouseWorldPos.z).magnitude;
                    var waveRadius = new Vector2(wave.ScaleX, wave.ScaleZ).magnitude * 2.0f;

                    if (distToMouse < waveRadius && distToMouse < minDistance)
                    {
                        minDistance = distToMouse;
                        minIdx = i;
                    }
                }
            }

            return minIdx;
        }

        bool IsCanUpdateShoreline()
        {
            if (isRequiredUpdateShorelineParams)
            {

                if (currentTimeBeforeShorelineUpdate > updateShorelineParamsEverySeconds)
                {
                    isRequiredUpdateShorelineParams = false;
                    currentTimeBeforeShorelineUpdate = 0;
                    return true;
                }
            }
            return false;
        }

        bool IsCanUpdateOrthoDepth()
        {

            if (currentTimeBeforeOrthoDepthUpdate > updateOrthoDepthEverySeconds)
            {
                currentTimeBeforeOrthoDepthUpdate = 0;
                return true;
            }
            return false;
        }

        private float currentTimeBeforeFlowMapUpdate;
        private const float updateFlowMapParamsEverySeconds = 0.5f;


        void SetEditorCameraPosition(Vector3 worldPos)
        {
            SceneView.lastActiveSceneView.LookAt(worldPos);
        }

        Vector3 GetMouseWorldPosProjectedToWater(float height, Event e)
        {
            var mousePos = e.mousePosition;
            var plane = new Plane(Vector3.down, height);
            var ray = HandleUtility.GUIPointToWorldRay(mousePos);
            float distanceToPlane;
            if (plane.Raycast(ray, out distanceToPlane))
            {
                return ray.GetPoint(distanceToPlane);
            }

            return Vector3.positiveInfinity;
        }

        Vector3 FindNearestRay(float raysCount, Vector3 startPos)
        {
            var nearestVector = Vector3.positiveInfinity;

            for (int i = 0; i < raysCount; i++)
            {
                var offset = i * (6.2831f / raysCount);
                var dir = new Vector3(Mathf.Sin(offset), 0, Mathf.Cos(offset));
                RaycastHit ray;
                if (Physics.Raycast(startPos, dir, out ray))
                {
                    if (ray.distance < nearestVector.magnitude) nearestVector = ray.point - startPos;
                }
            }

            return nearestVector;
        }


        enum WaterMeshType
        {
            Infinite,
            Finite,
            CustomMesh
        }

        private Texture2D CreateTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }

}
#endif