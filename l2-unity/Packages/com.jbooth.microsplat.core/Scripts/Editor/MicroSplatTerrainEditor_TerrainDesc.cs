//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JBooth.MicroSplat;

#if __MICROSPLAT__
namespace JBooth.MicroSplat
{
    public partial class MicroSplatTerrainEditor : Editor
    {


        public static Texture2D GenerateTerrainNormalMap(MicroSplatTerrain bt)
        {
            Terrain t = bt.terrain;
            int w = t.terrainData.heightmapResolution;
            int h = t.terrainData.heightmapResolution;

            Texture2D data = new Texture2D(w, h, TextureFormat.RGBA32, true, true);
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    Vector3 normal = t.terrainData.GetInterpolatedNormal((float)x / w, (float)y / h);
                    normal *= 0.5f;
                    normal.x += 0.5f; normal.y += 0.5f; normal.z += 0.5f;
                    data.SetPixel(x, y, new Color(normal.x, normal.y, normal.z));
                }
            }

            data.Apply();

            var path = MicroSplatUtilities.RelativePathFromAsset(t.terrainData);
            path += "/" + t.name + "_normal.tga";
            var bytes = data.EncodeToTGA();
            System.IO.File.WriteAllBytes(path, bytes);
            GameObject.DestroyImmediate(data);
            AssetDatabase.Refresh();
            bt.perPixelNormal = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var ai = AssetImporter.GetAtPath(path);
            var ti = ai as TextureImporter;
            var ps = ti.GetDefaultPlatformTextureSettings();
            if (ti.isReadable == true ||
               ti.wrapMode != TextureWrapMode.Clamp ||
               ti.npotScale != TextureImporterNPOTScale.None ||
               ps.overridden != true ||
               ti.isReadable != false ||
               ti.sRGBTexture != false ||
               ps.textureCompression != TextureImporterCompression.Uncompressed ||
               ti.textureType != TextureImporterType.Default)

            {
                ti.textureType = TextureImporterType.Default;
                ti.mipmapEnabled = true;
                ps.overridden = true;
                ti.sRGBTexture = false;
                ps.textureCompression = TextureImporterCompression.Uncompressed;
                ti.SetPlatformTextureSettings(ps);
                ti.wrapMode = TextureWrapMode.Clamp;
                ti.isReadable = false;
                ti.npotScale = TextureImporterNPOTScale.None;
                ti.SaveAndReimport();
            }

            EditorUtility.SetDirty(bt);
            EditorUtility.SetDirty(bt.terrain);
            MicroSplatTerrain.SyncAll();
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(ti.assetPath);
        }

#if __MICROSPLAT_PROCTEX__
        public static void ComputeCavityFlowMap(MicroSplatTerrain bt)
        {
            Terrain t = bt.terrain;
            Texture2D data = new Texture2D(t.terrainData.heightmapResolution, t.terrainData.heightmapResolution, TextureFormat.RGBA32, true, true);
            CurvatureMapGenerator.CreateMap(t.terrainData.GetHeights(0, 0, data.width, data.height), data);

            var path = MicroSplatUtilities.RelativePathFromAsset(t.terrainData);
            path += "/" + t.name + "_cavity.png";
            var bytes = data.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            GameObject.DestroyImmediate(data);
            AssetDatabase.Refresh();
            bt.cavityMap = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            var ai = AssetImporter.GetAtPath(path);
            var ti = ai as TextureImporter;

            if (ti.sRGBTexture != false)
            {
                ti.sRGBTexture = false;
                ti.SaveAndReimport();
            }

            EditorUtility.SetDirty(bt);
            EditorUtility.SetDirty(t);
            MicroSplatTerrain.SyncAll();
            AssetDatabase.SaveAssets();
        }
