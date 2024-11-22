using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

namespace Beautify.Universal {

    public class LUTBrowser : EditorWindow {

        const string MASTER_FOLDER_NAME = "LUT Pack";

        Material referenceMaterial;
        Vector2 scrollPos;
        static int columnCount = 4;
        VolumeProfile profile;

        struct LUTEntry {
            public Material mat;
            public Texture2D lutTex;
        }

        struct LUTGroup {
            public string categoryPath;
            public string categoryName;
            public List<LUTEntry> luts;
            public bool visible;
        }
        LUTGroup[] groups;


        [MenuItem("Window/Beautify/LUT Browser")]
        public static void ShowBrowser() {
            GetWindow<LUTBrowser>("LUT Browser");
        }

        static class ShaderParams {
            public static int lutTex = Shader.PropertyToID("_LUTTex");
        }

        private void OnEnable() {
            RefreshLUTs();
        }

        private void OnGUI() {

            if (groups == null) {
                EditorGUILayout.HelpBox("LUT Pack not found.", MessageType.Info);
                if (GUILayout.Button("View LUT Pack on the Unity Asset Store")) {
                    Application.OpenURL("https://assetstore.unity.com/packages/slug/202502");
                }
                if (GUILayout.Button("Reload LUT Pack")) {
                    RefreshLUTs();
                }
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find LUTs")) {
                RefreshLUTs();
            }
            if (GUILayout.Button("Capture SceneView")) {
                RequestCapture(CameraType.SceneView);
            }
            if (GUILayout.Button("Capture GameView")) {
                RequestCapture(CameraType.Game);
            }
            EditorGUILayout.EndHorizontal();

            if (profile == null) {
                profile = BeautifySettings.currentProfile;
            }
            EditorGUILayout.BeginHorizontal();
            profile = (VolumeProfile)EditorGUILayout.ObjectField("Current Profile:", profile, typeof(VolumeProfile), false);
            if (GUILayout.Button(new GUIContent("Auto Select", "Automatically selects the profile used in a scene volume using Beautify"), GUILayout.Width(90))) {
                profile = null;
            }
            EditorGUILayout.EndHorizontal();

            Texture2D wt = Texture2D.whiteTexture;
            float rowHeight = 0.5f * EditorGUIUtility.currentViewWidth / columnCount;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Beautify b = null;
            if (profile != null) {
                profile.TryGet(out b);
            }
            if (b == null) {
                EditorGUILayout.HelpBox("Beautify not found in the seleced profile.", MessageType.Warning);
            } else {
                columnCount = EditorGUILayout.IntSlider("Columns:", columnCount, 1, 5);
                EditorGUILayout.HelpBox("Click on the name of a LUT to toggle it on/off. Use the Intensity slider in Beautify inspector to customize the LUT strength.", MessageType.Info);
                for (int k = 0; k < groups.Length; k++) {
                    groups[k].visible = EditorGUILayout.Foldout(groups[k].visible, "Category: " + groups[k].categoryName);
                    if (groups[k].visible) {
                        int c = groups[k].luts.Count;
                        int matIndex = 0;
                        while (matIndex < c) {
                            EditorGUILayout.BeginVertical();
                            Rect rect = EditorGUILayout.GetControlRect();
                            float w = rect.width / columnCount;
                            rect.width = w - 5;
                            for (int col = 0; col < columnCount; col++) {
                                if (matIndex < c) {
                                    LUTEntry lutEntry = groups[k].luts[matIndex];
                                    if (lutEntry.mat != null) {
                                        rect.height = rowHeight;
                                        lutEntry.mat.SetTexture(ShaderParams.lutTex, lutEntry.lutTex);
                                        EditorGUI.DrawPreviewTexture(rect, wt, lutEntry.mat);
                                        rect.y += rowHeight;
                                        rect.height = 15;
                                        string lutName;
                                        if (b.lut.value && b.lut.overrideState && b.lutTexture == lutEntry.lutTex) {
                                            lutName = "✔ " + lutEntry.mat.name;
                                        } else {
                                            lutName = lutEntry.mat.name;
                                        }
                                        if (GUI.Button(rect, lutName)) {
                                            if (b.lut.value && b.lut.overrideState && b.lutTexture == lutEntry.lutTex) {
                                                b.lut.Override(false);
                                            } else {
                                                b.lut.Override(true);
                                                if (!b.lutIntensity.overrideState) {
                                                    b.lutIntensity.Override(1f);
                                                }
                                                b.lutTexture.Override(lutEntry.lutTex);
                                            }
                                            EditorUtility.SetDirty(b);
                                            if (!Application.isPlaying) {
                                                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                                            }
                                        }
                                        rect.y -= rowHeight;
                                        rect.x += w;
                                    }
                                    matIndex++;
                                }
                            }
                            GUILayout.Space(rowHeight);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Separator();
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void OnDestroy() {
            ReleaseGroups();
        }

        void ReleaseGroups() {
            if (groups != null) {
                foreach (LUTGroup g in groups) {
                    if (g.luts != null) {
                        foreach (LUTEntry l in g.luts) {
                            if (l.mat != null) {
                                DestroyImmediate(l.mat);
                            }
                        }
                        g.luts.Clear();
                    }
                }
            }
            groups = null;
        }

        void RequestCapture(CameraType cameraType) {

            BeautifySettings b;
#if UNITY_2023_1_OR_NEWER
            b = FindAnyObjectByType<BeautifySettings>();
#else
            b = FindObjectOfType<BeautifySettings>();
#endif
            if (b == null) {
                Debug.LogError("Beautify not found. It's requred for the LUT Browser functionality.");
                return;
            }
            BeautifyRendererFeature.captureCameraType = cameraType;
            BeautifyRendererFeature.requestScreenCapture = true;
            EditorUtility.SetDirty(BeautifySettings.sharedSettings);
        }


        void RefreshLUTs() {

            RequestCapture(BeautifyRendererFeature.captureCameraType);
            ReleaseGroups();
            if (referenceMaterial == null) {
                referenceMaterial = new Material(Shader.Find("Hidden/Beautify/LUTThumbnail"));
            }
            string[] res = Directory.GetDirectories(Application.dataPath, "*" + MASTER_FOLDER_NAME + "*", SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains("LUT Pack")) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                return;
            }
            string[] categories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            groups = new LUTGroup[categories.Length];
            for (int c = 0; c < categories.Length; c++) {
                LUTGroup group = new LUTGroup();
                group.categoryPath = categories[c];
                group.categoryName = Path.GetFileName(group.categoryPath);
                group.luts = new List<LUTEntry>();
                string[] luts = Directory.GetFiles(group.categoryPath, "*.png", SearchOption.AllDirectories);
                if (luts != null) {
                    for (int l = 0; l < luts.Length; l++) {
                        string lutPath = luts[l];
                        int i = lutPath.IndexOf("/Assets");
                        if (i < 0) continue;
                        lutPath = lutPath.Substring(i + 1);
                        Texture2D lutTex = AssetDatabase.LoadAssetAtPath<Texture>(lutPath) as Texture2D;
                        if (lutTex != null) {
                            Material mat = Instantiate(referenceMaterial);
                            mat.name = Path.GetFileNameWithoutExtension(lutPath);
                            LUTEntry entry = new LUTEntry();
                            entry.mat = mat;
                            entry.lutTex = lutTex;
                            group.luts.Add(entry);
                        }
                    }
                }
                groups[c] = group;
            }

        }

    }

}
