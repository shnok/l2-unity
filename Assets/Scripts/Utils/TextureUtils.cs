using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureUtils
{
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

	public static Texture2D LoadTexture2DFromInfo(string info, int size) {
		/*byte[] texBytes = File.ReadAllBytes(GetTexturePath(info));

		Texture2D texture = new Texture2D(size, size);
		texture.LoadImage(texBytes);*/
		Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetTexturePath(info));

		Debug.Log(texture);

		return texture;
	}

	public static Texture LoadTextureFromInfo(string info, int size) {
		Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(GetTexturePath(info));

		return texture;
	}


	public static string GetTexturePath(string value) {
		if(string.IsNullOrEmpty(value)) {
			return null;
		}
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], folderTexture[1] + ".png");
	}

	public static string GetMaterialPath(string value) {
		if(string.IsNullOrEmpty(value)) {
			return null;
		}
		Debug.Log(value);
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], "Materials", folderTexture[1] + ".mat");
	}

	public static string GetHeightMapPath(string value) {
		if(string.IsNullOrEmpty(value)) {
			return null;
		}
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], "Height." + folderTexture[1] + ".bmp");
	}
}
