using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DecoToMesh
{
    public static void ConvertDecoLayers(List<L2DecoLayer> decoLayers, GameObject terrain) {
        for(int i = 0; i < decoLayers.Count; i++) {
            Texture2D decoAlphamap = decoLayers[i].densityMap;

            string path = decoLayers[i].densityMapPath;

            Debug.Log(path);

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
                    Vector3 decorPosition = new Vector3(terrain.transform.localScale.x * xRatio, 0, terrain.transform.localScale.z * yRatio);
                    decorPosition = decorPosition + terrain.transform.position;

                    GameObject dummy = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Dummy.prefab"));
                    dummy.transform.localScale = decorScale;
                    dummy.transform.position = decorPosition;
                    Debug.Log(alpha);
                }
            }
        }
    }
}
