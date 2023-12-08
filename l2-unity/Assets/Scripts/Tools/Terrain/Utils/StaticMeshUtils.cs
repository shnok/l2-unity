#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class StaticMeshUtils {

    public static GameObject LoadMeshFromInfo(string info) {
        Debug.Log(info);

        string meshPath = GetMeshPath(info);

        return AssetDatabase.LoadAssetAtPath<GameObject>(meshPath);
    }

    public static string GetMeshPath(string value) {
        string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Data/StaticMeshes", folderTexture[0], folderTexture[1] + ".fbx");
    }

    public static string GetJsonPath(string mapName) {
        return Path.Combine("Assets/Data/Maps/Meta/", mapName, "StaticMeshActor.json");
    }
}
#endif
