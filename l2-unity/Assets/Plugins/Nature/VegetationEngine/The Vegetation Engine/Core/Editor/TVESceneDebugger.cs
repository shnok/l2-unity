// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using System.Collections.Generic;

namespace TheVegetationEngine
{
    public class TVESceneDebugger : EditorWindow
    {
        float GUI_HALF_EDITOR_WIDTH = 200;
        float GUI_FULL_EDITOR_WIDTH = 200;

        string[] DebugModeOptions = new string[]
        {
        "None", "Show Opaque", "Show Alpha"
        };

        string[] DebugTypeOptions = new string[]
        {
        "Material Conversion", "Material Type", "Material Scope", "Material Lighting", "Material Shader", "Material Sharing", "Texture Maps", "Texture Resolution", "Texture Mip Level", "Global Textures", "Global Volumes", "Mesh Attributes",
        };

        string[] DebugMeshOptions = new string[]
        {
        "Vertex Position", "Vertex Normals", "Vertex Tangents", "Vertex Tangents Sign", "Triangle Orientation",
        "Vertex Red", "Vertex Green", "Vertex Blue", "Vertex Alpha",         
        "Texture UVs", "Lightmap UVs", "TVE Detail UVs",
        "TVE Variation Mask", "TVE Occlusion Mask", "TVE Detail Mask", "TVE Height Mask",
        "TVE Variation ID", "TVE Branch Mask", "TVE Flutter Mask", 
        "TVE Pivot Positions",
        };

        string[] DebugMapsOptions = new string[]
        {
        "Main Albedo", "Main Alpha", "Main Normal",
        "Main Metallic (Mask R)", "Main Occlusion (Mask G)", "Main Mask (Mask B)", "Main Smoothness (Mask A)",
        "Detail Albedo", "Detail Alpha", "Detail Normal",
        "Detail Metallic (Mask R)", "Detail Occlusion (Mask G)", "Detail Mask (Mask B)", "Detail Smoothness (Mask A)",
        "Emissive",
        };

        string[] DebugResolutionOptions = new string[]
        {
        "Main Albedo", "Main Normal", "Main Mask",
        "Detail Albedo", "Detail Normal", "Detail Mask",
        "Emissive",
        };

        string[] DebugGlobalsOptions = new string[]
        {
        "Global Noise",
        "Colors Tint (RGB)", "Colors Effect (A)",
        "Extras Emissive (R)", "Extras Wetness (G)", "Extras Overlay (B)", "Extras Alpha (A)",
        "Motion Direction (RG)", "Motion Power (B)", "Motion Interaction (A)",
        "Vertex Orientation (RG)", "Vertex Offset (B)", "Vertex Size (A)",
        };

        string[] DebugVolumesOptions = new string[]
        {
        "Colors Volume", "Extras Volume", "Motion Volume", "Vertex Volume",
        };

        string[] DebugLayersOptions = new string[]
        {
        "Default", "Layer 1", "Layer 2", "Layer 3", "Layer 4", "Layer 5", "Layer 6", "Layer 7", "Layer 8",
        };

        string[] DebugMiscOptions = new string[]
        {
        "Polygonal Leaves Mask",
        };

        Shader debugShader;
        int debugModeIndex = 2;
        int debugTypeIndex = 0;
        int debugMeshIndex = 0;
        int debugMapsIndex = 0;
        int debugGlobalsIndex = 0;
        int debugVolumesIndex = 0;
        int debugLayersIndex = 0;
        bool showAllMaterials = true;

        float debugMin = 0;
        float debugMax = 1;

        List<Light> activeLights;

        GUIStyle stylePopup;
        GUIStyle styleBox;
        GUIStyle styleLabel;

        Color bannerColor;
        string bannerText;
        static TVESceneDebugger window;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Scene Debugger", false, 2009)]
        public static void ShowWindow()
        {
            window = GetWindow<TVESceneDebugger>(false, "Scene Debugger", true);
            window.minSize = new Vector2(400, 300);
        }

        void OnEnable()
        {
            bannerColor = new Color(0.890f, 0.745f, 0.309f);
            bannerText = "Scene Debugger";

            debugShader = Shader.Find("Hidden/BOXOPHOBIC/The Vegetation Engine/Helpers/Debug");
            Shader.SetGlobalTexture("TVE_DEBUG_MipTex",Resources.Load<Texture2D>("Internal MipTex"));

#if UNITY_2023_1_OR_NEWER
            var allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
#else
            var allLights = FindObjectsOfType<Light>();
#endif

            activeLights = new List<Light>();

            for (int i = 0; i < allLights.Length; i++)
            {
                if (allLights[i] != null && allLights[i].enabled)
                {
                    activeLights.Add(allLights[i]);
                }
            }
        }

        void OnDestroy()
        {
            DisableDebugger();
        }

        void OnDisable()
        {
            DisableDebugger();
        }

