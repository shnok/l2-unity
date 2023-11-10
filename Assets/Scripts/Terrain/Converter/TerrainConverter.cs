using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainConverter : MonoBehaviour
{
    public string mapToConvert;
    public int meshSubdivisions = 256;
    public float uvScale = 200;
    private TerrainToMesh terrainToMesh;
    private Texture2DArrayGenerator texture2DArrayGenerator;
    private Terrain terrainToConvert;

    // Start is called before the first frame update
    private void Awake() {
        if(!TryGetComponent(out terrainToMesh)) {
            terrainToMesh = gameObject.AddComponent<TerrainToMesh>();
        }
        if(!TryGetComponent(out texture2DArrayGenerator)) {
            texture2DArrayGenerator = gameObject.AddComponent<Texture2DArrayGenerator>();
        }
    }

    void Start() {
        string mapFolder = Path.Combine("Assets", "Data", "Maps", mapToConvert);
        GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapToConvert + ".prefab"));
        if(map != null) {
            map = GameObject.Instantiate(map);
            terrainToConvert = map.GetComponent<Terrain>();
        } else {
            Debug.LogWarning("Could not find map " + mapToConvert + " prefab.");
            return;
        }

        L2TerrainInfo terrainInfo = L2TerrainInfoParser.GetL2TerrainInfo(mapToConvert);

        // Generate mesh
        GameObject destObject = new GameObject(terrainToConvert.gameObject.name);
        Vector3 initialPosition = map.transform.position;
        map.transform.position = Vector3.zero;
        terrainToMesh.meshSubdivisions = meshSubdivisions;
        terrainToMesh.terrain = terrainToConvert;
        terrainToMesh.dest = destObject;
        terrainToMesh.BuildMeshBase();
        terrainToMesh.ConvertTerrain();
        map.transform.position = initialPosition;

        // Move terrain to original position
        destObject.transform.position = initialPosition;

        // Save mesh data
        string saveFolder = Path.Combine("Assets", "Data", "Maps", mapToConvert, "TerrainMesh");
        if(!Directory.Exists(saveFolder)) {
            Directory.CreateDirectory(saveFolder);
        }

        Mesh convertedTerrainMesh = destObject.GetComponent<MeshFilter>().mesh;
        string saveMeshPath = Path.Combine(saveFolder, mapToConvert + "_Mesh.asset");
        MeshSaverEditor.SaveMeshToPath(convertedTerrainMesh, saveMeshPath, false, true);

        // Create material
        Material terrainMat = new Material(Shader.Find("Shader Graphs/TerrainAlphamap"));

        // Generate alphamap
        Texture2D[] alphaMaps = new Texture2D[terrainInfo.uvLayers.Count - 1];
        for(int i = 0; i < terrainInfo.uvLayers.Count; i++) {
            if(i > 9) {
                Debug.LogError("Too many layers (" + terrainInfo.uvLayers.Count + ") for the material.");
                break;
            }

            if(i > 0) {
                alphaMaps[i - 1] = terrainInfo.uvLayers[i].alphaMap;
                terrainMat.SetFloat("_Layer_" + i + "_Enabled", 1.0f);
            }

            terrainMat.SetTexture("_Layer_" + i, terrainInfo.uvLayers[i].texture);
            terrainMat.SetTextureScale("_Layer_" + i, new Vector2(terrainInfo.uvLayers[i].uScale, terrainInfo.uvLayers[i].vScale));      
        }


        texture2DArrayGenerator.sourceTextures = alphaMaps;
        string saveAlphamapPath = Path.Combine(saveFolder, mapToConvert + "_Alphamap.asset");
        Texture2DArray alphamaps = texture2DArrayGenerator.GenerateTexture2DArray(saveAlphamapPath);

        terrainMat.SetTexture("_SplatMaps", alphamaps);
        terrainMat.SetVector("_UVSize", new Vector2(uvScale, uvScale));

        // Save material
        string materialPath = Path.Combine(saveFolder, mapToConvert + "_Mat.mat");

        // Apply material to terrain
        destObject.GetComponent<Renderer>().material = terrainMat;

        // Save the material as an asset
        AssetDatabase.CreateAsset(terrainMat, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Generate decolayer
        List<L2DecoLayer> decoLayers = terrainInfo.decoLayers;
        DecoToMesh.ConvertDecoLayers(decoLayers, destObject);

        destObject.isStatic = true;

        // Save terrain Prefab
        string prefabPath = Path.Combine(saveFolder, mapToConvert + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(destObject, prefabPath);
    }
}
