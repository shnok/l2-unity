//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace JBooth.MicroSplat
{
    // Diffuse always Color/Height
    // Fastest Packing, Normal in A/G, Smoothness in R, AO in B
    // Quality Packing, Normal in own array, Smoothness/AO in G/A of separate array
    public class TextureArrayConfigUI
    {
        static GUIContent CPlatformOverrides = new GUIContent("Platform Compression Overrides", "Override the compression type on a per platform basis");
        static GUIContent CTextureMode = new GUIContent("Texturing Mode", "Do you have just diffuse and normal, or a fully PBR pipeline with height, smoothness, and ao textures?");
        static GUIContent CSourceTextureSize = new GUIContent("Source Texture Size", "Reduce source texture size to save memory in builds");
        static GUIContent CPackingMode = new GUIContent("Packing Mode", "Can smoothness and ao be packed in with the normals?");
        static GUIContent CPBRWorkflow = new GUIContent("PBR Workflow", "Metallic or Specular workflow?");
#if __MICROSPLAT_TEXTURECLUSTERS__
        static GUIContent CClusterMode = new GUIContent("Cluster Mode", "Add extra slots for packing parallel arrays for texture clustering");
#endif
#if __MICROSPLAT_DETAILRESAMPLE__
        static GUIContent CAntiTileArray = new GUIContent("AntiTile Array", "Create an array for each texture to have it's own Noise Normal, Detail, and Distance noise texture");
#endif

#if __MICROSPLAT_TRAX__
        static GUIContent CTraxArray = new GUIContent("Trax Array", "Create an array for each texture to have it's own Trax texture arrays");
#endif


        static GUIContent CEmisMetalArray = new GUIContent("Emissive/Metal array", "Create a texture array for emissive and metallic materials");

        static GUIContent CDiffuse = new GUIContent("Diffuse", "Diffuse or Albedo texture");
        static GUIContent CNormal = new GUIContent("Normal", "Normal map");
        static GUIContent CAO = new GUIContent("AO", "Ambient Occlusion map");
        static GUIContent CSmoothness = new GUIContent("Smoothness", "Smoothness map, or roughness map with invert on");
        static GUIContent CHeight = new GUIContent("Height", "Height Map");
        static GUIContent CAlpha = new GUIContent("Alpha", "Alpha Map");
        static GUIContent CSpecular = new GUIContent("Specular", "Specular Map");
        static GUIContent CNoiseNormal = new GUIContent("Noise Normal", "Normal to bend in over a larger area");
        static GUIContent CDetailNoise = new GUIContent("Detail", "Noise texture to blend in when close");
        static GUIContent CDistanceNoise = new GUIContent("Distance", "Noise texture to blend in when far away");
        static GUIContent CDiffuseIsLinear = new GUIContent("Diffuse Is Linear", "Treat color textures as linear data");
        static GUIContent CSplat = new GUIContent("Splat", "Splat map for decal system");
#if __MICROSPLAT_SLOPETEXTURE__
        static GUIContent CMaxSyncTexture = new GUIContent("Max Sync Textures", "Maximum number of textures to sync back to Unity Terrains. This allows you to have a longer array than used by the terrain.");
#endif

        void DrawHeader(TextureArrayConfig cfg)
        {
            if (cfg.textureMode != TextureArrayConfig.TextureMode.Basic)
            {
                if (cfg.IsDecalSplat())
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField(CSplat, GUILayout.Width(64));

                    EditorGUILayout.EndVertical();
                }
                else
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField("", GUILayout.Width(30));
                    EditorGUILayout.LabelField("Channel", GUILayout.Width(64));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(20));
                    EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(64));
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField((cfg.IsScatter()) ? CAlpha : CHeight, GUILayout.Width(64));
                    cfg.allTextureChannelHeight = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelHeight, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(CSmoothness, GUILayout.Width(64));
                    cfg.allTextureChannelSmoothness = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelSmoothness, GUILayout.Width(64));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField(cfg.IsStarReach() || cfg.IsDecal() ? CAlpha : CAO, GUILayout.Width(64));
                    cfg.allTextureChannelAO = (TextureArrayConfig.AllTextureChannel)EditorGUILayout.EnumPopup(cfg.allTextureChannelAO, GUILayout.Width(64));

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.Box(Texture2D.blackTexture, GUILayout.Height(3), GUILayout.ExpandWidth(true));


            }
        }

        void DrawAntiTileEntry(TextureArrayConfig cfg, TextureArrayConfig.TextureEntry e, int i)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(CNoiseNormal, GUILayout.Width(92));
            e.noiseNormal = (Texture2D)EditorGUILayout.ObjectField(e.noiseNormal, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(CDetailNoise, GUILayout.Width(92));
            e.detailNoise = (Texture2D)EditorGUILayout.ObjectField(e.detailNoise, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            e.detailChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.detailChannel, GUILayout.Width(64));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(CDistanceNoise, GUILayout.Width(92));
            e.distanceNoise = (Texture2D)EditorGUILayout.ObjectField(e.distanceNoise, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
            e.distanceChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.distanceChannel, GUILayout.Width(64));
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            if (e.noiseNormal == null)
            {
                int index = (int)Mathf.Repeat(i, 3);
                e.noiseNormal = MicroSplatUtilities.GetAutoTexture("microsplat_def_detail_normal_0" + (index + 1).ToString());
            }

        }

        void SwapEntry(TextureArrayConfig cfg, int src, int targ)
        {
            if (src >= 0 && targ >= 0 && src < cfg.sourceTextures.Count && targ < cfg.sourceTextures.Count)
            {
                {
                    var s = cfg.sourceTextures[src];
                    cfg.sourceTextures[src] = cfg.sourceTextures[targ];
                    cfg.sourceTextures[targ] = s;
                }
                if (cfg.sourceTextures2.Count == cfg.sourceTextures.Count)
                {
                    var s = cfg.sourceTextures2[src];
                    cfg.sourceTextures2[src] = cfg.sourceTextures2[targ];
                    cfg.sourceTextures2[targ] = s;
                }
                if (cfg.sourceTextures3.Count == cfg.sourceTextures.Count)
                {
                    var s = cfg.sourceTextures3[src];
                    cfg.sourceTextures3[src] = cfg.sourceTextures3[targ];
                    cfg.sourceTextures3[targ] = s;
                }

            }

        }

        // returns -1 if it's not a texture on disk, 0 if it's not a normal, 1 if it is. 
        int IsNormal(Texture t)
        {
            var path = AssetDatabase.GetAssetPath(t);
            if (!string.IsNullOrEmpty(path))
            {
                AssetImporter ai = AssetImporter.GetAtPath(path);
                if (ai == null)
                {
                    return -1;
                }
                if ((ai as TextureImporter) == null)
                    return -1;
                var ti = (TextureImporter)ai;
                if (null == ti)
                {
                    return -1;
                }

                if (ti.textureType == TextureImporterType.NormalMap)
                {
                    return 1;
                }
                return 0;
            }
            return -1;
        }

        private void OnDestroy()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        private void OnDisable()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        void SetToNormal(Texture t)
        {
            var path = AssetDatabase.GetAssetPath(t);
            if (!string.IsNullOrEmpty(path))
            {
                var ti = (TextureImporter)AssetImporter.GetAtPath(path);
                ti.textureType = TextureImporterType.NormalMap;
                ti.SaveAndReimport();
            }
        }

        void DrawTraxEntry(TextureArrayConfig cfg, TextureArrayConfig.TextureEntry e, int i, bool controls = true)
        {
            using (new GUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Trax Array");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                if (controls)
                {
                    EditorGUILayout.LabelField(CDiffuse, GUILayout.Width(64));
                }
                e.traxDiffuse = (Texture2D)EditorGUILayout.ObjectField(e.traxDiffuse, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (controls)
                {
                    EditorGUILayout.LabelField(CNormal, GUILayout.Width(64));
                }
                e.traxNormal = (Texture2D)EditorGUILayout.ObjectField(e.traxNormal, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                if (e.traxNormal != null)
                {
                    if (IsNormal(e.traxNormal) == 0)
                    {
                        EditorGUILayout.HelpBox("not set to normal!", MessageType.Error);
                        if (GUILayout.Button("Fix"))
                        {
                            SetToNormal(e.traxNormal);
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                if (cfg.textureMode != TextureArrayConfig.TextureMode.Basic)
                {
                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(CHeight, GUILayout.Width(64));
                    }
                    e.traxHeight = (Texture2D)EditorGUILayout.ObjectField(e.traxHeight, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelHeight == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.traxHeightChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.traxHeightChannel, GUILayout.Width(64));
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(CSmoothness, GUILayout.Width(64));
                    }
                    e.traxSmoothness = (Texture2D)EditorGUILayout.ObjectField(e.traxSmoothness, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelSmoothness == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.traxSmoothnessChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.traxSmoothnessChannel, GUILayout.Width(64));
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Invert", GUILayout.Width(44));
                    e.traxIsRoughness = EditorGUILayout.Toggle(e.traxIsRoughness, GUILayout.Width(20));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();


                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(CAO, GUILayout.Width(64));
                    }
                    e.traxAO = (Texture2D)EditorGUILayout.ObjectField(e.traxAO, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelAO == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.traxAOChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.traxAOChannel, GUILayout.Width(64));
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        bool DrawTextureEntry(TextureArrayConfig cfg, TextureArrayConfig.TextureEntry e, int i, bool controls = true)
        {
            bool ret = false;
            if (controls)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20));

                if (e.HasTextures(cfg.pbrWorkflow))
                {
                    EditorGUILayout.LabelField(e.diffuse != null ? e.diffuse.name : "empty");
                    ret = GUILayout.Button("Clear Entry");
                }
                else
                {
                    EditorGUILayout.HelpBox("Removing an entry completely can cause texture choices to change on existing terrains. You can leave it blank to preserve the texture order and MicroSplat will put a dummy texture into the array.", MessageType.Warning);
                    ret = (GUILayout.Button("Delete Entry"));
                }

                if (GUILayout.Button("Up", GUILayout.Width(40)))
                {
                    SwapEntry(cfg, i, i - 1);
                }
                if (GUILayout.Button("Down", GUILayout.Width(42)))
                {
                    SwapEntry(cfg, i, i + 1);
                }
                EditorGUILayout.EndHorizontal();
                e.terrainLayer = (TerrainLayer)EditorGUILayout.ObjectField(new GUIContent("LayerFile", "Terrain Layer file to sync with"), e.terrainLayer, typeof(TerrainLayer), false);
            }

            EditorGUILayout.BeginHorizontal();

            if (cfg.IsDecalSplat())
            {
                EditorGUILayout.BeginVertical();
                if (controls)
                {
                    EditorGUILayout.LabelField(CSplat, GUILayout.Width(64));
                }
                e.splat = (Texture2D)EditorGUILayout.ObjectField(e.splat, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                EditorGUILayout.EndVertical();
            }
            else
            {

                EditorGUILayout.BeginVertical();
                if (controls)
                {
                    EditorGUILayout.LabelField(CDiffuse, GUILayout.Width(64));
                }
                e.diffuse = (Texture2D)EditorGUILayout.ObjectField(e.diffuse, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (controls)
                {
                    EditorGUILayout.LabelField(CNormal, GUILayout.Width(64));
                }
                e.normal = (Texture2D)EditorGUILayout.ObjectField(e.normal, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                if (e.normal != null)
                {
                    if (IsNormal(e.normal) == 0)
                    {
                        EditorGUILayout.HelpBox("not set to normal!", MessageType.Error);
                        if (GUILayout.Button("Fix"))
                        {
                            SetToNormal(e.normal);
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                if (cfg.textureMode != TextureArrayConfig.TextureMode.Basic || cfg.IsScatter() || cfg.IsDecal() || cfg.IsStarReach())
                {
                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(cfg.IsScatter() ? CAlpha : CHeight, GUILayout.Width(64));
                    }

                    e.height = (Texture2D)EditorGUILayout.ObjectField(e.height, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelHeight == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.heightChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.heightChannel, GUILayout.Width(64));
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(CSmoothness, GUILayout.Width(64));
                    }
                    e.smoothness = (Texture2D)EditorGUILayout.ObjectField(e.smoothness, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelSmoothness == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.smoothnessChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.smoothnessChannel, GUILayout.Width(64));
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Invert", GUILayout.Width(44));
                    e.isRoughness = EditorGUILayout.Toggle(e.isRoughness, GUILayout.Width(20));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();


                    EditorGUILayout.BeginVertical();
                    if (controls)
                    {
                        EditorGUILayout.LabelField(cfg.IsDecal() || cfg.IsStarReach() ? CAlpha : CAO, GUILayout.Width(64));
                    }
                    e.ao = (Texture2D)EditorGUILayout.ObjectField(e.ao, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    if (cfg.allTextureChannelAO == TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        e.aoChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.aoChannel, GUILayout.Width(64));
                    }

                    EditorGUILayout.EndVertical();

                    if (!cfg.IsScatter() && cfg.emisMetalArray)
                    {

                        EditorGUILayout.BeginVertical();
                        if (controls)
                        {
                            EditorGUILayout.LabelField(new GUIContent("Emissive"), GUILayout.Width(64));
                        }
                        e.emis = (Texture2D)EditorGUILayout.ObjectField(e.emis, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                        EditorGUILayout.EndVertical();

                        if (cfg.pbrWorkflow != TextureArrayConfig.PBRWorkflow.Specular)
                        {
                            EditorGUILayout.BeginVertical();
                            if (controls)
                            {
                                EditorGUILayout.LabelField(new GUIContent("Metal"), GUILayout.Width(64));
                            }
                            e.metal = (Texture2D)EditorGUILayout.ObjectField(e.metal, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                            e.metalChannel = (TextureArrayConfig.TextureChannel)EditorGUILayout.EnumPopup(e.metalChannel, GUILayout.Width(64));

                            EditorGUILayout.EndVertical();
                        }
                    }

                    if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
                    {
                        EditorGUILayout.BeginVertical();
                        if (controls)
                        {
                            EditorGUILayout.LabelField(CSpecular, GUILayout.Width(64));
                        }
                        e.specular = (Texture2D)EditorGUILayout.ObjectField(e.specular, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                        EditorGUILayout.EndVertical();
                    }

                }

            }
            EditorGUILayout.EndHorizontal();

            return ret;
        }



        void Remove(TextureArrayConfig cfg, int i)
        {
            cfg.sourceTextures.RemoveAt(i);
            cfg.sourceTextures2.RemoveAt(i);
            cfg.sourceTextures3.RemoveAt(i);
        }

        void Reset(TextureArrayConfig cfg, int i)
        {
            cfg.sourceTextures[i].Reset();
            cfg.sourceTextures2[i].Reset();
            cfg.sourceTextures3[i].Reset();
        }


        void DrawOverrideGUI(SerializedObject serializedObject, TextureArrayConfig cfg)
        {
            var prop = serializedObject.FindProperty("platformOverrides");
            EditorGUILayout.PropertyField(prop, CPlatformOverrides, true);
        }

        public void OnInspectorGUI(TextureArrayConfig cfg, SerializedObject serializedObject)
        {
            serializedObject.Update();
            TextureArrayConfigEditor.MatchArrayLength(cfg);
            EditorGUI.BeginChangeCheck();
            cfg.textureMode = (TextureArrayConfig.TextureMode)EditorGUILayout.EnumPopup(CTextureMode, cfg.textureMode);
            if (!cfg.IsDecal() && !cfg.IsScatter() && !cfg.IsDecalSplat())
            {
                cfg.packingMode = (TextureArrayConfig.PackingMode)EditorGUILayout.EnumPopup(CPackingMode, cfg.packingMode);
                cfg.pbrWorkflow = (TextureArrayConfig.PBRWorkflow)EditorGUILayout.EnumPopup(CPBRWorkflow, cfg.pbrWorkflow);
                EditorGUI.BeginChangeCheck();
                cfg.sourceTextureSize = (TextureArrayConfig.SourceTextureSize)EditorGUILayout.EnumPopup(CSourceTextureSize, cfg.sourceTextureSize);
            }
            cfg.diffuseIsLinear = EditorGUILayout.Toggle(CDiffuseIsLinear, cfg.diffuseIsLinear);
#if __MICROSPLAT_SLOPETEXTURE__
            if (!cfg.IsScatter() && !cfg.IsDecal() && !cfg.IsDecalSplat())
            {
                cfg.maxSyncCount = EditorGUILayout.IntSlider(CMaxSyncTexture, cfg.maxSyncCount, 4, 32);
            }
#endif


            if (cfg.IsScatter() || cfg.IsDecal() || cfg.IsDecalSplat())
            {
                cfg.clusterMode = TextureArrayConfig.ClusterMode.None;
                if (cfg.IsDecal())
                {
                    cfg.emisMetalArray = EditorGUILayout.Toggle(CEmisMetalArray, cfg.emisMetalArray);
                }
            }
            else
            {
#if __MICROSPLAT_DETAILRESAMPLE__
                cfg.antiTileArray = EditorGUILayout.Toggle(CAntiTileArray, cfg.antiTileArray);
#endif

#if __MICROSPLAT_TRAX__
                cfg.traxArray = EditorGUILayout.Toggle(CTraxArray, cfg.traxArray);
#endif

                if (cfg.textureMode != TextureArrayConfig.TextureMode.Basic)
                {
                    cfg.emisMetalArray = EditorGUILayout.Toggle(CEmisMetalArray, cfg.emisMetalArray);
                }

#if __MICROSPLAT_TEXTURECLUSTERS__

                cfg.clusterMode = (TextureArrayConfig.ClusterMode)EditorGUILayout.EnumPopup(CClusterMode, cfg.clusterMode);

#endif
            }

            var root = serializedObject.FindProperty("defaultTextureSettings");

            EditorGUILayout.PropertyField(root, false);
            if (root.isExpanded)
            {
                EditorGUI.indentLevel++;
                if (!cfg.IsDecalSplat())
                {
                    EditorGUILayout.PropertyField(root.FindPropertyRelative("diffuseSettings"), true);
                    EditorGUILayout.PropertyField(root.FindPropertyRelative("normalSettings"), true);
                }
                else
                {
                    EditorGUILayout.PropertyField(root.FindPropertyRelative("decalSplatSettings"), true);
                }
                if (cfg.textureMode != TextureArrayConfig.TextureMode.Basic)
                {
                    if (cfg.packingMode == TextureArrayConfig.PackingMode.Quality)
                    {
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("smoothSettings"), true);
                    }
                    if (cfg.antiTileArray)
                    {
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("antiTileSettings"), true);
                    }
                    if (cfg.traxArray)
                    {
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("traxDiffuseSettings"), true);
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("traxNormalSettings"), true);
                    }
                    if (cfg.emisMetalArray)
                    {
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("emissiveSettings"), true);
                    }
                    if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
                    {
                        EditorGUILayout.PropertyField(root.FindPropertyRelative("specularSettings"), true);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select PBR mode to provide custom height, smoothness, and ao textures to greatly increase quality!", MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }

            DrawOverrideGUI(serializedObject, cfg);


            if (MicroSplatUtilities.DrawRollup("Textures", true))
            {
                EditorGUILayout.HelpBox("Don't have a normal map? Any missing textures will be generated automatically from the best available source texture. Note they do not show up in the UI or write to files on disk, rather they are put directly into the array data.", MessageType.Info);
                bool disableClusters = cfg.IsScatter() || cfg.IsDecal();
                DrawHeader(cfg);
                for (int i = 0; i < cfg.sourceTextures.Count; ++i)
                {
                    using (new GUILayout.VerticalScope(GUI.skin.box))
                    {
                        bool remove = (DrawTextureEntry(cfg, cfg.sourceTextures[i], i));


                        if (cfg.clusterMode != TextureArrayConfig.ClusterMode.None && !disableClusters)
                        {
                            DrawTextureEntry(cfg, cfg.sourceTextures2[i], i, false);
                        }
                        if (cfg.clusterMode == TextureArrayConfig.ClusterMode.ThreeVariations && !disableClusters)
                        {
                            DrawTextureEntry(cfg, cfg.sourceTextures3[i], i, false);
                        }


                        if (remove)
                        {
                            var e = cfg.sourceTextures[i];
                            if (!e.HasTextures(cfg.pbrWorkflow))
                            {
                                Remove(cfg, i);
                                i--;
                            }
                            else
                            {
                                Reset(cfg, i);
                            }
                        }

                        if (cfg.antiTileArray)
                        {
                            DrawAntiTileEntry(cfg, cfg.sourceTextures[i], i);
                        }

                        if (cfg.traxArray)
                        {
                            DrawTraxEntry(cfg, cfg.sourceTextures[i], i);
                        }

                        GUILayout.Box(Texture2D.blackTexture, GUILayout.Height(3), GUILayout.ExpandWidth(true));
                    }
                }
                if (GUILayout.Button("Add Textures"))
                {
                    if (cfg.sourceTextures.Count > 0)
                    {
                        var entry = new TextureArrayConfig.TextureEntry();
                        entry.aoChannel = cfg.sourceTextures[0].aoChannel;
                        entry.heightChannel = cfg.sourceTextures[0].heightChannel;
                        entry.smoothnessChannel = cfg.sourceTextures[0].smoothnessChannel;
                        cfg.sourceTextures.Add(entry);
                    }
                    else
                    {
                        var entry = new TextureArrayConfig.TextureEntry();
                        entry.aoChannel = TextureArrayConfig.TextureChannel.G;
                        entry.heightChannel = TextureArrayConfig.TextureChannel.G;
                        entry.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
                        cfg.sourceTextures.Add(entry);
                    }
                }
            }
            if (GUILayout.Button("Update"))
            {
                TextureArrayConfigEditor.staticConfig = cfg;
                EditorApplication.delayCall += TextureArrayConfigEditor.DelayedCompileConfig;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(cfg);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }


    [CustomEditor(typeof(TextureArrayConfig))]
    public class TextureArrayConfigEditor : Editor
    {
        public TextureArrayConfigUI ui = new TextureArrayConfigUI();
        public override void OnInspectorGUI()
        {
            ui.OnInspectorGUI(target as TextureArrayConfig, serializedObject);
        }


        public static TextureArrayConfig staticConfig;
        public static void DelayedCompileConfig()
        {
            CompileConfig(staticConfig);
        }

        static string GetDiffPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_diff" + ext + "_tarray.asset");
        }

        static string GetNormPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            if (cfg.packingMode == TextureArrayConfig.PackingMode.Fastest)
            {
                return path.Replace(".asset", "_normSAO" + ext + "_tarray.asset");
            }
            else
            {
                return path.Replace(".asset", "_normal" + ext + "_tarray.asset");
            }
        }

        static string GetSmoothAOPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_smoothAO" + ext + "_tarray.asset");
        }

        static string GetSplatPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_decalsplat" + ext + "_tarray.asset");
        }

        static string GetSpecularPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_specular" + ext + "_tarray.asset");
        }


        static string GetAntiTilePath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_antiTile" + ext + "_tarray.asset");
        }

        static string GetTraxDiffusePath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_traxDiff" + ext + "_tarray.asset");
        }

        static string GetTraxNormalPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_traxNormSAO" + ext + "_tarray.asset");
        }

        static string GetEmisPath(TextureArrayConfig cfg, string ext)
        {
            string path = AssetDatabase.GetAssetPath(cfg);
            // create array path
            path = path.Replace("\\", "/");
            return path.Replace(".asset", "_emis" + ext + "_tarray.asset");
        }

        static void ShrinkSourceTexture(Texture2D tex, TextureArrayConfig.SourceTextureSize stz)
        {
            if (tex != null)
            {
                AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
                TextureImporter ti = (TextureImporter)ai;
                if (ti != null && ti.maxTextureSize != (int)stz)
                {
                    ti.maxTextureSize = (int)stz;
                    ti.SaveAndReimport();
                }
            }
        }

        static void ShrinkSourceTextures(List<TextureArrayConfig.TextureEntry> textures, TextureArrayConfig.SourceTextureSize stz)
        {
            if (textures == null)
                return;
            if (stz == TextureArrayConfig.SourceTextureSize.Unchanged)
                return;

            foreach (var t in textures)
            {
                ShrinkSourceTexture(t.ao, stz);
                ShrinkSourceTexture(t.diffuse, stz);
                ShrinkSourceTexture(t.distanceNoise, stz);
                ShrinkSourceTexture(t.normal, stz);
                ShrinkSourceTexture(t.noiseNormal, stz);
                ShrinkSourceTexture(t.detailNoise, stz);
                ShrinkSourceTexture(t.emis, stz);
                ShrinkSourceTexture(t.height, stz);
                ShrinkSourceTexture(t.metal, stz);
                ShrinkSourceTexture(t.smoothness, stz);
                ShrinkSourceTexture(t.specular, stz);
                ShrinkSourceTexture(t.traxAO, stz);
                ShrinkSourceTexture(t.traxDiffuse, stz);
                ShrinkSourceTexture(t.traxHeight, stz);
                ShrinkSourceTexture(t.traxNormal, stz);
                ShrinkSourceTexture(t.traxSmoothness, stz);
            }
        }


        static void RestoreSourceTexture(Texture2D tex)
        {
            if (tex != null)
            {
                AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex));
                TextureImporter ti = ai as TextureImporter;
                if (ti != null && ti.maxTextureSize <= 256)
                {
                    ti.maxTextureSize = 4096;
                    ti.textureCompression = TextureImporterCompression.Uncompressed;
                    ti.SaveAndReimport();
                }

            }
        }

        static void RestoreSourceTextures(List<TextureArrayConfig.TextureEntry> textures, TextureArrayConfig cfg, TextureArrayConfig.SourceTextureSize stz)
        {
            if (textures == null)
                return;

            foreach (var t in textures)
            {
                RestoreSourceTexture(t.ao);
                RestoreSourceTexture(t.diffuse);
                RestoreSourceTexture(t.distanceNoise);
                RestoreSourceTexture(t.normal);
                RestoreSourceTexture(t.noiseNormal);
                RestoreSourceTexture(t.detailNoise);
                RestoreSourceTexture(t.emis);
                RestoreSourceTexture(t.height);
                RestoreSourceTexture(t.metal);
                RestoreSourceTexture(t.smoothness);
                RestoreSourceTexture(t.specular);
                RestoreSourceTexture(t.traxAO);
                RestoreSourceTexture(t.traxDiffuse);
                RestoreSourceTexture(t.traxHeight);
                RestoreSourceTexture(t.traxNormal);
                RestoreSourceTexture(t.traxSmoothness);
            }
        }

        public static TextureArrayConfig.TextureArrayGroup GetSettingsGroup(TextureArrayConfig cfg, BuildTarget t)
        {
            foreach (var g in cfg.platformOverrides)
            {
                if (g.platform == t)
                {
                    return g.settings;
                }
            }
            return cfg.defaultTextureSettings;
        }

        static void CreateArrayAsset(Texture2DArray array, string path)
        {
            array.Apply(false, true);

            // On 2020.3LTS, the terrain turns black if you use the existing array,
            // but on previous versions, the reference gets broken on every material
            // if you don't use the existing array. I suspect this is some change to how
            // the asset database works, and suspect this isn't the end of the trouble here.
            //#if !UNITY_2020_3_OR_NEWER
            var existing = AssetDatabase.LoadAssetAtPath<Texture2DArray>(path);
            if (existing != null)
            {
                array.name = existing.name;

                EditorUtility.CopySerialized(array, existing);
            }
            else
            //#endif
            {
                AssetDatabase.CreateAsset(array, path);
            }
        }


        public static void MatchArrayLength(TextureArrayConfig cfg)
        {
            int srcCount = cfg.sourceTextures.Count;
            bool change = false;
            while (cfg.sourceTextures2.Count < srcCount)
            {
                var entry = new TextureArrayConfig.TextureEntry();
                entry.aoChannel = cfg.sourceTextures[0].aoChannel;
                entry.heightChannel = cfg.sourceTextures[0].heightChannel;
                entry.smoothnessChannel = cfg.sourceTextures[0].smoothnessChannel;
                cfg.sourceTextures2.Add(entry);
                change = true;
            }

            while (cfg.sourceTextures3.Count < srcCount)
            {
                var entry = new TextureArrayConfig.TextureEntry();
                entry.aoChannel = cfg.sourceTextures[0].aoChannel;
                entry.heightChannel = cfg.sourceTextures[0].heightChannel;
                entry.smoothnessChannel = cfg.sourceTextures[0].smoothnessChannel;
                cfg.sourceTextures3.Add(entry);
                change = true;
            }

            while (cfg.sourceTextures2.Count > srcCount)
            {
                cfg.sourceTextures2.RemoveAt(cfg.sourceTextures2.Count - 1);
                change = true;
            }
            while (cfg.sourceTextures3.Count > srcCount)
            {
                cfg.sourceTextures3.RemoveAt(cfg.sourceTextures3.Count - 1);
                change = true;
            }
            if (change)
            {
                EditorUtility.SetDirty(cfg);
            }
        }
        static void CopyTex(Texture2D tex, Texture2DArray array, int i)
        {
            if (tex != null && array != null)
            {
                tex.Apply();
                for (int mip = 0; mip < tex.mipmapCount; ++mip)
                {
                    Graphics.CopyTexture(tex, 0, mip, array, i, mip);
                }
                DestroyImmediate(tex);
            }
        }

        static Texture2D ToTexture2DAndRelease(RenderTexture rt)
        {
            if (rt == null)
                return null;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true, !rt.sRGB);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            rt.Release();
            return tex;
        }

        static void CompileConfig(TextureArrayConfig cfg,
           List<TextureArrayConfig.TextureEntry> src,
           string ext,
           bool isCluster = false)
        {
            if (cfg.sourceTextures.Count == 0)
            {
                var entry = new TextureArrayConfig.TextureEntry();
                entry.aoChannel = TextureArrayConfig.TextureChannel.G;
                entry.heightChannel = TextureArrayConfig.TextureChannel.G;
                entry.smoothnessChannel = TextureArrayConfig.TextureChannel.G;
                cfg.sourceTextures.Add(entry);
            }
#if UNITY_2022_2_OR_NEWER
            var oldTextureQuality = QualitySettings.globalTextureMipmapLimit;
            QualitySettings.globalTextureMipmapLimit = 0;
#else
            var oldTextureQuality = QualitySettings.masterTextureLimit;
            QualitySettings.masterTextureLimit = 0;
#endif
            RestoreSourceTextures(src, cfg, cfg.sourceTextureSize);

            bool diffuseIsLinear = cfg.diffuseIsLinear;

            var settings = GetSettingsGroup(cfg, UnityEditor.EditorUserBuildSettings.activeBuildTarget);


            int diffuseWidth = (int)settings.diffuseSettings.textureSize;
            int diffuseHeight = (int)settings.diffuseSettings.textureSize;
            int normalWidth = (int)settings.normalSettings.textureSize;
            int normalHeight = (int)settings.normalSettings.textureSize;
            int smoothWidth = (int)settings.smoothSettings.textureSize;
            int smoothHeight = (int)settings.smoothSettings.textureSize;
            int antiTileWidth = (int)settings.antiTileSettings.textureSize;
            int antiTileHeight = (int)settings.antiTileSettings.textureSize;
            int emisWidth = (int)settings.emissiveSettings.textureSize;
            int emisHeight = (int)settings.emissiveSettings.textureSize;
            int specularWidth = (int)settings.specularSettings.textureSize;
            int specularHeight = (int)settings.specularSettings.textureSize;
            int traxDiffuseWidth = (int)settings.traxDiffuseSettings.textureSize;
            int traxDiffuseHeight = (int)settings.traxDiffuseSettings.textureSize;
            int traxNormalWidth = (int)settings.traxNormalSettings.textureSize;
            int traxNormalHeight = (int)settings.traxNormalSettings.textureSize;
            int splatHeight = (int)settings.specularSettings.textureSize;
            int splatWidth = (int)settings.specularSettings.textureSize;

            int diffuseAnisoLevel = settings.diffuseSettings.Aniso;
            int normalAnisoLevel = settings.normalSettings.Aniso;
            int antiTileAnisoLevel = settings.antiTileSettings.Aniso;
            int emisAnisoLevel = settings.emissiveSettings.Aniso;
            int smoothAnisoLevel = settings.smoothSettings.Aniso;
            int specularAnisoLevel = settings.specularSettings.Aniso;
            int splatAnisoLevel = settings.decalSplatSettings.Aniso;

            int traxDiffuseAnisoLevel = settings.traxDiffuseSettings.Aniso;
            int traxNormalAnisoLevel = settings.traxNormalSettings.Aniso;

            FilterMode diffuseFilter = settings.diffuseSettings.filterMode;
            FilterMode normalFilter = settings.normalSettings.filterMode;
            FilterMode antiTileFilter = settings.antiTileSettings.filterMode;
            FilterMode emisFilter = settings.emissiveSettings.filterMode;
            FilterMode smoothFilter = settings.smoothSettings.filterMode;
            FilterMode specularFilter = settings.specularSettings.filterMode;

            FilterMode traxDiffuseFilter = settings.traxDiffuseSettings.filterMode;
            FilterMode traxNormalFilter = settings.traxNormalSettings.filterMode;
            FilterMode splatFilter = settings.decalSplatSettings.filterMode;


            int texCount = src.Count;
            if (texCount < 1)
                texCount = 1;

            // diffuse
            Texture2DArray diffuseArray = new Texture2DArray(diffuseWidth, diffuseHeight, texCount,
             GetTextureFormat(cfg, settings.diffuseSettings.compression, settings.diffuseSettings.compressionQuality),
               true, diffuseIsLinear);

            diffuseArray.wrapMode = TextureWrapMode.Repeat;
            diffuseArray.filterMode = diffuseFilter;
            diffuseArray.anisoLevel = diffuseAnisoLevel;

            // normal
            var nmlcomp = GetTextureFormat(cfg, settings.normalSettings.compression, settings.normalSettings.compressionQuality);
            Texture2DArray normalSAOArray = new Texture2DArray(normalWidth, normalHeight, texCount, nmlcomp, true, true);

            normalSAOArray.wrapMode = TextureWrapMode.Repeat;
            normalSAOArray.filterMode = normalFilter;
            normalSAOArray.anisoLevel = normalAnisoLevel;

            Texture2DArray splatArray = null;
            if (cfg.IsDecalSplat())
            {
                var spalcmp = GetTextureFormat(cfg, settings.decalSplatSettings.compression, settings.decalSplatSettings.compressionQuality);
                splatArray = new Texture2DArray(splatWidth, splatHeight, texCount, spalcmp, true, true);

                splatArray.wrapMode = TextureWrapMode.Clamp;
                splatArray.filterMode = splatFilter;
                splatArray.anisoLevel = splatAnisoLevel;
            }



            // trax
            Texture2DArray traxDiffuseArray = null;
            Texture2DArray traxNormalSAOArray = null;
            if (cfg.traxArray && !isCluster)
            {
                traxDiffuseArray = new Texture2DArray(traxDiffuseWidth, traxDiffuseHeight, texCount,
                 GetTextureFormat(cfg, settings.traxDiffuseSettings.compression, settings.traxDiffuseSettings.compressionQuality),
                   true, diffuseIsLinear);

                traxDiffuseArray.wrapMode = TextureWrapMode.Repeat;
                traxDiffuseArray.filterMode = traxDiffuseFilter;
                traxDiffuseArray.anisoLevel = traxDiffuseAnisoLevel;

                var traxNF = GetTextureFormat(cfg, settings.traxNormalSettings.compression, settings.traxNormalSettings.compressionQuality);
                traxNormalSAOArray = new Texture2DArray(traxNormalWidth, traxNormalHeight, texCount, traxNF, true, true);

                traxNormalSAOArray.wrapMode = TextureWrapMode.Repeat;
                traxNormalSAOArray.filterMode = traxNormalFilter;
                traxNormalSAOArray.anisoLevel = traxNormalAnisoLevel;
            }


            // smoothAOArray
            Texture2DArray smoothAOArray = null;
            if (cfg.packingMode == TextureArrayConfig.PackingMode.Quality)
            {
                smoothAOArray = new Texture2DArray((int)settings.smoothSettings.textureSize, (int)settings.smoothSettings.textureSize, texCount,
                   GetTextureFormat(cfg, settings.smoothSettings.compression, settings.smoothSettings.compressionQuality),
                   true, true);

                smoothAOArray.filterMode = smoothFilter;
                smoothAOArray.anisoLevel = smoothAnisoLevel;
            }

            Texture2DArray specularArray = null;
            if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
            {
                specularArray = new Texture2DArray((int)settings.specularSettings.textureSize, (int)settings.specularSettings.textureSize, texCount,
                   GetTextureFormat(cfg, settings.specularSettings.compression, settings.specularSettings.compressionQuality),
                   true, false);

                specularArray.filterMode = specularFilter;
                specularArray.anisoLevel = specularAnisoLevel;
            }


            // antitile
            Texture2DArray antiTileArray = null;
            if (!isCluster && cfg.antiTileArray)
            {
                antiTileArray = new Texture2DArray(antiTileWidth, antiTileHeight, texCount,
                   GetTextureFormat(cfg, settings.antiTileSettings.compression, settings.antiTileSettings.compressionQuality),
                   true, true);

                antiTileArray.wrapMode = TextureWrapMode.Repeat;
                antiTileArray.filterMode = antiTileFilter;
                antiTileArray.anisoLevel = antiTileAnisoLevel;
            }

            // emis/metal
            Texture2DArray emisArray = null;
            if (cfg.emisMetalArray && !cfg.IsScatter())
            {
                emisArray = new Texture2DArray(emisWidth, emisHeight, texCount,
                   GetTextureFormat(cfg, settings.emissiveSettings.compression, settings.emissiveSettings.compressionQuality),
                   true, diffuseIsLinear);

                emisArray.wrapMode = TextureWrapMode.Repeat;
                emisArray.filterMode = emisFilter;
                emisArray.anisoLevel = emisAnisoLevel;
            }

            for (int i = 0; i < src.Count; ++i)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("Packing textures", "", (float)i / (float)src.Count);

                    // first, generate any missing data. We generate a full NSAO map from diffuse or height map
                    // if no height map is provided, we then generate it from the resulting or supplied normal.
                    var e = src[i];
                    // resulting maps
                    RenderTexture diffuseHeightRT = ResizeTexture(e.diffuse == null ? Texture2D.whiteTexture : e.diffuse, diffuseWidth, diffuseHeight, diffuseIsLinear);

                    RenderTexture normalSAORT = null;
                    RenderTexture smoothAORT = null;
                    RenderTexture antiTileRT = null;
                    RenderTexture emisRT = null;
                    RenderTexture specularRT = null;
                    RenderTexture splatRT = null;

                    RenderTexture traxDiffuseHeightRT = null;
                    if (cfg.traxArray && !isCluster)
                    {
                        traxDiffuseHeightRT = ResizeTexture(e.diffuse == null ? Texture2D.whiteTexture : e.traxDiffuse, traxDiffuseWidth, traxDiffuseHeight, diffuseIsLinear);
                    }
                    RenderTexture traxNormalSAORT = null;

                    int traxHeightChannel = (int)e.traxHeightChannel;
                    int traxAOChannel = (int)e.traxAOChannel;
                    int traxSmoothChannel = (int)e.traxSmoothnessChannel;

                    int heightChannel = (int)e.heightChannel;
                    int aoChannel = (int)e.aoChannel;
                    int smoothChannel = (int)e.smoothnessChannel;
                    int detailChannel = (int)e.detailChannel;
                    int distanceChannel = (int)e.distanceChannel;
                    int metalChannel = (int)e.metalChannel;

                    if (cfg.allTextureChannelHeight != TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        heightChannel = (int)cfg.allTextureChannelHeight;
                        traxHeightChannel = (int)cfg.allTextureChannelHeight;
                    }
                    if (cfg.allTextureChannelAO != TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        aoChannel = (int)cfg.allTextureChannelAO;
                        traxAOChannel = (int)cfg.allTextureChannelAO;
                    }
                    if (cfg.allTextureChannelSmoothness != TextureArrayConfig.AllTextureChannel.Custom)
                    {
                        smoothChannel = (int)cfg.allTextureChannelSmoothness;
                        traxSmoothChannel = (int)cfg.allTextureChannelSmoothness;
                    }

                    if (e.normal == null)
                    {
                        if (e.height == null)
                        {
                            normalSAORT = RenderMissingTexture(diffuseHeightRT, "Hidden/MicroSplat/NormalSAOFromDiffuse", normalWidth, normalHeight);
                        }
                        else
                        {
                            normalSAORT = RenderMissingTexture(e.height, "Hidden/MicroSplat/NormalSAOFromHeight", normalWidth, normalHeight, heightChannel);
                        }
                    }
                    else
                    {
                        // copy, but go ahead and generate other channels in case they aren't provided later.
                        normalSAORT = RenderMissingTexture(e.normal, "Hidden/MicroSplat/NormalSAOFromNormal", normalWidth, normalHeight);
                    }
                    if (cfg.traxArray && !isCluster)
                    {
                        if (e.traxNormal == null)
                        {
                            if (e.traxHeight == null)
                            {
                                traxNormalSAORT = RenderMissingTexture(e.traxDiffuse, "Hidden/MicroSplat/NormalSAOFromDiffuse", traxNormalWidth, traxNormalHeight);
                            }
                            else
                            {
                                traxNormalSAORT = RenderMissingTexture(e.traxHeight, "Hidden/MicroSplat/NormalSAOFromHeight", traxNormalWidth, traxNormalHeight, traxHeightChannel);
                            }
                        }
                        else
                        {
                            // copy, but go ahead and generate other channels in case they aren't provided later.
                            traxNormalSAORT = RenderMissingTexture(e.traxNormal, "Hidden/MicroSplat/NormalSAOFromNormal", traxNormalWidth, traxNormalHeight);
                        }
                    }

                    if (!isCluster && cfg.antiTileArray)
                    {
                        antiTileRT = RenderMissingTexture(e.noiseNormal, "Hidden/MicroSplat/NormalSAOFromNormal", antiTileWidth, antiTileHeight);
                    }
                    if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
                    {
                        specularRT = ResizeTexture(e.specular != null ? e.specular : Texture2D.blackTexture, specularWidth, specularHeight, false);
                    }
                    if (cfg.IsDecalSplat())
                    {
                        splatRT = ResizeTexture(e.splat != null ? e.splat : Texture2D.blackTexture, splatWidth, splatHeight, true);
                    }


                    RenderTexture height = null;
                    if (e.height != null)
                    {
                        height = ResizeTexture(e.height, diffuseWidth, diffuseHeight, true);
                    }
                    else
                    {
                        height = RenderMissingTexture(normalSAORT, "Hidden/MicroSplat/HeightFromNormal", diffuseWidth, diffuseHeight);
                    }


                    RenderTexture traxHeight = null;
                    if (e.traxHeight)
                    {
                        traxHeight = ResizeTexture(e.traxHeight, traxDiffuseWidth, traxDiffuseHeight, true);
                    }
                    if (cfg.traxArray && !isCluster)
                    {
                        if (traxHeight == null)
                        {
                            traxHeight = RenderMissingTexture(normalSAORT, "Hidden/MicroSplat/HeightFromNormal", traxDiffuseWidth, traxDiffuseHeight);
                        }

                        MergeInChannel(traxDiffuseHeightRT, (int)TextureArrayConfig.TextureChannel.A, traxHeight, traxHeightChannel, diffuseIsLinear);
                    }


                    MergeInChannel(diffuseHeightRT, (int)TextureArrayConfig.TextureChannel.A, height, heightChannel, diffuseIsLinear);

                    if (cfg.emisMetalArray && !cfg.IsScatter())
                    {
                        emisRT = ResizeTexture(e.emis != null ? e.emis : Texture2D.blackTexture, emisWidth, emisHeight, diffuseIsLinear);
                        RenderTexture metal = ResizeTexture(e.metal != null ? e.metal : Texture2D.blackTexture, emisWidth, emisHeight, true);
                        MergeInChannel(emisRT, 3, metal, e.metal != null ? metalChannel : 0, true, false);
                        metal.Release();
                    }

                    if (e.ao != null)
                    {
                        MergeInChannel(normalSAORT, (int)TextureArrayConfig.TextureChannel.B, e.ao, aoChannel, true);
                    }

                    if (e.smoothness != null)
                    {
                        MergeInChannel(normalSAORT, (int)TextureArrayConfig.TextureChannel.R, e.smoothness, smoothChannel, true, e.isRoughness);
                    }

                    if (cfg.traxArray && !isCluster)
                    {
                        if (e.traxAO != null)
                        {
                            MergeInChannel(traxNormalSAORT, (int)TextureArrayConfig.TextureChannel.B, e.traxAO, traxAOChannel, true);
                        }

                        if (e.traxSmoothness != null)
                        {
                            MergeInChannel(traxNormalSAORT, (int)TextureArrayConfig.TextureChannel.R, e.traxSmoothness, traxSmoothChannel, true, e.traxIsRoughness);
                        }
                    }

                    if (cfg.packingMode == TextureArrayConfig.PackingMode.Quality)
                    {
                        {
                            Material clearNonNormalMat = new Material(Shader.Find("Hidden/MicroSplat/ClearNonNormalData"));
                            RenderTexture tmp = new RenderTexture(normalSAORT.descriptor);

                            Graphics.Blit(normalSAORT, tmp);
                            var tmp2 = normalSAORT;
                            normalSAORT = tmp;
                            RenderTexture.active = null;
                            tmp2.Release();
                        }
                        // generate missing maps for smoothness
                        if (e.normal == null)
                        {
                            if (e.height == null)
                            {
                                smoothAORT = RenderMissingTexture(diffuseHeightRT, "Hidden/MicroSplat/NormalSAOFromDiffuse", smoothWidth, smoothHeight);
                            }
                            else
                            {
                                smoothAORT = RenderMissingTexture(e.height, "Hidden/MicroSplat/NormalSAOFromHeight", smoothWidth, smoothHeight, heightChannel);
                            }
                        }
                        else
                        {
                            // copy, but go ahead and generate other channels in case they aren't provided later.
                            smoothAORT = RenderMissingTexture(e.normal, "Hidden/MicroSplat/NormalSAOFromNormal", smoothWidth, smoothHeight);
                        }



                        // now clear normal data, and swizzle channels into G/A
                        {
                            Material clearNonNormalMat = new Material(Shader.Find("Hidden/MicroSplat/ClearNonNormalData"));
                            clearNonNormalMat.SetFloat("_Swizzle", 1);
                            RenderTexture tmp = new RenderTexture(smoothAORT.descriptor);

                            Graphics.Blit(smoothAORT, tmp);
                            var tmp2 = smoothAORT;
                            smoothAORT = tmp;
                            RenderTexture.active = null;
                            tmp2.Release();
                        }

                        // merge in data if provided
                        if (e.ao != null)
                        {
                            MergeInChannel(smoothAORT, (int)TextureArrayConfig.TextureChannel.A, e.ao, aoChannel, true);
                        }

                        if (e.smoothness != null)
                        {
                            MergeInChannel(smoothAORT, (int)TextureArrayConfig.TextureChannel.G, e.smoothness, smoothChannel, true, e.isRoughness);
                        }

                    }


                    if (!isCluster && cfg.antiTileArray && antiTileRT != null)
                    {
                        Texture2D detail = e.detailNoise;
                        Texture2D distance = e.distanceNoise;
                        bool destroyDetail = false;
                        bool destroyDistance = false;
                        if (detail == null)
                        {
                            detail = new Texture2D(1, 1, TextureFormat.RGB24, true, true);
                            detail.SetPixel(0, 0, Color.grey);
                            detail.Apply();
                            destroyDetail = true;
                            detailChannel = (int)TextureArrayConfig.TextureChannel.G;
                        }
                        if (distance == null)
                        {
                            distance = new Texture2D(1, 1, TextureFormat.RGB24, true, true);
                            distance.SetPixel(0, 0, Color.grey);
                            distance.Apply();
                            destroyDistance = true;
                            distanceChannel = (int)TextureArrayConfig.TextureChannel.G;
                        }
                        MergeInChannel(antiTileRT, (int)TextureArrayConfig.TextureChannel.R, detail, detailChannel, true, false);
                        MergeInChannel(antiTileRT, (int)TextureArrayConfig.TextureChannel.B, distance, distanceChannel, true, false);

                        if (destroyDetail)
                        {
                            GameObject.DestroyImmediate(detail);
                        }
                        if (destroyDistance)
                        {
                            GameObject.DestroyImmediate(distance);
                        }
                    }

                    
                    int tq = (int)UnityEditor.TextureCompressionQuality.Normal;
                    Texture2D normalSAOTex = ToTexture2DAndRelease(normalSAORT);
                    Texture2D smoothAOTex = ToTexture2DAndRelease(smoothAORT);
                    Texture2D antiTileTex = ToTexture2DAndRelease(antiTileRT);
                    Texture2D emisTex = ToTexture2DAndRelease(emisRT);
                    Texture2D diffuseHeightTex = ToTexture2DAndRelease(diffuseHeightRT);
                    Texture2D traxDiffuseHeightTex = ToTexture2DAndRelease(traxDiffuseHeightRT);
                    Texture2D traxNormalSAOTex = ToTexture2DAndRelease(traxNormalSAORT);
                    Texture2D splatTex = ToTexture2DAndRelease(splatRT);
                    Texture2D specularTex = ToTexture2DAndRelease(specularRT);

                    

                    if (diffuseHeightTex != null && settings.diffuseSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(diffuseHeightTex, GetTextureFormat(cfg, settings.diffuseSettings.compression, settings.diffuseSettings.compressionQuality), tq);
                    }

                    if (normalSAOTex != null && settings.normalSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(normalSAOTex, GetTextureFormat(cfg, settings.normalSettings.compression, settings.normalSettings.compressionQuality), tq);
                    }

                    if (splatRT != null && settings.decalSplatSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(splatTex, GetTextureFormat(cfg, settings.decalSplatSettings.compression, settings.decalSplatSettings.compressionQuality), tq);
                    }

                    if (cfg.traxArray && !isCluster)
                    {
                        if (settings.traxDiffuseSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                        {
                            EditorUtility.CompressTexture(traxDiffuseHeightTex, GetTextureFormat(cfg, settings.traxDiffuseSettings.compression, settings.traxDiffuseSettings.compressionQuality), tq);
                        }

                        if (settings.traxNormalSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                        {
                            EditorUtility.CompressTexture(traxNormalSAOTex, GetTextureFormat(cfg, settings.traxNormalSettings.compression, settings.traxNormalSettings.compressionQuality), tq);
                        }
                    }

                    if (smoothAORT != null && cfg.packingMode != TextureArrayConfig.PackingMode.Fastest)
                    {
                        EditorUtility.CompressTexture(smoothAOTex, GetTextureFormat(cfg, settings.smoothSettings.compression, settings.smoothSettings.compressionQuality), tq);
                    }

                    if (antiTileRT != null && settings.antiTileSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(antiTileTex, GetTextureFormat(cfg, settings.antiTileSettings.compression, settings.antiTileSettings.compressionQuality), tq);
                    }

                    if (cfg.emisMetalArray && !cfg.IsScatter() && emisRT != null && settings.emissiveSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(emisTex, GetTextureFormat(cfg, settings.emissiveSettings.compression, settings.emissiveSettings.compressionQuality), tq);
                    }

                    if (specularTex != null && settings.specularSettings.compression != TextureArrayConfig.Compression.Uncompressed)
                    {
                        EditorUtility.CompressTexture(specularTex, GetTextureFormat(cfg, settings.specularSettings.compression, settings.specularSettings.compressionQuality), tq);
                    }

                    CopyTex(diffuseHeightTex, diffuseArray, i);
                    CopyTex(normalSAOTex, normalSAOArray, i);
                    CopyTex(traxDiffuseHeightTex, traxDiffuseArray, i);
                    CopyTex(traxNormalSAOTex, traxNormalSAOArray, i);
                    CopyTex(splatTex, splatArray, i);
                    CopyTex(smoothAOTex, smoothAOArray, i);
                    CopyTex(antiTileTex, antiTileArray, i);
                    CopyTex(emisTex, emisArray, i);
                    CopyTex(specularTex, specularArray, i);

                    RenderTexture.active = null;

                    if (normalSAOTex != null) GameObject.DestroyImmediate(normalSAOTex);
                    if (smoothAOTex != null) GameObject.DestroyImmediate(smoothAOTex);
                    if (antiTileTex != null) GameObject.DestroyImmediate(antiTileTex);
                    if (emisTex != null) GameObject.DestroyImmediate(emisTex);
                    if (diffuseHeightTex != null) GameObject.DestroyImmediate(diffuseHeightTex);
                    if (traxDiffuseHeightTex != null) GameObject.DestroyImmediate(traxDiffuseHeightTex);
                    if (traxNormalSAOTex != null) GameObject.DestroyImmediate(traxNormalSAOTex);
                    if (splatTex != null) GameObject.DestroyImmediate(splatTex);
                    if (specularTex != null) GameObject.DestroyImmediate(specularTex);

                    //Resources.UnloadUnusedAssets();
                    //System.GC.Collect();
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }

            }
            EditorUtility.ClearProgressBar();

            if (cfg.IsStarReach())
            {
                ext += "_starreach_";
            }

            if (!cfg.IsDecalSplat())
            {
                CreateArrayAsset(diffuseArray, GetDiffPath(cfg, ext));
            }

            if (!cfg.IsDecalSplat())
            {
                CreateArrayAsset(normalSAOArray, GetNormPath(cfg, ext));
            }

            if (cfg.traxArray && !isCluster)
            {
                CreateArrayAsset(traxDiffuseArray, GetTraxDiffusePath(cfg, ext));
            }

            if (cfg.traxArray && !isCluster)
            {
                CreateArrayAsset(traxNormalSAOArray, GetTraxNormalPath(cfg, ext));
            }

            if (cfg.packingMode != TextureArrayConfig.PackingMode.Fastest && smoothAOArray != null)
            {
                CreateArrayAsset(smoothAOArray, GetSmoothAOPath(cfg, ext));
            }

            if (cfg.antiTileArray && antiTileArray != null)
            {
                CreateArrayAsset(antiTileArray, GetAntiTilePath(cfg, ext));
            }

            if (cfg.emisMetalArray && emisArray != null)
            {
                CreateArrayAsset(emisArray, GetEmisPath(cfg, ext));
            }

            if (cfg.IsDecalSplat() && splatArray != null)
            {
                CreateArrayAsset(splatArray, GetSplatPath(cfg, ext));
            }

            if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular && specularArray != null)
            {
                CreateArrayAsset(specularArray, GetSpecularPath(cfg, ext));
            }

            EditorUtility.SetDirty(cfg);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            MicroSplatUtilities.ClearPreviewCache();
            MicroSplatObject.SyncAll();
