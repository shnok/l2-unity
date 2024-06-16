#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainConverter : MonoBehaviour {
    public static string mapToConvert;
    public static int meshSubdivisions = 128;
    public static float uvScale = 150;
    private static bool saveAssets = true;
    private static bool optimizeMesh = true;
    private static bool convertTerrain = true;
    private static bool convertDecoLayer = true;
    private static bool createMapSafenet = true;

    private static TerrainToMesh terrainToMesh;
    private static Texture2DArrayGenerator texture2DArrayGenerator;
    private static Terrain terrainToConvert;


    [MenuItem("Shnok/6. [Terrain] Convert terrain to mesh")]
    static void ConvertTerrainMenu() {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);
            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);

            mapToConvert = mapName;

            ConvertTerrainToMesh(terrainInfo);
        }
    }

    [MenuItem("Shnok/7. [Terrain] Generate deco layer mesh")]
    static void DecoLayers() {
        string title = "Select terrain t3d";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            string mapName = Path.GetFileNameWithoutExtension(fileToProcess);
            L2TerrainInfo terrainInfo = L2T3DInfoParser.LoadMetadata(mapName);

            mapToConvert = mapName;

            optimizeMesh = false;
            convertTerrain = false;
            convertDecoLayer = false;
            createMapSafenet = false;
            convertDecoLayer = true;

            ConvertTerrainToMesh(terrainInfo);
        }
    }

    static void ConvertTerrainToMesh(L2TerrainInfo terrainInfo) {
        terrainToMesh = new TerrainToMesh();
        texture2DArrayGenerator = new Texture2DArrayGenerator();

        string saveFolder = Path.Combine("Assets", "Resources", "Data", "Maps", mapToConvert);
        if(saveAssets) {
            if(!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);
            }
        }

        string mapFolder = Path.Combine("Assets", "Resources", "Data", "Maps", mapToConvert, "TerrainData");
        GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapToConvert + ".prefab"));
        if(map != null) {
            map = Instantiate(map);
            terrainToConvert = map.GetComponent<Terrain>();
        } else {
            Debug.LogWarning("Could not find map " + mapToConvert + " prefab.");
            return;
        }

        if(convertTerrain) {
            Material terrainMat = GenerateMaterial(saveFolder, terrainInfo);

            GameObject destObject = GenerateMesh(terrainToConvert, terrainMat, true);

            if(saveAssets) {
                //SaveMesh(saveFolder, destObject, false);
                SaveMaterial(saveFolder, terrainMat);
                SaveTerrain(saveFolder, destObject);
            }
        }

        if(createMapSafenet) {
            GameObject destObject = GenerateMesh(terrainToConvert, null, false);
            SaveMesh(saveFolder, destObject, true);
        }

        // Generate decolayer
        if (convertDecoLayer) {
            List<L2DecoLayer> decoLayers = terrainInfo.decoLayers;
            GameObject decoLayer = DecoToMesh.ConvertDecoLayers(decoLayers, terrainToConvert);

            if (saveAssets) {
                string decoLayerPrefabPath = Path.Combine(saveFolder, mapToConvert + "_DecoLayer.prefab");
                PrefabUtility.SaveAsPrefabAsset(decoLayer, decoLayerPrefabPath);
            }
        }

        // Disable initial terrain
        terrainToConvert.gameObject.SetActive(false);
    }

    private static GameObject GenerateMesh(Terrain map, Material terrainMat, bool generateHeightmap) {
        // Generate mesh
        GameObject destObject = new GameObject(terrainToConvert.gameObject.name);
        Vector3 initialPosition = map.transform.position;
        map.transform.position = Vector3.zero;
        terrainToMesh.meshSubdivisions = meshSubdivisions;
        terrainToMesh.terrain = terrainToConvert;
        terrainToMesh.dest = destObject;
        terrainToMesh.BuildMeshBase();
        if(generateHeightmap) {
            terrainToMesh.ConvertTerrain(optimizeMesh);
        }
        map.transform.position = initialPosition;

        // Move terrain to original position
        destObject.transform.position = initialPosition;
        destObject.isStatic = true;


        // Apply material to terrain
        if(terrainMat != null) {
            destObject.GetComponent<Renderer>().material = terrainMat;
        }

        return destObject;
    }

    private static void SaveMesh(string saveFolder, GameObject destObject, bool safenet) {
        Mesh convertedTerrainMesh = destObject.GetComponent<MeshFilter>().mesh;
        string filename = mapToConvert + "_Mesh.asset";
        if(safenet) {
            filename = "Safenet_Mesh.asset";
        }
        string saveMeshPath = Path.Combine(filename);
        MeshSaverEditor.SaveMeshToPath(convertedTerrainMesh, saveMeshPath, false, true);
    }

    private static Material GenerateMaterial(string saveFolder, L2TerrainInfo terrainInfo) {
        // Create material
        Material terrainMat = new Material(Shader.Find("Shader Graphs/TerrainAlphamapDouble"));

        // Generate alphamap
        Texture2D[] alphaMaps = new Texture2D[terrainInfo.uvLayers.Count - 1];
        Texture2D[] layers = new Texture2D[terrainInfo.uvLayers.Count - 1];

        for (int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            //if(i > 10) {
            //    Debug.LogWarning("Too many layers (" + terrainInfo.uvLayers.Count + ") for the material.");
            //}

            if (i > 0) {
                alphaMaps[i - 1] = terrainInfo.uvLayers[i].alphaMap;
                layers[i - 1] = terrainInfo.uvLayers[i].texture;
                terrainMat.SetFloat("_Layer_" + i + "_Enabled", 1.0f);
            } else {
                terrainMat.SetTexture("_Layer_" + i, terrainInfo.uvLayers[i].texture);
            }

            //terrainMat.SetTexture("_Layer_" + i, terrainInfo.uvLayers[i].texture);
            //terrainMat.SetTextureScale("_Layer_" + i, new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale));
            terrainMat.SetVector("_Layer_" + i + "_UV", new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale));
        }

        // Build Texture2D array for textures
        if (terrainInfo.uvLayers.Count - 1 > 0) {
            texture2DArrayGenerator.sourceTextures = layers;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Layer.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            terrainMat.SetTexture("_Layers", alphamaps);
        }

        // Build Texture2D array for alphamaps
        if (alphaMaps.Length > 0) {
            texture2DArrayGenerator.sourceTextures = alphaMaps;
            string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Alphamap.asset");
            Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);
            terrainMat.SetTexture("_SplatMaps", alphamaps);
        }

        terrainMat.SetVector("_Global_UV_Size", new Vector2(uvScale, uvScale));

        return terrainMat;
    }

    private static void SaveMaterial(string saveFolder, Material terrainMat) {
        // Save material
        string materialPath = Path.Combine(saveFolder, mapToConvert + "_Mat.mat");

        // Save the material as an asset
        AssetDatabase.CreateAsset(terrainMat, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SaveTerrain(string saveFolder, GameObject destObject) {
        // Save terrain Prefab
        string prefabPath = Path.Combine(saveFolder, mapToConvert + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(destObject, prefabPath);
    }
}
#endif