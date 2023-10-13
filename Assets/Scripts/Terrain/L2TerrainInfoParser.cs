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
		terrainInfo.layers = new List<L2TerrainLayer>();

		using(StreamReader reader = new StreamReader(dataPath)) {
			string line;
			while((line = reader.ReadLine()) != null) {
				if(line.StartsWith("TerrainMap=")) {
					terrainInfo.terrainMapPath = GetMapPath(mapID, line);
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

	private string GetMapPath(string mapID, string line) {
		int equalsIndex = line.IndexOf('=');
		string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 1);
		var path = Path.Combine("Assets/Data/Texture", "t_" + mapID, valueString.Substring(valueString.Length - 13, 12) + ".bmp");

		Debug.Log("Texture path: " + path);
		return path;
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

		string textureInfo = valueParts[0].Trim();
		string alphaMapInfo = valueParts[1].Trim();
		string uScaleInfo = valueParts[2].Trim();
		string vScaleInfo = valueParts[3].Trim();

		if(!textureInfo.Contains("Texture")) {
			Debug.LogWarning("Skipping non-Texture layer: " + line);
			return null;
		}

		layer.texture = LoadTextureFromInfo(textureInfo, MapLoader.TEXTURE_SIZE);
		layer.alphaMap = LoadTextureFromInfo(alphaMapInfo, MapLoader.ALPHAMAP_SIZE);

		// Parse uScale and vScale
		float uScale = ParseValueFromInfo(uScaleInfo);
		float vScale = ParseValueFromInfo(vScaleInfo);

		layer.uScale = uScale;
		layer.vScale = vScale;

		return layer;

	}

	private Texture2D LoadTextureFromInfo(string info, int size) {
		string textureName = info.Replace("'", string.Empty);
		textureName = textureName.Replace(".Texture", string.Empty);
		textureName = textureName.Replace("Height.", string.Empty);
		textureName = textureName.Substring(16);

		string[] folderTexture = textureName.Split('.');
		string texPath = Path.Combine("Assets/Data/Texture", folderTexture[0], folderTexture[1] + ".png");
		byte[] texBytes = File.ReadAllBytes(texPath);

		Texture2D texture = new Texture2D(size, size);
		texture.LoadImage(texBytes);

		return texture;
	}

	private float ParseValueFromInfo(string info) {
		int equalsIndex = info.IndexOf('=');
		string valueString = info.Substring(equalsIndex + 1, info.Length - equalsIndex - 2);
		return float.Parse(valueString);
	}

}
