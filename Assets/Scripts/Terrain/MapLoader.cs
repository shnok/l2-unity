using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapLoader : MonoBehaviour { 

    public static int TEXTURE_SIZE;
    public static int ALPHAMAP_SIZE;
    public static float UV_TILE_SIZE;
    public static float MAP_SCALE;
    public static bool GENERATE_LAYERS;
    public static bool GENERATE_HEIGHTMAPS;

    public int TextureSize = 256;
    public int AlphaMapSize = 1024;
    public float UVTileSize = 5f;
    public float MapScale = 1f;

    public bool generateMap = false;
    public bool generateLayers = false;
    public bool generateHeightmaps = false;

    public List<L2TerrainInfo> terrainInfos;

    void Start()
    {
        InitializeVariables();

        if(generateMap) {
            ClearGeneratedAssets();
            MapGenerator generator = new MapGenerator();
            L2TerrainInfoParser l2TerrainInfoParser = new L2TerrainInfoParser();

            string[] maps = new string[] { "17_25", "16_25", "17_24", "16_24" };

            for(int i = 0; i < maps.Length; i++) {
                L2TerrainInfo terrainInfo = l2TerrainInfoParser.GetL2TerrainInfo(maps[i]);

                terrainInfos.Add(terrainInfo);

                Terrain terrain = generator.InstantiateTerrain(terrainInfo, maps[i]);

                string prefabPath = Path.Combine("Assets/Prefab/", maps[i] + ".prefab");

                // Create a prefab from the selected GameObject
                PrefabUtility.SaveAsPrefabAsset(terrain.gameObject, prefabPath);
            }

        }
    }

    private void InitializeVariables() {
        TEXTURE_SIZE = TextureSize;
        ALPHAMAP_SIZE = AlphaMapSize;
        UV_TILE_SIZE = UVTileSize;
        MAP_SCALE = MapScale;
        GENERATE_HEIGHTMAPS = generateHeightmaps;
        GENERATE_LAYERS = generateLayers;
    }

    [MenuItem("Custom/Clear Generated Assets")]
    public static void ClearGeneratedAssets() {
        string folderPath = "Assets/TerrainGen";
        if(AssetDatabase.IsValidFolder(folderPath)) {
            AssetDatabase.DeleteAsset(folderPath);
            AssetDatabase.Refresh();
        } else {
            Debug.LogWarning("Folder does not exist: " + folderPath);
        }
    }
}