        void OnGUI()
        {
            SetGUIStyles();

            GUI_HALF_EDITOR_WIDTH = (this.position.width / 2.0f - 24) - 5;
            GUI_FULL_EDITOR_WIDTH = this.position.width - 40;

            StyledGUI.DrawWindowBanner(bannerColor, bannerText);

            GUILayout.Space(-2);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 120));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug Mode", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
            debugModeIndex = EditorGUILayout.Popup(debugModeIndex, DebugModeOptions, stylePopup);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug Type", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
            debugTypeIndex = EditorGUILayout.Popup(debugTypeIndex, DebugTypeOptions, stylePopup);
            GUILayout.EndHorizontal();

            // Conversion
            if (debugTypeIndex == 0)
            {
                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("3rd Party Materials", new Color(0.3f, 0.3f, 0.3f, 0.75f));
                StyledLegend("Converted Materials", new Color(0.9f, 0.7f, 0.4f, 0.75f));
                //StyledLegend("Collected Materials", new Color(0.0f, 0.75f, 0.75f, 0.75f));
            }

            // Type
            if (debugTypeIndex == 1)
            {
                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("3rd Party Shaders", new Color(0.3f, 0.3f, 0.3f, 0.75f));
                StyledLegend("Prop Shaders", new Color(0.33f, 0.61f, 0.81f, 0.75f));
                StyledLegend("Plant Shaders", new Color(0.62f, 0.77f, 0.15f, 0.75f));
            }

            // Scope
            if (debugTypeIndex == 2)
            {
                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("3rd Party Shaders", new Color(0.3f, 0.3f, 0.3f, 0.75f));
                StyledLegend("Core Shaders", new Color(0.9f, 0.7f, 0.4f, 0.75f));
                StyledLegend("Blanket Shaders", new Color(0.62f, 0.77f, 0.15f, 0.75f));
                StyledLegend("Impostor Shaders", new Color(0.97f, 0.32f, 0.48f, 0.75f));
                StyledLegend("Polygonal Shaders", new Color(0.33f, 0.61f, 0.81f, 0.75f));
                StyledLegend("Lite Shaders", new Color(0.6f, 0.6f, 0.6f, 0.75f));
            }

            // Lighting
            if (debugTypeIndex == 3)
            {
                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("3rd Party Shaders", new Color(0.3f, 0.3f, 0.3f, 0.75f));
                StyledLegend("Vertex Lit Shaders", new Color(0.62f, 0.77f, 0.15f, 0.75f));
                StyledLegend("Simple Lit Shaders", new Color(0.33f, 0.61f, 0.81f, 0.75f));
                StyledLegend("Standard Lit Shaders", new Color(0.66f, 0.34f, 0.85f, 0.75f));
                StyledLegend("Subsurface Lit Shaders", new Color(0.92f, 0.84f, 0.18f, 0.75f));
            }

            // Custom Shaders
            if (debugTypeIndex == 4)
            {
                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("3rd Party Shaders", new Color(0.3f, 0.3f, 0.3f, 0.75f));
                StyledLegend("Built-in Shaders", new Color(0.9f, 0.7f, 0.4f, 0.75f));
                StyledLegend("Custom Shaders", new Color(0.25f, 0.85f, 0.55f, 0.75f));
            }

