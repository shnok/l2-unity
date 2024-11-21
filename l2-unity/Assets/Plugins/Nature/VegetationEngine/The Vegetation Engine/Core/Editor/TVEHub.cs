// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;
using System.IO;
using System.Globalization;
using UnityEngine.Rendering;

namespace TheVegetationEngine
{
    public class TVEHub : EditorWindow
    {
        float GUI_HALF_EDITOR_WIDTH = 220;

        string autorunPath;
        string assetFolder = "Assets/BOXOPHOBIC/The Vegetation Engine";
        string userFolder = "Assets/BOXOPHOBIC/User";

        List<string> settingsLines;
        List<int> vertexLayers;

        string[] pipelinePaths;
        string[] pipelineOptions;
        string pipelinesPath;
        int pipelineIndex = 0;
        string pipelinePath;
        string pipelineCurrent = "Standard";

        int assetVersion;

        bool requiresCompressionUpgrade = false;
        bool requiresPipelineSetup = false;

        bool showAdditionalSettings = false;

        GUIStyle stylePopup;
        GUIStyle styledToolbar;
        GUIStyle styleLabelCentered;

        Color bannerColor;
        string bannerText;
        string bannerVersion;
        static TVEHub window;
        //Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Hub/The Vegetation Engine", false, 1000)]
        public static void ShowWindow()
        {
            window = GetWindow<TVEHub>(false, "The Vegetation Engine", true);
            window.minSize = new Vector2(800, 300);
        }

        void OnEnable()
        {
            assetFolder = TVEUtils.FindAsset("The Vegetation Engine.pdf");
            assetFolder = assetFolder.Replace("/The Vegetation Engine.pdf", "");

            userFolder = TVEUtils.GetUserFolder();

            pipelinesPath = assetFolder + "/Core/Pipelines";
            pipelinePath = userFolder + "/Pipeline.asset";
            autorunPath = assetFolder + "/Core/Editor/TVEHubAutoRun.cs";

            // GetUser Settings
            assetVersion = SettingsUtils.LoadSettingsData(assetFolder + "/Core/Editor/Version.asset", -99);

            GetPackages();
            GetPipeline();

            GetVertexChannelCompression();

            bannerColor = new Color(0.890f, 0.745f, 0.309f);
            bannerText = "The Vegetation Engine";
            bannerVersion = assetVersion.ToString();
            bannerVersion = bannerVersion.Insert(2, ".");
            bannerVersion = bannerVersion.Insert(4, ".");

            // Check for latest version
            //StartBackgroundTask(StartRequest("https://boxophobic.com/s/thevegetationengine", () =>
            //{
            //    int.TryParse(www.downloadHandler.text, out latestVersion);
            //    Debug.Log("hubhub" + latestVersion);
            //}));
        }

        void OnFocus()
        {
            GetVertexChannelCompression();
        }

        void OnGUI()
        {
            GUI_HALF_EDITOR_WIDTH = this.position.width / 2.0f - 24;

            SetGUIStyles();
            DrawToolbar();

            StyledGUI.DrawWindowBanner(bannerColor, bannerText, bannerVersion);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 120));

            if (EditorApplication.isCompiling)
            {
                GUI.enabled = false;
            }

            //if (validatorVersion < assetVersion)
            //{
            //    EditorGUILayout.HelpBox("Previous version detected! You must delete the Vegetation Engine folder before installing the new version!", MessageType.Error, true);

            //    GUILayout.Space(15);

            //    if (GUILayout.Button("Show Upgrading Steps", GUILayout.Height(24)))
            //    {
            //        Application.OpenURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.cm34ujnxxd9d");
            //    }

            //    EndLayout();

            //    return;
            //}

            if (pipelinePaths == null)
            {
                EditorGUILayout.HelpBox("The pipelines packages are missing from the Core > Pipelines folder. Please make sure you have the full package or to include the *.unitypackage files when using version control!", MessageType.Error, true);

                EndLayout();

                return;
            }

