// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using Boxophobic.Utils;
using System.IO;

namespace AtmosphericHeightFog
{
    public class HeightFogHub : EditorWindow
    {
        float GUI_HALF_EDITOR_WIDTH = 220;

        string autorunPath;
        string assetFolder = "Assets/BOXOPHOBIC/Atmospheric Height Fog";
        string userFolder = "Assets/BOXOPHOBIC/User";

        int assetVersion;

        string[] pipelinePaths;
        string[] pipelineOptions;
        string pipelinesPath;
        int pipelineIndex;
        string pipelinePath;
        string pipelineCurrent = "Standard";

        bool requiresPipelineSetup = false;
        bool showAdditionalSettings = false;

        GUIStyle stylePopup;
        GUIStyle styledToolbar;
        GUIStyle styleLabelCentered;

        Color bannerColor;
        string bannerText;
        string bannerVersion;
        static HeightFogHub window;
        //Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/Atmospheric Height Fog/Hub", false, 1031)]
        public static void ShowWindow()
        {
            window = GetWindow<HeightFogHub>(false, "Atmospheric Height Fog", true);
            window.minSize = new Vector2(800, 300);
        }

        void OnEnable()
        {
            //Safer search, there might be many user folders
            string[] searchFolders;

            searchFolders = AssetDatabase.FindAssets("Atmospheric Height Fog");

            for (int i = 0; i < searchFolders.Length; i++)
            {
                if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("Atmospheric Height Fog.pdf"))
                {
                    assetFolder = AssetDatabase.GUIDToAssetPath(searchFolders[i]);
                    assetFolder = assetFolder.Replace("/Atmospheric Height Fog.pdf", "");
                }
            }

            searchFolders = AssetDatabase.FindAssets("User");

            for (int i = 0; i < searchFolders.Length; i++)
            {
                if (AssetDatabase.GUIDToAssetPath(searchFolders[i]).EndsWith("User.pdf"))
                {
                    userFolder = AssetDatabase.GUIDToAssetPath(searchFolders[i]);
                    userFolder = userFolder.Replace("/User.pdf", "");
                    userFolder += "/Atmospheric Height Fog";
                }
            }

            pipelinesPath = assetFolder + "/Core/Pipelines";
            pipelinePath = userFolder + "/Pipeline.asset";
            autorunPath = assetFolder + "/Core/Editor/HeightFogHubAutoRun.cs";

            GetPackages();

            for (int i = 0; i < pipelineOptions.Length; i++)
            {
                if (pipelineOptions[i] == SettingsUtils.LoadSettingsData(pipelinePath, ""))
                {
                    pipelineIndex = i;
                }
            }

            assetVersion = SettingsUtils.LoadSettingsData(assetFolder + "/Core/Editor/Version.asset", -99);

            bannerColor = new Color(0.55f, 0.75f, 1f);
            bannerText = "Atmospheric Height Fog";
            bannerVersion = assetVersion.ToString();
            bannerVersion = bannerVersion.Insert(1, ".");
            bannerVersion = bannerVersion.Insert(3, ".");
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

            if (EditorApplication.isCompiling)
            {
                GUI.enabled = false;
            }

            if (pipelinePaths == null)
            {
                EditorGUILayout.HelpBox("The pipelines packages are missing from the Core > Pipelines folder. Please make sure you have the full package or to include the *.unitypackage files when using version control!", MessageType.Error, true);

                EndLayout();

                return;
            }

            if (File.Exists(autorunPath) || requiresPipelineSetup)
            {
                EditorGUILayout.HelpBox("Welcome to the Atmospheric Height Fog! Choose the render pipeline used in your project to make the shaders compatible with the current pipeline!", MessageType.Info, true);

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

                    GetPipeline();
                    InstallAsset();

                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Atmospheric Height Fog is installed for the " + pipelineCurrent + "! Use the additional setting below to restart the render pipeline setup.", MessageType.Info, true);

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
                    }
                }
            }

            GUI.enabled = true;

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
                Application.OpenURL("https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.u2ox035i3s3h");
            }
            GUILayout.Space(-1);

            if (GUILayout.Button("Demo Scene", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(assetFolder + "/Demo/Demo.unity"));
            }
            GUILayout.Space(-1);

            if (GUILayout.Button("More Assets", styledToolbar, GUILayout.Width(GUI_TOOLBAR_EDITOR_WIDTH)))
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
                Application.OpenURL("https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/atmospheric-height-fog-optimized-fog-for-consoles-mobile-and-vr-143825#reviews");
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
            GUI.enabled = false;

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
                EditorGUI.LabelField(new Rect(0, this.position.height - 25, this.position.width, 20), "<size=10>Installation Progress</size>", styleLabelCentered);

                EditorGUI.DrawRect(new Rect(0, this.position.height - 30, this.position.width / 2, 1), progressColor);
            }
            else
            {
                EditorGUI.LabelField(new Rect(0, this.position.height - 25, this.position.width, 20), "<size=10>Installation Completed</size>", styleLabelCentered);

                EditorGUI.DrawRect(new Rect(0, this.position.height - 30, this.position.width, 1), progressColor);
            }

            GUI.enabled = true;
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
            for (int i = 0; i < pipelineOptions.Length; i++)
            {
                if (pipelineOptions[i] == SettingsUtils.LoadSettingsData(pipelinePath, ""))
                {
                    pipelineIndex = i;
                    pipelineCurrent = pipelineOptions[i];
                }
            }

            pipelineCurrent = pipelineCurrent.Replace("Standard", "Standard Render Pipeline");
            pipelineCurrent = pipelineCurrent.Replace("Universal", "Universal Render Pipeline");
            pipelineCurrent = pipelineCurrent.Replace("High Definition", "High Definition Render Pipeline");
        }

        void InstallAsset()
        {
            FileUtil.DeleteFileOrDirectory(autorunPath);
            FileUtil.DeleteFileOrDirectory(autorunPath + ".meta");

            AssetDatabase.Refresh();

            SetDefineSymbols();

            GUIUtility.ExitGUI();
        }

        void ImportPackage()
        {
            AssetDatabase.ImportPackage(pipelinePaths[pipelineIndex], false);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SetDefineSymbols()
        {
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (!defineSymbols.Contains("ATMOSPHERIC_HEIGHT_FOG"))
            {
                defineSymbols += ";ATMOSPHERIC_HEIGHT_FOG;";
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
        }
    }
}

