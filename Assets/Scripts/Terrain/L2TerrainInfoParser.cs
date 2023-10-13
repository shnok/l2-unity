using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class L2TerrainInfoParser
{
	public L2TerrainInfo GetL2TerrainInfo(string mapID) {
		string dataPath = "Assets/Data/Maps/" + mapID + "/TerrainInfo0.txt";
		if(!File.Exists(dataPath)) {
			Debug.LogWarning("File not found: " + dataPath);
			return null;
		}

		var terrainInfo = new L2TerrainInfo();
		terrainInfo.mapName = mapID;
		terrainInfo.layers = new List<L2TerrainLayer>();

		using(StreamReader reader = new StreamReader(dataPath)) {
			string line;
			while((line = reader.ReadLine()) != null) {
				if(line.StartsWith("TerrainMap=")) {
					terrainInfo.terrainMapPath = TextureUtils.GetHeightMapPath(line);
				} else if(line.StartsWith("GeneratedSectorCounter=")) {
					terrainInfo.generatedSectorCounter = ParseGeneratedSectorCounter(line);
				} else if(line.StartsWith("TerrainScale=")) {
					terrainInfo.terrainScale = ParseVector3(line);
				} else if(line.StartsWith("Location=")) {
					terrainInfo.location = ParseVector3(line);
				} else if(line.StartsWith("Layer")) {
					L2TerrainLayer layer = ParseL2TerrainLayer(line);
					if(layer != null) {
						terrainInfo.layers.Add(layer);
					}
				}
			}
		}

		return terrainInfo;
	}

	private int ParseGeneratedSectorCounter(string line) {
		// TODO: Handle parsing of GeneratedSectorCounter if needed
		return 256; // Temporary value
	}

	private Vector3 ParseVector3(string line) {
		int equalsIndex = line.IndexOf('=');
		string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
		string[] valueParts = valueString.Split(',');
		float x = float.Parse(valueParts[0].Substring(valueParts[0].IndexOf('=') + 1));
		float y = float.Parse(valueParts[1].Substring(valueParts[1].IndexOf('=') + 1));
		float z = float.Parse(valueParts[2].Substring(valueParts[2].IndexOf('=') + 1));
		return new Vector3(x, y, z);
	}

	private L2TerrainLayer ParseL2TerrainLayer(string line) {
		L2TerrainLayer layer = new L2TerrainLayer();

		int equalsIndex = line.IndexOf('=');
		string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
		string[] valueParts = valueString.Split(',');

		if(valueParts.Length < 4) {
			Debug.LogError("Invalid layer line: " + line);
			return null;
		}

		string textureInfo = string.Empty;
		string alphaMapInfo = string.Empty;
		string uScaleInfo = string.Empty;
		string vScaleInfo = string.Empty;

		for(int i = 0; i < valueParts.Length; i++) {
			if(valueParts[i].Contains("Texture=")) {
				textureInfo = valueParts[i].Trim();
			}
			if(valueParts[i].Contains("AlphaMap=")) {
				alphaMapInfo = valueParts[i].Trim();
			}
			if(valueParts[i].Contains("UScale=")) {
				uScaleInfo = valueParts[i].Trim();
			}
			if(valueParts[i].Contains("VScale=")) {
				vScaleInfo = valueParts[i].Trim();
			}
		}

		if(!textureInfo.Contains("Texture")) {
			return null;
		}

		layer.texture = TextureUtils.LoadTextureFromInfo(textureInfo, MapGenerator.TEXTURE_SIZE);
		if(alphaMapInfo != string.Empty) {
			layer.alphaMap = TextureUtils.LoadTextureFromInfo(alphaMapInfo, MapGenerator.ALPHAMAP_SIZE);
		}

		// Parse uScale and vScale
		float uScale = ParseFloatFromInfo(uScaleInfo);
		float vScale = ParseFloatFromInfo(vScaleInfo);

		layer.uScale = uScale;
		layer.vScale = vScale;

		return layer;
	}

	private float ParseFloatFromInfo(string info) {
		int equalsIndex = info.IndexOf('=');
		string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 2);
		return float.Parse(valueString);
	}

}
