using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour { 

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
    public L2TerrainGenerator generator;
    public L2TerrainInfoParser parser;
    public List<Terrain> terrains;
    public Dictionary<string, Terrain> terrainsDict;

    private void InitializeVariables() {
        TEXTURE_SIZE = TextureSize;
        ALPHAMAP_SIZE = AlphaMapSize;
        UV_TILE_SIZE = UVTileSize;
        MAP_SCALE = MapScale;
        GENERATE_HEIGHTMAPS = generateHeightmaps;
        GENERATE_LAYERS = generateLayers;
    }

    void Start()
    {
        InitializeVariables();

        if(generateMap) {
            ClearGeneratedAssets();

            generator = new L2TerrainGenerator();
            parser = new L2TerrainInfoParser();
            terrains = new List<Terrain>();
            terrainsDict = new Dictionary<string, Terrain>();

            string[] maps = new string[] { "17_25", "16_25", "17_24", "16_24" };

            GenerateMap(maps);
            SavePrefabs(maps);
        }
    }

    private void GenerateMap(string[] maps) {
        for(int i = 0; i < maps.Length; i++) {
            L2TerrainInfo terrainInfo = parser.GetL2TerrainInfo(maps[i]);

            terrainInfos.Add(terrainInfo);

            Terrain terrain = generator.InstantiateTerrain(terrainInfo);

            terrains.Add(terrain);

            terrainsDict.Add(maps[i], terrain);
        }

        generator.StitchTerrainSeams(terrainsDict);
        generator.StitchTerrainSeams(terrainsDict);
    }


    private void SavePrefabs(string[] maps) {
        for(int i = 0; i < terrains.Count; i++) {
            string prefabPath = Path.Combine("Assets/Prefab/", maps[i] + ".prefab");

            // Create a prefab from the selected GameObject
            PrefabUtility.SaveAsPrefabAsset(terrains[i].gameObject, prefabPath);
        }
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
