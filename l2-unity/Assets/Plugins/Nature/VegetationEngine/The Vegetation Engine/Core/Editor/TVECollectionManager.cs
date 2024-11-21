// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace TheVegetationEngine
{
    public class TVECollectionManager : EditorWindow
    {
        float GUI_HALF_EDITOR_WIDTH = 220;
        float GUI_HALF_OPTION_WIDTH = 220;
        const int GUI_SELECTION_HEIGHT = 24;

        string[] collectionPaths;
        List<TVECollectionData> collectionDataSet;
        //List<string> collectionStatus;
        //List<string> collectionOnline;
        //List<string> collectionMessage;

        bool showSelection = true;

        GUIStyle stylePopup;
        GUIStyle styleLabel;
        GUIStyle styleHelpBox;
        GUIStyle styleCenteredHelpBox;

        Color bannerColor;
        string bannerText;
        static TVECollectionManager window;
        Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Window/BOXOPHOBIC/The Vegetation Engine/Collection Manager", false, 3000)]
        public static void ShowWindow()
        {
            window = GetWindow<TVECollectionManager>(false, "Collection Manager", true);
            window.minSize = new Vector2(400, 300);
        }

        void OnEnable()
        {
            GetCollections();

            bannerColor = new Color(0.890f, 0.745f, 0.309f);
            bannerText = "Collection Manager";
        }

        void OnGUI()
        {
            SetGUIStyles();

            GUI_HALF_EDITOR_WIDTH = this.position.width / 2.0f - 24;
            GUI_HALF_OPTION_WIDTH = GUI_HALF_EDITOR_WIDTH - 38;

            StyledGUI.DrawWindowBanner(bannerColor, bannerText);

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);

            GUILayout.BeginVertical();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(this.position.width - 28), GUILayout.Height(this.position.height - 80));

            EditorGUILayout.HelpBox("The collection manager works by collecting all the necessary textures from the original package so everything is in one place and ready to be used in your project!", MessageType.Info, true);

            GUILayout.Space(15);

            if (showSelection)
            {
                if (StyledButton("Hide Collection Selection"))
                    showSelection = !showSelection;
            }
            else
            {
                if (StyledButton("Show Collection Selection"))
                    showSelection = !showSelection;
            }

            if (showSelection)
            {
                for (int i = 0; i < collectionPaths.Length; i++)
                {
                    StyledCollection(i);
                }
            }

            GUILayout.Space(10);

            GUILayout.FlexibleSpace();

            GUILayout.Space(20);

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.Space(13);
            GUILayout.EndHorizontal();
        }

        void OnFocus()
        {
            GetCollections();

            Repaint();
        }

        void SetGUIStyles()
        {
            stylePopup = new GUIStyle(EditorStyles.popup)
            {
                alignment = TextAnchor.MiddleCenter
            };

            styleLabel = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                richText = true,
                alignment = TextAnchor.MiddleRight,
            };

            styleHelpBox = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft,
            };

            styleCenteredHelpBox = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        bool StyledButton(string text)
        {
            bool value = GUILayout.Button("<b>" + text + "</b>", styleCenteredHelpBox, GUILayout.Height(GUI_SELECTION_HEIGHT));

            return value;
        }

        void StyledCollection(int index)
        {
            var collectionData = collectionDataSet[index];
            var collectionName = Path.GetFileNameWithoutExtension(collectionPaths[index]);

            GUILayout.Label("<size=10><b>" + collectionName + "</b></size>", styleHelpBox, GUILayout.Height(GUI_SELECTION_HEIGHT));

            var lastRect = GUILayoutUtility.GetLastRect();
            var selectionRect = new Rect(lastRect.x, lastRect.y, lastRect.width / 2 - 20, lastRect.height);
            var installRect = new Rect(lastRect.width / 2, lastRect.y + 3, lastRect.width / 2, 18);
            var iconRect = new Rect(lastRect.width - 18, lastRect.y + 2, 20, 20);
            var processRect = new Rect(lastRect.width / 2 -20, lastRect.y + 3, 18, 18);

            GUI.Label(selectionRect, new GUIContent("", collectionData.message));

            if (GUI.Button(selectionRect, "", GUIStyle.none))
            {
                Application.OpenURL(collectionData.online);
            }

            if (collectionData.status == "Installed")
            {
                GUI.color = new Color(0.0f, 1f, 0.5f);
                GUI.Label(iconRect, EditorGUIUtility.IconContent("Installed@2x"));
                GUI.color = Color.white;
                GUI.Label(iconRect, new GUIContent("", "The collection is installed!"));
            }
            else if (collectionData.status == "Invalid")
            {
                GUI.Label(iconRect, EditorGUIUtility.IconContent("console.erroricon.sml"));
                GUI.Label(iconRect, new GUIContent("", "The original package is required for the collection to be installed!"));
            }
            else
            {
                if (GUI.Button(installRect, new GUIContent("Install Collection", "")))
                {
                    InstallCollection(index, true);
                }
            }

#if THE_VEGETATION_ENGINE_DEVELOPMENT
            if (GUI.Button(processRect, new GUIContent("P", "")))
            {
                InstallCollection(index, false);
            }
#endif
        }

        void GetCollections()
        {
            collectionDataSet = new List<TVECollectionData>();

            collectionPaths = TVEUtils.FindAssets("*.tvecollection", false);

            for (int i = 0; i < collectionPaths.Length; i++)
            {
                var collectionData = new TVECollectionData();

                StreamReader reader = new StreamReader(collectionPaths[i]);

                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                reader.Close();

                var infoDecode = "";

                for (int s = 0; s < lines.Count; s++)
                {
                    if (lines[s].Contains("InfoDecode"))
                    {
                        infoDecode = lines[s].Replace("InfoDecode ", "");
                    }

                    if (lines[s].Contains("InfoStatus"))
                    {
                        var value = lines[s].Replace("InfoStatus ", "");

                        if (value.Contains("Installed"))
                        {
                            collectionData.status = "Installed";
                        }
                        else
                        {
                            var decodePath = AssetDatabase.GUIDToAssetPath(infoDecode);

                            if (File.Exists(decodePath))
                            {
                                var decodeName = Path.GetFileName(decodePath);

                                var decodeHash = "0." + Mathf.Abs(decodeName.GetHashCode());
                                var desodeTrim = decodeHash.Substring(0, 6);
                                var decodeValue = float.Parse(desodeTrim, CultureInfo.InvariantCulture);

                                collectionData.status = "Valid";
                                collectionData.decode = decodeValue;
                            }
                            else
                            {
                                collectionData.status = "Invalid";
                            }
                        }
                    }
                    
                    if (lines[s].Contains("InfoOnline"))
                    {
                        var value = lines[s].Replace("InfoOnline ", "");

                        collectionData.online = value;
                    }
                    
                    if (lines[s].Contains("InfoMessage"))
                    {
                        var value = lines[s].Replace("InfoMessage ", "");

                        collectionData.message = value;
                    }
                }

                collectionDataSet.Add(collectionData);
            }
        }

        void InstallCollection(int index, bool install)
        {
            var decodeHash = collectionDataSet[index].decode;

            var collectionFolder = Path.GetDirectoryName(collectionPaths[index]);
            var collectionModelsAll = Directory.GetFiles(collectionFolder, "*.asset", SearchOption.AllDirectories);
            var collectionTexturesPng = Directory.GetFiles(collectionFolder, "*.png", SearchOption.AllDirectories);
            var collectionTexturesTga = Directory.GetFiles(collectionFolder, "*.tga", SearchOption.AllDirectories);
            var collectionMaterials = Directory.GetFiles(collectionFolder, "*.mat", SearchOption.AllDirectories);

            List<string> collectionTextures = new List<string>();

            for (int i = 0; i < collectionTexturesPng.Length; i++)
            {
                var name = Path.GetFileNameWithoutExtension(collectionTexturesPng[i]);

                if (name.Contains("TVE") && name.Contains("Texture"))
                {
                    collectionTextures.Add(collectionTexturesPng[i]);
                }
            }

            for (int i = 0; i < collectionTexturesTga.Length; i++)
            {
                var name = Path.GetFileNameWithoutExtension(collectionTexturesTga[i]);

                if (name.Contains("TVE") && name.Contains("Texture"))
                {
                    collectionTextures.Add(collectionTexturesTga[i]);
                }
            }

            List<string> collectionModels = new List<string>();

            for (int i = 0; i < collectionModelsAll.Length; i++)
            {
                var name = Path.GetFileNameWithoutExtension(collectionModelsAll[i]);

                if (name.Contains("TVE") && name.Contains("Model"))
                {
                    collectionModels.Add(collectionModelsAll[i]);
                }
            }

            var allFoldersInCollection = Directory.GetDirectories(collectionFolder, "Textures", SearchOption.AllDirectories);
            var texturesFolder = "";

            for (int i = 0; i < allFoldersInCollection.Length; i++)
            {
                var folder = allFoldersInCollection[i];

                if (folder.Contains("Textures"))
                {
                    texturesFolder = folder;
                }
            }

            for (int i = 0; i < collectionModels.Count; i++)
            {
                var assetPath = collectionModels[i];
                var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                var instanceMesh = Instantiate(mesh);
                instanceMesh.name = mesh.name;

                var vertices = mesh.vertices;

                for (int j = 0; j < vertices.Length; j++)
                {
                    var sin = Mathf.Sin(j * decodeHash);

                    if (Mathf.Sign(sin) < 0)
                    {
                        vertices[j] = -1 * vertices[j];
                    }
                }

                instanceMesh.vertices = vertices;
                mesh.Clear();

                EditorUtility.CopySerialized(instanceMesh, mesh);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var progress = (float)i * 1.0f / (float)collectionModels.Count;
                EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing Meshes", progress);
            }

            for (int i = 0; i < collectionTextures.Count; i++)
            {
                var assetPath = collectionTextures[i];

                TextureImporter importer;

                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                var pixels = texture.GetPixels();

                for (int j = 0; j < pixels.Length; j++)
                {
                    var sin = Mathf.Sin(j * decodeHash);

                    if (Mathf.Sign(sin) < 0)
                    {
                        pixels[j] = Color.white - pixels[j];
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                if (assetPath.EndsWith("png"))
                {
                    File.WriteAllBytes(assetPath, texture.EncodeToPNG());
                }
                else
                {
                    File.WriteAllBytes(assetPath, texture.EncodeToTGA());
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                importer.isReadable = false;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SaveAndReimport();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var progress = (float)i * 1.0f / (float)collectionTextures.Count;
                EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing Textures", progress);
            }

            if (install)
            {
                for (int i = 0; i < collectionMaterials.Length; i++)
                {
                    var assetPath = collectionMaterials[i];
                    var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                    var shader = material.shader;

                    for (int s = 0; s < ShaderUtil.GetPropertyCount(shader); s++)
                    {
                        var propName = ShaderUtil.GetPropertyName(shader, s);
                        var propType = ShaderUtil.GetPropertyType(shader, s);

                        if (propType == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            var texture = material.GetTexture(propName);

                            if (texture != null)
                            {
                                var texturePath = AssetDatabase.GetAssetPath(texture);

                                // Skip TVE textures because they are available already
                                if (!texturePath.Contains("The Vegetation Engine"))
                                {
                                    var textureName = Path.GetFileName(texturePath);
                                    var savePath = texturesFolder + "/" + textureName;

                                    if (!File.Exists(savePath))
                                    {
                                        AssetDatabase.CopyAsset(texturePath, savePath);
                                        AssetDatabase.SaveAssets();
                                        AssetDatabase.Refresh();
                                    }

                                    material.SetTexture(propName, AssetDatabase.LoadAssetAtPath<Texture>(savePath));
                                } 
                            }
                        }
                    }

                    TVEUtils.SetMaterialSettings(material);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    var progress = (float)i * 1.0f / (float)collectionMaterials.Length;
                    EditorUtility.DisplayProgressBar("The Vegetatin Engine", "Processing Materials", progress);
                }

                StreamReader reader = new StreamReader(collectionPaths[index]);

                List<string> lines = new List<string>();

                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                reader.Close();

                for (int s = 0; s < lines.Count; s++)
                {
                    if (lines[s].Contains("InfoStatus"))
                    {
                        lines[s] = "InfoStatus Installed";
                    }
                }

                StreamWriter writer = new StreamWriter(collectionPaths[index], false);

                for (int s = 0; s < lines.Count; s++)
                {
                    writer.WriteLine(lines[s]);
                }

                writer.Close();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                var collectionName = Path.GetFileNameWithoutExtension(collectionPaths[index]);

                Debug.Log("<b>[The Vegetation Engine]</b> Collection " + collectionName + " is installed.");

                GetCollections();
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
