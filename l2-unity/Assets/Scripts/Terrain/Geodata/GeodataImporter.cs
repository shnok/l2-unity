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
    private Dictionary<Vector3, List<Node>> _mapNodes;
    private GeodataImporterFactory.ImportJobComplete _completeCallback;


    private volatile bool jobDone = false;
    public bool JobDone { get { return jobDone; } }

    public GeodataImporter(string mapId, float nodeSize, Vector3 origin, GeodataImporterFactory.ImportJobComplete callback) {
        _mapId = mapId;
        _nodeSize = nodeSize;
        _completeCallback = callback;
        _origin = origin;
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

        jobDone = true;
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

    public Dictionary<Vector3, List<Node>> LoadMapGeodata(byte[] data, Vector3 mapOrigin, float nodeSize) {
        long startLoadingTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _mapNodes = new Dictionary<Vector3, List<Node>>();
        int count = 0;

        List<Node> layers;

        // Create a BinaryReader from the memory stream
        using(BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                short x = reader.ReadInt16();
                short y = reader.ReadInt16();
                short z = reader.ReadInt16();
                count++;

                Vector3 scaledPos = new Vector3(x, y, z);
                Vector3 nodeId = new Vector3(x, 0, z);
                Node n = new Node(scaledPos, Geodata.Instance.FromNodeToWorldPos(scaledPos, mapOrigin), nodeSize);

                if(!_mapNodes.TryGetValue(nodeId, out layers)) {
                    layers = new List<Node>();
                    _mapNodes[nodeId] = layers;
                }

                layers.Add(n);
            }
        }

        long elapsed = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - startLoadingTime);
        Debug.Log($"Imported {count} nodes in {elapsed}ms.");

        return _mapNodes;
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
