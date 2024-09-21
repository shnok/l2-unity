#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2ParticleEmitterParser
{
    [MenuItem("Shnok/[SkillEffects] (UC) Build ambient skilleffect")]
    static void BuildSkillMenu()
    {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "uc";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess))
        {
            Debug.Log("Selected file: " + fileToProcess);

            GameObject container = new GameObject(Path.GetFileNameWithoutExtension(fileToProcess));

            foreach (L2Emitter emitter in ParseParticleEmitterFile(fileToProcess))
            {
                GameObject emitterObject = BuildEmitter(emitter);
                if (emitterObject != null)
                {
                    emitterObject.transform.SetParent(container.transform);
                }

                for (int i = 0; i < emitter.maxParticles - 1; i++)
                {
                    GameObject copies = GameObject.Instantiate(emitterObject);
                    if (emitterObject != null)
                    {
                        copies.transform.SetParent(container.transform);
                    }
                }
            }

            container.SetActive(false);
            container.AddComponent<ParticleTimerResetGroup>().enabled = false;
            container.SetActive(true);

            string saveFolder = Path.Combine("Assets", "Resources", "Data", "Effects", container.name);
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            string containerPrefabPath = Path.Combine(saveFolder, container.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(container, containerPrefabPath);
        }
    }

    private static List<L2Emitter> ParseParticleEmitterFile(string path)
    {
        List<L2Emitter> emitters = new List<L2Emitter>();

        using (StreamReader reader = new StreamReader(path))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                // Debug.Log(line);
                line = line.Trim();
                if (line.StartsWith("Begin Object Class=SpriteEmitter Name=") || line.StartsWith("Begin Object Class=MeshEmitter Name="))
                {
                    L2Emitter emitter = new L2Emitter();
                    emitter.effectName = Path.GetFileNameWithoutExtension(path);
                    emitter.objectName = line.Replace("Begin Object Class=SpriteEmitter Name=", "").Replace("Begin Object Class=MeshEmitter Name=", "");
                    Debug.Log("ObjectName=" + emitter.objectName);

                    while ((line = reader.ReadLine()) != null && !line.Contains("End Object"))
                    {
                        line = line.Replace("(", "").Replace(")", "");
                        line = line.Trim();
                        //Debug.Log(line);
                        if (line.StartsWith("StaticMesh="))
                        {
                            emitter.staticMesh = line.Replace("StaticMesh=StaticMesh'", "").Replace("'", "");
                            Debug.Log("StaticMesh=" + emitter.staticMesh);
                        }

                        if (line.StartsWith("Texture="))
                        {
                            emitter.texture = line.Replace("Texture=Texture'", "").Replace("'", "");
                            Debug.Log("Texture=" + emitter.texture);
                        }

                        if (line.StartsWith("UseColorScale="))
                        {
                            emitter.useColorScale = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("UseColorScale=" + emitter.useColorScale);
                        }

                        if (line.StartsWith("ColorScale"))
                        {
                            if (emitter.colorScales == null)
                            {
                                emitter.colorScales = new List<ColorScale>();
                            }

                            emitter.colorScales.Add(L2MetaDataUtils.ParseColorScale(line));
                            Debug.Log($"ColorScale({emitter.colorScales.Count - 1})={emitter.colorScales[emitter.colorScales.Count - 1]}");
                        }

                        if (line.StartsWith("ColorMultiplierRange"))
                        {
                            emitter.colorMultiplierRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log($"ColorMultiplierRange={emitter.colorMultiplierRange}");
                        }

                        if (line.StartsWith("Opacity="))
                        {
                            emitter.opacity = L2MetaDataUtils.ParseFloat(line);
                            Debug.Log("Opacity=" + emitter.opacity);
                        }

                        if (line.StartsWith("FadeIn="))
                        {
                            emitter.fadeIn = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("FadeIn=" + emitter.fadeIn);
                        }

                        if (line.StartsWith("FadeInEndTime="))
                        {
                            emitter.fadeInEndTime = L2MetaDataUtils.ParseFloat(line);
                            Debug.Log("FadeInEndTime=" + emitter.fadeInEndTime);
                        }

                        if (line.StartsWith("FadeOut="))
                        {
                            emitter.fadeOut = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("FadeOut=" + emitter.fadeOut);
                        }

                        if (line.StartsWith("FadeOutStartTime="))
                        {
                            emitter.fadeOutStartTime = L2MetaDataUtils.ParseFloat(line);
                            Debug.Log("FadeOutStartTime=" + emitter.fadeOutStartTime);
                        }

                        if (line.StartsWith("MaxParticles="))
                        {
                            emitter.maxParticles = L2MetaDataUtils.ParseInt(line);
                            Debug.Log("MaxParticles=" + emitter.maxParticles);
                        }

                        if (line.StartsWith("StartLocationOffset="))
                        {
                            emitter.startLocationOffset = L2MetaDataUtils.ParseVector3(line);
                            Debug.Log("StartLocationOffset=" + emitter.startLocationOffset);
                        }

                        if (line.StartsWith("StartLocationRange="))
                        {
                            emitter.startLocationRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("StartLocationRange=" + emitter.startLocationRange);
                        }

                        if (line.StartsWith("SpinParticles="))
                        {
                            emitter.spinParticles = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("SpinParticles=" + emitter.spinParticles);
                        }

                        if (line.StartsWith("SpinsPerSecondRange="))
                        {
                            emitter.spinsPerSecondRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("SpinsPerSecondRange=" + emitter.spinsPerSecondRange);
                        }

                        if (line.StartsWith("StartSpinRange="))
                        {
                            emitter.startSpinRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("StartSpinRange=" + emitter.startSpinRange);
                        }

                        if (line.StartsWith("UseSizeScale="))
                        {
                            emitter.useSizeScale = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("UseSizeScale=" + emitter.useSizeScale);
                        }

                        if (line.StartsWith("SizeScale"))
                        {
                            if (emitter.sizeScales == null)
                            {
                                emitter.sizeScales = new List<SizeScale>();
                            }

                            emitter.sizeScales.Add(L2MetaDataUtils.ParseSizeScale(line));
                            Debug.Log($"SizeScale({emitter.sizeScales.Count - 1})={emitter.sizeScales[emitter.sizeScales.Count - 1]}");
                        }

                        if (line.StartsWith("StartSizeRange="))
                        {
                            emitter.startSizeRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("StartSizeRange=" + emitter.startSizeRange);
                        }

                        if (line.StartsWith("StartVelocityRange="))
                        {
                            emitter.startVelocityRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("StartVelocityRange=" + emitter.startVelocityRange);
                        }

                        if (line.StartsWith("LifetimeRange="))
                        {
                            int equalsIndex = line.IndexOf('=');
                            line = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);

                            emitter.lifetimeRange = L2MetaDataUtils.ParseRange(line);
                            Debug.Log("LifetimeRange=" + emitter.lifetimeRange);
                        }

                        if (line.StartsWith("Name="))
                        {
                            emitter.name = line.Split("=")[1].Replace("\"", "");
                            Debug.Log("Name=" + emitter.name);
                        }

                        if (line.StartsWith("TextureUSubdivisions="))
                        {
                            emitter.TextureUSubdivisions = L2MetaDataUtils.ParseInt(line);
                            Debug.Log("TextureUSubdivisions=" + emitter.TextureUSubdivisions);
                        }

                        if (line.StartsWith("TextureVSubdivisions="))
                        {
                            emitter.TextureVSubdivisions = L2MetaDataUtils.ParseInt(line);
                            Debug.Log("TextureVSubdivisions=" + emitter.TextureVSubdivisions);
                        }

                        if (line.StartsWith("UseRandomSubdivision="))
                        {
                            emitter.UseRandomSubdivision = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("UseRandomSubdivision=" + emitter.UseRandomSubdivision);
                        }

                        if (line.StartsWith("SubdivisionStart="))
                        {
                            emitter.SubdivisionStart = L2MetaDataUtils.ParseInt(line);
                            Debug.Log("SubdivisionStart=" + emitter.SubdivisionStart);
                        }

                        if (line.StartsWith("SubdivisionEnd="))
                        {
                            emitter.SubdivisionEnd = L2MetaDataUtils.ParseInt(line);
                            Debug.Log("SubdivisionEnd=" + emitter.SubdivisionEnd);
                        }

                        if (line.StartsWith("DrawStyle="))
                        {
                            emitter.drawStyle = line.Split("=")[1];
                            Debug.Log("DrawStyle=" + emitter.drawStyle);
                        }
                    }

                    emitters.Add(emitter);
                }
            }
        }

        Debug.Log($"Parsed {emitters.Count} emitters.");

        return emitters;
    }

    private static GameObject BuildEmitter(L2Emitter emitter)
    {
        GameObject go;

        string texturePath;

        bool isTextureEmitter = false;
        if (emitter.texture != null && emitter.texture.Length > 0)
        {
            GameObject resource = (GameObject)Resources.Load("Prefab/SpriteEmitter");
            go = GameObject.Instantiate(resource);

            string[] textureValues = emitter.texture.Split(".");
            string textureName = textureValues.Length > 1 ? textureValues[2] : textureValues[1];
            texturePath = $"Data/SysTextures/{textureValues[0]}/{textureName}";
            isTextureEmitter = true;
        }
        else if (emitter.staticMesh != null && emitter.staticMesh.Length > 0)
        {

            string meshPath = StaticMeshUtils.GetMeshPath(emitter.staticMesh);
            GameObject resource = StaticMeshUtils.LoadMeshFromInfo(emitter.staticMesh);

            if (resource == null)
            {
                Debug.LogError($"Couldn't load emitter mesh {emitter.staticMesh} at path {meshPath}.");
                return null;
            }

            if (resource.transform.childCount > 0)
            {
                resource = resource.transform.GetChild(0).gameObject;
            }

            string materialName = resource.GetComponent<Renderer>().sharedMaterial.name;
            texturePath = $"Data/SysTextures/LineageEffectsTextures/{materialName}";
            //GameObject go = (GameObject)Resources.Load("Prefab/SpriteEmitter");
            go = GameObject.Instantiate(resource);
        }
        else
        {
            Debug.LogError("Emitter is neither a SpriteEmitter not a MeshEmitter.");
            return null;
        }


        Texture2D effectTexture = (Texture2D)Resources.Load(texturePath);
        Debug.Log(effectTexture);

        Material material = BuildMaterial(emitter, effectTexture, isTextureEmitter);

        // Save material
        string saveFolder = Path.Combine("Assets", "Resources", "Data", "Effects", emitter.effectName);
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string materialPath = Path.Combine(saveFolder, emitter.objectName + ".mat");

        // Save the material as an asset
        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        go.GetComponent<Renderer>().sharedMaterial = material;
        if (isTextureEmitter)
        {
            go.transform.localScale = new Vector3(1 / 52.5f, 1 / 52.5f, 1 / 52.5f);
            // Global scale multiplier... Dont know what the correct size, it seems the L2 size of texture emitters are not 1 by default
            go.transform.localScale *= 2f;
        }
        go.transform.name = emitter.objectName;
        return go;
    }

    private static Material BuildMaterial(L2Emitter emitter, Texture2D effectTexture, bool spriteEmitter)
    {
        Material material = new Material(Shader.Find("Shader Graphs/L2SkillEffect"));

        // Texture
        if (spriteEmitter)
        {
            material.SetFloat("_Billboard", 1f);
        }

        material.SetTexture("_MainTexture", effectTexture);
        material.SetFloat("_TextureUSubdivisions", emitter.TextureUSubdivisions > 0 ? emitter.TextureUSubdivisions : 1);
        material.SetFloat("_TextureVSubdivisions", emitter.TextureVSubdivisions > 0 ? emitter.TextureVSubdivisions : 1);
        material.SetFloat("_SubdivisionStart", emitter.SubdivisionStart > 0 ? emitter.SubdivisionStart - 1 : 1);
        material.SetFloat("_SubdivisionEnd", emitter.SubdivisionEnd > 0 ? emitter.SubdivisionEnd - 1 : 1);
        material.SetFloat("_Alpha", emitter.opacity > 0 && spriteEmitter ? emitter.opacity : 1);
        material.SetFloat("_Brighten", emitter.drawStyle != null && emitter.drawStyle == "PTDS_Brighten" ? 1 : 0);

        // Timer
        material.SetFloat("_HasLifetime", emitter.lifetimeRange.max > 0 || emitter.lifetimeRange.min > 0 ? 1 : 0);
        material.SetVector("_LifetimeRange", emitter.lifetimeRange.max > 0 || emitter.lifetimeRange.min > 0 ? new Vector2(emitter.lifetimeRange.min, emitter.lifetimeRange.max) : Vector2.zero);
        material.SetFloat("_Fadeout", emitter.fadeOut ? 1 : 0);
        material.SetFloat("_FadeoutStartTime", emitter.fadeOutStartTime);
        material.SetFloat("_FadeIn", emitter.fadeIn ? 1 : 0);
        material.SetFloat("_FadeInEndTime", emitter.fadeInEndTime);

        // SizeRange
        material.SetVector("_SizeRangeX", new Vector2(emitter.startSizeRange.x.min, emitter.startSizeRange.x.max));
        material.SetVector("_SizeRangeY", new Vector2(emitter.startSizeRange.y.min, emitter.startSizeRange.y.max)); //TODO verify x-y-z convertion between unity and unreal 
        material.SetVector("_SizeRangeZ", new Vector2(emitter.startSizeRange.z.min, emitter.startSizeRange.z.max));

        // ColorMultiplierRange
        material.SetVector("_ColorMultiplierRangeR", new Vector2(emitter.colorMultiplierRange.x.min, emitter.colorMultiplierRange.x.max));
        material.SetVector("_ColorMultiplierRangeG", new Vector2(emitter.colorMultiplierRange.y.min, emitter.colorMultiplierRange.y.max)); //TODO verify RGB conversion BGR ?
        material.SetVector("_ColorMultiplierRangeB", new Vector2(emitter.colorMultiplierRange.z.min, emitter.colorMultiplierRange.z.max));

        // SizeScale
        const int MAXMIMUM_SIZESCALE_COUNT = 10;
        material.SetFloat("_UseSizeScale", emitter.useSizeScale ? 1 : 0);
        if (emitter.sizeScales != null)
        {
            if (emitter.sizeScales.Count > MAXMIMUM_SIZESCALE_COUNT)
            {
                Debug.LogWarning($"SizeScale count[{emitter.sizeScales.Count}] is over the limit[{MAXMIMUM_SIZESCALE_COUNT}] for emitter {emitter.objectName}.");
            }
            for (int i = 0; i < Mathf.Min(emitter.sizeScales.Count, MAXMIMUM_SIZESCALE_COUNT); i++)
            {
                material.SetVector("_SizeScale" + i, new Vector2(emitter.sizeScales[i].relativeTime, emitter.sizeScales[i].relativeSize));
            }

            if (emitter.sizeScales.Count < MAXMIMUM_SIZESCALE_COUNT && emitter.sizeScales.Count > 0)
            {
                int lastIndex = emitter.sizeScales.Count - 1;
                for (int i = lastIndex; i < MAXMIMUM_SIZESCALE_COUNT; i++)
                {
                    material.SetVector("_SizeScale" + i, new Vector2(emitter.sizeScales[lastIndex].relativeTime, emitter.sizeScales[lastIndex].relativeSize));
                }
            }
        }

        // ColorScale
        const int MAXMIMUM_COLORSCALE_COUNT = 10;
        material.SetFloat("_UseColorScale", emitter.useColorScale ? 1 : 0);
        if (emitter.colorScales != null)
        {
            if (emitter.colorScales.Count > MAXMIMUM_COLORSCALE_COUNT)
            {
                Debug.LogWarning($"ColorScale count[{emitter.colorScales.Count}] is over the limit[{MAXMIMUM_COLORSCALE_COUNT}] for emitter {emitter.objectName}.");
            }
            for (int i = 0; i < Mathf.Min(emitter.colorScales.Count, MAXMIMUM_COLORSCALE_COUNT); i++)
            {
                material.SetFloat($"_ColorScale{i}Time", emitter.colorScales[i].relativeTime);

                material.SetColor($"_ColorScale{i}Color", new Color(
                    emitter.colorScales[i].r / 255f,
                    emitter.colorScales[i].g / 255f,
                    emitter.colorScales[i].g / 255f,
                    emitter.colorScales[i].a / 255f)); //TODO verify color conversions 

            }

            if (emitter.colorScales.Count < MAXMIMUM_COLORSCALE_COUNT && emitter.colorScales.Count > 0)
            {
                int lastIndex = emitter.colorScales.Count - 1;
                for (int i = lastIndex; i < MAXMIMUM_COLORSCALE_COUNT; i++)
                {
                    material.SetFloat($"_ColorScale{i}Time", emitter.colorScales[lastIndex].relativeTime);
                    material.SetColor($"_ColorScale{i}Color", new Color(
                        emitter.colorScales[lastIndex].r / 255f,
                        emitter.colorScales[lastIndex].g / 255f,
                        emitter.colorScales[lastIndex].g / 255f,
                        emitter.colorScales[lastIndex].a / 255f)); //TODO verify color conversions 

                }
            }
        }

        // Spin
        material.SetFloat("_SpinParticles", emitter.spinParticles ? 1 : 0);
        if (emitter.startSpinRange != null)
        {
            material.SetVector("_StartSpinRangeZ", new Vector2(emitter.startSpinRange.x.min, emitter.startSpinRange.x.max));
            material.SetVector("_StartSpinRangeY", new Vector2(emitter.startSpinRange.y.min, emitter.startSpinRange.y.max)); //TODO verify x-y-z convertion between unity and unreal 
            material.SetVector("_StartSpinRangeX", new Vector2(emitter.startSpinRange.z.min, emitter.startSpinRange.z.max));
        }
        if (emitter.spinsPerSecondRange != null)
        {
            material.SetVector("_SpinsPerSecondRangeZ", new Vector2(emitter.spinsPerSecondRange.x.min, emitter.spinsPerSecondRange.x.max));
            material.SetVector("_SpinsPerSecondRangeY", new Vector2(emitter.spinsPerSecondRange.y.min, emitter.spinsPerSecondRange.y.max)); //TODO verify x-y-z convertion between unity and unreal 
            material.SetVector("_SpinsPerSecondRangeX", new Vector2(emitter.spinsPerSecondRange.z.min, emitter.spinsPerSecondRange.z.max));
        }

        // Velocity
        if (emitter.startVelocityRange != null)
        {
            if (-emitter.startVelocityRange.x.max != emitter.startVelocityRange.x.min) // Can't understand the rule when min = -x and max = x....
            {
                material.SetVector("_StartVelocityRangeX", new Vector2(-emitter.startVelocityRange.x.max / 52.5f, -emitter.startVelocityRange.x.min / 52.5f));
            }
            if (-emitter.startVelocityRange.y.max != emitter.startVelocityRange.y.min) // Can't understand the rule when min = -x and max = x....
            {
                material.SetVector("_StartVelocityRangeY", new Vector2(-emitter.startVelocityRange.y.max / 52.5f, -emitter.startVelocityRange.y.min / 52.5f));
            }
            if (-emitter.startVelocityRange.z.max != emitter.startVelocityRange.z.min) // Can't understand the rule when min = -x and max = x....
            {
                material.SetVector("_StartVelocityRangeZ", new Vector2(-emitter.startVelocityRange.z.max / 52.5f, -emitter.startVelocityRange.z.min / 52.5f));
            }
        }

        // Location
        material.SetVector("_StartLocationOffset", -emitter.startLocationOffset / 52.5f);
        if (emitter.startLocationRange != null)
        {
            material.SetVector("_StartLocationRangeX", new Vector2(emitter.startLocationRange.x.min / 52.5f, emitter.startLocationRange.x.max / 52.5f));
            material.SetVector("_StartLocationRangeY", new Vector2(emitter.startLocationRange.y.min / 52.5f, emitter.startLocationRange.y.max / 52.5f)); //TODO verify x-y-z convertion between unity and unreal 
            material.SetVector("_StartLocationRangeZ", new Vector2(emitter.startLocationRange.z.min / 52.5f, emitter.startLocationRange.z.max / 52.5f));
        }

        return material;
    }
}
#endif
