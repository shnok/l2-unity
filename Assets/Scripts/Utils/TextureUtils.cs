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

		byte[] texBytes = File.ReadAllBytes(getTexturePath(info));

		Texture2D texture = new Texture2D(size, size);
		texture.LoadImage(texBytes);

		return texture;
	}

	private static string getTexturePath(string value) {
		string[] folderTexture = getFolderAndFile(value);
		return Path.Combine("Assets/Data/Texture", folderTexture[0], folderTexture[1] + ".png");
	}

	public static string GetHeightMapPath(string value) {
		string[] folderTexture = getFolderAndFile(value);
		return Path.Combine("Assets/Data/Texture", folderTexture[0], "Height." + folderTexture[1] + ".bmp");
	}

	private static string[] getFolderAndFile(string value) {
		string textureName = value.Split('=')[1];

		textureName = textureName.Replace("Texture'", string.Empty);
		textureName = textureName.Replace(".Texture", string.Empty);
		textureName = textureName.Replace("Height.", string.Empty);
		textureName = textureName.Replace("'", string.Empty);

		return textureName.Split('.');
	}
}