            if (requiresCompressionUpgrade)
            {
                if (EditorSettings.serializationMode == UnityEditor.SerializationMode.ForceText)
                {
                    EditorGUILayout.HelpBox("The Vertex Compression for Tex Coord 0 and Tex Coord 3 under Project Settings > Player Settings > Other Settings must be disabled in order for the converted meshes to render in build! Optimize Mesh Data will be enabled to support the shaders as terrain details! Make sure to save the project settings for the chnages to take effect!", MessageType.Warning, true);

                    GUILayout.Space(15);

                    if (GUILayout.Button("Disable Vertex Compression", GUILayout.Height(24)))
                    {
                        SetVertexChannelCompression();
                    }

                    EndLayout();

                    return;
                }
            }

            if (File.Exists(autorunPath) || requiresPipelineSetup)
            {
                EditorGUILayout.HelpBox("Welcome to the Vegetation Engine! Choose the render pipeline used in your project to make the shaders compatible with the current pipeline!", MessageType.Info, true);

                GUILayout.Space(15);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Render Pipeline Support", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                pipelineIndex = EditorGUILayout.Popup(pipelineIndex, pipelineOptions, stylePopup);

                GUILayout.EndHorizontal();

                GUILayout.Space(15);

                if (GUILayout.Button("Choose Render Pipeline Support", GUILayout.Height(24)))
                {
                    SettingsUtils.SaveSettingsData(pipelinePath, pipelineOptions[pipelineIndex]);

                    if (requiresPipelineSetup || pipelineOptions[pipelineIndex].Contains("High") || pipelineOptions[pipelineIndex].Contains("Universal"))
                    {
                        ImportPackage();

                        requiresPipelineSetup = false;
                    }

                    EditorApplication.ExecuteMenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Material Upgrader");

                    GetPipeline();
                    InstallAsset();

                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("The Vegetation Engine is installed for the " + pipelineCurrent + "! Use the additional setting below to restart the render pipeline setup.", MessageType.Info, true);

                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Show Additional Settings", GUILayout.Width(GUI_HALF_EDITOR_WIDTH - 200));
                showAdditionalSettings = EditorGUILayout.Toggle(showAdditionalSettings);
                GUILayout.EndHorizontal();

                GUILayout.Space(15);

                if (showAdditionalSettings)
                {
                    if (GUILayout.Button("Restart Render Pipeline Setup", GUILayout.Height(24)))
                    {
                        GetPipeline();

                        requiresPipelineSetup = true;
                        showAdditionalSettings = false;
                    }
                }
            }

            GUI.enabled = true;

            //GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.Space(13);
            GUILayout.EndHorizontal();

            DrawInstall();
        }

        void SetGUIStyles()
        {
            stylePopup = new GUIStyle(EditorStyles.popup)
            {
                alignment = TextAnchor.MiddleCenter
            };

            styleLabelCentered = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
            };

