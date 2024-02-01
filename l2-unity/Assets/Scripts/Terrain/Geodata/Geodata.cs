using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public class Geodata : MonoBehaviour
{
    [SerializeField] private float _nodeSize = 0.5f;
    [SerializeField] private List<string> _mapsToLoad;
    [SerializeField] private Dictionary<string, Vector3> _mapsOrigin = new Dictionary<string, Vector3>();
    [SerializeField] private Dictionary<Vector3, Node> _nodes = new Dictionary<Vector3, Node>();
    [SerializeField] private bool _loaded = false;
    [SerializeField] private bool _drawGizmos = true;
    private LayerMask _obstacleMask;
    public float NodeSize { get { return _nodeSize; } }
    public bool Loaded { get { return _loaded; } }

    private static Geodata _instance;
    public static Geodata Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    void Start() {

        foreach(var mapId in _mapsToLoad) {
            LoadMapGeodata(mapId);
        }
        _loaded = true;
    }

    public void SetMask(LayerMask mask) {
        _obstacleMask = mask;
    }

    public LayerMask GetMask() {
        return _obstacleMask;
    }

    public Node GetNodeAt(string mapId, Vector3 pos) {
        if(_mapsOrigin.ContainsKey(mapId)) {
            Vector3 nodePos = FromWorldToNodePos(pos, mapId);
            //Debug.Log(pos + " , " + nodePos);
            Node node;
            _nodes.TryGetValue(nodePos, out node);

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
                        Vector3 origin = VectorUtils.floorToNearest(map.transform.position, _nodeSize);
                        _mapsOrigin.Add(mapId, origin);

                        int count = 0;
                        using(BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
                            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short z = reader.ReadInt16();
                                count++;

                                Vector3 scaledPos = new Vector3(x, y, z);

                                Node n = new Node(scaledPos, FromNodeToWorldPos(scaledPos, origin), _nodeSize);
                                n.walkable = true;
                                _nodes.Add(scaledPos, n);
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
        Vector3 worldPos = nodePos * _nodeSize + origin;
        worldPos = new Vector3(
            VectorUtils.floorToNearest(worldPos.x, _nodeSize),
            VectorUtils.floorToNearest(worldPos.y, _nodeSize),
            VectorUtils.floorToNearest(worldPos.z, _nodeSize));
        return worldPos;
    }

    private Vector3 FromWorldToNodePos(Vector3 worldPos, String mapId) {
        Vector3 terrainPos;
        if(!_mapsOrigin.TryGetValue(mapId, out terrainPos)) {
            throw new Exception("Terrain not found.");
        }

        Vector3 offsetPos = worldPos - terrainPos;
        Vector3 nodePos = new Vector3(
            Mathf.Floor(offsetPos.x / _nodeSize),
            Mathf.Floor(offsetPos.y / _nodeSize),
            Mathf.Floor(offsetPos.z / _nodeSize));

        return nodePos;
    }

    public Node GetNode(int x, int y, int z) {
        Node node;
        _nodes.TryGetValue(new Vector3(x, y, z), out node);
        return node;
    }

    void OnDrawGizmos() {
        if(!_drawGizmos || !Application.isPlaying)
            return;

        int count = 0;
        if(_nodes.Count > 0) {
            foreach(KeyValuePair<Vector3, Node> n in _nodes) {
                if(count >= 250000) {
                    return;
                }

                Vector3 cubeSize = new Vector3(_nodeSize - _nodeSize / 10f, 0.1f, _nodeSize - _nodeSize / 10f);
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
