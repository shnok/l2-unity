//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JBooth.MicroSplat;

namespace JBooth.MicroSplat
{
    public partial class MicroSplatTerrainEditor : Editor
    {

        public static TextureArrayConfig ConvertTerrains(Terrain[] terrains, TerrainLayer[] terrainLayers)
        {
            if (terrains.Length == 0)
                return null;

            string[] defaultKeywords = new string[] { "_MICROSPLAT", "_BRANCHSAMPLES", "_BRANCHSAMPLESAGR", "_USEGRADMIP", "_MICROTERRAIN" };

            foreach (Terrain ter in terrains)
            {
                var mst = ter.GetComponent<MicroSplatTerrain>();
                if (mst == null)
                    ter.gameObject.AddComponent<MicroSplatTerrain>();
            }
            var terrain = terrains[0];
            MicroSplatTerrain t = terrain.GetComponent<MicroSplatTerrain>();

            int texcount = terrain.terrainData.terrainLayers.Length;
            List<string> keywords = new List<string>(defaultKeywords);
            // set initial render pipleine
            var pipeline = MicroSplatUtilities.DetectPipeline();
            if (pipeline == MicroSplatUtilities.PipelineType.HDPipeline)
            {
                keywords.Add("_MSRENDERLOOP_UNITYHD");
                keywords.Add("_MSRENDERLOOP_UNITYHDRP2020");
                keywords.Add("_MSRENDERLOOP_UNITYHDRP2021");
                keywords.Add("_MSRENDERLOOP_UNITYHDRP2022");
            }
            else if (pipeline == MicroSplatUtilities.PipelineType.UniversalPipeline)
            {
                keywords.Add("_MSRENDERLOOP_UNITYLD");
                keywords.Add("_MSRENDERLOOP_UNITYURP2020");
                keywords.Add("_MSRENDERLOOP_UNITYURP2021");
                keywords.Add("_MSRENDERLOOP_UNITYURP2022");
            }

            // this just looks better, IMO..
            keywords.Add("_HYBRIDHEIGHTBLEND");

            // Because new users won't read the manual or read settings before they act, we don't clamp the texture count
            // down for maximum performance. Way to many support requests complaining of black terrain after adding textures because
            // they didn't realize they needed to up the max texture count. So now, 16 minimum. This is why we can't have nice things.
            /*
        if (texcount <= 4)
        {
        keywords.Add ("_MAX4TEXTURES");
        }
        else if (texcount <= 8)
        {
        keywords.Add ("_MAX8TEXTURES");
        }
        else if (texcount <= 12)
        {
        keywords.Add ("_MAX12TEXTURES");
        }
        */
            if (texcount > 16 && texcount <= 20)
            {
                keywords.Add("_MAX20TEXTURES");
            }
            else if (texcount > 16 && texcount <= 24)
            {
                keywords.Add("_MAX24TEXTURES");
            }
            else if (texcount > 16 && texcount <= 28)
            {
                keywords.Add("_MAX28TEXTURES");
            }
            else if (texcount > 16 && texcount > 28)
            {
                keywords.Add("_MAX32TEXTURES");
            }

            // setup this terrain
            t.templateMaterial = MicroSplatShaderGUI.NewShaderAndMaterial(terrain, keywords.ToArray());

            var config = TextureArrayConfigEditor.CreateConfig(terrain);
            t.templateMaterial.SetTexture("_Diffuse", config.diffuseArray);
            t.templateMaterial.SetTexture("_NormalSAO", config.normalSAOArray);
            // mask map format, now common.
            config.allTextureChannelHeight = TextureArrayConfig.AllTextureChannel.B;
            config.allTextureChannelAO = TextureArrayConfig.AllTextureChannel.G;
            config.allTextureChannelSmoothness = TextureArrayConfig.AllTextureChannel.A;
            t.propData = MicroSplatShaderGUI.FindOrCreatePropTex(t.templateMaterial);
            EditorUtility.SetDirty(t);
            if (terrainLayers != null)
            {
                if (terrainLayers.Length > 0)
                {
                    Vector2 min = new Vector2(99999, 99999);
                    Vector2 max = Vector2.zero;


                    for (int x = 0; x < terrainLayers.Length; ++x)
                    {
                        var uv = terrainLayers[x].tileSize;
                        if (min.x > uv.x)
                            min.x = uv.x;
                        if (min.y > uv.y)
                            min.y = uv.y;
                        if (max.x < uv.x)
                            max.x = uv.x;
                        if (max.y < uv.y)
                            max.y = uv.y;
                    }
                    Vector2 average = Vector2.Lerp(min, max, 0.5f);
                    // use per texture UVs instead..
                    float diff = Vector2.Distance(min, max);
                    if (diff > 0.1)
                    {
                        keywords.Add("_PERTEXUVSCALEOFFSET");

                        // if the user has widely different UVs, use the LOD sampler. This is because the gradient mode blends between mip levels,
                        // which looks bad with hugely different UVs. I still don't understand why people do this kind of crap though, ideally
                        // your UVs should not differ per texture, and if so, not by much..
                        if (diff > 10)
                        {
                            Debug.LogWarning("Terrain has wildly varing UV scales, it's best to keep consistent texture resolution. ");
                        }
                        if (!keywords.Contains("_USEGRADMIP"))
                        {
                            keywords.Add("_USEGRADMIP");
                        }
                        Vector4 scaleOffset = new Vector4(1, 1, 0, 0);
                        t.templateMaterial.SetVector("_UVScale", scaleOffset);
                        var propData = MicroSplatShaderGUI.FindOrCreatePropTex(t.templateMaterial);

                        for (int x = 0; x < terrain.terrainData.terrainLayers.Length; ++x)
                        {
                            var uvScale = terrain.terrainData.terrainLayers[x].tileSize;
                            var uvOffset = terrain.terrainData.terrainLayers[x].tileOffset;
                            uvScale = MicroSplatRuntimeUtil.UnityUVScaleToUVScale(uvScale, terrain);
                            uvScale.x = Mathf.RoundToInt(uvScale.x);
                            uvScale.y = Mathf.RoundToInt(uvScale.y);
                            propData.SetValue(x, MicroSplatPropData.PerTexVector2.SplatUVScale, uvScale);
                            propData.SetValue(x, MicroSplatPropData.PerTexVector2.SplatUVOffset, Vector2.zero);
                        }
                        for (int x = terrain.terrainData.terrainLayers.Length; x < 32; ++x)
                        {
                            propData.SetValue(x, MicroSplatPropData.PerTexVector2.SplatUVScale, average);
                            propData.SetValue(x, MicroSplatPropData.PerTexVector2.SplatUVOffset, Vector2.zero);
                        }
                        // must init the data, or the editor will write over it.

                        propData.SetValue(0, 15, Color.white);
                        EditorUtility.SetDirty(propData);

                        t.templateMaterial.SetVector("_TriplanarUVScale",
                            new Vector4(
                                10.0f / t.terrain.terrainData.size.x,
                                10.0f / t.terrain.terrainData.size.x, 0, 0));

                    }
                    else
                    {
                        var uvScale = terrain.terrainData.terrainLayers[0].tileSize;
                        var uvOffset = terrain.terrainData.terrainLayers[0].tileOffset;

                        uvScale = MicroSplatRuntimeUtil.UnityUVScaleToUVScale(uvScale, terrain);
                        uvOffset.x = uvScale.x / terrain.terrainData.size.x * 0.5f * uvOffset.x;
                        uvOffset.y = uvScale.y / terrain.terrainData.size.x * 0.5f * uvOffset.y;
                        Vector4 scaleOffset = new Vector4(uvScale.x, uvScale.y, uvOffset.x, uvOffset.y);
                        t.templateMaterial.SetVector("_UVScale", scaleOffset);
                        t.templateMaterial.SetVector("_TriplanarUVScale",
                            new Vector4(
                                10.0f / t.terrain.terrainData.size.x,
                                10.0f / t.terrain.terrainData.size.x, 0, 0));
                    }
                }
            }


            // now make sure others all have the same settings as well.
            for (int i = 0; i < terrains.Length; ++i)
            {
                var nt = terrains[i];
                var mgr = nt.GetComponent<MicroSplatTerrain>();
                mgr.templateMaterial = t.templateMaterial;
                mgr.keywordSO = MicroSplatUtilities.FindOrCreateKeywords(t.templateMaterial);

                if (mgr.propData == null)
                {
                    mgr.propData = MicroSplatShaderGUI.FindOrCreatePropTex(mgr.templateMaterial);
                }
            }

            t.keywordSO.keywords.Clear();
            t.keywordSO.keywords = new List<string>(keywords);

            // force recompile, so that basemap shader name gets reset correctly..
            MicroSplatShaderGUI.MicroSplatCompiler comp = new MicroSplatShaderGUI.MicroSplatCompiler();
            comp.Compile(t.templateMaterial);


            MicroSplatTerrain.SyncAll();
            return config;
        }

        bool DoConvertGUI(MicroSplatTerrain t)
        {
            if (t.templateMaterial == null)
            {
                if (GUILayout.Button("Convert to MicroSplat"))
                {
                    // get all terrains in selection, not just this one, and treat as one giant terrain
                    var objs = Selection.gameObjects;
                    List<Terrain> terrains = new List<Terrain>();
                    for (int i = 0; i < objs.Length; ++i)
                    {
                        Terrain ter = objs[i].GetComponent<Terrain>();
                        if (ter != null)
                        {
                            terrains.Add(ter);
                        }
                        Terrain[] trs = objs[i].GetComponentsInChildren<Terrain>();
                        for (int x = 0; x < trs.Length; ++x)
                        {
                            if (!terrains.Contains(trs[x]))
                            {
                                terrains.Add(trs[x]);
                            }
                        }
                    }

                    var config = ConvertTerrains(terrains.ToArray(), terrains[0].terrainData.terrainLayers);
                    if (config != null)
                    {
                        Selection.SetActiveObjectWithContext(config, config);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}