#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace KWS
{
    public class KW_MoveWaterDataFolderToStreamingAsset : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
#if KWS_DEBUG
            return;
#else
foreach (var importedAsset in importedAssets)
        {
            if (importedAsset.Contains("WaterSystemData"))
            {
                if (!Directory.Exists(Application.streamingAssetsPath)) Directory.CreateDirectory(Application.streamingAssetsPath);

                var newDataPath = Path.Combine(Application.streamingAssetsPath, "WaterSystemData");
                if (Directory.Exists(newDataPath)) return;

                var dataFolder = Directory.GetDirectories(Application.dataPath, "WaterSystemData", SearchOption.AllDirectories);
                new DirectoryInfo(dataFolder[0]).MoveTo(newDataPath);

                Debug.Log("Imported Water data to StreamingAssets folder. " +
                          "This folder include data files for correct shoreline and flowmap rendering. " +
                          "Unity will include this folder to build");
                return;
            }
        }
#endif
        }
    }
}
#endif