#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class StaticMeshUtils {

    public static GameObject LoadMeshFromInfo(string info) {
        Debug.Log(info);

        string meshPath = GetMeshPath(info);

        // try checking if prefab exists first
        string prefabPath = Path.Combine(Path.GetDirectoryName(meshPath), "Prefab", Path.GetFileNameWithoutExtension(meshPath) + ".prefab");

        if (File.Exists(prefabPath)) {
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
            
        return AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
    }

    public static string GetMeshPath(string value) {
        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/StaticMeshes", folderTexture[0], folderTexture[1] + ".fbx");
    }

    public static string GetJsonPath(string mapName) {
        return Path.Combine("Assets/Resources/Data/Maps/", mapName, "Meta", "StaticMeshActor.json");
    }

    public static string GetT3DPath(string mapName) {
        return Path.Combine("Assets/Resources/Data/Maps/", mapName, "Meta", mapName + ".t3d");
    }
}
#endif
