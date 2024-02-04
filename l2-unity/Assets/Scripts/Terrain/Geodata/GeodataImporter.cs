using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class GeodataImporter
{
    private string _geodataEntryName = "geodata";
    private string _mapId;
    private float _nodeSize;
    private Vector3 _origin;
    private long _startTime;
    private byte _maximumLayerCount = 8;
    private int _mapSize = 625;
    private long _timeout = 10000;
    private Node[,,] _mapNodes;
    private GeodataImporterFactory.ImportJobComplete _completeCallback;


    private volatile bool _jobDone = false;
    public bool JobDone { get { return _jobDone; } }

    public GeodataImporter(string mapId, float nodeSize, byte maximumLayerCount, Vector3 origin, GeodataImporterFactory.ImportJobComplete callback) {
        _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _mapId = mapId;
        _nodeSize = nodeSize;
        _completeCallback = callback;
        _origin = origin;
        _maximumLayerCount = maximumLayerCount;
    }

    public void ImportGeodata() {
        try {
            byte[] data = null;
            try {
                data = GetGeodataBinaryForMap(_mapId);
            } catch(Exception e) {
                Debug.LogWarning(e.Message);
            }

            if(data != null) {
                _mapNodes = LoadMapGeodata(data, _origin, _nodeSize);
            }
        } catch(Exception e) {
            Debug.LogWarning("Error while finding path: " + e.Message);
        }

        _jobDone = true;
    }

    public void NotifyComplete() {
        if(_completeCallback != null) {
            _completeCallback(_mapNodes);
        }
    }

    public byte[] GetGeodataBinaryForMap(string mapId) {
        try {
            string geodataFilePath = Path.Combine("Assets/Resources/Data/Maps/", mapId, mapId + ".geodata");

            using(ZipArchive archive = ZipFile.OpenRead(geodataFilePath)) {
                ZipArchiveEntry entry = archive.GetEntry(_geodataEntryName);

                if(entry == null) {
                    throw new Exception("File not found in the ZIP archive.");
                }

                using(Stream stream = entry.Open()) {
                    // Read the entire content into memory
                    byte[] data;
                    using(MemoryStream memoryStream = new MemoryStream()) {
                        stream.CopyTo(memoryStream);
                        data = memoryStream.ToArray();
                    }

                    return data;
                }            
            }
        } catch(IOException ex) {
            throw new Exception("Error reading ZIP file: " + ex.Message);
        }
    }

    public Node[,,] LoadMapGeodata(byte[] data, Vector3 mapOrigin, float nodeSize) {
        long startLoadingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        Vector3 lastNodeIndex = new Vector3(-1, -1, -1);
        Vector3 scaledPos = Vector3.zero;
        Vector3 nodeId = Vector3.zero;

        int rowCount = (int)Mathf.Ceil(_mapSize / _nodeSize);
        Node[,,] nodes = new Node[rowCount, _maximumLayerCount, rowCount];
        int layer = 0;
        int count = 0;
        int nodesToImport = data.Length / 6;

        // Create a BinaryReader from the memory stream
        using (BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                if(DateTimeOffset.Now.ToUnixTimeMilliseconds() - _startTime > _timeout) {
                    Debug.LogWarning($"Geodata import timeout. Imported nodes: {count}.");
                    return null;
                }
                short x = reader.ReadInt16();
                short y = reader.ReadInt16();
                short z = reader.ReadInt16();
                count++;

                scaledPos.x = x;
                scaledPos.y = y;
                scaledPos.z = z;

                nodeId.x = x;
                nodeId.z = z;

                Node n = new Node(scaledPos, Geodata.Instance.FromNodeToWorldPos(scaledPos, mapOrigin), nodeSize);

                if (lastNodeIndex == nodeId) {
                    layer++;
                } else {
                    layer = 0;
                }

                nodes[x, layer, z] = n;
                lastNodeIndex = nodeId;
            }
        }

        long elapsed = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - startLoadingTime);
        Debug.Log($"Imported {count}/{nodesToImport} nodes in {elapsed}ms.");

        return nodes;
    }

    public static Vector3 LoadMapOrigin(string mapId, float nodeSize) {
        string mapFolder = Path.Combine("Data", "Maps", mapId);
        GameObject map = Resources.Load<GameObject>(Path.Combine(mapFolder, mapId));
        if(map == null) {
            throw new Exception($"Map {mapId} prefab not found at {mapFolder}.");
        }

        Vector3 origin = VectorUtils.floorToNearest(map.transform.position, nodeSize);
        return origin;
    }
}
