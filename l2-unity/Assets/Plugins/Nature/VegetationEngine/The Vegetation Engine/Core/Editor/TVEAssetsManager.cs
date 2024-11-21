// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace TheVegetationEngine
{
    public class TVEAssetsManager : EditorWindow
    {
        float GUI_HALF_EDITOR_WIDTH = 200;
        float GUI_FULL_EDITOR_WIDTH = 200;

        string[] SeletionEnum = new string[]
        {
        "Selected Folder", "Selected Assets",
        };

        int selectionIndex = 0;

        List<string> allAssetsPaths;

        GUIStyle stylePopup;

        Color bannerColor;
        string bannerText;
        static TVEAssetsManager window;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Assets Manager", false, 2005)]
        public static void ShowWindow()
        {
            window = GetWindow<TVEAssetsManager>(false, "Assets Manager", true);
            window.minSize = new Vector2(400, 300);
        }

        void OnEnable()
        {
            bannerColor = new Color(0.890f, 0.745f, 0.309f);
            bannerText = "Assets Manager";
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

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 80));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Selection Mode", GUILayout.Width(GUI_HALF_EDITOR_WIDTH));
            selectionIndex = EditorGUILayout.Popup(selectionIndex, SeletionEnum, stylePopup);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            StyledGUI.DrawWindowCategory("Processing Settings");
            GUILayout.Space(10);

            if (GUILayout.Button("Validate TVE Materials", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("", "*.mat");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];
                    var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                    TVEUtils.SetMaterialSettings(material);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " materials have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Validate TVE Elements", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("", "*.mat");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];
                    var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                    TVEUtils.SetElementSettings(material);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " elements have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Mark Model As Readable", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    string fileText = File.ReadAllText(assetPath);
                    fileText = fileText.Replace("m_IsReadable: 0", "m_IsReadable: 1");
                    File.WriteAllText(assetPath, fileText);

                    //Not working for some reasons
                    //var meshOriginal = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                    //var meshInstance = Instantiate(meshOriginal);
                    //meshInstance.name = meshOriginal.name;

                    //meshInstance.UploadMeshData(false);
                    //meshOriginal.Clear();

                    //EditorUtility.CopySerialized(meshInstance, meshOriginal);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " meshes have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Mark Model As Non Readable", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    string fileText = File.ReadAllText(assetPath);
                    fileText = fileText.Replace("m_IsReadable: 1", "m_IsReadable: 0");
                    File.WriteAllText(assetPath, fileText);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " meshes have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Recalcuate Mesh Normals", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    var meshOriginal = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                    var meshInstance = Instantiate(meshOriginal);
                    meshInstance.name = meshOriginal.name;

                    meshInstance.RecalculateNormals();
                    meshOriginal.Clear();

                    EditorUtility.CopySerialized(meshInstance, meshOriginal);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " meshes have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Recalcuate Mesh Tangents", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    var meshOriginal = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                    var meshInstance = Instantiate(meshOriginal);
                    meshInstance.name = meshOriginal.name;

                    meshInstance.RecalculateTangents();
                    meshOriginal.Clear();

                    EditorUtility.CopySerialized(meshInstance, meshOriginal);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " meshes have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Delete Backup Prefabs", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Backup", "*.prefab");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    FileUtil.DeleteFileOrDirectory(assetPath);
                    FileUtil.DeleteFileOrDirectory(assetPath + ".meta");

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " backups have been deleted.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Delete Prefab Component", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("", "*.prefab");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (prefabObject.GetComponent<TVEPrefab>() != null)
                    {
                        var prefabInstance = Instantiate(prefabObject);

                        DestroyImmediate(prefabInstance.GetComponent<TVEPrefab>(), true);

                        PrefabUtility.SaveAsPrefabAssetAndConnect(prefabInstance, assetPath, InteractionMode.AutomatedAction);

                        var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                        EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                    }
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " prefabs have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Delete Converted Assets", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");
                GetAssetsPaths("TVE Material", "*.mat");
                GetAssetsPaths("TVE Texture", "*.png");
                GetAssetsPaths("TVE Texture", "*.tga");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    FileUtil.DeleteFileOrDirectory(assetPath);
                    FileUtil.DeleteFileOrDirectory(assetPath + ".meta");

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " assets have been deleted.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Cleanup All Name GUID", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");
                GetAssetsPaths("TVE Material", "*.mat");
                GetAssetsPaths("TVE Texture", "*.png");
                GetAssetsPaths("TVE Texture", "*.tga");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    var pathData = TVEUtils.GetPathData(assetPath);

                    AssetDatabase.RenameAsset(assetPath, pathData.name + " (TVE Texture)");

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " assets have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Cleanup Model Name GUID", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Model", "*.asset");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];
                    var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                    var pathData = TVEUtils.GetPathData(assetPath);

                    mesh.name = pathData.name + " (TVE Model)";

                    AssetDatabase.RenameAsset(assetPath, mesh.name);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " meshes have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Cleanup Material Name GUID", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Material", "*.mat");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];
                    var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                    var pathData = TVEUtils.GetPathData(assetPath);

                    material.name = pathData.name + " (TVE Material)";

                    AssetDatabase.RenameAsset(assetPath, material.name);

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " materials have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Cleanup Texture Name GUID", GUILayout.Height(24)))
            {
                allAssetsPaths = new List<string>();

                GetAssetsPaths("TVE Texture", "*.png");
                GetAssetsPaths("TVE Texture", "*.tga");

                for (int i = 0; i < allAssetsPaths.Count; i++)
                {
                    var assetPath = allAssetsPaths[i];

                    var pathData = TVEUtils.GetPathData(assetPath);

                    AssetDatabase.RenameAsset(assetPath, pathData.name + " (TVE Texture)");

                    var progress = (float)i * 1.0f / (float)allAssetsPaths.Count;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing " + Path.GetFileName(assetPath), progress);
                }

                EditorUtility.ClearProgressBar();

                Debug.Log("<b>[The Vegetation Engine]</b> " + allAssetsPaths.Count.ToString() + " textures have been processed.");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
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
        }

        void GetAssetsPaths(string searchPattern, string extension)
        {
            if (selectionIndex == 0)
            {
                MethodInfo getActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
                string selectedFolder = (string)getActiveFolderPath.Invoke(null, null);

                var selection = Directory.GetFiles(selectedFolder, extension, SearchOption.AllDirectories);

                for (int i = 0; i < selection.Length; i++)
                {
                    if (selection[i].Contains(searchPattern))
                    {
                        allAssetsPaths.Add(selection[i]);
                    }
                }
            }
            else
            {
                var selection = Selection.objects;

                for (int i = 0; i < selection.Length; i++)
                {
                    var assetPath = AssetDatabase.GetAssetPath(selection[i]);

                    if (assetPath.Contains(searchPattern))
                    {
                        allAssetsPaths.Add(assetPath);
                    }
                }
            }
        }
    }
}


