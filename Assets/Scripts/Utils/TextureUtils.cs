using System.Collections;
using System.Collections.Generic;
using System.IO;
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

	public static Texture2D LoadTextureFromInfo(string info, int size) {

		byte[] texBytes = File.ReadAllBytes(GetTexturePath(info));

		Texture2D texture = new Texture2D(size, size);
		texture.LoadImage(texBytes);

		return texture;
	}

	public static string GetTexturePath(string value) {
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], folderTexture[1] + ".png");
	}

	public static string GetMaterialPath(string value) {
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], "Materials", folderTexture[1] + ".mat");
	}

	public static string GetHeightMapPath(string value) {
		string[] folderTexture = L2TerrainInfoParser.GetFolderAndFileFromInfo(value);
		return Path.Combine("Assets/Data/Textures", folderTexture[0], "Height." + folderTexture[1] + ".bmp");
	}
}
