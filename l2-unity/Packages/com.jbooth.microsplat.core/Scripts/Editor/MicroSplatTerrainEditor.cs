//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using JBooth.MicroSplat;

namespace JBooth.MicroSplat
{
    [CustomEditor(typeof(MicroSplatTerrain))]
    [CanEditMultipleObjects]
    public partial class MicroSplatTerrainEditor : Editor
    {
#if __MICROSPLAT__

#if __MICROSPLAT_GLOBALTEXTURE__
        static GUIContent geoTexOverride = new GUIContent("Geo Texture Override", "If you want each terrain object to have it's own geo texture instead of the one defined in the material, add it here");
        static GUIContent geoTintOverride = new GUIContent("Tint Texture Override", "If you want each terrain object to have it's own global tint instead of the one defined in the material, add it here");
        static GUIContent geoNormalOverride = new GUIContent("Global Normal Override", "If you want each terrain object to have it's own global normal instead of the one defined in the material, add it here");
        static GUIContent geoSAOMOverride = new GUIContent("Global SOAM Override", "If you want each terrain to have it's own Smoothness(R), AO(G) and Metallic (B) map instead of the one defined in the material, add it here");
        static GUIContent geoEmisOverride = new GUIContent("Global Emissive Override", "If you want each terrain to have it's own Emissive map instead of the one defined in the material, set it here");
#endif

#if __MICROSPLAT_SCATTER__
        static GUIContent scatterMapOverride = new GUIContent("Scatter Map Override", "Scatter Control Texture");
#endif

#if __MICROSPLAT_SNOW__
        static GUIContent snowMaskOverride = new GUIContent("Snow Mask Override", "If you want each terrain to have it's own snow mask, assign it here");
#endif

        static GUIContent perPixelNormal = new GUIContent("Per Pixel Normal", "Per Pixel normal map if using non-instanced terrain rendering");
#if __MICROSPLAT_ALPHAHOLE__
        static GUIContent clipMapOverride = new GUIContent("Clip Map Override", "Provide a unique clip map for each terrain");
#endif
#if __MICROSPLAT_PROCTEX__
        static GUIContent biomeOverride = new GUIContent("Biome Map Override", "Biome map for this terrain");
        static GUIContent biomeOverride2 = new GUIContent("Biome Map2 Override", "Biome map for this terrain");
        static GUIContent CCavityMap = new GUIContent("Cavity Map Override", "Cavity map for this terrain");
#endif

#if __MICROSPLAT_STREAMS__
        static GUIContent streamOverride = new GUIContent("Stream Map Override", "Wetness, Puddles, Streams and Lava map for this terrain");
#endif

        static GUIContent CTemplateMaterial = new GUIContent("Template Material", "Material to use for this terrain");

#if (VEGETATION_STUDIO || VEGETATION_STUDIO_PRO)
   static GUIContent CVSGrassMap = new GUIContent("Grass Map", "Grass Map from Vegetation Studio");
   static GUIContent CVSShadowMap = new GUIContent("Shadow Map", "Shadow map texture from Vegetation Studio");
#endif
        static GUIContent CBaseMap = new GUIContent("Base Map Shader", "Asset Bundles/Addressables have a bug that they don't work with the base map shader, so we keep a hard reference to it so the bundle will load it and it will be found");
        static GUIContent CBlendMat = new GUIContent("Blend Mat", "Blending material for terrain blending");
        static GUIContent CCustomSplat0 = new GUIContent("Custom Splat 0", "Custom splat map for textures 0-3");
        static GUIContent CCustomSplat1 = new GUIContent("Custom Splat 1", "Custom splat map for textures 4-7");
        static GUIContent CCustomSplat2 = new GUIContent("Custom Splat 2", "Custom splat map for textures 8-11");
        static GUIContent CCustomSplat3 = new GUIContent("Custom Splat 3", "Custom splat map for textures 12-15");
        static GUIContent CCustomSplat4 = new GUIContent("Custom Splat 4", "Custom splat map for textures 16-19");
        static GUIContent CCustomSplat5 = new GUIContent("Custom Splat 5", "Custom splat map for textures 20-23");
        static GUIContent CCustomSplat6 = new GUIContent("Custom Splat 6", "Custom splat map for textures 24-27");
        static GUIContent CCustomSplat7 = new GUIContent("Custom Splat 7", "Custom splat map for textures 28-31");
        static GUIContent CPatchBoundsMultiplier = new GUIContent("Patch Bounds Multiplier", "Unity can clip vertices when tessellation is enabled. Unfortunately it doesn't expose a way to expand the bounds by the max displacement amount, only a mutliplier. So if this is happening, you can scale the bounds here. A value of 1.15 on each axis is usually enough. Higher values will cause more off-screen terrain to be drawn");
        static GUIContent CMegaSplatMap = new GUIContent("Splat Map", "MegaSplat format control map");

