#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class L2TerrainInfoParser {
    public static L2TerrainInfo ParseTerrainInfo0(string mapID) {
        string dataPath = "Assets/Data/Maps/Meta/" + mapID + "/TerrainInfo0.txt";
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return null;
        }

        var terrainInfo = new L2TerrainInfo();
        terrainInfo.mapName = mapID;
        terrainInfo.uvLayers = new List<L2TerrainLayer>();
        terrainInfo.decoLayers = new List<L2DecoLayer>();

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (line.StartsWith("TerrainMap=")) {
                    terrainInfo.terrainMapPath = TextureUtils.GetHeightMapPath(line);
                } else if (line.StartsWith("GeneratedSectorCounter=")) {
                    terrainInfo.generatedSectorCounter = ParseGeneratedSectorCounter(line);
                } else if (line.StartsWith("TerrainScale=")) {
                    terrainInfo.terrainScale = ParseVector3(line);
                } else if (line.StartsWith("Location=")) {
                    terrainInfo.location = ParseVector3(line);
                } else if (line.StartsWith("Layer")) {
                    L2TerrainLayer layer = ParseL2TerrainLayer(line);
                    if (layer != null) {
                        terrainInfo.uvLayers.Add(layer);
                    }
                } else if (line.StartsWith("DecoLayers")) {
                    L2DecoLayer layer = ParseL2DecoLayer(line);
                    if (layer != null) {
                        terrainInfo.decoLayers.Add(layer);
                    }
                }
            }
        }

        return terrainInfo;
    }

    private static int ParseGeneratedSectorCounter(string line) {
        // TODO: Handle parsing of GeneratedSectorCounter if needed
        return 256; // Temporary value
    }

    private static Vector3 ParseVector3(string line) {
        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');
        float x = float.Parse(valueParts[0].Substring(valueParts[0].IndexOf('=') + 1));
        float y = float.Parse(valueParts[1].Substring(valueParts[1].IndexOf('=') + 1));
        float z = float.Parse(valueParts[2].Substring(valueParts[2].IndexOf('=') + 1));
        return new Vector3(x, y, z);
    }

    public static L2TerrainLayer ParseL2TerrainLayer(string line) {
        L2TerrainLayer layer = new L2TerrainLayer();

        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        if (valueParts.Length < 4) {
            Debug.LogError("Invalid layer line: " + line);
            return null;
        }

        string textureInfo = string.Empty;
        string alphaMapInfo = string.Empty;
        string uScaleInfo = string.Empty;
        string vScaleInfo = string.Empty;

        for (int i = 0; i < valueParts.Length; i++) {
            if (valueParts[i].Contains("Texture=")) {
                textureInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("AlphaMap=")) {
                alphaMapInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("UScale=")) {
                uScaleInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("VScale=")) {
                vScaleInfo = valueParts[i].Trim();
            }
        }

        if (!textureInfo.Contains("Texture")) {
            return null;
        }

        layer.texture = TextureUtils.LoadTexture2DFromInfo(textureInfo, L2TerrainGeneratorTool.UV_TEXTURE_SIZE);
        if (alphaMapInfo != string.Empty) {
            layer.alphaMap = TextureUtils.LoadTexture2DFromInfo(alphaMapInfo, L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE);
        }

        // Parse uScale and vScale
        float uScale = L2MetaDataUtils.ParseFloatFromInfo(uScaleInfo);
        float vScale = L2MetaDataUtils.ParseFloatFromInfo(vScaleInfo);

        layer.uScale = uScale;
        layer.vScale = vScale;

        return layer;
    }

    public static L2DecoLayer ParseL2DecoLayer(string line) {
        L2DecoLayer layer = new L2DecoLayer();

        string showOnTerrain = string.Empty;
        string densityMap = string.Empty;
        string staticMesh = string.Empty;
        string scaleMultiplier = string.Empty;

        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        for (int i = 0; i < valueParts.Length; i++) {
            if (valueParts[i].Contains("ShowOnTerrain=")) {
                showOnTerrain = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("DensityMap=")) {
                densityMap = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("StaticMesh=")) {
                staticMesh = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("ScaleMultiplier=")) {
                int index = line.IndexOf("ScaleMultiplier=");
                string fromIndex = line.Substring(index);
                int argEndIndex = fromIndex.IndexOf("))");
                scaleMultiplier = fromIndex.Substring(0, argEndIndex + 2);
            }
        }

        layer.showOnTerrain = L2MetaDataUtils.ParseBoolFromInfo(showOnTerrain);
        layer.densityMapPath = TextureUtils.GetTexturePath(densityMap);
        layer.densityMap = TextureUtils.LoadTexture2DFromInfo(densityMap, L2TerrainGeneratorTool.DECO_LAYER_ALPHAMAP_SIZE);
        layer = UpdateDecoLayerScale(layer, scaleMultiplier);
        layer.staticMesh = StaticMeshUtils.LoadMeshFromInfo(staticMesh);

        return layer;
    }

    private static L2DecoLayer UpdateDecoLayerScale(L2DecoLayer layer, string scaleMultiplier) {
        int equalsIndex = scaleMultiplier.IndexOf('=');
        string valueString = scaleMultiplier.Substring(equalsIndex + 1, scaleMultiplier.Length - equalsIndex - 2);

        valueString = valueString
            .Replace("(", String.Empty)
            .Replace(")", String.Empty)
            .Replace("Min", String.Empty)
            .Replace("Max", String.Empty)
            .Replace("=", String.Empty)
            .Replace("X", String.Empty)
            .Replace("Y", String.Empty)
            .Replace("Z", String.Empty);

        string[] parts = valueString.Split(",");

        layer.minWidth = float.Parse(parts[0]);
        layer.maxWidth = float.Parse(parts[1]);
        layer.minHeight = float.Parse(parts[2]);
        layer.maxHeight = float.Parse(parts[3]);

        return layer;
    }


    public static AmbientSound[] ParseAmbientSoundFile(string path) {
        try {
            string json = File.ReadAllText(path);
            AmbientDataContainer sounds = JsonUtility.FromJson<AmbientDataContainer>(json);
            return sounds.data;

        } catch (IOException e) {
            Debug.LogError("An error occurred: " + e.Message);
        }

        return null;
    }


}
#endif