            styledToolbar = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal,
                fontSize = 11,
            };
        }

        void EndLayout()
        {
            GUI.enabled = true;

            //GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.Space(13);
            GUILayout.EndHorizontal();

            //GUIUtility.ExitGUI();
        }

        void DrawToolbar()
        {
            var GUI_TOOLBAR_EDITOR_WIDTH = this.position.width / 5.0f + 1;

            GUILayout.Space(1);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Discord Server", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                Application.OpenURL("https://discord.com/invite/znxuXET");
            }
            GUILayout.Space(-1);

            if (GUILayout.Button("Documentation", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                Application.OpenURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#");
            }
            GUILayout.Space(-1);

            if (GUILayout.Button("Demo Scenes", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(assetFolder + "/Demo/Demo Diorama.unity"));
            }
            GUILayout.Space(-1);

            if (GUILayout.Button("More Modules", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                Application.OpenURL("https://assetstore.unity.com/publishers/20529");
            }

#if UNITY_2020_3_OR_NEWER
            var rectModules = GUILayoutUtility.GetLastRect();
            var iconModules = new Rect(rectModules.xMax - 24, rectModules.y, 20, 20);
            GUI.color = new Color(0.2f, 1.0f, 1.0f);
            GUI.Label(iconModules, EditorGUIUtility.IconContent("d_SceneViewFx"));
            GUI.color = Color.white;
#endif
            GUILayout.Space(-1);

            if (GUILayout.Button("Write A Review", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/the-vegetation-engine-159647#reviews");
            }

#if UNITY_2020_3_OR_NEWER
            var rectReview = GUILayoutUtility.GetLastRect();
            var iconReview = new Rect(rectReview.xMax - 24, rectReview.y, 20, 20);
            GUI.color = new Color(1.0f, 1.0f, 0.5f);
            GUI.Label(iconReview, EditorGUIUtility.IconContent("d_Favorite"));
            GUI.color = Color.white;
#endif
            GUILayout.Space(-1);

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        void DrawInstall()
        {
            Color progressColor;

            if (EditorGUIUtility.isProSkin)
            {
                progressColor = new Color(1, 1, 1, 0.2f);
            }
            else
            {
                progressColor = new Color(0, 0, 0, 0.2f);
            }

            if (File.Exists(autorunPath) || requiresPipelineSetup)
            {
                EditorGUI.LabelField(new Rect(0, this.position.height - 25, this.position.width, 20), "<size=10><color=#808080>Installation Progress</color></size>", styleLabelCentered);
                EditorGUI.DrawRect(new Rect(0, this.position.height - 30, this.position.width / 2, 1), progressColor);
            }
            else
            {
                EditorGUI.LabelField(new Rect(0, this.position.height - 25, this.position.width, 20), "<size=10><color=#808080>Installation Completed</color></size>", styleLabelCentered);
                EditorGUI.DrawRect(new Rect(0, this.position.height - 30, this.position.width, 1), progressColor);
            }
        }

        void GetPackages()
        {
            pipelinePaths = Directory.GetFiles(pipelinesPath, "*.unitypackage", SearchOption.TopDirectoryOnly);

            if (pipelinePaths != null)
            {
                pipelineOptions = new string[pipelinePaths.Length];

                for (int i = 0; i < pipelineOptions.Length; i++)
                {
                    pipelineOptions[i] = Path.GetFileNameWithoutExtension(pipelinePaths[i].Replace("Built-in Pipeline", "Standard"));
                }
            }
        }

        void GetPipeline()
        {
            var pipeline = SettingsUtils.LoadSettingsData(pipelinePath, "");
            var pipelineValid = false;

            if (pipeline != "")
            {
                for (int i = 0; i < pipelineOptions.Length; i++)
                {
                    if (pipelineOptions[i] == pipeline)
                    {
                        pipelineIndex = i;
                        pipelineValid = true;
                    }
                }
            }

            // Get recommended pipeline
            if (!pipelineValid)
            {
                if (GraphicsSettings.defaultRenderPipeline != null)
                {
                    if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("Universal"))
                    {
                        pipeline = "Universal";
                    }

                    if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HD"))
                    {
                        pipeline = "High Definition";
                    }
                }

                var version = Application.unityVersion;

                if (version.Contains("2021.3") || version.Contains("2021.2"))
                {
                    pipeline = pipeline + " " + "2021.3+";
                }

                if (version.Contains("2022.3") || version.Contains("2022.2"))
                {
                    pipeline = pipeline + " " + "2022.3+";
                }

                if (version.Contains("2023.3") || version.Contains("2023.2"))
                {
                    pipeline = pipeline + " " + "2023.3+";
                }

                for (int i = 0; i < pipelineOptions.Length; i++)
                {
                    if (pipelineOptions[i] == pipeline)
                    {
                        pipelineIndex = i;
                    }
                }
            }

            if (pipelineOptions[pipelineIndex].Contains("Standard"))
            {
                pipelineCurrent = "Standard Render Pipeline";
            }

            if (pipelineOptions[pipelineIndex].Contains("Universal"))
            {
                pipelineCurrent = "Universal Render Pipeline";
            }

            if (pipelineOptions[pipelineIndex].Contains("High"))
            {
                pipelineCurrent = "High Definition Render Pipeline";
            }
        }

        void ImportPackage()
        {
            AssetDatabase.ImportPackage(pipelinePaths[pipelineIndex], false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void InstallAsset()
        {
            FileUtil.DeleteFileOrDirectory(autorunPath);
            FileUtil.DeleteFileOrDirectory(autorunPath + ".meta");

            //SettingsUtils.SaveSettingsData(assetFolder + "/Core/Editor/Validator.asset", assetVersion);
            SettingsUtils.SaveSettingsData(userFolder + "/Version.asset", assetVersion);

            SetDefineSymbols();
            SetScriptExecutionOrder();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GUIUtility.ExitGUI();
        }

        void SetDefineSymbols()
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            if (!defineSymbols.Contains("THE_VEGETATION_ENGINE"))
            {
                defineSymbols += ";THE_VEGETATION_ENGINE;";

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineSymbols);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
#endif
            }
        }

        void SetScriptExecutionOrder()
        {
            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

            foreach (MonoScript script in scripts)
            {
                if (script.GetClass() == typeof(TVEManager))
                {
                    MonoImporter.SetExecutionOrder(script, -122);
                }

                if (script.GetClass() == typeof(TVECustomRenderData))
                {
                    MonoImporter.SetExecutionOrder(script, -121);
                }
            }
        }

        void GetVertexChannelCompression()
        {
            if (EditorSettings.serializationMode != UnityEditor.SerializationMode.ForceText)
            {
                return;
            }

            var projectSettingsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ProjectSettings", "ProjectSettings.asset");
            requiresCompressionUpgrade = false;

            if (File.Exists(projectSettingsPath))
            {
                StreamReader reader = new StreamReader(projectSettingsPath);

                int bitmask = 0;
                vertexLayers = new List<int>();
                settingsLines = new List<string>();

                while (!reader.EndOfStream)
                {
                    settingsLines.Add(reader.ReadLine());
                }

                reader.Close();

                for (int i = 0; i < settingsLines.Count; i++)
                {
                    if (settingsLines[i].Contains("VertexChannelCompressionMask"))
                    {
                        string line = settingsLines[i].Replace("  VertexChannelCompressionMask: ", "");
                        bitmask = int.Parse(line, CultureInfo.InvariantCulture);
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    if (((1 << i) & bitmask) != 0)
                    {
                        vertexLayers.Add(1);
                    }
                    else
                    {
                        vertexLayers.Add(0);
                    }
                }

                if (vertexLayers[4] == 1 || vertexLayers[7] == 1)
                {
                    requiresCompressionUpgrade = true;
                }
            }
        }

        void SetVertexChannelCompression()
        {
            if (EditorSettings.serializationMode != UnityEditor.SerializationMode.ForceText)
            {
                return;
            }

            var projectSettingsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ProjectSettings", "ProjectSettings.asset");

            if (File.Exists(projectSettingsPath))
            {
                // Disable layers
                vertexLayers[4] = 0;
                vertexLayers[7] = 0;

                int layerMask = 0;

                for (int i = 0; i < 9; i++)
                {
                    if (vertexLayers[i] == 1)
                    {
                        layerMask = layerMask + (int)Mathf.Pow(2, i);
                    }
                }

                StreamWriter writer = new StreamWriter(projectSettingsPath);

                for (int i = 0; i < settingsLines.Count; i++)
                {
                    if (settingsLines[i].Contains("VertexChannelCompressionMask"))
                    {
                        settingsLines[i] = "  VertexChannelCompressionMask: " + layerMask;
                    }

                    if (settingsLines[i].Contains("StripUnusedMeshComponents"))
                    {
                        settingsLines[i] = "  StripUnusedMeshComponents: 1";
                    }

                    writer.WriteLine(settingsLines[i]);
                }

                writer.Close();

                requiresCompressionUpgrade = false;
            }
        }

        // Check for latest version
        //UnityWebRequest www;

        //IEnumerator StartRequest(string url, Action success = null)
        //{
        //    using (www = UnityWebRequest.Get(url))
        //    {
        //        yield return www.Send();

        //        while (www.isDone == false)
        //            yield return null;

        //        if (success != null)
        //            success();
        //    }
        //}

        //public static void StartBackgroundTask(IEnumerator update, Action end = null)
        //{
        //    EditorApplication.CallbackFunction closureCallback = null;

        //    closureCallback = () =>
        //    {
        //        try
        //        {
        //            if (update.MoveNext() == false)
        //            {
        //                if (end != null)
        //                    end();
        //                EditorApplication.update -= closureCallback;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (end != null)
        //                end();
        //            Debug.LogException(ex);
        //            EditorApplication.update -= closureCallback;
        //        }
        //    };

        //    EditorApplication.update += closureCallback;
        //}
    }
}