        SerializedProperty templateMaterial;

        void OnEnable()
        {
            templateMaterial = serializedObject.FindProperty("templateMaterial");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MicroSplatTerrain t = target as MicroSplatTerrain;
            if (t == null)
            {
                EditorGUILayout.HelpBox("No Terrain Present, please put this component on a terrain", MessageType.Error);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://docs.google.com/document/d/1t_ZEKW8bHJqVWulH9Tcu-V5PIMr-sBOV_Ob7k2y1XY8/");
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(templateMaterial, CTemplateMaterial);
            if (EditorGUI.EndChangeCheck())
            {
                MicroSplatTerrain.SyncAll();
            }
            EditorGUI.BeginChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (DoConvertGUI(t))
            {
                return;
            }

            if (t.templateMaterial == null)
            {
                return;
            }

            // People love to rename stuff, but there's no way to associate various objects
            // with a material in Unity, and the material needs the keywords, propdata, etc obejcts,
            // so this is looked up via name. People are also sloppy about files, and end up with
            // 10 different microsplat setups in one directory. To help reduce support, we automatically
            // create these files with the correct names when missing. We also see if they are assigned,
            // but named wrong, meaning the material will just create it later. When this happens, we
            // reassign it to one with the correct name. 

            if (t.propData == null)
            {
                t.propData = MicroSplatShaderGUI.FindOrCreatePropTex(t.templateMaterial);
                EditorUtility.SetDirty(t);
                MicroSplatObject.SyncAll();
            }
            else if (!t.propData.name.StartsWith(t.templateMaterial.name + "_propdata"))
            {
                Debug.LogWarning("PropData object assigned to terrain was not the one associated with the material, reassigning or creating one with the correct name");
                t.propData = MicroSplatShaderGUI.FindOrCreatePropTex(t.templateMaterial);
                EditorUtility.SetDirty(t);
            }


            if (t.keywordSO == null)
            {
                t.keywordSO = MicroSplatUtilities.FindOrCreateKeywords(t.templateMaterial);
                EditorUtility.SetDirty(t);
            }
            else
            {
                if (!t.keywordSO.name.StartsWith(t.templateMaterial.name + "_keywords"))
                {
                    Debug.LogWarning("Keyword file is not named correctly! Attempting to find or create a new one");
                    t.keywordSO = MicroSplatUtilities.FindOrCreateKeywords(t.templateMaterial);
                    EditorUtility.SetDirty(t);
                }
            }

            EditorGUI.BeginChangeCheck();

#if __MICROSPLAT_PROCTEX__
            if (t.keywordSO.IsKeywordEnabled("_PROCEDURALTEXTURE") || t.keywordSO.IsKeywordEnabled("_PCHEIGHTGRADIENT") || t.keywordSO.IsKeywordEnabled("_PCHEIGHTHSV"))
            {
                var old = t.procTexCfg;
                t.procTexCfg = MicroSplatProceduralTexture.FindOrCreateProceduralConfig(t.templateMaterial);
                if (old != t.procTexCfg)
                {
                    EditorUtility.SetDirty(t);
                    MicroSplatObject.SyncAll();
                }
            }
#endif

#if __MICROSPLAT_TERRAINBLEND__ || __MICROSPLAT_STREAMS__
            DoTerrainDescGUI();
#endif

            DoPerPixelNormalGUI();

#if __MICROSPLAT_PROCTEX__
            if (t.keywordSO.IsKeywordEnabled(MicroSplatProceduralTexture.GetFeatureName(MicroSplatProceduralTexture.DefineFeature._PCCAVITY))
               || t.keywordSO.IsKeywordEnabled(MicroSplatProceduralTexture.GetFeatureName(MicroSplatProceduralTexture.DefineFeature._PCFLOW)))
            {
                DoCavityMapGUI();
            }
#endif
            // could move this to some type of interfaced component created by the module if this becomes a thing,
            // but I think this will be most of the cases?

            MicroSplatUtilities.DrawTextureField(t, CCustomSplat0, ref t.customControl0, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat1, ref t.customControl1, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat2, ref t.customControl2, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat3, ref t.customControl3, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat4, ref t.customControl4, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat5, ref t.customControl5, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat6, ref t.customControl6, "_CUSTOMSPLATTEXTURES");
            MicroSplatUtilities.DrawTextureField(t, CCustomSplat7, ref t.customControl7, "_CUSTOMSPLATTEXTURES");
#if __MICROSPLAT_MEGA__
            MicroSplatUtilities.DrawTextureField(t, CMegaSplatMap, ref t.megaSplatMap, "_MEGASPLATTEXTURE");
#endif
            // perpixel normal
            MicroSplatUtilities.DrawTextureField(t, perPixelNormal, ref t.perPixelNormal, "_PERPIXELNORMAL");

            // global texture overrides
#if __MICROSPLAT_GLOBALTEXTURE__
            MicroSplatUtilities.DrawTextureField(t, geoTexOverride, ref t.geoTextureOverride, "_GEOMAP");
            MicroSplatUtilities.DrawTextureField(t, geoTintOverride, ref t.tintMapOverride, "_GLOBALTINT");
            MicroSplatUtilities.DrawTextureField(t, geoNormalOverride, ref t.globalNormalOverride, "_GLOBALNORMALS");
            MicroSplatUtilities.DrawTextureField(t, geoSAOMOverride, ref t.globalSAOMOverride, "_GLOBALSMOOTHAOMETAL");
            MicroSplatUtilities.DrawTextureField(t, geoEmisOverride, ref t.globalEmisOverride, "_GLOBALEMIS");
#endif

#if __MICROSPLAT_SCATTER__
            MicroSplatUtilities.DrawTextureField(t, scatterMapOverride, ref t.scatterMapOverride, "_SCATTER");
#endif

#if __MICROSPLAT_SNOW__
            MicroSplatUtilities.DrawTextureField(t, snowMaskOverride, ref t.snowMaskOverride, "_SNOWMASK");
#endif

#if __MICROSPLAT_ALPHAHOLE__
            // alpha hole override
            MicroSplatUtilities.DrawTextureField(t, clipMapOverride, ref t.clipMap, "_ALPHAHOLETEXTURE");
#endif

#if (VEGETATION_STUDIO || VEGETATION_STUDIO_PRO)
      // vsstudio overrides
      MicroSplatUtilities.DrawTextureField(t, CVSGrassMap, ref t.vsGrassMap, "_VSGRASSMAP");
      MicroSplatUtilities.DrawTextureField(t, CVSShadowMap, ref t.vsShadowMap, "_VSSHADOWMAP");
#endif


#if __MICROSPLAT_PROCTEX__
            MicroSplatUtilities.DrawTextureField(t, biomeOverride, ref t.procBiomeMask, "_PCBIOMEMASK", "_PCBIOMEMASK16", null, null, false);
            MicroSplatUtilities.DrawTextureField(t, biomeOverride2, ref t.procBiomeMask2, "_PCBIOMEMASK2");
            MicroSplatUtilities.DrawTextureField(t, CCavityMap, ref t.cavityMap, "_PROCEDURALTEXTURE");
#endif


#if __MICROSPLAT_STREAMS__
            MicroSplatUtilities.DrawTextureField(t, streamOverride, ref t.streamTexture, "_WETNESS", "_PUDDLES", "_STREAMS", "_LAVA", false);
#endif

#if __MICROSPLAT_TESSELLATION__
            t.patchBoundsMultiplier = EditorGUILayout.Vector3Field(CPatchBoundsMultiplier, t.patchBoundsMultiplier);
#endif
            if (EditorGUI.EndChangeCheck())
            {
                MicroSplatTerrain.SyncAll();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Sync"))
            {
                var mgr = target as MicroSplatTerrain;
                mgr.Sync();
                EditorUtility.SetDirty(mgr);
            }
            if (GUILayout.Button("Sync All"))
            {
                MicroSplatTerrain.SyncAll();
                var mgr = target as MicroSplatTerrain;
                EditorUtility.SetDirty(mgr);
            }
            EditorGUILayout.EndHorizontal();

            BakingGUI(t);
            WeightLimitingGUI(t);
            ImportExportGUI();


            if (MicroSplatUtilities.DrawRollup("Debug", false, true))
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.HelpBox("These should not need to be edited unless something funky has happened. They are automatically managed by MicroSplat.", MessageType.Info);
                t.propData = EditorGUILayout.ObjectField("Per Texture Data", t.propData, typeof(MicroSplatPropData), false) as MicroSplatPropData;
#if __MICROSPLAT_PROCTEX__
                t.procTexCfg = EditorGUILayout.ObjectField("Procedural Config", t.procTexCfg, typeof(MicroSplatProceduralTextureConfig), false) as MicroSplatProceduralTextureConfig;
#endif
                t.keywordSO = EditorGUILayout.ObjectField("Keywords", t.keywordSO, typeof(MicroSplatKeywords), false) as MicroSplatKeywords;
                t.blendMat = EditorGUILayout.ObjectField(CBlendMat, t.blendMat, typeof(Material), false) as Material;
                t.baseMapShader = EditorGUILayout.ObjectField(CBaseMap, t.baseMapShader, typeof(Shader), false) as Shader;
                EditorGUI.indentLevel -= 2;
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(t);
            }

        }

#endif
    }
}
