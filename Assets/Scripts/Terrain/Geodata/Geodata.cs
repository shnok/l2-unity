using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class Geodata : MonoBehaviour
{
    public List<string> mapsToLoad;

    // Start is called before the first frame update
    void Start()
    {
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
                    /*using(Stream stream = entry.Open())
                    using(BinaryReader reader = new BinaryReader(stream)) {
                        byte[] bytes = reader.ReadBytes((int)stream.Length);
                        for(int i = 0; i < bytes.Length; i += 6) {
                            short x = BitConverter.ToInt16(bytes, i);
                            short y = BitConverter.ToInt16(bytes, i + 2);
                            short z = BitConverter.ToInt16(bytes, i + 4);
                            Debug.Log(new Vector3(x, y, z));
                        }
                    }*/
                    using(Stream stream = entry.Open()) {
                        // Read the entire content into memory
                        byte[] data;
                        using(MemoryStream memoryStream = new MemoryStream()) {
                            stream.CopyTo(memoryStream);
                            data = memoryStream.ToArray();
                        }

                        // Create a BinaryReader from the memory stream
                        using(BinaryReader reader = new BinaryReader(new MemoryStream(data))) {
                            while(reader.BaseStream.Position < reader.BaseStream.Length) {
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short z = reader.ReadInt16();
                            //    Debug.Log(new Vector3(x, y, z));
                            }
                        }
                    }
                } else {
                    Debug.LogError("File not found in the ZIP archive.");
                }
            }
        } catch(IOException ex) {
            Debug.LogError("Error reading ZIP file: " + ex.Message);
        }
    }
}
