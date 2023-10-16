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

    public int uvTextureSize = 256;
    public int uvLayerAlphaMapSize = 1024;
    public int decoLayerAlphaMapSize = 512;
    public float uvTileSize = 5f;
    public float mapScale = 1f;

    public bool generateMap = false;
    public bool savePrefabs = false;

    public List<L2TerrainInfo> terrainInfos;
    public List<L2StaticMeshActor> meshActors;
    public List<Terrain> terrains;
    public Dictionary<string, Terrain> terrainsDict;
    public List<MapGenerationData> mapsToGenerate;

    private void InitializeVariables() {
        UV_TEXTURE_SIZE = uvTextureSize;
        UV_LAYER_ALPHAMAP_SIZE = uvLayerAlphaMapSize;
        DECO_LAYER_ALPHAMAP_SIZE = decoLayerAlphaMapSize;
        UV_TILE_SIZE = uvTileSize;
        MAP_SCALE = mapScale;
    }

    void Start()
    {
        InitializeVariables();

        if(generateMap) {
            ClearGeneratedAssets();

            if(mapsToGenerate == null) {
                mapsToGenerate = new List<MapGenerationData>();
            }

            GenerateMap(mapsToGenerate);
        }

        if(savePrefabs) {
            SavePrefabs(mapsToGenerate);
        }
    }

    private void GenerateMap(List<MapGenerationData> mapsToGenerate) {
        L2TerrainInfoParser terrainInfoParser = new L2TerrainInfoParser();
        L2StaticMeshActorParser meshActorParser = new L2StaticMeshActorParser();
        L2TerrainGenerator generator = new L2TerrainGenerator();
        terrains = new List<Terrain>();
        terrainsDict = new Dictionary<string, Terrain>();

        for(int i = 0; i < mapsToGenerate.Count; i++) {

            if(!mapsToGenerate[i].enabled)
                continue;

            L2TerrainInfo terrainInfo = terrainInfoParser.GetL2TerrainInfo(mapsToGenerate[i].mapName);
            terrainInfos.Add(terrainInfo);

            L2StaticMeshActor staticMeshActor = null;
            if(mapsToGenerate[i].generateStaticMeshes)
                staticMeshActor = meshActorParser.GetL2StaticMeshActor(mapsToGenerate[i].mapName);

            Terrain terrain = generator.InstantiateTerrain(mapsToGenerate[i], terrainInfo, staticMeshActor);

            terrains.Add(terrain);

            terrainsDict.Add(mapsToGenerate[i].mapName, terrain);
        }

        generator.StitchTerrainSeams(terrainsDict);
        generator.StitchTerrainSeams(terrainsDict);
    }


    private void SavePrefabs(List<MapGenerationData> mapsToGenerate) {
        for(int i = 0; i < terrains.Count; i++) {
            string prefabPath = Path.Combine("Assets/Prefab/", mapsToGenerate[i].mapName + ".prefab");

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
