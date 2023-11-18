using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeodataExporter {
    public static void Export(string filename, Dictionary<Vector3, Node> terrainData) {
        using(BinaryWriter binWriter = new BinaryWriter(File.Open(Path.Combine(Application.dataPath, filename), FileMode.Create))) {
            int writeCount = 0;
            foreach(KeyValuePair<Vector3, Node> entry in terrainData) {
                if(entry.Value.walkable) {
                    binWriter.Write((short)Mathf.Round(entry.Key.x));
                    binWriter.Write((short)Mathf.Round(entry.Key.y));
                    binWriter.Write((short)Mathf.Round(entry.Key.z));
                }
                writeCount++;
            }
            Debug.Log("Exported " + writeCount + " nodes.");
        }      
    }
}