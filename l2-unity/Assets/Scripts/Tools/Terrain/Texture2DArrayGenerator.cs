#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class Texture2DArrayGenerator : MonoBehaviour {
    public Texture2D[] sourceTextures; // Assign your source textures in the Inspector

    public bool generateMipmaps = true;
    public TextureFormat format = TextureFormat.RGBA32;

    // Start is called before the first frame update
    void Start() {
        if(sourceTextures == null || sourceTextures.Length == 0) {
            Debug.LogWarning("No source textures assigned.");
            return;
        }

        GenerateTexture2DArray(Path.Combine("Assets", "array.asset"));
    }

    public Texture2DArray GenerateTexture2DArray(string path) {
        int width = sourceTextures[0].width;
        int height = sourceTextures[0].height;
        Texture2DArray textureArray = new Texture2DArray(width, height, sourceTextures.Length, format, generateMipmaps);

        // Assign each texture to the Texture2DArray layers
        if(generateMipmaps) {
            for(int i = 0; i < sourceTextures.Length; i++) {
                Texture2D newTex = Resize(sourceTextures[i], width, height);
                for(int mip = 0; mip < sourceTextures[i].mipmapCount; mip++) {
                    Graphics.CopyTexture(newTex, 0, mip, textureArray, i, mip);
                }

            }
        } else {
            for(int i = 0; i < sourceTextures.Length; i++) {
                Texture2D newTex = Resize(sourceTextures[i], width, height);

                Graphics.CopyTexture(newTex, 0, 0, textureArray, i, 0);
            }
        }

        textureArray.Apply(true);


        // Set filter and wrap mode as needed
        textureArray.filterMode = FilterMode.Bilinear;
        textureArray.wrapMode = TextureWrapMode.Repeat;
        textureArray.mipMapBias = 0;

        AssetDatabase.CreateAsset(textureArray, path);
        AssetDatabase.SaveAssets();

        return textureArray;
    }

    Texture2D Resize(Texture2D texture2D, int targetX, int targetY) {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
}
#endif