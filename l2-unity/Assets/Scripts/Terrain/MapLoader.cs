using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapLoader : MonoBehaviour {
    //TODO: Refactor
    public static int UV_TEXTURE_SIZE = 256;
    public static int UV_LAYER_ALPHAMAP_SIZE = 1024;
    public static int DECO_LAYER_ALPHAMAP_SIZE = 512;
    public static float UV_TILE_SIZE = 5f;
    public static float MAP_SCALE = 1f;

    public int uvTextureSize = 256;
    public int uvLayerAlphaMapSize = 1024;
    public int decoLayerAlphaMapSize = 512;
    public float uvTileSize = 5f;
    public float mapScale = 1f;

    //public bool generateMap = false;

    //public static List<L2TerrainInfo> terrainInfos;
    //public List<L2StaticMeshActor> meshActors;
    //public static List<Terrain> terrains;
    //public static Dictionary<string, Terrain> terrainsDict;
    //public List<MapGenerationData> maps;

    //private void InitializeVariables() {
    //    UV_TEXTURE_SIZE = uvTextureSize;
    //    UV_LAYER_ALPHAMAP_SIZE = uvLayerAlphaMapSize;
    //    DECO_LAYER_ALPHAMAP_SIZE = decoLayerAlphaMapSize;
    //    UV_TILE_SIZE = uvTileSize;
    //    MAP_SCALE = mapScale;
    //}

    //void Start() {
    //    InitializeVariables();

    //    if (generateMap) {
    //        if (maps == null) {
    //            maps = new List<MapGenerationData>();
    //        }

    //        GenerateMap(maps);
    //    }
    //}

    //[MenuItem("Shnok/[Terrain] Generate")]
    //static void SetupMaterials() {
    //    List<MapGenerationData> mapsToGenerate = new List<MapGenerationData>();
    //    MapGenerationData data = new MapGenerationData();
    //    data.mapName = "17_25";
    //    mapsToGenerate.Add(data);
    //    data = new MapGenerationData();
    //    data.mapName = "16_25";
    //    mapsToGenerate.Add(data);
    //    data = new MapGenerationData();
    //    data.mapName = "17_24";
    //    mapsToGenerate.Add(data);
    //    data = new MapGenerationData();
    //    data.mapName = "16_24";
    //    mapsToGenerate.Add(data);
    //    GenerateMap(mapsToGenerate);
    //}

    //private static void GenerateMap(List<MapGenerationData> mapsToGenerate) {
    //    L2StaticMeshActorImporter meshActorParser = new L2StaticMeshActorImporter();
    //    L2TerrainGenerator generator = new L2TerrainGenerator();
    //    terrainInfos = new List<L2TerrainInfo>();
    //    terrains = new List<Terrain>();
    //    terrainsDict = new Dictionary<string, Terrain>();

    //    for (int i = 0; i < mapsToGenerate.Count; i++) {

    //        if (!mapsToGenerate[i].enabled) {
    //            continue;
    //        }

    //        if (mapsToGenerate[i].generationMode == GenerationMode.Generate) {
    //            ClearGeneratedAssets(mapsToGenerate[i].mapName);

    //            L2TerrainInfo terrainInfo = L2TerrainInfoParser.GetL2TerrainInfo(mapsToGenerate[i].mapName);
    //            terrainInfos.Add(terrainInfo);

    //            L2StaticMeshActor staticMeshActor = null;
    //            if (mapsToGenerate[i].generateStaticMeshes) {
    //                staticMeshActor = meshActorParser.GetL2StaticMeshActor(mapsToGenerate[i].mapName);
    //            }

    //            Terrain terrain = generator.InstantiateTerrain(mapsToGenerate[i], terrainInfo, staticMeshActor);

    //            terrains.Add(terrain);

    //            terrainsDict.Add(mapsToGenerate[i].mapName, terrain);
    //        } else {
    //            //TODO: UPDATE TO SCENE LOAD
    //            string mapFolder = Path.Combine("Assets", "Data", "Maps", mapsToGenerate[i].mapName);
    //            GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapsToGenerate[i].mapName + ".prefab"));
    //            if (map != null) {
    //                map = GameObject.Instantiate(map);
    //            } else {
    //                Debug.LogWarning("Could not find map " + mapsToGenerate[i].mapName + " prefab.");
    //            }
    //            if (mapsToGenerate[i].generateStaticMeshes) {
    //                GameObject staticmeshes = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, "StaticMeshes.prefab"));
    //                if (staticmeshes != null) {
    //                    staticmeshes = GameObject.Instantiate(staticmeshes);
    //                    staticmeshes.transform.parent = map.transform;
    //                    staticmeshes.transform.position = Vector3.zero;
    //                } else {
    //                    Debug.LogWarning("Could not find staticmeshes for map " + mapsToGenerate[i].mapName + ".");
    //                }
    //            }
    //            if (mapsToGenerate[i].generateBrushes) {
    //                GameObject brushes = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, "Brushes.prefab"));
    //                if (brushes != null) {
    //                    brushes = GameObject.Instantiate(brushes);
    //                    brushes.transform.parent = map.transform;
    //                    brushes.transform.position = Vector3.zero;
    //                } else {
    //                    Debug.LogWarning("Could not find brushes for map " + mapsToGenerate[i].mapName + ".");
    //                }
    //            }
    //        }
    //    }

    //    generator.StitchTerrainSeams(terrainsDict);
    //    generator.StitchTerrainSeams(terrainsDict);
    //}

    //public static void ClearGeneratedAssets(string mapName) {
    //    string folderPath = Path.Combine("Assets", "Data", "Maps", mapName, "TerrainData");
    //    if (AssetDatabase.IsValidFolder(folderPath)) {
    //        AssetDatabase.DeleteAsset(folderPath);
    //        AssetDatabase.Refresh();
    //    } else {
    //        Debug.LogWarning("Folder does not exist: " + folderPath);
    //    }
    //}
}
