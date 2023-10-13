using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapLoader : MonoBehaviour { 

    public static int TEXTURE_SIZE;
    public static int ALPHAMAP_SIZE;
    public static float UV_TILE_SIZE;
    public static float MAP_SCALE;

    public int TextureSize = 256;
    public int AlphaMapSize = 1024;
    public float UVTileSize = 5f;
    public float MapScale = 1f;

    public bool generateMap = false;

    public L2TerrainInfo terrainInfo;

    void Start()
    {
        InitializeVariables();

        if(generateMap) {
            ClearGeneratedAssets();

            L2TerrainInfoParser l2TerrainInfoParser = new L2TerrainInfoParser();
            terrainInfo = l2TerrainInfoParser.GetL2TerrainInfo("17_25");

            MapGenerator generator = new MapGenerator();
            Terrain terrain = generator.InstantiateTerrain(terrainInfo, "17_25");


            string prefabPath = "Assets/Prefab/17_25.prefab";

            // Create a prefab from the selected GameObject
            PrefabUtility.SaveAsPrefabAsset(terrain.gameObject, prefabPath);
        }
    }

    private void InitializeVariables() {
        TEXTURE_SIZE = TextureSize;
        ALPHAMAP_SIZE = AlphaMapSize;
        UV_TILE_SIZE = UVTileSize;
        MAP_SCALE = MapScale;
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