            // Maps
            if (debugTypeIndex == 6)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Maps", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugMapsIndex = EditorGUILayout.Popup(debugMapsIndex, DebugMapsOptions, stylePopup);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Remap", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                EditorGUILayout.MinMaxSlider(ref debugMin, ref debugMax, 0.0f, 1.0f);
                GUILayout.EndHorizontal();
            }

            // Resolution
            if (debugTypeIndex == 7)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Maps", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugMapsIndex = EditorGUILayout.Popup(debugMapsIndex, DebugResolutionOptions, stylePopup);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("256 or Smaller", new Color(0.48f, 0.86f, 0.92f, 0.75f));
                StyledLegend("512", new Color(0.19f, 0.74f, 1f, 0.75f));
                StyledLegend("1024", new Color(0.44f, 0.79f, 0.18f, 0.75f));
                StyledLegend("2048", new Color(1f, 0.69f, 0.07f, 0.75f));
                StyledLegend("4096 or Higher", new Color(1f, 0.21f, 0.1f, 0.75f));
            }

            // Mip maps
            if (debugTypeIndex == 8)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Maps", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugMapsIndex = EditorGUILayout.Popup(debugMapsIndex, DebugResolutionOptions, stylePopup);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                StyledGUI.DrawWindowCategory("Debug Legend");
                GUILayout.Space(10);

                StyledLegend("Too much texture detail", new Color(1f, 0.21f, 0.1f, 0.75f));
                StyledLegend("1:1 texels to pixels ratio", new Color(0.5f, 0.5f, 0.5f, 0.75f));
                StyledLegend("Too little texture detail", new Color(0.2f, 0.4f, 1f, 0.75f));
            }

            // Global Textures
            if (debugTypeIndex == 9)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Globals", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugGlobalsIndex = EditorGUILayout.Popup(debugGlobalsIndex, DebugGlobalsOptions, stylePopup);
                GUILayout.EndHorizontal();

                if (debugGlobalsIndex > 1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Debug Layers", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                    debugLayersIndex = EditorGUILayout.Popup(debugLayersIndex, DebugLayersOptions, stylePopup);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Remap", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                EditorGUILayout.MinMaxSlider(ref debugMin, ref debugMax, 0.0f, 1.0f);
                GUILayout.EndHorizontal();
            }

            // Global Volumes
            if (debugTypeIndex == 10)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Volumes", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugVolumesIndex = EditorGUILayout.Popup(debugVolumesIndex, DebugVolumesOptions, stylePopup);
                GUILayout.EndHorizontal();
            }

            // Mesh
            if (debugTypeIndex == 11)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Mesh", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                debugMeshIndex = EditorGUILayout.Popup(debugMeshIndex, DebugMeshOptions, stylePopup);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Debug Remap", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                EditorGUILayout.MinMaxSlider(ref debugMin, ref debugMax, 0.0f, 1.0f);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Include All Scene Objects", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
                showAllMaterials = EditorGUILayout.Toggle(showAllMaterials);
                GUILayout.EndHorizontal();
            }

            if (debugModeIndex > 0)
            {
                EnableDebugger();
            }
            else
            {
                DisableDebugger();
            }

            GUILayout.FlexibleSpace();

            GUILayout.Space(20);

            GUILayout.EndScrollView();

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

            styleBox = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                fontSize = 14,               
            };

            styleLabel = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                richText = true,
                fontSize = 11,
            };
        }

        void StyledLegend(string label, Color color)
        {
            GUILayout.Label("", styleBox);

            var rect = GUILayoutUtility.GetLastRect();
            EditorGUI.DrawRect(rect, color);
            EditorGUI.LabelField(rect, "<b>" + label + "</b>", styleLabel);
        }

        void EnableDebugger()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                if (debugModeIndex == 2)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Clip", 1);
                }
                else
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Clip", 0);
                }

                // Debug Converted
                if (debugTypeIndex == 0)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 0);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                // Debug Material Type
                if (debugTypeIndex == 1)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                // Debug Material Scope
                if (debugTypeIndex == 2)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 2);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                // Debug Material Lighting
                if (debugTypeIndex == 3)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 3);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                //Debug Material Shader
                if (debugTypeIndex == 4)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 4);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                //Debug Material Sharing
                if (debugTypeIndex == 5)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 5);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                //Debug Texture Maps
                if (debugTypeIndex == 6)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 6);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugMapsIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 0);

                    Shader.SetGlobalFloat("TVE_DEBUG_Min", debugMin);
                    Shader.SetGlobalFloat("TVE_DEBUG_Max", debugMax);
                }

                //Debug Texture Resolution
                if (debugTypeIndex == 7)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 7);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugMapsIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                //Debug Texture Mip Level
                if (debugTypeIndex == 8)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 8);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugMapsIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 1);
                }

                // Debug Global Textures
                if (debugTypeIndex == 9)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 9);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugGlobalsIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Layer", debugLayersIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Filter", 0);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 0);

                    Shader.SetGlobalFloat("TVE_DEBUG_Min", debugMin);
                    Shader.SetGlobalFloat("TVE_DEBUG_Max", debugMax);
                }

                // Debug Volumes Textures
                if (debugTypeIndex == 10)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 10);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugVolumesIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 0);
                }

                // Debug Mesh Attributes
                if (debugTypeIndex == 11)
                {
                    Shader.SetGlobalFloat("TVE_DEBUG_Type", 11);
                    Shader.SetGlobalFloat("TVE_DEBUG_Index", debugMeshIndex);
                    Shader.SetGlobalFloat("TVE_DEBUG_Shading", 0);

                    Shader.SetGlobalFloat("TVE_DEBUG_Min", debugMin);
                    Shader.SetGlobalFloat("TVE_DEBUG_Max", debugMax);

                    if (showAllMaterials)
                    {
                        Shader.SetGlobalFloat("TVE_DEBUG_Filter", 0);
                    }
                    else
                    {
                        Shader.SetGlobalFloat("TVE_DEBUG_Filter", 1);
                    }
                }

                SceneView.lastActiveSceneView.SetSceneViewShaderReplace(debugShader, null);
                SceneView.lastActiveSceneView.Repaint();

                foreach (var light in activeLights)
                {
                    if (light != null)
                    {
                        light.gameObject.SetActive(false);
                    }
                }
            }
        }

        void DisableDebugger()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.SetSceneViewShaderReplace(null, null);
                SceneView.lastActiveSceneView.Repaint();

                foreach (var light in activeLights)
                {
                    if (light != null)
                    {
                        light.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
