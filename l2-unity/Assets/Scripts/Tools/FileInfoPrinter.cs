#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using System.Text;

public class FileInfoPrinter {
    [MenuItem("Assets/FileInfo/Show Asset Ids")]
    static void MenuShowIds() {
        var stringBuilder = new StringBuilder();

        foreach(var obj in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Selection.activeObject))) {
            string guid;
            long file;

            if(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out file)) {
                stringBuilder.AppendFormat("Asset: " + obj.name +
                    "\n  Instance ID: " + obj.GetInstanceID() +
                    "\n  GUID: " + guid +
                    "\n  File ID: " + file);
            }
        }

        Debug.Log(stringBuilder.ToString());
    }
}
#endif
