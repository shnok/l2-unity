using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public class Geodata : MonoBehaviour
{
    public float nodeSize = 0.5f;
    public List<string> mapsToLoad;
    public Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
    public int gizmosLimit = 10000;
    public bool drawGizmos = true;

    // Start is called before the first frame update
    void Start() {
        foreach(var mapId in mapsToLoad) {
            LoadMapGeodata(mapId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMapGeodata(string mapId) {
        try {
            string geodataFilePath = Path.Combine("Assets/Data/Maps/", mapId, mapId + ".geodata");
            string innerFile = "geodata";
            using(ZipArchive archive = ZipFile.OpenRead(geodataFilePath)) {
                ZipArchiveEntry entry = archive.GetEntry(innerFile);

                if(entry != null) {
                    using(Stream stream = entry.Open()) {
                        // Read the entire content into memory
                        byte[] data;
                        using(MemoryStream memoryStream = new MemoryStream()) {
                            stream.CopyTo(memoryStream);
                            data = memoryStream.ToArray();
                        }

                        // Create a BinaryReader from the memory stream

                        string mapFolder = Path.Combine("Assets", "Data", "Maps", mapId);
                        GameObject map = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(mapFolder, mapId + ".prefab"));
                        Vector3 origin = VectorUtils.floorToNearest(map.transform.position, nodeSize);

                        Debug.Log(origin);

                        int count = 0;
                        using(BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
                            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short z = reader.ReadInt16();
                                count++;

                                if(count >= gizmosLimit && drawGizmos) {
                                    return;
                                }

                                Vector3 nodePos = new Vector3(x, y, z);
                                nodePos = FromNodeToWorldPos(nodePos, nodeSize, origin);

                                Node n = new Node(nodePos, nodeSize);
                                n.walkable = true;
                                nodes.Add(nodePos, n);
                            }
                        }

                        Debug.Log("Imported " + count + " nodes");
                    }
                } else {
                    Debug.LogError("File not found in the ZIP archive.");
                }
            }
        } catch(IOException ex) {
            Debug.LogError("Error reading ZIP file: " + ex.Message);
        }
    }

    private Vector3 FromNodeToWorldPos(Vector3 nodePos, float nodeSize, Vector3 origin) {
        Vector3 worldPos = nodePos * nodeSize + origin;
        worldPos = new Vector3(
            VectorUtils.floorToNearest(worldPos.x, nodeSize),
            VectorUtils.floorToNearest(worldPos.y, nodeSize),
            VectorUtils.floorToNearest(worldPos.z, nodeSize));
        return worldPos;
    }

    void OnDrawGizmos() {
        if(!drawGizmos)
            return;

        if(nodes.Count > 0) {
            foreach(KeyValuePair<Vector3, Node> n in nodes) {
                Vector3 cubeSize = new Vector3(nodeSize - nodeSize / 10f, 0.1f, nodeSize - nodeSize / 10f);
                if(n.Value.walkable) {
                    Gizmos.color = Color.green;
                } else {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawCube(n.Value.center, cubeSize);
            }
        }

    }
}
