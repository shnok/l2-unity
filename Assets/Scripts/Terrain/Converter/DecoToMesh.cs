using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DecoToMesh
{
    public static GameObject ConvertDecoLayers(List<L2DecoLayer> decoLayers, Terrain terrain) {
        GameObject decoLayerBase = new GameObject("DecoLayer");
        decoLayerBase.transform.position = terrain.transform.position;
        decoLayerBase.isStatic = true;

        for(int i = 0; i < decoLayers.Count; i++) {
            //Texture2D decoAlphamap = TextureUtils.FlipTextureVertically(decoLayers[i].densityMap);
            Texture2D decoAlphamap = TextureUtils.RotateTexture(decoLayers[i].densityMap);
            decoAlphamap = TextureUtils.FlipTextureVertically(decoAlphamap);

            string path = decoLayers[i].densityMapPath;

            Debug.Log(path);
            GameObject decoLayerSubBase = new GameObject("DecoLayer_" + i);
            decoLayerSubBase.transform.parent = decoLayerBase.transform;
            decoLayerSubBase.transform.position = decoLayerBase.transform.position;
            decoLayerSubBase.isStatic = true;

            Color32[] pixels = decoAlphamap.GetPixels32();
            for(int y = 0; y < decoAlphamap.height; y++) {
                for(int x = 0; x < decoAlphamap.width; x++) {
                    int index = y * decoAlphamap.width + x;

                    float alpha = pixels[index].a;
                    if(alpha == 0) {
                        continue;
                    }

                    float xRatio = (x + 1f) / decoAlphamap.width;
                    float yRatio = (decoAlphamap.height - (y + 1f)) / decoAlphamap.height;

                    Vector3 basePosition = new Vector3(terrain.terrainData.size.x * xRatio, 0, terrain.terrainData.size.z * yRatio);
                    basePosition = basePosition + terrain.transform.position;

                    // Density noise
                    float noiseDensityMultiplier = 5.25f;
                    Vector2 noiseDensityCoord = new Vector2(basePosition.x, basePosition.z) * noiseDensityMultiplier;
                    float densityNoise = Mathf.PerlinNoise(noiseDensityCoord.x, noiseDensityCoord.y);

                    // Scatter noise
                    float scatterNoiseMultiplier = 5.25f;
                    float scatterRadius = 3f;
                    float scatterStep = .5f;
                    for(float scatterX = -scatterRadius / 2f; scatterX < scatterRadius / 2f; scatterX += scatterStep) {
                        for(float scatterY = -scatterRadius / 2f; scatterY < scatterRadius / 2f; scatterY += scatterStep) {
                            Vector2 scatterNoiseCoord = new Vector2(basePosition.x + scatterX, basePosition.z + scatterY) * scatterNoiseMultiplier;
                            float scatterNoise = Mathf.PerlinNoise(scatterNoiseCoord.x, scatterNoiseCoord.y);

                            if(Mathf.Abs(scatterNoise - densityNoise) <= 0.012f * (i*0.9f)) {
                                //GameObject dummy = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Dummy.prefab"));
                                GameObject dummy = GameObject.Instantiate(decoLayers[i].staticMesh);

                                Vector3 detailPos = basePosition + new Vector3(scatterX, 0, scatterY);

                                // Scale noise
                                float noiseScaleMultiplier = 10f;
                                Vector2 noiseScaleCoord = new Vector2(detailPos.x, detailPos.z) * noiseScaleMultiplier;
                                float scaleNoise = Mathf.PerlinNoise(noiseScaleCoord.x, noiseScaleCoord.y);

                                float decorScaleMultiplier = 1.0f / 50f;
                                float scaleX = Mathf.Lerp(decoLayers[i].minWidth, decoLayers[i].maxWidth, scaleNoise);
                                float scaleY = Mathf.Lerp(decoLayers[i].minHeight, decoLayers[i].maxHeight, scaleNoise);
                                Vector3 decorScale = new Vector3(scaleX, scaleY, scaleX);

                                // Rotation noise
                                float noiseRotationMultiplier = 1000f;
                                Vector2 noiseRotationCoord = new Vector2(detailPos.x, detailPos.z) * noiseRotationMultiplier;
                                float rotationNoise = Mathf.PerlinNoise(noiseRotationCoord.x, noiseRotationCoord.y);
                                float rotation = Mathf.Lerp(0f, 325f, rotationNoise);

                                dummy.transform.eulerAngles = new Vector3(dummy.transform.eulerAngles.x, rotation, dummy.transform.eulerAngles.z);
                                dummy.transform.localScale = decorScale * decorScaleMultiplier;                             
                                dummy.transform.position = new Vector3(detailPos.x, terrain.SampleHeight(detailPos) + detailPos.y, detailPos.z);
                                dummy.transform.parent = decoLayerSubBase.transform;

                                dummy.isStatic = true;
                            }
                        }
                    }             
                }
            }
        }

        return decoLayerBase;
    }
}
