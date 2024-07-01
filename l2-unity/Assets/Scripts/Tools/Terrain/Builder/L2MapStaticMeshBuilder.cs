using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class L2MapStaticMeshBuilder : MonoBehaviour
{
    [MenuItem("Shnok/[StaticMeshes] (JSON) Build static meshes")]
    static void BuildSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            L2JSONStaticMeshActorImporter meshActorParser = new L2JSONStaticMeshActorImporter();
            L2StaticMeshActor data = meshActorParser.GetL2StaticMeshActorFromFile(fileToProcess);
            GenerateStaticMeshes(data);
        }
    }

    public static void GenerateStaticMeshes(L2StaticMeshActor staticMeshActor) {
        float ueToUnityUnitScale = 1 / 52.5f;
        Vector3 basePosition = new Vector3(staticMeshActor.y, staticMeshActor.z, staticMeshActor.x) * ueToUnityUnitScale;
        GameObject staticMeshesGo = new GameObject("StaticMeshes");
        GameObject container = new GameObject("StaticMeshes");
        staticMeshesGo.transform.parent = container.transform;

        foreach (var staticMesh in staticMeshActor.staticMeshes) {
            BuildSingleStaticMesh(staticMesh, container);
        }
    }

    public static void BuildSingleStaticMesh(L2StaticMesh staticMesh, GameObject container) {
        Vector3 basePosition = Vector3.zero;
        string meshPath = StaticMeshUtils.GetMeshPath(staticMesh.staticMesh);
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
        if (go != null) {
            Vector3 position = new Vector3(staticMesh.y, staticMesh.z, staticMesh.x) * (1/52.5f) * L2TerrainGeneratorTool.MAP_SCALE;
            Vector3 eulerAngles = go.transform.eulerAngles + VectorUtils.ConvertRotToUnity(staticMesh.eulerAngles);

            float meshDataScaleMultiplier = staticMesh.scale != 0 ? staticMesh.scale : 1f;
            float meshDataScaleX = staticMesh.scaleX != 0 ? staticMesh.scaleX : 1f;
            float meshDataScaleY = staticMesh.scaleY != 0 ? staticMesh.scaleY : 1f;
            float meshDataScaleZ = staticMesh.scaleZ != 0 ? staticMesh.scaleZ : 1f;
            Vector3 meshDataScale = new Vector3(meshDataScaleX, meshDataScaleY, meshDataScaleZ);

            GameObject instantiated = GameObject.Instantiate(go, position + basePosition, Quaternion.Euler(eulerAngles));
            instantiated.name = staticMesh.staticMesh;
            instantiated.transform.localScale = Vector3.Scale(instantiated.transform.localScale, meshDataScale) *
                meshDataScaleMultiplier *
                //ueToUnityUnitScale *
                L2TerrainGeneratorTool.MAP_SCALE;

            instantiated.transform.parent = container.transform;
        } else {
            Debug.LogError("Can't find StaticMesh FBX " + staticMesh.staticMesh + " at path " + meshPath);
        }
    }
}
#endif