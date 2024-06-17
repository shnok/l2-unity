using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapLoader : MonoBehaviour {

//    public bool generateMap = true;
//    public static List<L2TerrainInfo> terrainInfos;
//    public List<L2StaticMeshActor> meshActors;
//    public static List<Terrain> terrains;
//    public static Dictionary<string, Terrain> terrainsDict;

//    void Start() {
//        // InitializeVariables();
//    }

//    private static void GenerateMap() {
//        terrainInfos = new List<L2TerrainInfo>();
//        terrains = new List<Terrain>();
//        terrainsDict = new Dictionary<string, Terrain>();

//        for (int i = 0; i < mapsToGenerate.Count; i++) {

//            if (!mapsToGenerate[i].enabled) {
//                continue;
//            }

////TODO: UPDATE TO SCENE LOAD
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
}
