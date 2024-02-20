using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public class Geodata : MonoBehaviour {
    [SerializeField] private float _nodeSize = 0.5f;
    [SerializeField] private float _mapSize = 624.1524f;
    [SerializeField] private int _maximumLayerCount = 4;
    [SerializeField] private float _maximumElevationError = 5f;
    [SerializeField] private List<string> _mapsToLoad;
    [SerializeField] private Dictionary<string, Vector3> _mapsOrigin = new Dictionary<string, Vector3>();
    [SerializeField] private Dictionary<string, Node[,,]> _geodata;
    [SerializeField] private bool _loaded = false;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private bool _drawMapGizmos = true;
    [SerializeField] private int _drawMapGizmosLimit = 25000;
    [SerializeField] private bool _drawGizmosAroundPlayer = true;
    [SerializeField] private int _drawGizmosAroundPlayerRadius = 10;

    private LayerMask _obstacleMask;
    public LayerMask ObstacleMask { get { return _obstacleMask; } set { _obstacleMask = value; } }

    public float NodeSize { get { return _nodeSize; } }
    public bool Loaded { get { return _loaded; } }

    private static Geodata _instance;
    public static Geodata Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _geodata.Clear();
        _instance = null;
    }

    void Start() {
        _geodata = new Dictionary<string, Node[,,]>();

        foreach(var mapId in _mapsToLoad) {

            Vector3 origin;
            try {
                origin = GeodataImporter.LoadMapOrigin(mapId, _nodeSize);
                _mapsOrigin.Add(mapId, origin);
            } catch(Exception e) {
                Debug.LogError(e.Message);
                continue;
            }

            GeodataImporterFactory.Instance.RequestImportGeodata(mapId, _nodeSize, (byte) _maximumLayerCount, origin, (callback) => {
                if(callback != null) {
                    _geodata[mapId] = callback;
                }

                if(_geodata.Keys.Count == _mapsToLoad.Count) {
                    _loaded = true;
                }
            });
        }
    }


    public string GetCurrentMap(Vector3 location) {
        foreach(var mapName in _mapsOrigin.Keys) {
            if(IsInMapBounds(location, _mapsOrigin[mapName])) {
                return mapName;
            }
        }
        throw new Exception("Outside bounds.");
    }

    private bool IsInMapBounds(Vector3 location, Vector3 mapOrigin) {
        if(location.x < mapOrigin.x) {
            return false;
        }
        if(location.x > mapOrigin.x + _mapSize) {
            return false;
        }
        if(location.z < mapOrigin.z) {
            return false;
        }
        if(location.z > mapOrigin.z + _mapSize) {
            return false;
        }

        return true;
    }

    public Node GetNodeAt(Vector3 nodePos, string mapId) {
        return GetNodeAt(nodePos, mapId, false);
    }

    public Node GetNodeAt(Vector3 nodePos) {
        return GetNodeAt(nodePos, GetCurrentMap(nodePos), false);
    }

    public Node GetNodeAt(Vector3 nodePos, bool exactElevation) {
        return GetNodeAt(nodePos, GetCurrentMap(nodePos), exactElevation);
    }

    // Get node at a given world position
    public Node GetNodeAt(Vector3 nodePos, string mapId, bool exactElevation) {
        Vector3 nodeIndex = FromWorldToNodePos(nodePos, mapId);

        // Checks if the map index exists
        if (!_geodata.ContainsKey(mapId)) {
            throw new Exception("Map Id not found");
        }

        int x = (int) nodeIndex.x;
        int z = (int) nodeIndex.z;

        Vector3 geodataIndex = new Vector3(nodeIndex.x, 0, nodeIndex.z);
        for (int y = 0; y < _geodata[mapId].GetLength(1); y++) {
            Node layer = _geodata[mapId][x, y, z];
            if (layer == null) {
                continue;
            }

            float layerOffset = Math.Abs(layer.nodeIndex.y - nodeIndex.y);
            // verify if the node y diff is lower or higher than _maximumElevationError
            if (layerOffset >= 0 && layerOffset <= _maximumElevationError) {
                return new Node(layer);
            }
        }

        throw new Exception("Node not found");
    }

    public Node GetClosestNodeAt(Vector3 nodePos) {
        return GetClosestNodeAt(nodePos, GetCurrentMap(nodePos));
    }

    // Get closest node at a given world position
    public Node GetClosestNodeAt(Vector3 nodePos, string mapId) {
        List<Node> layers = GetAllNodesAt(nodePos, mapId);

        // find the closest node of given point
        // layers should at the highest be 2-3
        Node closest = null;
        float lowestDiff = -1;
        foreach(Node n in layers) {
            float nodeHeightDiff = Math.Abs(n.center.y - nodePos.y);
            if(closest == null || nodeHeightDiff < lowestDiff || lowestDiff == -1) {
                closest = n;
                lowestDiff = nodeHeightDiff;
            }
        }

        return closest;
    } 

    public List<Node> GetAllNodesAt(Vector3 nodePos) {
        return GetAllNodesAt(nodePos, GetCurrentMap(nodePos));
    }

    public List<Node> GetAllNodesAt(Vector3 nodePos, string mapId) {
        Vector3 nodeIndex = FromWorldToNodePos(nodePos, mapId);

        // Checks if the map index exists
        if (!_geodata.ContainsKey(mapId)) {
            throw new Exception("Map Id not found");
        }

        int x = (int)nodeIndex.x;
        int z = (int)nodeIndex.z;

        List<Node> layers = new List<Node>();

        for (int y = 0; y < _geodata[mapId].GetLength(1); y++) {
            Node layer = _geodata[mapId][x, y, z];
            if (layer == null) {
                continue;
            }

            layers.Add(layer);
        }

        return layers;

        throw new Exception("Nodes not found");
    }

    public Vector3 FromNodeToWorldPos(Vector3 nodePos, Vector3 origin) {
        Vector3 worldPos = nodePos * _nodeSize + origin;
        worldPos = new Vector3(
            NumberUtils.FloorToNearest(worldPos.x, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.y, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.z, _nodeSize));
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

    void OnDrawGizmos() {
        if(!_drawGizmos || !Application.isPlaying)
            return;

        Gizmos.color = Color.green;

        if(_drawMapGizmos) {
            DrawMapGizmos("17_25");
        }

        if(_drawGizmosAroundPlayer) {
            DrawGizmosAroundPlayer();
        }
    }

    private void DrawMapGizmos(string mapId) {
        int count = 0;
        Vector3 cubeSize = new Vector3(_nodeSize - _nodeSize / 10f, 0.1f, _nodeSize - _nodeSize / 10f);

        Node[,,] nodes;
        if(!_geodata.TryGetValue(mapId, out nodes)) {
            Debug.LogWarning("Can't draw gizmos for map " + mapId);
            return;
        }

        for (int x = 0; x < nodes.GetLength(0); x++) {
            for (int z = 0; z < nodes.GetLength(2); z++) {
                for (int y = 0; y < nodes.GetLength(1); y++) {
                    if(nodes[x, y, z] != null) {
                        if(count++ > _drawMapGizmosLimit) {
                            return;
                        }

                        Gizmos.DrawCube(nodes[x, y, z].center, cubeSize);
                    }
                }
            }
        }
    }

    private void DrawGizmosAroundPlayer() {
        if(PlayerController.Instance == null) {
            return;
        }

        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 cubeSize = new Vector3(_nodeSize - _nodeSize / 10f, 0.1f, _nodeSize - _nodeSize / 10f);
        Gizmos.color = Color.green;

        for(int x = -_drawGizmosAroundPlayerRadius; x <= _drawGizmosAroundPlayerRadius; x++) {
            for(int z = -_drawGizmosAroundPlayerRadius; z <= _drawGizmosAroundPlayerRadius; z++) {
                try {
                    List<Node> nodes = GetAllNodesAt(playerPos + new Vector3(x * _nodeSize, 0, z * _nodeSize));
                    nodes.ForEach(n => {
                        Gizmos.DrawCube(n.center, cubeSize);
                    });
                } catch(Exception) {
                }
            }
        }
    }
}
