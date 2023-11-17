using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeodataExporter {

    public static int Flatten(int width, int height, int x, int y, int z) {
        return (y * height * 2 * width * 2) + (z * width * 2) + x;
    }

    public static void Export(string filename, Dictionary<Vector3, Node> terrainData, int terrainWidth, int terrainHeight) {
        using(BinaryWriter binWriter = new BinaryWriter(File.Open(filename, FileMode.Create))) {
            int writeCount = 0;
            foreach(KeyValuePair<Vector3, Node> entry in terrainData) {
                Debug.Log(entry.Key + " : " + Flatten(terrainWidth, terrainHeight, (int)entry.Key.x, (int)entry.Key.y, (int)entry.Key.z));
                binWriter.Write(Flatten(terrainWidth, terrainHeight, (int)entry.Key.x, (int)entry.Key.y, (int)entry.Key.z));
                binWriter.Write(entry.Value.walkable ? (byte)1: (byte)0);
                writeCount++;
            }
            Debug.Log(writeCount);
        }      
    }
}
