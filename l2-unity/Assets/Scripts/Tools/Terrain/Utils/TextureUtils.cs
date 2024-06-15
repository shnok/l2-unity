#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextureUtils {
    public static Texture2D FlipTextureVertically(Texture2D originalTexture) {
        int width = originalTexture.width;
        int height = originalTexture.height;

        Texture2D flippedTexture = new Texture2D(width, height);
        Color[] originalPixels = originalTexture.GetPixels();
        Color[] flippedPixels = new Color[originalPixels.Length];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int sourceIndex = x + (height - y - 1) * width;
                int targetIndex = x + y * width;
                flippedPixels[targetIndex] = originalPixels[sourceIndex];
            }
        }

        flippedTexture.SetPixels(flippedPixels);
        flippedTexture.Apply();

        return flippedTexture;
    }

    public static Texture2D RotateTexture(Texture2D original) {
        int width = original.width;
        int height = original.height;

        Color[] pixels = original.GetPixels();
        Color[] rotatedPixels = new Color[width * height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Calculate the new coordinates for the rotated pixel
                int newX = height - y - 1;
                int newY = x;

                // Calculate the indices in the 1D pixel arrays
                int originalIndex = y * width + x;
                int rotatedIndex = newY * height + newX;

                // Copy the pixel data
                rotatedPixels[rotatedIndex] = pixels[originalIndex];
            }
        }

        // Create a new Texture2D to store the rotated pixels
        Texture2D rotatedTexture = new Texture2D(height, width);
        rotatedTexture.SetPixels(rotatedPixels);
        rotatedTexture.Apply();

        return rotatedTexture;
    }

    public static Texture2D LoadTexture2DFromInfo(string info, int size) {
        /*byte[] texBytes = File.ReadAllBytes(GetTexturePath(info));

		Texture2D texture = new Texture2D(size, size);
		texture.LoadImage(texBytes);*/
        string path = GetTexturePath(info);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        if (texture == null) {
            Debug.LogWarning("Can't load textue at path " + path);
        }

        return texture;
    }

    public static Texture LoadTextureFromInfo(string info, int size) {
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(GetTexturePath(info));

        return texture;
    }


    public static string GetTexturePath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }
        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], folderTexture[1] + ".png");
    }

    public static string GetMaterialPath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }

        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], "Materials", folderTexture[1] + ".mat");
    }

    public static string GetHeightMapPath(string value) {
        if (string.IsNullOrEmpty(value)) {
            return null;
        }
        string[] folderTexture = L2MetaDataUtils.GetFolderAndFileFromInfo(value);
        return Path.Combine("Assets/Resources/Data/Textures", folderTexture[0], "Height." + folderTexture[1] + ".bmp");
    }
}
#endif
