using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataContainer {
    public Brush[] data;
}

public class L2BrushImporter
{
    public static Brush[] ParseBrushFile(string path) {

        try {
            string json = File.ReadAllText(path);
            DataContainer brushes = JsonUtility.FromJson<DataContainer>(json);
            Debug.Log(brushes.data.Length);
            return brushes.data;

        } catch(IOException e) {
            Debug.LogError("An error occurred: " + e.Message);
        }

        return null;
    }
}
