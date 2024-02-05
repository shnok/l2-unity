#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2MaterialBuilder {

    [MenuItem("Shnok/[Material] Generate")]
    static void SetupMaterials() {

        bool overwrite = false;
        if(overwrite) {
            ClearMaterials();
        }

        ProcessProps(overwrite);
        CreateBaseMaterials(overwrite);
        //AssetDatabase.DeleteAsset(materialPath);
    }

    [MenuItem("Shnok/[Material] Remap")]
    static void RemapMaterials() {
        string[] meshes = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Resources/Data/StaticMeshes" });
        for(int i = 0; i < meshes.Length; i++) {

            string mesh = AssetDatabase.GUIDToAssetPath(meshes[i]);

            AssetDatabase.ImportAsset(mesh, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
    }

    static void ClearMaterials() {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        for(int i = 0; i < materialGUIDs.Length; i++) {
            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUIDs[i]);
            AssetDatabase.DeleteAsset(materialPath);
        }
    }

    static void CreateBaseMaterials(bool overwrite) {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        for(int i = 0; i < textureGUIDs.Length; i++) {
            string texturePath = AssetDatabase.GUIDToAssetPath(textureGUIDs[i]);
            string materialDirectory = Path.Combine(Path.GetDirectoryName(texturePath), "Materials");
            string materialPath = Path.Combine(materialDirectory, Path.GetFileNameWithoutExtension(texturePath) + ".mat");

            string ignorePath = Path.Combine(Path.GetDirectoryName(texturePath), ".ignore");
            if(File.Exists(ignorePath)) {
                Debug.Log("Ignoring folder");
                continue;
            }

            if(materialPath.EndsWith("_ori.mat") || materialPath.EndsWith("_sp.mat")) {
                Debug.Log("Skipping materials with props");
                continue;
            }

            if (!overwrite && File.Exists(materialPath)) {
                continue;
            }

            if(!Directory.Exists(materialDirectory)) {
                Directory.CreateDirectory(materialDirectory);
            }

            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Smoothness", 0);
            material.SetFloat("_EnvironmentReflections", 0f);
            material.SetFloat("_SpecularHighlights", 0f);


            if (materialPath.EndsWith("_h.mat")) {
                material.SetFloat("_AlphaClip", 1f);
                material.SetFloat("_Cutoff", 0.5f);
                Debug.Log("Setting hair as alpha");
            }

            ApplyTextureToMaterial(material, texturePath, null);
            Debug.Log(materialPath);
            AssetDatabase.CreateAsset(material, materialPath);
        }
    }

    static void ProcessProps(bool overwrite) {
        string[] propsTxtGUIDs = AssetDatabase.FindAssets("t:TextAsset", new string[] { "Assets/Resources/Data/Textures", "Assets/Resources/Data/SysTextures" });
        //Debug.Log("Found " + propsTxtGUIDs.Length + " props.");

        for(int i = 0; i < propsTxtGUIDs.Length; i++) {
            string textureName = string.Empty;
            string specularTextureName = string.Empty;
            string propsPath = AssetDatabase.GUIDToAssetPath(propsTxtGUIDs[i]);
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Smoothness", 0);
            material.SetFloat("_EnvironmentReflections", 0f);
            material.SetFloat("_SpecularHighlights", 0f);

            using(StreamReader reader = new StreamReader(propsPath)) {
                string line;
                while((line = reader.ReadLine()) != null) {
                    if(line.StartsWith("Diffuse") || line.StartsWith("Material")) {
                        string value = line.Split("=")[1].Trim();
                        if(value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            textureName = texRefEntries[texRefEntries.Length - 1];
                            Debug.Log("Texture: " + textureName);
                        }
                    } else if (line.StartsWith("SpecularityMask")) {
                        string value = line.Split("=")[1].Trim();
                        if (value.StartsWith("Texture")) {
                            string texRef = value.Substring(8);
                            texRef = texRef.Substring(0, texRef.Length - 1);
                            string[] texRefEntries = texRef.Split('.');
                            specularTextureName = texRefEntries[texRefEntries.Length - 1];
                            Debug.Log("Specular texture: " + specularTextureName);
                        }
                    } else if(line.StartsWith("TwoSided")) {
                        string value = line.Split("=")[1].Trim();
                        float state = (value == "true") ? 0f : 2f;
                        Debug.Log("TwoSided:" + state);
                        material.SetFloat("_Cull", state);
                    } else if(line.StartsWith("AlphaTest")) {
                        string value = line.Split("=")[1].Trim();
                        if(value == "true") {
                            material.SetFloat("_AlphaClip", 1f);
                        }
                        Debug.Log("AlphaTest:" + value);
                    } else if(line.StartsWith("OutputBlending")) {
                        string value = line.Split("=")[1].Trim();
                        Debug.Log("OutputBlending:");

                        if (value.StartsWith("OB_Masked")) {
                            material.SetFloat("_AlphaClip", 1f);
                        } else if(value.StartsWith("OB_Brighten")) {
                            material.shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                            material.SetFloat("_Surface", 1f);
                            material.SetFloat("_Blend", 2f);
                            material.SetFloat("_BlendOp", 0f);
                            material.SetFloat("_ColorMode", 0f);
                            material.SetFloat("_DstBlend", 1f);
                            material.SetFloat("_DstBlendAlpha", 1f);
                        }
                    }
                }
            }


            string materialPath = Path.Combine(
                Path.GetDirectoryName(propsPath), 
                Path.GetFileNameWithoutExtension(propsPath)
                    .Replace(".props", string.Empty)
                    .Replace("_sh", string.Empty) + ".mat");

            if(!overwrite && File.Exists(materialPath)) {
                continue;
            }

            string oldMaterialPath = Path.Combine(Path.GetDirectoryName(propsPath), textureName + ".mat");
            AssetDatabase.DeleteAsset(oldMaterialPath);

            string materialDirectory = Path.GetDirectoryName(materialPath);
            string parentFolder = Directory.GetParent(materialDirectory).FullName;
            string texturePath = Path.Combine(parentFolder, textureName + ".png");
            texturePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, texturePath));
            string specularTexturePath = null;
            if(specularTextureName.Length > 0) {
                specularTexturePath = Path.Combine(parentFolder, specularTextureName + ".png");
                specularTexturePath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, specularTexturePath));
            }

            ApplyTextureToMaterial(material, texturePath, specularTexturePath);

            if(!Directory.Exists(materialDirectory)) {
                Directory.CreateDirectory(materialDirectory);
            }

            AssetDatabase.CreateAsset(material, materialPath);
        }
    }

    static void ApplyTextureToMaterial(Material material, string texturePath, string specularTexturePath) {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        Texture2D specularMap = null;
        if (specularTexturePath != null) {
            specularMap = AssetDatabase.LoadAssetAtPath<Texture2D>(specularTexturePath);
            if(specularMap == null) {
                Debug.LogError("NO SPEC TEX: " + specularTexturePath);
            } else {
                Debug.Log("Loaded specular map " + specularTexturePath);
            }
        }

        if(texture != null) {
            if (specularMap != null) {
                Debug.Log("Setting specular map");
                material.EnableKeyword("_METALLICSPECGLOSSMAP");
                material.EnableKeyword("_SPECULAR_SETUP");
                material.SetFloat("_WorkflowMode", 0);
                material.SetFloat("_Smoothness", 1);
                material.SetTexture("_SpecGlossMap", specularMap);
                material.SetTexture("_METALLICSPECGLOSSMAP", specularMap);
                material.SetTexture("_Specular", specularMap);

            }
            material.SetTexture("_MainTex", texture);
            material.SetTexture("_BaseMap", texture);
  
        } else {
            Debug.LogError("NO TEX: " + texturePath);
        }

        Debug.Log("Applied texture to " + texturePath);
    }
}
#endif