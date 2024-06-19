#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2TerrainGenerator {
    public float ueToUnityUnitScale = (1f / 52.5f); // 1 meter = 52.5 UU
    public float worldPositionOffset = 1f;
    private string terrainContainerName = "terrain_";

    public Terrain InstantiateTerrain(MapGenerationData generationData, L2TerrainInfo terrainInfo) {
        string directoryPath = Path.Combine("Assets", "Resources", "Data", "Maps", generationData.mapName, "TerrainData");
        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        if (generationData.generateStaticMeshes) {
            GenerateStaticMeshes(terrainInfo);
        }

        if (!generationData.generateDecoLayers && !generationData.generateUVLayers && !generationData.generateHeightmaps) {
            return null;
        }

        // Create the terrain object
        GameObject terrainObj = Terrain.CreateTerrainGameObject(new TerrainData());
        terrainObj.name = terrainContainerName + terrainInfo.mapName;

        // Get the Terrain component and TerrainData
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        terrain.heightmapPixelError = 3;
        terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        terrain.drawInstanced = true;
        terrain.detailObjectDistance = 150;

        TerrainData terrainData = terrain.terrainData;
        terrainData.baseMapResolution = L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE;
        terrainData.alphamapResolution = L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE;

        terrainData.SetDetailResolution(512, 32);

        // Just to initialize
        terrainData.size = new Vector3(1015f, 603f, 1015f);

        // Save the terrainData asset
        string savePath = Path.Combine(directoryPath, terrainInfo.mapName + ".asset");
        AssetDatabase.CreateAsset(terrainData, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign the saved asset to the terrain object
        terrain.terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(savePath);

        if (generationData.generateUVLayers) {
            Debug.Log(terrainInfo.mapName);
            Debug.Log(terrainData);
            Debug.Log(terrainInfo);
            GenerateUVLayers(terrainInfo.mapName, terrainData, terrainInfo);
        }

        if (generationData.generateHeightmaps) {
            GenerateHeightmaps(terrainData, terrainInfo);
        }

        if (generationData.generateDecoLayers) {
            GenerateDecoLayers(terrainData, terrainInfo);
        }

        float tx = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.y;
        float ty = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.z;
        float tz = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.x;
        terrainData.size = new Vector3(tx, ty, tz) * ueToUnityUnitScale * L2TerrainGeneratorTool.MAP_SCALE;

        Debug.Log("TerrainData Size:" + terrainData.size);

        var uxHalfTerrainWidthAdjustment = (float)tx * 0.5f;
        var uyHalfTerrainWidthAdjustment = (float)ty * 0.5F;
        var uzHalfTerrainWidthAdjustment = (float)tz * 0.5F;

        // Terrain is shifted by one sector size to accomodate the terrain seam.
        var unityPos = new Vector3(
            terrainInfo.location.y - uxHalfTerrainWidthAdjustment - terrainInfo.terrainScale.y,
            terrainInfo.location.z - uyHalfTerrainWidthAdjustment,
            terrainInfo.location.x - uzHalfTerrainWidthAdjustment - terrainInfo.terrainScale.x
        ) * ueToUnityUnitScale * L2TerrainGeneratorTool.MAP_SCALE * worldPositionOffset;

        terrain.transform.position = unityPos;
        terrain.transform.name = terrainInfo.mapName;

        return terrain;
    }


    private void GenerateHeightmaps(TerrainData terrainData, L2TerrainInfo terrainInfo) {
        byte[] terrainMap = File.ReadAllBytes(terrainInfo.terrainMapPath);

        // Calculate the resolution based on the file size
        int resolution = (int)Mathf.Sqrt(terrainMap.Length / 2); // each height is 2 bytes (16 bits)

        terrainData.heightmapResolution = resolution + 1; // Set the resolution of the heightmap

        // Create a new array for the heightmap
        float[,] heights = new float[resolution + 1, resolution + 1];

        // Read the heights from the file
        using (BinaryReader reader = new BinaryReader(new MemoryStream(terrainMap))) {
            reader.ReadBytes(54);

            for (int i = resolution - 1; i >= 0; i--)
                for (int j = 0; j < resolution; j++) {
                    // Unity uses a value between 0 and 1 for the heightmap data
                    // ushort.MaxValue is 65535
                    heights[j + 1, i + 1] = reader.ReadUInt16() / (float)ushort.MaxValue;
                }
        }

        //Filling out the terrain seam.
        for (int i = 0; i < resolution + 1; i++) {
            heights[0, i] = heights[1, i];
        }
        for (int i = 0; i < resolution + 1; i++) {
            heights[i, 0] = heights[i, 1];
        }

        terrainData.heightmapResolution = resolution;
        terrainData.SetHeights(0, 0, heights);

    }

    public void GenerateUVLayers(string mapID, TerrainData terrainData, L2TerrainInfo terrainInfo) {
        // Create terrain layers
        TerrainLayer[] terrainLayers = new TerrainLayer[terrainInfo.uvLayers.Count];
        terrainData.terrainLayers = new TerrainLayer[terrainInfo.uvLayers.Count];

        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.diffuseTexture = terrainInfo.uvLayers[i].texture;
            terrainLayer.metallic = 0;
            terrainLayer.specular = Color.black;
            terrainLayer.smoothness = 0;
            //terrainLayer.smoothnessSource = TerrainLayerSmoothnessSource.Constant;
            terrainLayer.tileOffset = Vector2.zero;
            terrainLayer.tileSize = new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale) * L2TerrainGeneratorTool.MAP_SCALE * L2TerrainGeneratorTool.UV_TILE_SIZE;

            string savePath = Path.Combine("Assets", "Resources", "Data", "Maps", mapID, "TerrainData", mapID + "_layer_" + i + "_" + terrainInfo.uvLayers[i].texture.name + ".asset");
            AssetDatabase.CreateAsset(terrainLayer, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            terrainLayers[i] = AssetDatabase.LoadAssetAtPath<TerrainLayer>(savePath);
        }

        // Set terrain layers to terrain data
        terrainData.terrainLayers = terrainLayers;

        // Flip vertically
        Texture2D[] flippedAlphaMaps = new Texture2D[terrainInfo.uvLayers.Count];
        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            if (terrainInfo.uvLayers[i].alphaMap != null) {
                flippedAlphaMaps[i] = TextureUtils.FlipTextureVertically(terrainInfo.uvLayers[i].alphaMap);
            }
        }

        float uvMultiplier = 256f / 257f;

        // Now you can set up your splatmap using your masks
        float[,,] map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainInfo.uvLayers.Count];
        for (int y = 0; y < terrainData.alphamapHeight; y++) {
            for (int x = 0; x < terrainData.alphamapWidth; x++) {

                // Initialize all weights to zero
                for (int i = 0; i < terrainInfo.uvLayers.Count; i++)
                    map[x, y, i] = 0;

                float remainingWeight = 1; // keep track of the remaining weight available

                for (int i = terrainInfo.uvLayers.Count - 1; i >= 0; i--) {
                    float u = (x) / (float)(terrainData.alphamapWidth);
                    float v = (y) / (float)(terrainData.alphamapHeight);

                    float weight = 0;

                    if (flippedAlphaMaps[i] != null) {
                        float maskValue = flippedAlphaMaps[i].GetPixelBilinear(u * uvMultiplier, v * uvMultiplier).grayscale;

                        // Calculate the weight for this layer, ensuring that it doesn't exceed the remaining available weight
                        weight = Mathf.Min(maskValue, remainingWeight);
                    }

                    map[x, y, i] = weight;

                    // Subtract the weight assigned to this layer from the remaining available weight
                    remainingWeight -= weight;
                }
            }

            terrainData.SetAlphamaps(0, 0, map);
        }
    }

    public void GenerateDecoLayers(TerrainData terrainData, L2TerrainInfo terrainInfo) {
        // Flip vertically
        Texture2D[] flippedAlphaMaps = new Texture2D[terrainInfo.decoLayers.Count];
        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            if (terrainInfo.decoLayers[i].densityMap != null) {
                flippedAlphaMaps[i] = TextureUtils.FlipTextureVertically(terrainInfo.decoLayers[i].densityMap);
            }
        }

        DetailPrototype[] detailPrototypes = new DetailPrototype[terrainInfo.decoLayers.Count];
        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            detailPrototypes[i] = new DetailPrototype();
            detailPrototypes[i].prototype = terrainInfo.decoLayers[i].staticMesh;
            detailPrototypes[i].renderMode = DetailRenderMode.VertexLit;
            detailPrototypes[i].usePrototypeMesh = true;
            detailPrototypes[i].useInstancing = true;
            detailPrototypes[i].dryColor = Color.white;
            detailPrototypes[i].healthyColor = Color.white;
            detailPrototypes[i].minHeight = terrainInfo.decoLayers[i].minHeight * ueToUnityUnitScale;
            detailPrototypes[i].maxHeight = terrainInfo.decoLayers[i].maxHeight * ueToUnityUnitScale;
            detailPrototypes[i].minWidth = terrainInfo.decoLayers[i].minWidth * ueToUnityUnitScale;
            detailPrototypes[i].maxWidth = terrainInfo.decoLayers[i].maxWidth * ueToUnityUnitScale;
        }

        terrainData.detailPrototypes = detailPrototypes;

        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            Texture2D densityTexture = flippedAlphaMaps[i];

            var detailHeight = densityTexture.height;
            var detailWidth = densityTexture.width;

            int[,] detailLayer = new int[detailHeight, detailWidth];

            // Convert the density texture to a 2D array of density values
            Color32[] pixels = densityTexture.GetPixels32();

            for (int y = 0; y < detailHeight; y++) {
                for (int x = 0; x < detailWidth; x++) {

                    // Extract the density value from the corresponding pixel
                    int density = pixels[y * detailWidth + x].r;

                    // Set the density value for the detail layer
                    detailLayer[x, y] = density;
                }
            }

            // Assign the detail layer to the terrain data
            terrainData.SetDetailLayer(0, 0, i, detailLayer);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void GenerateStaticMeshes(L2TerrainInfo terrainInfo) {
        GameObject staticMeshesGo = new GameObject("StaticMeshes");

        foreach (var staticMesh in terrainInfo.staticMeshes) {
            L2MapStaticMeshBuilder.BuildSingleStaticMesh(staticMesh, staticMeshesGo);
        }
    }

    public void StitchTerrainSeams(Dictionary<string, Terrain> mapTerrains) {
        string[] keys = new string[mapTerrains.Keys.Count];
        mapTerrains.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; ++i) {
            string mapID = keys[i];

            Terrain targetTerrain = mapTerrains[mapID];

            string[] zxMapID = mapID.Split('_');

            string zNeighbourMapID = (int.Parse(zxMapID[0]) - 1).ToString() + "_" + zxMapID[1].ToString();
            string xNeighbourMapID = zxMapID[0].ToString() + "_" + (int.Parse(zxMapID[1]) - 1).ToString();


            if (mapTerrains.ContainsKey(zNeighbourMapID)) {
                Terrain neighbourTerrain = mapTerrains[zNeighbourMapID];
                var res = neighbourTerrain.terrainData.heightmapResolution;
                float[,] neighbourHeights = neighbourTerrain.terrainData.GetHeights(0, res - 1, res, 1);
                float verticalDisplacement = neighbourTerrain.transform.position.y - targetTerrain.transform.position.y;
                AdjustHeightsWithVerticalOffset(neighbourHeights, verticalDisplacement, neighbourTerrain.terrainData.heightmapScale.y);
                targetTerrain.terrainData.SetHeights(0, 0, neighbourHeights);
            }

            if (mapTerrains.ContainsKey(xNeighbourMapID)) {
                Terrain neighbourTerrain = mapTerrains[xNeighbourMapID];
                var res = neighbourTerrain.terrainData.heightmapResolution;
                float[,] neighbourHeights = neighbourTerrain.terrainData.GetHeights(res - 1, 0, 1, res);

                float verticalDisplacement = neighbourTerrain.transform.position.y - targetTerrain.transform.position.y;
                AdjustHeightsWithVerticalOffset(neighbourHeights, verticalDisplacement, neighbourTerrain.terrainData.heightmapScale.y);
                targetTerrain.terrainData.SetHeights(0, 0, neighbourHeights);
            }
        }
    }

    private void AdjustHeightsWithVerticalOffset(float[,] neighbourHeights, float verticalDisplacement, float neighbourHeightmapScale) {
        float offsetRatio = verticalDisplacement / neighbourHeightmapScale;
        for (int i = 0; i < neighbourHeights.GetLength(0); i++) {
            for (int j = 0; j < neighbourHeights.GetLength(1); j++) {
                neighbourHeights[i, j] = neighbourHeights[i, j] + offsetRatio;
            }
        }
    }
}
#endif