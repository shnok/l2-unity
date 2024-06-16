#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public class DecoToMesh {
    public static GameObject ConvertDecoLayers(List<L2DecoLayer> decoLayers, Terrain terrain) {
        GameObject decoLayerBase = new GameObject("DecoLayer");
        decoLayerBase.transform.position = terrain.transform.position;
        decoLayerBase.isStatic = true;

        for(int i = 0; i < decoLayers.Count; i++) {
            Debug.Log($"Generating decolayer {i}, {decoLayers[i].densityMap.name} : {decoLayers[i].staticMesh.name}");
            Texture2D decoAlphamap = TextureUtils.RotateTexture(decoLayers[i].densityMap);
            decoAlphamap = TextureUtils.FlipTextureVertically(decoAlphamap);

            string path = decoLayers[i].densityMapPath;

            int decoGroupSize = 25;

            int decoCount = 0;
            int alphaCount = 0;

            Color32[] pixels = decoAlphamap.GetPixels32();
            for(int y = 0; y < decoAlphamap.height; y++) {
                for(int x = 0; x < decoAlphamap.width; x++) {
                    int index = y * decoAlphamap.width + x;

                    float alpha = pixels[index].a;
                    if(alpha == 0) {
                        continue;
                    }
                    alphaCount++;

                    // Calculate deco position
                    float xRatio = (x + 1f) / decoAlphamap.width;
                    float yRatio = (decoAlphamap.height - (y + 1f)) / decoAlphamap.height;
                    Vector3 basePosition = new Vector3(terrain.terrainData.size.x * xRatio, 0, terrain.terrainData.size.z * yRatio);
                    basePosition = basePosition + terrain.transform.position;

                    // Generate grid
                    float gridX = Mathf.Round(basePosition.x * decoGroupSize) / decoGroupSize;
                    float gridZ = Mathf.Round(basePosition.z * decoGroupSize) / decoGroupSize;
                    int gridIndexX = Mathf.FloorToInt((gridX - terrain.transform.position.x) / decoGroupSize);
                    int gridIndexZ = Mathf.FloorToInt((gridZ - terrain.transform.position.z) / decoGroupSize);

                    // Density noise
                    float noiseDensityMultiplier = 5.25f;
                    Vector2 noiseDensityCoord = new Vector2(basePosition.x, basePosition.z) * noiseDensityMultiplier;
                    float densityNoise = Mathf.PerlinNoise(noiseDensityCoord.x, noiseDensityCoord.y);

                    // Scatter noise
                    float scatterNoiseMultiplier = 12.25f;
                    float scatterRadius = 3f;
                    float scatterStep = .5f;
                    
                    for(float scatterX = -scatterRadius / 2f; scatterX < scatterRadius / 2f; scatterX += scatterStep) {
                        for(float scatterY = -scatterRadius / 2f; scatterY < scatterRadius / 2f; scatterY += scatterStep) {
                            Vector2 scatterNoiseCoord = new Vector2(basePosition.x + scatterX, basePosition.z + scatterY) * scatterNoiseMultiplier;
                            float scatterNoise = Mathf.PerlinNoise(scatterNoiseCoord.x, scatterNoiseCoord.y);

                            //bool shouldSpawnDeco = Mathf.Abs(scatterNoise - densityNoise) <= 0.012f * (i * 0.9f);

                            bool shouldSpawnDeco = Mathf.Abs(scatterNoise - densityNoise) <= 0.03f;
                            //if (i == 4) {
                            //    shouldSpawnDeco = Mathf.Abs(scatterNoise - densityNoise) <= 0.042f;
                            //}

                            if (shouldSpawnDeco) {
                                //GameObject dummy = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Dummy.prefab"));

                                Vector3 detailPos = basePosition + new Vector3(scatterX, 0, scatterY);

                                // Scale noise
                                float noiseScaleMultiplier = 10f;
                                Vector2 noiseScaleCoord = new Vector2(detailPos.x, detailPos.z) * noiseScaleMultiplier;
                                float scaleNoise = Mathf.PerlinNoise(noiseScaleCoord.x, noiseScaleCoord.y);

                                //float decorScaleMultiplier = 1.0f / 52.5f; // UE to unity ratio
                                float maxWidth = decoLayers[i].maxWidth;
                                float maxHeight = decoLayers[i].maxHeight;
                                //if (i == 4) {
                                //    maxWidth = maxWidth * 1.7f;
                                //    maxHeight = maxHeight * 1.7f;
                                //}

                                float scaleX = Mathf.Lerp(decoLayers[i].minWidth, maxWidth, scaleNoise);
                                float scaleY = Mathf.Lerp(decoLayers[i].minHeight, maxHeight, scaleNoise);
                                Vector3 decorScale = new Vector3(scaleX, scaleY, scaleX);

                                // Rotation noise
                                float noiseRotationMultiplier = 1000f;
                                Vector2 noiseRotationCoord = new Vector2(detailPos.x, detailPos.z) * noiseRotationMultiplier;
                                float rotationNoise = Mathf.PerlinNoise(noiseRotationCoord.x, noiseRotationCoord.y);
                                float rotation = Mathf.Lerp(0f, 325f, rotationNoise);

                                decoCount++;
                                //Generate GRID Object if missing
                                //GameObject gridBaseObject = GameObject.Find(terrain.name + "_" + gridIndexX + "_" + gridIndexZ);
                                //if (gridBaseObject == null) {
                                //    gridBaseObject = new GameObject(terrain.name + "_" + gridIndexX + "_" + gridIndexZ);
                                //    gridBaseObject.transform.parent = decoLayerBase.transform;
                                //    gridBaseObject.transform.position = new Vector3(gridX + decoGroupSize / 2f, 0, gridZ + decoGroupSize / 2f);
                                //    gridBaseObject.isStatic = true;
                                //}

                                GameObject dummy = GameObject.Instantiate(decoLayers[i].staticMesh);
                                dummy.transform.eulerAngles = new Vector3(dummy.transform.eulerAngles.x, rotation, dummy.transform.eulerAngles.z);
                                dummy.transform.localScale = decorScale;
                                
                       
                                Vector3 originalPos = new Vector3(detailPos.x, detailPos.y, detailPos.z);
                                float height = GetGroundHeight(originalPos);
                                originalPos.y = height;
                                dummy.transform.position = originalPos;
                                dummy.transform.parent = decoLayerBase.transform;
                                dummy.isStatic = true;
                            }
                        }
                    }
                }           
            }

            Debug.Log($"Deco count for layer {i} : {decoCount}");
            Debug.Log($"Alpha count for layer {i} : {alphaCount}");
        }

        return decoLayerBase;
    }

    private static float GetGroundHeight(Vector3 pos) {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 200.0f, Vector3.down, out hit, 200f, 1 << 9)) {
            return hit.point.y;
        }

        return -1;
    }
}
#endif
