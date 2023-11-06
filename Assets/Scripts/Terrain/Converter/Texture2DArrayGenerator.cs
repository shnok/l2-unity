using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Texture2DArrayGenerator : MonoBehaviour
{
    public Texture2D[] sourceTextures; // Assign your source textures in the Inspector

    public bool generateMipmaps = false;
    public TextureFormat format = TextureFormat.RGBA32;

    // Start is called before the first frame update
    void Start()
    {
        if(sourceTextures.Length == 0) {
            Debug.LogError("No source textures assigned.");
            return;
        }

        // Create a new Texture2DArray
        //Texture2DArray textureArray = new Texture2DArray(sourceTextures[0].width, sourceTextures[0].height, sourceTextures.Length, TextureFormat.RGBA32, 10, true);
        
        int width = sourceTextures[0].width;
        int height = sourceTextures[0].height;
        Texture2DArray textureArray = new Texture2DArray(width, height, sourceTextures.Length, format, generateMipmaps);

        // Assign each texture to the Texture2DArray layers
        if(generateMipmaps) {
            for(int i = 0; i < sourceTextures.Length; i++) {
                for(int mip = 0; mip < sourceTextures[i].mipmapCount; mip++) {
                    

                    Graphics.CopyTexture(sourceTextures[i], 0, mip, textureArray, i, mip);
                    // Calculate the size of the mipmap level
                    /*int mipmapWidth = sourceTextures[i].width >> mip;
                    int mipmapHeight = sourceTextures[i].height >> mip;

                    // Create a temporary RenderTexture with the same size as the mipmap level
                    RenderTexture tempRT = new RenderTexture(mipmapWidth, mipmapHeight, 0);

                    // Copy the mipmap level from the source texture to the temporary RenderTexture
                    Graphics.CopyTexture(sourceTextures[i], 0, mip, tempRT, 0, 0);

                    // Copy the mipmap level from the temporary RenderTexture to the Texture2DArray
                    Graphics.CopyTexture(tempRT, 0, 0, textureArray, i, mip);

                    // Release the temporary RenderTexture
                    tempRT.Release();*/
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

        AssetDatabase.CreateAsset(textureArray, Path.Combine("Assets", "array.asset"));
        AssetDatabase.SaveAssets();

        // Assign the Texture2DArray to a material or use it in your project
        // e.g., GetComponent<Renderer>().material.SetTexture("_ArrayTexture", textureArray);
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
