using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Beautify.Universal {

    public class FrameBrowser : EditorWindow {

        const string MASTER_FOLDER_NAME = "Frame Pack";

        Material referenceMaterial;
        Vector2 scrollPos;
        static int columnCount = 4;

        struct FrameEntry {
            public Material mat;
            public Texture2D frameMask;
        }

        struct FrameGroup {
            public string categoryPath;
            public string categoryName;
            public List<FrameEntry> frames;
            public bool visible;
        }
        FrameGroup[] groups;


        [MenuItem("Window/Beautify/Frame Browser")]
        public static void ShowBrowser() {
            GetWindow<FrameBrowser>("Frame Browser");
        }

        static class ShaderParams {
            public static int frameMaskTexture = Shader.PropertyToID("_FrameMask");
            public static int lutPreview = Shader.PropertyToID("_LUTPreview");
        }

        private void OnEnable() {
            RefreshFrames();
            ClearBackground();
        }

        void ClearBackground() {
            Shader.SetGlobalTexture(ShaderParams.lutPreview, Texture2D.whiteTexture);
        }

        private void OnGUI() {

            if (groups == null) {
                EditorGUILayout.HelpBox("Frame Pack not found.", MessageType.Info);
                if (GUILayout.Button("View Frame Pack on the Unity Asset Store")) {
                    Application.OpenURL("https://assetstore.unity.com/packages/slug/204058");
                }
                if (GUILayout.Button("Reload Frame Pack")) {
                    RefreshFrames();
                }
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Frames")) {
                RefreshFrames();
            }
            if (GUILayout.Button("Capture SceneView")) {
                RequestCapture(CameraType.SceneView);
            }
            if (GUILayout.Button("Capture GameView")) {
                RequestCapture(CameraType.Game);
            }
            if (GUILayout.Button("White Background")) {
                ClearBackground();
            }
            EditorGUILayout.EndHorizontal();

            columnCount = EditorGUILayout.IntSlider("Columns:", columnCount, 1, 5);

            Texture2D wt = Texture2D.whiteTexture;
            float rowHeight = 0.5f * EditorGUIUtility.currentViewWidth / columnCount;

            EditorGUILayout.HelpBox("Click on the name of a Frame to toggle it on/off.", MessageType.Info);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Beautify b = BeautifySettings.sharedSettings;
            if (b == null) {
                EditorGUILayout.HelpBox("Beautify not found in the scene.", MessageType.Warning);
            } else {
                for (int k = 0; k < groups.Length; k++) {
                    groups[k].visible = EditorGUILayout.Foldout(groups[k].visible, "Category: " + groups[k].categoryName);
                    if (groups[k].visible) {
                        int c = groups[k].frames.Count;
                        int matIndex = 0;
                        while (matIndex < c) {
                            EditorGUILayout.BeginVertical();
                            Rect rect = EditorGUILayout.GetControlRect();
                            float w = rect.width / columnCount;
                            rect.width = w - 5;
                            for (int col = 0; col < columnCount; col++) {
                                if (matIndex < c) {
                                    FrameEntry frameEntry = groups[k].frames[matIndex];
                                    if (frameEntry.mat != null) {
                                        rect.height = rowHeight;
                                        frameEntry.mat.SetTexture(ShaderParams.frameMaskTexture, frameEntry.frameMask);
                                        EditorGUI.DrawPreviewTexture(rect, wt, frameEntry.mat);
                                        rect.y += rowHeight;
                                        rect.height = 15;
                                        string frameName;
                                        if (b.frame.value && b.frame.overrideState && b.frameMask == frameEntry.frameMask) {
                                            frameName = "✔ " + frameEntry.mat.name;
                                        } else {
                                            frameName = frameEntry.mat.name;
                                        }
                                        if (GUI.Button(rect, frameName)) {
                                            if (b.frame.value && b.frame.overrideState && b.frameMask == frameEntry.frameMask) {
                                                b.frame.Override(false);
                                            } else {
                                                b.frame.Override(true);
                                                b.frameMask.Override(frameEntry.frameMask);
                                                b.frameColor.Override(Color.white);
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
                foreach (FrameGroup g in groups) {
                    if (g.frames != null) {
                        foreach (FrameEntry l in g.frames) {
                            if (l.mat != null) {
                                DestroyImmediate(l.mat);
                            }
                        }
                        g.frames.Clear();
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


        void RefreshFrames() {

            RequestCapture(BeautifyRendererFeature.captureCameraType);
            ReleaseGroups();
            if (referenceMaterial == null) {
                referenceMaterial = new Material(Shader.Find("Hidden/Beautify/FrameThumbnail"));
            }
            string[] res = Directory.GetDirectories(Application.dataPath, "*" + MASTER_FOLDER_NAME + "*", SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains("Frame Pack")) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                return;
            }
            string[] categories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            groups = new FrameGroup[categories.Length];
            for (int c = 0; c < categories.Length; c++) {
                FrameGroup group = new FrameGroup();
                group.categoryPath = categories[c];
                group.categoryName = Path.GetFileName(group.categoryPath);
                group.frames = new List<FrameEntry>();
                string[] frames = Directory.GetFiles(group.categoryPath, "*.png", SearchOption.AllDirectories);
                if (frames != null) {
                    for (int l = 0; l < frames.Length; l++) {
                        string framePath = frames[l];
                        int i = framePath.IndexOf("/Assets");
                        if (i < 0) continue;
                        framePath = framePath.Substring(i + 1);
                        Texture2D frameMask = AssetDatabase.LoadAssetAtPath<Texture>(framePath) as Texture2D;
                        if (frameMask != null) {
                            Material mat = Instantiate(referenceMaterial);
                            mat.name = Path.GetFileNameWithoutExtension(framePath);
                            FrameEntry entry = new FrameEntry();
                            entry.mat = mat;
                            entry.frameMask = frameMask;
                            group.frames.Add(entry);
                        }
                    }
                }
                groups[c] = group;
            }

        }

    }

}