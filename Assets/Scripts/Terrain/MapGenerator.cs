using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapGenerator {
	public float ueToUnityUnitScale = 0.1f;
	public float worldPositionOffset = 1f;
	private string terrainContainerName = "terrain_";

	public Terrain InstantiateTerrain(L2TerrainInfo terrainInfo, string mapID) {
		string directoryPath = "Assets/TerrainGen";
		// Create the directory if it doesn't exist
		if(!Directory.Exists(directoryPath)) {
			Directory.CreateDirectory(directoryPath);
			AssetDatabase.Refresh();
		}

		// Create the terrain object
		GameObject terrainObj = Terrain.CreateTerrainGameObject(new TerrainData());
		terrainObj.name = terrainContainerName + mapID;

		// Get the Terrain component and TerrainData
		Terrain terrain = terrainObj.GetComponent<Terrain>();
		terrain.heightmapPixelError = 3;
		terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		terrain.drawInstanced = true;
		terrain.detailObjectDistance = 150;

		byte[] terrainMap = File.ReadAllBytes(terrainInfo.terrainMapPath);

		// Calculate the resolution based on the file size
		int resolution = (int)Mathf.Sqrt(terrainMap.Length / 2); // each height is 2 bytes (16 bits)

		Debug.Log("Resolution:" + resolution);

		TerrainData terrainData = terrain.terrainData;
		terrainData.baseMapResolution = 1024;
		terrainData.alphamapResolution = 1024;
		terrainData.heightmapResolution = resolution + 1; // Set the resolution of the heightmap
		terrainData.SetDetailResolution(512, 32);

		// Just to initialize
		terrainData.size = new Vector3(1015f, 603f, 1015f);

		// Save the terrainData asset
		string savePath = Path.Combine(directoryPath, mapID + ".asset");
		AssetDatabase.CreateAsset(terrainData, savePath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		// Assign the saved asset to the terrain object
		terrain.terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(savePath);

		SetupTerrainLayers(mapID, terrainData, terrainInfo);

		// Create a new array for the heightmap
		float[,] heights = new float[resolution + 1, resolution + 1];

		//// Read the heights from the file
		using(BinaryReader reader = new BinaryReader(new MemoryStream(terrainMap))) {
			reader.ReadBytes(54);

			for(int i = resolution - 1; i >= 0; i--)
				for(int j = 0; j < resolution; j++) {
					// Unity uses a value between 0 and 1 for the heightmap data
					// ushort.MaxValue is 65535
					heights[j + 1, i + 1] = reader.ReadUInt16() / (float)ushort.MaxValue;
				}
		}

		//Filling out the terrain seam.
		for(int i = 0; i < resolution + 1; i++) {
			heights[0, i] = heights[1, i];
		}
		for(int i = 0; i < resolution + 1; i++) {
			heights[i, 0] = heights[i, 1];
		}

		float tx = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.y;
		float ty = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.z;
		float tz = terrainInfo.generatedSectorCounter * terrainInfo.terrainScale.x;
		terrainData.size = new Vector3(tx, ty, tz) * ueToUnityUnitScale * MapLoader.MAP_SCALE;

		terrainData.heightmapResolution = resolution; 
		terrainData.SetHeights(0, 0, heights);

		var uxHalfTerrainWidthAdjustment = (float)tx * 0.5f;
		var uyHalfTerrainWidthAdjustment = (float)ty * 0.5F; 
		var uzHalfTerrainWidthAdjustment = (float)tz * 0.5F;

		// Terrain is shifted by one sector size to accomodate the terrain seam.
		var unityPos = new Vector3(
			terrainInfo.location.y - uxHalfTerrainWidthAdjustment - terrainInfo.terrainScale.y, 
			terrainInfo.location.z - uyHalfTerrainWidthAdjustment,
			terrainInfo.location.x - uzHalfTerrainWidthAdjustment - terrainInfo.terrainScale.x 
		) * 0.01f * MapLoader.MAP_SCALE * worldPositionOffset;

		terrain.transform.position = unityPos;

		return terrain;
	}

	public void SetupTerrainLayers(string mapID, TerrainData terrainData, L2TerrainInfo terrainInfo) {
		// Create terrain layers
		TerrainLayer[] terrainLayers = new TerrainLayer[terrainInfo.layers.Count];
		for(int i = 0; i < terrainInfo.layers.Count; i++) {
			terrainLayers[i] = new TerrainLayer();
			terrainLayers[i].diffuseTexture = terrainInfo.layers[i].texture;
			terrainLayers[i].metallic = 0;
			terrainLayers[i].specular = Color.black;
			terrainLayers[i].smoothness = 0;
			terrainLayers[i].smoothnessSource = TerrainLayerSmoothnessSource.Constant;
			terrainLayers[i].tileOffset = Vector2.zero;
			terrainLayers[i].tileSize = new Vector2(terrainInfo.layers[i].uScale, terrainInfo.layers[i].vScale) * MapLoader.MAP_SCALE * MapLoader.UV_TILE_SIZE;

			AssetDatabase.CreateAsset(terrainLayers[i], "Assets/TerrainGen/" + mapID + "_layer_" + i + ".asset");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		Debug.Log("Terrain layers:" + terrainLayers.Length);

		// Set terrain layers to terrain data
		terrainData.terrainLayers = terrainLayers;

		// Flip vertically
		Texture2D[] flippedAlphaMaps = new Texture2D[terrainInfo.layers.Count];
		for(int i = 0; i < terrainInfo.layers.Count; i++) {
			flippedAlphaMaps[i] = FlipTextureVertically(terrainInfo.layers[i].alphaMap);
		}

		float uvMultiplier = 256f / 257f;

		// Now you can set up your splatmap using your masks
		float[,,] map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainInfo.layers.Count];
		for(int y = 0; y < terrainData.alphamapHeight; y++) {
			for(int x = 0; x < terrainData.alphamapWidth; x++) {

				// Initialize all weights to zero
				for(int i = 0; i < terrainInfo.layers.Count; i++)
					map[x, y, i] = 0;

				float remainingWeight = 1; // keep track of the remaining weight available

				for(int i = terrainInfo.layers.Count - 1; i >= 0; i--) {
					float u = (x) / (float)(terrainData.alphamapWidth);
					float v = (y) / (float)(terrainData.alphamapHeight);

					float maskValue = flippedAlphaMaps[i].GetPixelBilinear(u * uvMultiplier, v * uvMultiplier).grayscale;

					// Calculate the weight for this layer, ensuring that it doesn't exceed the remaining available weight
					float weight = Mathf.Min(maskValue, remainingWeight);
					map[x, y, i] = weight;

					// Subtract the weight assigned to this layer from the remaining available weight
					remainingWeight -= weight;
				}
			}

			terrainData.SetAlphamaps(0, 0, map);

		}
	}

	public static Texture2D FlipTextureVertically(Texture2D originalTexture) {
		int width = originalTexture.width;
		int height = originalTexture.height;

		Texture2D flippedTexture = new Texture2D(width, height);
		Color[] originalPixels = originalTexture.GetPixels();
		Color[] flippedPixels = new Color[originalPixels.Length];

		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {
				int sourceIndex = x + (height - y - 1) * width;
				int targetIndex = x + y * width;
				flippedPixels[targetIndex] = originalPixels[sourceIndex];
			}
		}

		flippedTexture.SetPixels(flippedPixels);
		flippedTexture.Apply();

		return flippedTexture;
	}
}