#if UNITY_2022_2_OR_NEWER
         QualitySettings.globalTextureMipmapLimit = oldTextureQuality;
#else
            QualitySettings.masterTextureLimit = oldTextureQuality;
#endif

            if (cfg.sourceTextureSize != TextureArrayConfig.SourceTextureSize.Unchanged)
            {
                ShrinkSourceTextures(src, cfg.sourceTextureSize);
            }

        }

        static RenderTexture ResizeTexture(Texture2D source, int width, int height, bool linear)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Resize Texture");
            // uncompress for better quality- note, slow on asset database v1, but fast on v2
            AssetImporter ai = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(source));
            TextureImporter ti = ai as TextureImporter;
            TextureImporterCompression comp = TextureImporterCompression.Uncompressed;
            if (ti != null)
            {
                if (ti.textureCompression != TextureImporterCompression.Uncompressed)
                {
                    comp = ti.textureCompression;
                    ti.textureCompression = TextureImporterCompression.Uncompressed;
                    ti.SaveAndReimport();
                }
            }

            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear) && !linear;
            Graphics.Blit(source, rt);
            GL.sRGBWrite = false;
            RenderTexture.active = null; 
            // restore compression

            if (comp != TextureImporterCompression.Uncompressed)
            {
                ti.textureCompression = comp;
                ti.SaveAndReimport();
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return rt;
        }

        public static TextureFormat GetTextureFormat(TextureArrayConfig cfg, TextureArrayConfig.Compression cmp, TextureArrayConfig.CompressionQuality q)
        {
            if (cmp == TextureArrayConfig.Compression.ForceETC2)
            {
                return (TextureFormat.ETC2_RGBA8);
            }
            else if (cmp == TextureArrayConfig.Compression.ForcePVR)
            {
                if (q == TextureArrayConfig.CompressionQuality.High || q == TextureArrayConfig.CompressionQuality.Medium)
                {
                    return (TextureFormat.PVRTC_RGBA4);
                }
                else
                {
                    return TextureFormat.PVRTC_RGBA2;
                }
            }
            else if (cmp == TextureArrayConfig.Compression.ForceASTC)
            {
                if (q == TextureArrayConfig.CompressionQuality.High)
                {
                    return (TextureFormat.ASTC_4x4);
                }
                else if (q == TextureArrayConfig.CompressionQuality.Medium)
                {
                    return TextureFormat.ASTC_6x6;
                }
                else
                {
                    return TextureFormat.ASTC_8x8;
                }
            }
            else if (cmp == TextureArrayConfig.Compression.ForceDXT)
            {
                return (TextureFormat.DXT5);
            }
            else if (cmp == TextureArrayConfig.Compression.ForceBC7)
            {
                return (TextureFormat.BC7);
            }
            else if (cmp == TextureArrayConfig.Compression.ForceCrunch)
            {
                return (TextureFormat.DXT5Crunched);
            }
            else if (cmp == TextureArrayConfig.Compression.Uncompressed)
            {
                return TextureFormat.ARGB32;
            }

            var platform = EditorUserBuildSettings.activeBuildTarget;
            if (platform == BuildTarget.Android || platform == BuildTarget.iOS ||
                platform == BuildTarget.tvOS)
            {
                if (q == TextureArrayConfig.CompressionQuality.High)
                {
                    return (TextureFormat.ASTC_4x4);
                }
                else if (q == TextureArrayConfig.CompressionQuality.Medium)
                {
                    return TextureFormat.ASTC_6x6;
                }
                else
                {
                    return TextureFormat.ASTC_8x8;
                }
            }
            else
            {
                if (q == TextureArrayConfig.CompressionQuality.High)
                {
                    return TextureFormat.BC7;
                }
                return (TextureFormat.DXT5);
            }
        }

        static RenderTexture RenderMissingTexture(RenderTexture src, string shaderPath, int width, int height, int channel = -1)
        {
            // used to use get temporary, but it's not trust worthy as it can corrupt existing
            // buffers. I think there may be a max count to temporary buffers.
            RenderTexture resRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Shader s = Shader.Find(shaderPath);
            if (s == null)
            {
                Debug.LogError("Could not find shader " + shaderPath);
                return resRT;
            }
            Material genMat = new Material(s);
            if (channel >= 0)
            {
                genMat.SetInt("_Channel", channel);
            }

            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(src, resRT, genMat);
            GL.sRGBWrite = false;

            RenderTexture.active = null;
            resRT.Release();
            DestroyImmediate(resRT);
            GameObject.DestroyImmediate(genMat);
            return resRT;
        }


        static RenderTexture RenderMissingTexture(Texture2D src, string shaderPath, int width, int height, int channel = -1)
        {
            RenderTexture resRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            resRT.DiscardContents();
            Shader s = Shader.Find(shaderPath);
            if (s == null)
            {
                Debug.LogError("Could not find shader " + shaderPath);
                return resRT;
            }
            Material genMat = new Material(s);
            if (channel >= 0)
            {
                genMat.SetInt("_Channel", channel);
            }

            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(src, resRT, genMat);
            GL.sRGBWrite = false;

            RenderTexture.active = null;
            GameObject.DestroyImmediate(genMat);
            return resRT;
        }

        static Shader mergeInChannelShader = null;
        static void MergeInChannel(RenderTexture target, int targetChannel,
         Texture merge, int mergeChannel, bool linear, bool invert = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Merge");
            RenderTexture resRT = new RenderTexture(target.width, target.height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
            resRT.DiscardContents();
            if (mergeInChannelShader == null)
            {
                mergeInChannelShader = Shader.Find("Hidden/MicroSplat/MergeInChannel");
            }
            if (mergeInChannelShader == null)
            {
                Debug.LogError("Could not find shader for merge");
                DestroyImmediate(resRT);
                return;
            }
            Material genMat = new Material(mergeInChannelShader);
            genMat.SetInt("_TargetChannel", targetChannel);
            genMat.SetInt("_MergeChannel", mergeChannel);
            genMat.SetInt("_Invert", invert ? 1 : 0);
            genMat.SetTexture("_TargetTex", target);

            GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(merge, resRT, genMat);
            Graphics.Blit(resRT, target);
            GL.sRGBWrite = false;

            resRT.Release();
            GameObject.DestroyImmediate(resRT);
            GameObject.DestroyImmediate(genMat);

            UnityEngine.Profiling.Profiler.EndSample();
        }

        public static void CompileConfig(TextureArrayConfig cfg)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Compile Config");
            MatchArrayLength(cfg);

            CompileConfig(cfg, cfg.sourceTextures, "", false);
            if (cfg.clusterMode != TextureArrayConfig.ClusterMode.None)
            {
                CompileConfig(cfg, cfg.sourceTextures2, "_C2", true);
            }
            if (cfg.clusterMode == TextureArrayConfig.ClusterMode.ThreeVariations)
            {
                CompileConfig(cfg, cfg.sourceTextures3, "_C3", true);
            }


            cfg.diffuseArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(GetDiffPath(cfg, ""));
            cfg.normalSAOArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(GetNormPath(cfg, ""));
            if (cfg.packingMode != TextureArrayConfig.PackingMode.Fastest)
            {
                cfg.smoothAOArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(GetSmoothAOPath(cfg, ""));
                if (cfg.pbrWorkflow == TextureArrayConfig.PBRWorkflow.Specular)
                {
                    cfg.specularArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(GetSpecularPath(cfg, ""));
                }
            }
            if (cfg.IsDecalSplat())
            {
                cfg.splatArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>(GetSplatPath(cfg, ""));
            }
            if (cfg != null)
            {
                EditorUtility.SetDirty(cfg);
            }
            if (!TextureArrayPreProcessor.sIsPostProcessing)
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                MicroSplatObject.SyncAll();
            }

            MicroSplatUtilities.ClearPreviewCache();
            UnityEngine.Profiling.Profiler.EndSample();
        }

        static void SetDefaultTextureSize(TextureArrayConfig cfg, int size)
        {
            if (size > 2048)
            {
                cfg.defaultTextureSettings.diffuseSettings.textureSize = TextureArrayConfig.TextureSize.k4096;
                cfg.defaultTextureSettings.normalSettings.textureSize = TextureArrayConfig.TextureSize.k4096;
            }
            else if (size > 1024)
            {
                cfg.defaultTextureSettings.diffuseSettings.textureSize = TextureArrayConfig.TextureSize.k2048;
                cfg.defaultTextureSettings.normalSettings.textureSize = TextureArrayConfig.TextureSize.k2048;
            }
            else if (size > 512)
            {
                cfg.defaultTextureSettings.diffuseSettings.textureSize = TextureArrayConfig.TextureSize.k1024;
                cfg.defaultTextureSettings.normalSettings.textureSize = TextureArrayConfig.TextureSize.k1024;
            }
            else if (size > 256)
            {
                cfg.defaultTextureSettings.diffuseSettings.textureSize = TextureArrayConfig.TextureSize.k512;
                cfg.defaultTextureSettings.normalSettings.textureSize = TextureArrayConfig.TextureSize.k512;
            }
            else
            {
                cfg.defaultTextureSettings.diffuseSettings.textureSize = TextureArrayConfig.TextureSize.k256;
                cfg.defaultTextureSettings.normalSettings.textureSize = TextureArrayConfig.TextureSize.k256;
            }
        }

        public static bool GetFromTerrain(TextureArrayConfig cfg, Terrain t)
        {
            if (t != null && cfg.sourceTextures.Count == 0 && t.terrainData != null)
            {
                int maxTexSize = 1024;
                int count = t.terrainData.terrainLayers.Length;
                for (int i = 0; i < count; ++i)
                {
                    // Metalic, AO, Height, Smooth
                    var proto = t.terrainData.terrainLayers[i];
                    var e = new TextureArrayConfig.TextureEntry();
                    if (proto != null)
                    {
                        e.terrainLayer = proto;
                        e.diffuse = proto.diffuseTexture;
                        e.normal = proto.normalMapTexture;
                        e.metal = proto.maskMapTexture;
                        e.metalChannel = TextureArrayConfig.TextureChannel.R;
                        e.height = proto.maskMapTexture;
                        e.heightChannel = TextureArrayConfig.TextureChannel.B;
                        e.smoothness = proto.maskMapTexture;
                        e.smoothnessChannel = TextureArrayConfig.TextureChannel.A;
                        e.ao = proto.maskMapTexture;
                        e.aoChannel = TextureArrayConfig.TextureChannel.G;
                    }
                    if (e.smoothness != null)
                    {
                        cfg.allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;
                        cfg.allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.B;
                        cfg.allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.A;
                    }
                    cfg.sourceTextures.Add(e);
                    if (proto != null && proto.diffuseTexture != null && proto.diffuseTexture.width > maxTexSize)
                    {
                        maxTexSize = proto.diffuseTexture.width;
                    }
                }
                if (cfg.sourceTextures.Count == 0)
                {
                    cfg.sourceTextures.Add(new TextureArrayConfig.TextureEntry());
                }
                SetDefaultTextureSize(cfg, maxTexSize);
                return true;

            }
            return false;
        }


        public static TextureArrayConfig CreateConfig(string path)
        {
            string configPath = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplatConfig.asset");
            TextureArrayConfig cfg = TextureArrayConfig.CreateInstance<TextureArrayConfig>();

            AssetDatabase.CreateAsset(cfg, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(configPath);
            CompileConfig(cfg);
            return cfg;
        }

        public static TextureArrayConfig CreateConfig(Terrain t)
        {
            string path = MicroSplatUtilities.RelativePathFromAsset(t.terrainData);
            string configPath = AssetDatabase.GenerateUniqueAssetPath(path + "/MicroSplatConfig.asset");
            TextureArrayConfig cfg = TextureArrayConfig.CreateInstance<TextureArrayConfig>();
            GetFromTerrain(cfg, t);
            AssetDatabase.CreateAsset(cfg, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            cfg = AssetDatabase.LoadAssetAtPath<TextureArrayConfig>(configPath);
            CompileConfig(cfg);
            return cfg;

        }

        // Not used internally, but rather from other assets (such as MicroVerse)
        // which want to shuffle terrain layers around..
        public class LayerSort
        {
            public TerrainLayer terrainLayer;
            public Color[] propDataValues = null;
            public TextureArrayConfig.TextureEntry source = null;
            public TextureArrayConfig.TextureEntry source2 = null;
            public TextureArrayConfig.TextureEntry source3 = null;
        }

        static bool IsInConfig(TextureArrayConfig config, TerrainLayer l)
        {
            foreach (var c in config.sourceTextures)
            {
                if (c.terrainLayer == l)
                    return true;
            }
            return false;
        }



        public static void MatchAndSortTerrainLayers(TextureArrayConfig config, MicroSplatPropData propData, TerrainLayer[] terrainLayers)
        {
            // add new layers
            for (int i = 0; i < terrainLayers.Length; ++i)
            {
                if (!IsInConfig(config, terrainLayers[i]))
                {
                    config.AddTerrainLayer(terrainLayers[i]);
                }
            }
            // build sortable list
            List<LayerSort> layers = new List<LayerSort>();
            for (int i = 0; i < config.sourceTextures.Count; ++i)
            {
                if (terrainLayers.Contains(config.sourceTextures[i].terrainLayer))
                {
                    LayerSort ls = new LayerSort();
                    ls.terrainLayer = config.sourceTextures[i].terrainLayer;
                    ls.propDataValues = propData.GetAllValues(i);
                    ls.source = config.sourceTextures[i];
                    if (config.sourceTextures2 != null && i < config.sourceTextures2.Count) ls.source2 = config.sourceTextures2[i];
                    if (config.sourceTextures3 != null && i < config.sourceTextures3.Count) ls.source3 = config.sourceTextures3[i];
                    layers.Add(ls);
                }
            }

            layers.Sort((x, y) => x.terrainLayer.name.CompareTo(y.terrainLayer.name));

            config.sourceTextures.Clear();
            config.sourceTextures2?.Clear();
            config.sourceTextures3?.Clear();
            for (int i = 0; i < layers.Count; ++i)
            {
                var l = layers[i];
                propData.SetAllValues(i, l.propDataValues);
                config.sourceTextures.Add(l.source);
                if (l.source2 != null) config.sourceTextures2?.Add(l.source2);
                if (l.source3 != null) config.sourceTextures3?.Add(l.source3);
            }
            CompileConfig(config);
        }
    }
}
