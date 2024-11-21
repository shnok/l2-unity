// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic.StyledGUI;

namespace TheVegetationEngine
{
#if UNITY_EDITOR
    [HelpURL("https://docs.google.com/document/d/145JOVlJ1tE-WODW45YoJ6Ixg23mFc56EnB_8Tbwloz8/edit#heading=h.q4fstlrr3cw4")]
    [ExecuteInEditMode]
    [AddComponentMenu("BOXOPHOBIC/The Vegetation Engine/TVE Terrain")]
#endif
    public class TVETerrain : StyledMonoBehaviour
    {
        [StyledBanner(0.890f, 0.745f, 0.309f, "Terrain")]
        public bool styledBanner;

#if !THE_VEGETATION_ENGINE_TERRAIN
        [StyledMessage("Error", "The Terrain Shader Module is required for the terrain component to work!", 5, 10)]
        public bool styledMessage = true;
#endif
        [Tooltip("Sync the terrain data with the material in editor if the terrain is modified by external tools.")]
        public TVETerrainRefreshMode terrainRefresh = TVETerrainRefreshMode.Selection;
        [Tooltip("Sets the terrain bounds multiplier used to avoid patches culling when using vertex offset elements.")]
        public float terrainBounds = 1;
        [Tooltip("Override the terrain layer maps and settings without modifying the actual terrain layer.")]
        public TVETerrainSettings terrainSettings = new TVETerrainSettings();

        [HideInInspector]
        public bool isInitialized = false;

        [HideInInspector]
        public MaterialPropertyBlock terrainPropertyBlock;
        [HideInInspector]
        public Material terrainMaterial;

        Terrain terrain;
        Renderer meshRenderer;

        Texture2D whiteTex;
        Texture2D normalTex;

        [StyledSpace(5)]
        public bool styledSpace1;

        void OnEnable()
        {
            InitializeTerrain();
        }

        void Start()
        {
            UpdateTerrainSettings();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
            {
                if (Selection.Contains(gameObject) || terrainRefresh == TVETerrainRefreshMode.Always)
                {
                    UpdateTerrainSettings();
                    UpdateOverrideNames();
                }
            }
        }
#endif

        void InitializeTerrain()
        {
            terrain = GetComponent<Terrain>();
            meshRenderer = GetComponent<Renderer>();

            //if (terrain.materialTemplate != null)
            //{
            //    if (terrain.materialTemplate.shader.name.Contains("Error"))
            //    {
            //        terrain.materialTemplate.shader = Shader.Find("BOXOPHOBIC/The Vegetation Engine/Landscape/Terrain Standard Lit");
            //    }
            //}

            whiteTex = Resources.Load<Texture2D>("Internal WhiteTex");
            normalTex = Resources.Load<Texture2D>("Internal NormalTex");
        }