#endif


        static GUIContent CPerPixelNormal = new GUIContent("Per Pixel Normal", "Per Pixel normal map");
        public void DoTerrainDescGUI()
        {
            MicroSplatTerrain bt = target as MicroSplatTerrain;
            Terrain t = bt.GetComponent<Terrain>();
            if (t == null || t.terrainData == null)
            {
                return;
            }
            if (t.materialTemplate == null)
            {
                return;
            }
            if (bt.keywordSO == null)
            {
                return;
            }

            if (!bt.keywordSO.IsKeywordEnabled("_TERRAINBLENDING") && !bt.keywordSO.IsKeywordEnabled("_DYNAMICFLOWS"))
            {
                return;
            }
            EditorGUILayout.Space();

            if (bt.blendMat == null && bt.templateMaterial != null && bt.keywordSO != null && bt.keywordSO.IsKeywordEnabled("_TERRAINBLENDING"))
            {
                var path = AssetDatabase.GetAssetPath(bt.templateMaterial);
                path = path.Replace(".mat", "_TerrainObjectBlend.mat");
                bt.blendMat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (bt.blendMat == null)
                {
                    string shaderPath = path.Replace(".mat", ".shader");
                    Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                    if (shader == null)
                    {
                        shaderPath = path.Replace(".shader", ".surfshader");
                        shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                    }

                    if (shader == null)
                    {
                        shaderPath = AssetDatabase.GetAssetPath(bt.templateMaterial.shader);
                        shaderPath = shaderPath.Replace(".shader", "_TerrainObjectBlend.shader");
                        shaderPath = shaderPath.Replace(".surfshader", "_TerrainObjectBlend.surfshader");
                        shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
                    }
                    if (shader != null)
                    {
                        Material mat = new Material(shader);
                        AssetDatabase.CreateAsset(mat, path);
                        AssetDatabase.SaveAssets();
                        MicroSplatTerrain.SyncAll();
                    }
                }
            }
            EditorUtility.SetDirty(bt);
            EditorUtility.SetDirty(bt.terrain);
        }


        public void DoPerPixelNormalGUI()
        {
            MicroSplatTerrain bt = target as MicroSplatTerrain;
            Terrain t = bt.GetComponent<Terrain>();
            if (t == null || t.terrainData == null)
            {
                EditorGUILayout.HelpBox("No Terrain data found", MessageType.Error);
                return;
            }
            if (t.materialTemplate == null)
            {
                return;
            }

            if (bt.keywordSO == null)
                return;

            if (bt.terrain.drawInstanced && bt.perPixelNormal != null && !bt.keywordSO.IsKeywordEnabled("_TERRAINBLENDING"))
            {
                EditorGUILayout.HelpBox("Per Pixel Normal is assigned, but shader is using Instance rendering, which automatically provides per-pixel normal. You may turn off per pixel normal if it's on and clear the normal data to save memory.", MessageType.Warning);
                if (bt.perPixelNormal != null && GUILayout.Button("Clear"))
                {
                    bt.perPixelNormal = null;
                    EditorUtility.SetDirty(bt);
                    EditorUtility.SetDirty(bt.terrain);
                }
            }

            MicroSplatUtilities.DrawTextureField(bt, CPerPixelNormal, ref bt.perPixelNormal, "_PERPIXNORMAL", "_TERRAINBLENDING", null, null, false);

            if (bt.perPixelNormal == null &&
               (bt.keywordSO.IsKeywordEnabled("_PERPIXNORMAL") || bt.keywordSO.IsKeywordEnabled("_TERRAINBLENDING")))
            {
                EditorGUILayout.HelpBox("Terrain Normal Data is not present, please generate", MessageType.Warning);
            }

            if (bt.keywordSO.IsKeywordEnabled("_PERPIXNORMAL") || bt.keywordSO.IsKeywordEnabled("_TERRAINBLENDING"))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Normal Data");
                if (GUILayout.Button(bt.perPixelNormal == null ? "Generate" : "Update"))
                {
                    GenerateTerrainNormalMap(bt);
                    EditorUtility.SetDirty(bt);
                    EditorUtility.SetDirty(bt.terrain);
                }

                if (bt.perPixelNormal != null && GUILayout.Button("Clear"))
                {
                    bt.perPixelNormal = null;
                    EditorUtility.SetDirty(bt);
                    EditorUtility.SetDirty(bt.terrain);
                }
                EditorGUILayout.EndHorizontal();
            }





        }

#if __MICROSPLAT_PROCTEX__
        public void DoCavityMapGUI()
        {
            MicroSplatTerrain bt = target as MicroSplatTerrain;
            Terrain t = bt.GetComponent<Terrain>();
            if (t == null || t.terrainData == null)
            {
                EditorGUILayout.HelpBox("No Terrain data found", MessageType.Error);
                return;
            }
            if (t.materialTemplate == null)
            {
                return;
            }

            if (bt.keywordSO == null)
                return;

            if (bt.cavityMap == null)
            {
                EditorGUILayout.HelpBox("Cavity Map Data is not present, please generate or provide", MessageType.Warning);

            }
            bt.cavityMap = (Texture2D)EditorGUILayout.ObjectField("Cavity Map", bt.cavityMap, typeof(Texture2D), false);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Cavity Data");
            if (GUILayout.Button(bt.cavityMap == null ? "Generate" : "Update"))
            {
                ComputeCavityFlowMap(bt);
            }

            if (bt.cavityMap != null && GUILayout.Button("Clear"))
            {
                bt.cavityMap = null;
            }
            EditorGUILayout.EndHorizontal();



        }
#endif
    }
#endif
}
