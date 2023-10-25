using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Texture2DArrayGenerator : MonoBehaviour
{
    public Texture2D[] sourceTextures; // Assign your source textures in the Inspector


    // Start is called before the first frame update
    void Start()
    {
        if(sourceTextures.Length == 0) {
            Debug.LogError("No source textures assigned.");
            return;
        }

        // Create a new Texture2DArray
        //Texture2DArray textureArray = new Texture2DArray(sourceTextures[0].width, sourceTextures[0].height, sourceTextures.Length, TextureFormat.RGBA32, 10, true);
        Texture2DArray textureArray = new Texture2DArray(sourceTextures[0].width, sourceTextures[0].height, sourceTextures.Length, TextureFormat.RGBA32, true);

        // Assign each texture to the Texture2DArray layers
        for(int i = 0; i < sourceTextures.Length; i++) {
            for(int mip = 0; mip < sourceTextures[i].mipmapCount; mip++) {
                Graphics.CopyTexture(sourceTextures[i], 0, mip, textureArray, i, mip);
            }
       // Graphics.CopyTexture(sourceTextures[i], 0, 0, textureArray, i, 0);
        }

     textureArray.Apply(true);

        
        // Set filter and wrap mode as needed
        textureArray.filterMode = FilterMode.Bilinear;
        textureArray.wrapMode = TextureWrapMode.Repeat;
        textureArray.mipMapBias = 0;

        AssetDatabase.CreateAsset(textureArray, Path.Combine("Assets", "Scripts", "Rendering", "array.asset"));
        AssetDatabase.SaveAssets();

        // Assign the Texture2DArray to a material or use it in your project
        // e.g., GetComponent<Renderer>().material.SetTexture("_ArrayTexture", textureArray);
    }
}