        public void UpdateTerrainSettings()
        {
            if (terrainPropertyBlock == null)
            {
                terrainPropertyBlock = new MaterialPropertyBlock();
            }

            bool validTerrain = terrain != null && terrain.terrainData != null && terrain.materialTemplate != null;
            bool validRenderer = meshRenderer != null && meshRenderer.sharedMaterial != null;

            if (validTerrain)
            {
                if (terrain.terrainData.holesTexture != null)
                {
                    terrainPropertyBlock.SetTexture("_HolesTex", terrain.terrainData.holesTexture);
                }

                for (int i = 0; i < terrain.terrainData.alphamapTextures.Length; i++)
                {
                    var splat = terrain.terrainData.alphamapTextures[i];
                    var index = i + 1;

                    if (splat != null)
                    {
                        terrainPropertyBlock.SetTexture("_ControlTex" + index, splat);
                    }
                }

                for (int i = 0; i < terrain.terrainData.terrainLayers.Length; i++)
                {
                    var layer = terrain.terrainData.terrainLayers[i];
                    var index = i + 1;

                    if (layer == null)
                    {
                        continue;
                    }

                    CopyLayerSettings(terrainPropertyBlock, layer, index);
                }
            }

            if (validTerrain || validRenderer)
            {
                if (terrainSettings.useCustomTextures)
                {
                    if (terrainSettings.terrainHolesMask != null)
                    {
                        terrainPropertyBlock.SetTexture("_HolesTex", terrainSettings.terrainHolesMask);
                    }

                    if (terrainSettings.terrainControl01 != null)
                    {
                        terrainPropertyBlock.SetTexture("_ControlTex1", terrainSettings.terrainControl01);
                    }

                    if (terrainSettings.terrainControl02 != null)
                    {
                        terrainPropertyBlock.SetTexture("_ControlTex2", terrainSettings.terrainControl02);
                    }

                    if (terrainSettings.terrainControl03 != null)
                    {
                        terrainPropertyBlock.SetTexture("_ControlTex3", terrainSettings.terrainControl03);
                    }

                    if (terrainSettings.terrainControl04 != null)
                    {
                        terrainPropertyBlock.SetTexture("_ControlTex4", terrainSettings.terrainControl04);
                    }
                }

                // Terrain Layer Overrides
                for (int i = 0; i < terrainSettings.terrainLayers.Count; i++)
                {
                    var overrideElement = terrainSettings.terrainLayers[i];

                    if (!overrideElement.isInitialized)
                    {
                        terrainSettings.terrainLayers[i] = new TVETerrainLayerSettings();
                        terrainSettings.terrainLayers[i].isInitialized = true;
                    }

                    int index;

                    if (terrainSettings.useLayersOrderAsID)
                    {
                        index = i;
                    }
                    else
                    {
                        index = overrideElement.layerID;
                    }

                    if (/*terrainLayerSettings.overrideAllLayers && */ overrideElement.useCustomLayer)
                    {
                        if (overrideElement.terrainLayer != null)
                        {
                            CopyLayerSettings(terrainPropertyBlock, overrideElement.terrainLayer, index);
                        }
                    }

                    if (/*terrainLayerSettings.overrideAllTextures && */ overrideElement.useCustomTextures)
                    {
                        if (overrideElement.layerAlbedo != null)
                        {
                            terrainPropertyBlock.SetTexture("_AlbedoTex" + index, overrideElement.layerAlbedo);
                        }

                        if (overrideElement.layerNormal != null)
                        {
                            terrainPropertyBlock.SetTexture("_NormalTex" + index, overrideElement.layerNormal);
                        }

                        if (overrideElement.layerMask != null)
                        {
                            terrainPropertyBlock.SetTexture("_MaskTex" + index, overrideElement.layerMask);
                        }
                    }

                    if (/*terrainLayerSettings.overrideAllSettings &&*/ overrideElement.useCustomSettings)
                    {
                        terrainPropertyBlock.SetVector("_MaskMin" + index, overrideElement.layerRemapMin);
                        terrainPropertyBlock.SetVector("_MaskMax" + index, overrideElement.layerRemapMax);
                        terrainPropertyBlock.SetVector("_Specular" + index, overrideElement.layerSpecular);
                        terrainPropertyBlock.SetVector("_Params" + index, new Vector4(0, 0, overrideElement.layerNormalScale, overrideElement.layerSmoothness));
                        terrainPropertyBlock.SetVector("_Coords" + index, new Vector4(1 / overrideElement.layerScaleAndOffset.x, 1 / overrideElement.layerScaleAndOffset.y, overrideElement.layerScaleAndOffset.x, overrideElement.layerScaleAndOffset.y));
                    }
                }
            }

            if (validTerrain)
            {
                terrain.SetSplatMaterialPropertyBlock(terrainPropertyBlock);
                terrainMaterial = terrain.materialTemplate;

                // Terrain Transform
                terrainPropertyBlock.SetVector("_TerrainPosition", terrain.transform.position);
                terrainPropertyBlock.SetVector("_TerrainSize", terrain.terrainData.size);
                terrainPropertyBlock.SetFloat("_TerrainMeshMode", 0);

                terrain.patchBoundsMultiplier = new Vector3(terrainBounds, terrainBounds, terrainBounds);
            }

            if (validRenderer)
            {
                meshRenderer.SetPropertyBlock(terrainPropertyBlock);
                terrainMaterial = meshRenderer.sharedMaterial;

                terrainPropertyBlock.SetVector("_TerrainPosition", meshRenderer.bounds.center);
                terrainPropertyBlock.SetVector("_TerrainSize", meshRenderer.bounds.size);
                terrainPropertyBlock.SetFloat("_TerrainMeshMode", 1);
            }
        }

