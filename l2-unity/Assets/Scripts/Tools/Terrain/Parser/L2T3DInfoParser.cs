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
                            mesh.eulerAngles = L2MetaDataUtils.ParseRotation(line);
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

    public static List<Brush> ParseBrushInfo(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }
        terrainInfo.brushes = new List<Brush>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=Brush Name=")) {
                    Brush brush = new Brush();
                    brush.model = new Model();
                    brush.model.name = line.Replace("Begin Actor Class=Brush Name=", "");
                    brush.name = line.Replace("Begin Actor Class=Brush Name=", "");
                    brush.model.poly = new Poly();
                    brush.model.poly.name = "Poly";
                    brush.polyFlags = L2MetaDataUtils.ParsePolyFlags(0);

                    List<PolyData> polydatas = new List<PolyData>();
                    Debug.Log("New Brush " + brush.name);

                    int polyIndex = 0;
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();

                        if (line.StartsWith("CsgOper=")) {
                            brush.csgOper = line.Replace("CsgOper=", "");
                        } else if (line.StartsWith("Location=")) {
                            brush.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("PrePivot=")) {
                            brush.prePivot = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Begin Polygon")) {
                            PolyData polydata = new PolyData();
                            polydata.polyIndex = polyIndex++;

                            
                            string[] polyInfos = line.Replace("Begin Polygon", "").Trim().Split(" ");

                            foreach (var polyInfo in polyInfos) {
                                if (polyInfo.Contains("Texture=")) {
                                    polydata.texture = polyInfo.Replace("Texture=", "");
                                }
                                if (polyInfo.Contains("Flags=")) {
                                    string flags = polyInfo.Replace("Flags=", "");
                                    polydata.polyFlags = L2MetaDataUtils.ParsePolyFlags(int.Parse(flags));
                                }
                            }

                            List<Vector3> vertices = new List<Vector3>();
                            while ((line = reader.ReadLine()) != null && !line.Contains("End Polygon")) {
                                line = line.Trim();
                                if (line.Contains("Origin")) {
                                    polydata.origin = L2MetaDataUtils.ParseVector3Poly(line.Replace("Origin", "").Trim());
                                }
                                if (line.Contains("TextureU")) {
                                    polydata.origin = L2MetaDataUtils.ParseVector3Poly(line.Replace("TextureU", "").Trim());
                                }
                                if (line.Contains("TextureV")) {
                                    polydata.origin = L2MetaDataUtils.ParseVector3Poly(line.Replace("TextureV", "").Trim());
                                }
                                if (line.Contains("Normal")) {
                                    polydata.origin = L2MetaDataUtils.ParseVector3Poly(line.Replace("Normal", "").Trim());
                                }
                                if (line.Contains("Vertex")) {
                                    vertices.Add(L2MetaDataUtils.ParseVector3Poly(line.Replace("Vertex", "").Trim()));
                                }
                            }

                            Debug.Log("Poly Verticle count " + vertices.Count);

                            polydata.vertices = vertices.ToArray();

                            if (polydata.polyFlags == null || polydata.polyFlags.Length == 0) {
                                polydata.polyFlags = L2MetaDataUtils.ParsePolyFlags(0);
                            }

                            polydatas.Add(polydata);

                            brush.model.poly.polyData = polydatas.ToArray();
                            brush.model.poly.polyCount = polydatas.Count;
                        }
                    }

                    Debug.Log(brush);
                    Debug.Log(brush.model.poly);
                    Debug.Log("Brush poly count " + brush.model.poly.polyCount);
                    terrainInfo.brushes.Add(brush);
                }
            }
        }

        Debug.Log($"Loaded {terrainInfo.brushes.Count} brushe(s).");

        return terrainInfo.brushes;
    }

    public static List<L2InterpolationPoint> ParseCameraInfo(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }

        terrainInfo.interpolationPoints = new List<L2InterpolationPoint>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=InterpolationPoint Name=")) {
                    L2InterpolationPoint interpolationPoint = new L2InterpolationPoint();
                    interpolationPoint.name = line.Replace("Begin Actor Class=InterpolationPoint Name=", "");
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();
                        
                        if (line.StartsWith("Location=")) {
                            interpolationPoint.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Rotation=")) {
                            interpolationPoint.eulerAngles = L2MetaDataUtils.ParseRotation(line);
                        }
                    }

                    terrainInfo.interpolationPoints.Add(interpolationPoint);

                }
            }
        }

        Debug.Log($"Loaded {terrainInfo.interpolationPoints.Count} interpolation points.");

        return terrainInfo.interpolationPoints;
    }

    public static List<AmbientSound> ParseAmbientSounds(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }

        terrainInfo.ambientSounds = new List<AmbientSound>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin LevelObject Class=AmbientSoundObject Name=")) {
                    AmbientSound ambientSound = new AmbientSound();
                    ambientSound.name = line.Replace("Begin LevelObject Class=AmbientSoundObject Name=", "");

                    while ((line = reader.ReadLine()) != null && !line.Contains("End LevelObject")) {
                        line = line.Trim();

                        if (line.StartsWith("AmbientRandom=")) {
                            ambientSound.ambientRandom = L2MetaDataUtils.ParseIntFromInfo(line);
                            Debug.Log(ambientSound.ambientRandom);
                        } else if (line.StartsWith("AmbientSoundStartTime=")) {
                            ambientSound.ambientSoundStartTime = L2MetaDataUtils.ParseFloatFromInfo(line);
                        } else if (line.StartsWith("AmbientSound=")) {
                            string ambientSoundName = line.Replace("AmbientSound=Sound'", "").Replace("'", "");
                            ambientSound.ambientSoundName = ambientSoundName;
                        } else if (line.StartsWith("SoundRadius=")) {
                            ambientSound.soundRadius = L2MetaDataUtils.ParseFloatFromInfo(line);
                        } else if (line.StartsWith("SoundVolume=")) {
                            ambientSound.soundVolume = L2MetaDataUtils.ParseIntFromInfo(line);
                        } else if (line.StartsWith("SoundPitch=")) {
                            ambientSound.soundPitch = L2MetaDataUtils.ParseIntFromInfo(line);
                        } else if (line.StartsWith("Group=")) {
                            ambientSound.groupName = line.Replace("Group=", "").Replace("\"", "");
                        } else if (line.StartsWith("Location=")) {
                            ambientSound.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("AmbientSoundType=")) {
                            ambientSound.ambientSoundType = line.Replace("AmbientSoundType=", "");                   
                        }
                    }

                    if(ambientSound.ambientSoundName != null && ambientSound.ambientSoundName.Length > 0) {
                        terrainInfo.ambientSounds.Add(ambientSound);
                    }

                    if(ambientSound.ambientSoundType == null || ambientSound.ambientSoundType.Length == 0) {
                        ambientSound.ambientSoundType = "AST1_Always";
                    }
                }
            }
        }

        Debug.Log($"Loaded {terrainInfo.ambientSounds.Count} ambientsound(s).");

        return terrainInfo.ambientSounds;
    }

}

#endif