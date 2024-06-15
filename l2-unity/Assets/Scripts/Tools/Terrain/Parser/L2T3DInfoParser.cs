#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class L2T3DInfoParser {
    public static L2TerrainInfo terrainInfo;
    public static string t3dPath;

    public static L2TerrainInfo LoadMetadata(string mapName) {
        t3dPath = StaticMeshUtils.GetT3DPath(mapName);
        if (!File.Exists(t3dPath)) {
            Debug.Log("No t3d for map " + mapName + " at path " + t3dPath);
            return null;
        }

        terrainInfo = new L2TerrainInfo();

        ParseStaticMeshInfo(null);
        ParseTerrainInfo(mapName);

        return terrainInfo;
    }

    public static L2TerrainInfo LoadStaticMeshInfo(string filePath) {
        terrainInfo = new L2TerrainInfo();

        ParseStaticMeshInfo(filePath);

        return terrainInfo;
    }

    private static void ParseStaticMeshInfo(string givenpath) {
        terrainInfo.staticMeshes = new List<L2StaticMesh>();

        string path = t3dPath;
        if (givenpath != null && givenpath.Length != 0) {
            Debug.LogWarning("Reading staticmeshactors from file " + givenpath);
            path = givenpath;
        }

        using (StreamReader reader = new StreamReader(path)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                L2StaticMesh mesh = new L2StaticMesh();
                if (line.StartsWith("Begin Actor Class=StaticMeshActor Name=")) {
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();

                        if (line.StartsWith("StaticMesh=StaticMesh'")) {
                            mesh.staticMesh = line.Replace("StaticMesh=StaticMesh'", "").Replace("'", "");
                        }

                        if (line.StartsWith("SwayRotationOrig=")) {
                            string rot = line.Replace("SwayRotationOrig=(", "").Replace(")", "");
                            string[] axis = rot.Split(",");
                            foreach (string s in axis) {
                                if (s.Contains("Yaw")) {
                                    mesh.yaw = int.Parse(s.Split("=")[1]);
                                } else if (s.Contains("Roll")) {
                                    mesh.roll = int.Parse(s.Split("=")[1]);
                                } else if (s.Contains("Pitch")) {
                                    mesh.pitch = int.Parse(s.Split("=")[1]);
                                }
                            }
                        }

                        if (line.StartsWith("Location=")) {
                            Vector3 pos = L2MetaDataUtils.ParseVector3(line);
                            mesh.x = pos.x;
                            mesh.y = pos.y;
                            mesh.z = pos.z;
                        }

                        if (line.StartsWith("DrawScale=")) {
                            string[] parts = line.Split("=");
                            mesh.scaleX = float.Parse(parts[1]);
                            mesh.scaleY = float.Parse(parts[1]);
                            mesh.scaleZ = float.Parse(parts[1]);
                        }
                    }
                    terrainInfo.staticMeshes.Add(mesh);
                }
            }
        }

        Debug.Log($"Loaded {terrainInfo.staticMeshes.Count} staticmeshes data.");
    }

    public static void ParseTerrainInfo(string mapName) {
        terrainInfo.mapName = mapName;
        terrainInfo.uvLayers = new List<L2TerrainLayer>();
        terrainInfo.decoLayers = new List<L2DecoLayer>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=TerrainInfo Name=TerrainInfo0")) {
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();
                        if (line.StartsWith("TerrainMap=")) {
                            terrainInfo.terrainMapPath = TextureUtils.GetHeightMapPath(line);
                        } else if (line.StartsWith("GeneratedSectorCounter=")) {
                            terrainInfo.generatedSectorCounter = 256;
                        } else if (line.StartsWith("TerrainScale=")) {
                            terrainInfo.terrainScale = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Location=")) {
                            terrainInfo.location = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Layer")) {
                            L2TerrainLayer layer = L2TerrainInfoParser.ParseL2TerrainLayer(line);
                            if (layer != null) {
                                terrainInfo.uvLayers.Add(layer);
                            }
                        } else if (line.StartsWith("DecoLayers")) {
                            L2DecoLayer layer = L2TerrainInfoParser.ParseL2DecoLayer(line);
                            if (layer != null) {
                                terrainInfo.decoLayers.Add(layer);
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"Loaded {terrainInfo.uvLayers.Count} uv layers data.");
        Debug.Log($"Loaded {terrainInfo.decoLayers.Count} deco layers data.");
    }
}

#endif