        void CopyLayerSettings(MaterialPropertyBlock materialPropertyBlock, TerrainLayer layer, int id)
        {
            if (layer.diffuseTexture != null)
            {
                materialPropertyBlock.SetTexture("_AlbedoTex" + id, layer.diffuseTexture);
            }
            else
            {
                materialPropertyBlock.SetTexture("_AlbedoTex" + id, whiteTex);
            }

            if (layer.normalMapTexture != null)
            {
                materialPropertyBlock.SetTexture("_NormalTex" + id, layer.normalMapTexture);
            }
            else
            {
                materialPropertyBlock.SetTexture("_NormalTex" + id, normalTex);
            }

            if (layer.maskMapTexture != null)
            {
                materialPropertyBlock.SetTexture("_MaskTex" + id, layer.maskMapTexture);
            }
            else
            {
                materialPropertyBlock.SetTexture("_MaskTex" + id, whiteTex);
            }

            materialPropertyBlock.SetVector("_MaskMin" + id, layer.maskMapRemapMin);
            materialPropertyBlock.SetVector("_MaskMax" + id, layer.maskMapRemapMax);
            materialPropertyBlock.SetVector("_Params" + id, new Vector4(layer.metallic, 0, layer.normalScale, layer.smoothness));
            materialPropertyBlock.SetVector("_Specular" + id, layer.specular);
            materialPropertyBlock.SetVector("_Coords" + id, new Vector4(1 / layer.tileSize.x, 1 / layer.tileSize.y, layer.tileOffset.x, layer.tileOffset.y));
        }

#if UNITY_EDITOR
        void UpdateOverrideNames()
        {
            // Terrain Layer Overrides
            for (int i = 0; i < terrainSettings.terrainLayers.Count; i++)
            {
                var overrideElement = terrainSettings.terrainLayers[i];
                
                int index;

                if (terrainSettings.useLayersOrderAsID)
                {
                    index = i;
                }
                else
                {
                    index = overrideElement.layerID - 1;
                }

                bool validTerrain = terrain != null && terrain.terrainData != null;

                if (validTerrain && terrain.terrainData.terrainLayers != null && terrain.terrainData.terrainLayers.Length > index && terrain.terrainData.terrainLayers[index] != null)
                {
                    overrideElement.name = terrain.terrainData.terrainLayers[index].name;
                }
                else
                {
                    overrideElement.name = "Layer " + overrideElement.layerID + " (Missing)";
                }

                if (/*terrainLayerSettings.overrideAllLayers &&*/ overrideElement.useCustomLayer)
                {
                    if (overrideElement.terrainLayer != null)
                    {
                        overrideElement.name = overrideElement.terrainLayer.name;
                    }
                }
            }
        }

        //void UpdateOverrideDebugData()
        //{
        //    var perLayerMaps = 3;

        //    if (terrainShaderSettings.shaderMaps == TVETerrainMapsMode.Packed)
        //    {
        //        perLayerMaps = 2;
        //    }

        //    var sampleCountList = new List<int>();

        //    for (int i = 0; i < (int)terrainShaderSettings.shaderLayers; i++)
        //    {
        //        sampleCountList.Add(1);
        //    }

        //    // Terrain Shader Overrides
        //    for (int i = 0; i < terrainShaderSettings.shaderLayerOverrides.Count; i++)
        //    {
        //        var overrideElement = terrainShaderSettings.shaderLayerOverrides[i];
        //        var overrideID = overrideElement.layerID - 1;

        //        if (terrain.terrainData.terrainLayers != null && terrain.terrainData.terrainLayers.Length > overrideID && terrain.terrainData.terrainLayers[overrideID] != null)
        //        {
        //            if (terrainShaderSettings.useAllFeatureOverrides)
        //            {
        //                if (overrideElement.textureCoords == TVETextureCoordsMode.Planar)
        //                {
        //                    if (overrideElement.textureSample == TVETextureSampleMode.Stochastic)
        //                    {
        //                        sampleCountList[overrideID] = 3;
        //                    }
        //                    else
        //                    {
        //                        sampleCountList[overrideID] = 1;
        //                    }
        //                }

        //                if (overrideElement.textureCoords == TVETextureCoordsMode.Triplanar)
        //                {
        //                    if (overrideElement.textureSample == TVETextureSampleMode.Stochastic)
        //                    {
        //                        sampleCountList[overrideID] = 9;
        //                    }
        //                    else
        //                    {
        //                        sampleCountList[overrideID] = 3;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    var sampleCount = 0;

        //    for (int i = 0; i < sampleCountList.Count; i++)
        //    {
        //        sampleCount += sampleCountList[i];
        //    }

        //    terrainShaderSettings.debugData = "<size=10>Layers: " + (int)terrainShaderSettings.shaderLayers + " supported | Maps: " + (int)terrainShaderSettings.shaderLayers * perLayerMaps + " textures | Samples: " + sampleCount * perLayerMaps + " texture samples</size>";
        //}

        void OnValidate()
        {
            InitializeTerrain();
        }
#endif
    }
}


