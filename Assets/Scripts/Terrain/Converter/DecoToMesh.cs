using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DecoToMesh
{
    public static void ConvertDecoLayers(List<L2DecoLayer> decoLayers, Terrain terrain) {
        GameObject decoLayerBase = new GameObject("DecoLayer");

        for(int i = 0; i < decoLayers.Count; i++) {
            //Texture2D decoAlphamap = TextureUtils.FlipTextureVertically(decoLayers[i].densityMap);
            Texture2D decoAlphamap = TextureUtils.RotateTexture(decoLayers[i].densityMap);
            decoAlphamap = TextureUtils.FlipTextureVertically(decoAlphamap);

            string path = decoLayers[i].densityMapPath;

            Debug.Log(path);
            GameObject decoLayerSubBase = new GameObject("DecoLayer_" + i);
            decoLayerSubBase.transform.parent = decoLayerBase.transform;

            Color32[] pixels = decoAlphamap.GetPixels32();
            for(int y = 0; y < decoAlphamap.height; y++) {
                for(int x = 0; x < decoAlphamap.width; x++) {
                    int index = y * decoAlphamap.width + x;

                    float alpha = pixels[index].a;
                    if(alpha == 0) {
                        continue;
                    }

                    float scaleX = Mathf.Lerp(decoLayers[i].minWidth, decoLayers[i].maxWidth, alpha / 255f);
                    float scaleY = Mathf.Lerp(decoLayers[i].minHeight, decoLayers[i].maxHeight, alpha / 255f);
                    Vector3 decorScale = new Vector3(scaleX, scaleY, scaleX);

                    float xRatio = (x + 1f) / decoAlphamap.width;
                    float yRatio = (decoAlphamap.height - (y + 1f)) / decoAlphamap.height;

                    Vector3 decorPosition = new Vector3(terrain.terrainData.size.x * xRatio, 0, terrain.terrainData.size.z * yRatio);
                    decorPosition = decorPosition + terrain.transform.position;
                    decorPosition = new Vector3(decorPosition.x, terrain.SampleHeight(decorPosition) + decorPosition.y, decorPosition.z);

                    //GameObject dummy = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Dummy.prefab"));
                    GameObject dummy = GameObject.Instantiate(decoLayers[i].staticMesh);

                    dummy.transform.localScale = decorScale / 75f;
                    dummy.transform.position = decorPosition;
                    dummy.transform.parent = decoLayerSubBase.transform;
                }
            }
        }
    }
}
