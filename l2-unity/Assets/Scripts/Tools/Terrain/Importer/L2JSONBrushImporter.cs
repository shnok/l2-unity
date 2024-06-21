#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DataContainer {
    public Brush[] data;
}

public class L2JSONBrushImporter : AssetImporter {

    [MenuItem("Shnok/[Brush] Import Textures")]
    static void ImportBrushTextures() {
        string title = "Select Brush list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string dataFolder = @"D:\Stock\Projects\L2-Unity\Tools\umodel_win32\export";
        bool overwrite = false;

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            List<string> files = ProcessDataFile(dataFolder, fileToProcess);
            ImportFiles(dataFolder, files, overwrite);
        }
    }

    private static List<string> ProcessDataFile(string dataFolder, string fileToProcess) {
        List<string> files = new List<string>();

        Brush[] brushes = ParseBrushFile(fileToProcess);
        foreach(Brush brush in brushes) {
            if(brush.model != null) {
                if(brush.model.poly != null) {
                    for(int i = 0; i < brush.model.poly.polyData.Length; i++) {
                        if(!string.IsNullOrEmpty(brush.model.poly.polyData[i].texture)) {
                            string textureName = brush.model.poly.polyData[i].texture;
                            if(textureName.Contains("Shaders")) {
                                Debug.Log("Skipping shaders " + textureName);
                                continue;
                            }

                            string[] parts = textureName.Split('.');
                            string folder = parts[0];
                            string file = parts[parts.Length - 1];

                            files.Add(Path.Combine(dataFolder, folder, file + ".png"));
                        }
                    }
                }
            }
        }

        files = files.Distinct().ToList();
        Debug.LogWarning("Total files to import: " + files.Count);

        return files;
    }

    public static Brush[] ParseBrushFile(string path) {
        try {
            string json = File.ReadAllText(path);
            DataContainer brushes = JsonUtility.FromJson<DataContainer>(json);
            return brushes.data;

        } catch(IOException e) {
            Debug.LogError("An error occurred: " + e.Message);
        }

        return null;
    }
}
#endif