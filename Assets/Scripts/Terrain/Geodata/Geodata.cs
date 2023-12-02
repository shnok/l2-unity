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
    public LayerMask obstacleMask;
    public Dictionary<string, Vector3> mapsOrigin = new Dictionary<string, Vector3>();
    public Dictionary<Vector3, Node> nodes = new Dictionary<Vector3, Node>();
    public bool loaded = false;
    public bool drawGizmos = true;

    private static Geodata _instance;
    public static Geodata GetInstance() {
        return _instance;
    }

    private void Awake() {
        _instance = this;
    }

    void Start() {
        foreach(var mapId in mapsToLoad) {
            LoadMapGeodata(mapId);
        }
        loaded = true;
    }

    public Node GetNodeAt(string mapId, Vector3 pos) {
        if(mapsOrigin.ContainsKey(mapId)) {
            Vector3 nodePos = FromWorldToNodePos(pos, mapId);
            Node node;
            nodes.TryGetValue(nodePos, out node);

            return node;
        }
        return null;
    }


    public void LoadMapGeodata(string mapId) {
        try {
            string geodataFilePath = Path.Combine("Assets/Resources/Data/Maps/", mapId, mapId + ".geodata");
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
                        string mapFolder = Path.Combine("Data", "Maps", mapId);
                        GameObject map = Resources.Load<GameObject>(Path.Combine(mapFolder, mapId));
                        Vector3 origin = VectorUtils.floorToNearest(map.transform.position, nodeSize);
                        mapsOrigin.Add(mapId, origin);

                        int count = 0;
                        using(BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
                            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short z = reader.ReadInt16();
                                count++;

                                Vector3 scaledPos = new Vector3(x, y, z);

                                Node n = new Node(scaledPos, FromNodeToWorldPos(scaledPos, origin), nodeSize);
                                n.walkable = true;
                                nodes.Add(scaledPos, n);
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

    private Vector3 FromNodeToWorldPos(Vector3 nodePos, Vector3 origin) {
        Vector3 worldPos = nodePos * nodeSize + origin;
        worldPos = new Vector3(
            VectorUtils.floorToNearest(worldPos.x, nodeSize),
            VectorUtils.floorToNearest(worldPos.y, nodeSize),
            VectorUtils.floorToNearest(worldPos.z, nodeSize));
        return worldPos;
    }

    private Vector3 FromWorldToNodePos(Vector3 worldPos, String mapId) {
        Vector3 terrainPos;
        if(!mapsOrigin.TryGetValue(mapId, out terrainPos)) {
            throw new Exception("Terrain not found.");
        }

        Vector3 offsetPos = worldPos - terrainPos;
        Vector3 nodePos = new Vector3(
            Mathf.Floor(offsetPos.x / nodeSize),
            Mathf.Floor(offsetPos.y / nodeSize),
            Mathf.Floor(offsetPos.z / nodeSize));

        return nodePos;
    }

    public Node GetNode(int x, int y, int z) {
        Node node;
        nodes.TryGetValue(new Vector3(x, y, z), out node);
        return node;
    }

    void OnDrawGizmos() {
        if(!drawGizmos || !Application.isPlaying)
            return;

        int count = 0;
        if(nodes.Count > 0) {
            foreach(KeyValuePair<Vector3, Node> n in nodes) {
                if(count >= 250000) {
                    return;
                }

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
