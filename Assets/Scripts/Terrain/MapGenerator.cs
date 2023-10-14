using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour { 

    public static int UV_TEXTURE_SIZE;
    public static int UV_LAYER_ALPHAMAP_SIZE;
    public static float UV_TILE_SIZE;
    public static int DECO_LAYER_ALPHAMAP_SIZE;
    public static float MAP_SCALE;
    public static bool GENERATE_UV_LAYERS;
    public static bool GENERATE_DECO_LAYERS;
    public static bool GENERATE_HEIGHTMAPS;

    public int uvTextureSize = 256;
    public int uvLayerAlphaMapSize = 1024;
    public int decoLayerAlphaMapSize = 512;
    public float uvTileSize = 5f;
    public float mapScale = 1f;

    public bool generateMap = false;
    public bool generateUVLayers = false;
    public bool generateDecoLayers = false;
    public bool generateHeightmaps = false;

    public List<L2TerrainInfo> terrainInfos;
    public L2TerrainGenerator generator;
    public L2TerrainInfoParser parser;
    public List<Terrain> terrains;
    public Dictionary<string, Terrain> terrainsDict;
    public List<MapToGenerate> mapsToGenerate;

    [System.Serializable]
    public struct MapToGenerate {
        public string mapName;
        public bool enabled;
    }


    private void InitializeVariables() {
        UV_TEXTURE_SIZE = uvTextureSize;
        UV_LAYER_ALPHAMAP_SIZE = uvLayerAlphaMapSize;
        DECO_LAYER_ALPHAMAP_SIZE = decoLayerAlphaMapSize;
        UV_TILE_SIZE = uvTileSize;
        MAP_SCALE = mapScale;
        GENERATE_HEIGHTMAPS = generateHeightmaps;
        GENERATE_UV_LAYERS = generateUVLayers;
        GENERATE_DECO_LAYERS = generateDecoLayers;
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
            if(mapsToGenerate == null) {
                mapsToGenerate = new List<MapToGenerate>();
            }

            List<string> maps = new List<string>();
            for(int i = 0; i < mapsToGenerate.Count; i++) {
                if(mapsToGenerate[i].enabled) {
                    maps.Add(mapsToGenerate[i].mapName);
                }
             }


            GenerateMap(maps.ToArray());
            SavePrefabs(maps.ToArray());
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
