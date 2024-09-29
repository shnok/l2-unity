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

        float drawScale = 1;

        using (StreamReader reader = new StreamReader(path))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("DrawScale="))
                {
                    drawScale = L2MetaDataUtils.ParseFloat(line);
                    Debug.Log("DrawScale=" + drawScale);
                }
            }
        }

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
                    emitter.drawScale = drawScale;
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

                        if (line.StartsWith("SizeScale") && !line.StartsWith("SizeScaleRepeats"))
                        {
                            if (emitter.sizeScales == null)
                            {
                                emitter.sizeScales = new List<SizeScale>();
                            }

                            emitter.sizeScales.Add(L2MetaDataUtils.ParseSizeScale(line));
                            Debug.Log($"SizeScale({emitter.sizeScales.Count - 1})={emitter.sizeScales[emitter.sizeScales.Count - 1]}");
                        }

                        if (line.StartsWith("SizeScaleRepeats"))
                        {
                            emitter.sizeScaleRepeats = L2MetaDataUtils.ParseFloat(line);
                            Debug.Log("SizeScaleRepeats=" + emitter.sizeScaleRepeats);
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

                        if (line.StartsWith("GetVelocityDirectionFrom="))
                        {
                            emitter.getVelocityDirectionFrom = line.Split("=")[1];
                            Debug.Log("GetVelocityDirectionFrom=" + emitter.getVelocityDirectionFrom);
                        }

                        if (line.StartsWith("VelocityLossRange="))
                        {
                            emitter.velocityLossRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("VelocityLossRange=" + emitter.velocityLossRange);
                        }

                        if (line.StartsWith("StartLocationShape="))
                        {
                            emitter.StartLocationShape = line.Split("=")[1];
                            Debug.Log("StartLocationShape=" + emitter.StartLocationShape);
                        }

                        if (line.StartsWith("StartLocationPolarRange="))
                        {
                            emitter.startLocationPolarRange = L2MetaDataUtils.ParseRange3D(line);
                            Debug.Log("StartLocationPolarRange=" + emitter.startLocationPolarRange);
                        }

                        if (line.StartsWith("SphereRadiusRange="))
                        {
                            int equalsIndex = line.IndexOf('=');
                            line = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);

                            emitter.sphereRadiusRange = L2MetaDataUtils.ParseRange(line);
                            Debug.Log("SphereRadiusRange=" + emitter.sphereRadiusRange);
                        }

                        if (line.StartsWith("UseDirectionAs="))
                        {
                            emitter.useDirectionAs = line.Split("=")[1];
                            Debug.Log("UseDirectionAs=" + emitter.useDirectionAs);
                        }

                        if (line.StartsWith("ProjectionNormal="))
                        {
                            emitter.projectionNormal = L2MetaDataUtils.ParseVector3(line);
                            Debug.Log("ProjectionNormal=" + emitter.projectionNormal);
                        }

                        if (line.StartsWith("UniformSize="))
                        {
                            emitter.uniformSize = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("UniformSize=" + emitter.uniformSize);
                        }

                        if (line.StartsWith("Acceleration="))
                        {
                            emitter.acceleration = L2MetaDataUtils.ParseVector3(line);
                            Debug.Log("Acceleration=" + emitter.acceleration);
                        }

                        if (line.StartsWith("InitialDelayRange="))
                        {
                            int equalsIndex = line.IndexOf('=');
                            line = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
                            emitter.initialDelayRange = L2MetaDataUtils.ParseRange(line);
                            Debug.Log("InitialDelayRange=" + emitter.initialDelayRange);
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
            //go.transform.localScale = new Vector3(100 * emitter.drawScale, 100 * emitter.drawScale, 100 * emitter.drawScale);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            Debug.LogError("Emitter is neither a SpriteEmitter not a MeshEmitter.");
            return null;
        }


        Texture2D effectTexture = (Texture2D)Resources.Load(texturePath);
        if (effectTexture == null)
        {
            Debug.LogWarning("Missing texture: " + texturePath);
        }
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

        material.SetTexture("_MainTexture", effectTexture);
        material.SetFloat("_TextureUSubdivisions", emitter.TextureUSubdivisions > 0 ? emitter.TextureUSubdivisions : 1);
        material.SetFloat("_TextureVSubdivisions", emitter.TextureVSubdivisions > 0 ? emitter.TextureVSubdivisions : 1);
        material.SetFloat("_SubdivisionStart", emitter.SubdivisionStart > 0 ? emitter.SubdivisionStart - 1 : 1);
        material.SetFloat("_SubdivisionEnd", emitter.SubdivisionEnd > 0 ? emitter.SubdivisionEnd - 1 : 1);
        material.SetFloat("_Alpha", emitter.opacity > 0 ? emitter.opacity : 1);
        material.SetFloat("_Brighten", emitter.drawStyle != null && emitter.drawStyle == "PTDS_Brighten" ? 1 : 0);

        // Timer
        if (emitter.initialDelayRange != null)
        {
            material.SetVector("_InitialDelayRange", new Vector2(emitter.initialDelayRange.min, emitter.initialDelayRange.max > 0 ? emitter.initialDelayRange.max : (emitter.initialDelayRange.min > 0 ? emitter.initialDelayRange.min : 0)));
        }

        if (emitter.lifetimeRange != null)
        {
            material.SetFloat("_HasLifetime", emitter.lifetimeRange.max > 0 || emitter.lifetimeRange.min > 0 ? 1 : 0);
            material.SetVector("_LifetimeRange", emitter.lifetimeRange.max > 0 || emitter.lifetimeRange.min > 0 ? new Vector2(emitter.lifetimeRange.min, emitter.lifetimeRange.max) : Vector2.zero);
        }
        else
        {
            material.SetFloat("_HasLifetime", 0);
        }
        material.SetFloat("_Fadeout", emitter.fadeOut ? 1 : 0);
        material.SetFloat("_FadeoutStartTime", emitter.fadeOutStartTime);
        material.SetFloat("_FadeIn", emitter.fadeIn ? 1 : 0);
        material.SetFloat("_FadeInEndTime", emitter.fadeInEndTime);

        // SizeRange
        if (spriteEmitter)
        {
            if (emitter.startSizeRange != null)
            {
                material.SetVector("_SizeRangeX", new Vector2(emitter.startSizeRange.x.min, emitter.startSizeRange.x.max));
                material.SetVector("_SizeRangeY", new Vector2(emitter.startSizeRange.y.min, emitter.startSizeRange.y.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_SizeRangeZ", new Vector2(emitter.startSizeRange.z.min, emitter.startSizeRange.z.max));
            }
        }
        else
        {
            if (emitter.startSizeRange != null)
            {
                material.SetVector("_SizeRangeX", new Vector2(emitter.startSizeRange.x.min, emitter.startSizeRange.x.max));
                material.SetVector("_SizeRangeY", new Vector2(emitter.startSizeRange.z.min, emitter.startSizeRange.z.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_SizeRangeZ", new Vector2(emitter.startSizeRange.y.min, emitter.startSizeRange.y.max));
            }

        }

        // ColorMultiplierRange
        if (emitter.colorMultiplierRange != null)
        {
            material.SetVector("_ColorMultiplierRangeR", new Vector2(emitter.colorMultiplierRange.x.min, emitter.colorMultiplierRange.x.max > 0 ? emitter.colorMultiplierRange.x.max : (emitter.colorMultiplierRange.x.min > 0 ? emitter.colorMultiplierRange.x.min : 0)));
            material.SetVector("_ColorMultiplierRangeG", new Vector2(emitter.colorMultiplierRange.y.min, emitter.colorMultiplierRange.y.max > 0 ? emitter.colorMultiplierRange.y.max : (emitter.colorMultiplierRange.y.min > 0 ? emitter.colorMultiplierRange.y.min : 0)));
            material.SetVector("_ColorMultiplierRangeB", new Vector2(emitter.colorMultiplierRange.z.min, emitter.colorMultiplierRange.z.max > 0 ? emitter.colorMultiplierRange.z.max : (emitter.colorMultiplierRange.z.min > 0 ? emitter.colorMultiplierRange.z.min : 0)));
        }

        // SizeScale
        const int MAXMIMUM_SIZESCALE_COUNT = 10;
        material.SetFloat("_UseSizeScale", emitter.useSizeScale ? 1 : 0);
        material.SetFloat("_UniformSize", emitter.uniformSize ? 1 : 0);
        material.SetFloat("_SizeScaleRepeats", emitter.sizeScaleRepeats);
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
        if (!spriteEmitter)
        {
            if (emitter.startSpinRange != null)
            {
                material.SetVector("_StartSpinRangeX", new Vector2(emitter.startSpinRange.z.min, emitter.startSpinRange.z.max));
                material.SetVector("_StartSpinRangeY", new Vector2(emitter.startSpinRange.x.min, emitter.startSpinRange.x.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_StartSpinRangeZ", new Vector2(emitter.startSpinRange.y.min, emitter.startSpinRange.y.max));
            }
            if (emitter.spinsPerSecondRange != null)
            {
                material.SetVector("_SpinsPerSecondRangeX", new Vector2(emitter.spinsPerSecondRange.z.min, emitter.spinsPerSecondRange.z.max));
                material.SetVector("_SpinsPerSecondRangeY", new Vector2(emitter.spinsPerSecondRange.x.min, emitter.spinsPerSecondRange.x.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_SpinsPerSecondRangeZ", new Vector2(emitter.spinsPerSecondRange.y.min, emitter.spinsPerSecondRange.y.max));
            }
        }
        else
        {
            if (emitter.startSpinRange != null)
            {
                material.SetVector("_StartSpinRangeX", new Vector2(emitter.startSpinRange.y.min, emitter.startSpinRange.y.max));
                material.SetVector("_StartSpinRangeY", new Vector2(emitter.startSpinRange.z.min, emitter.startSpinRange.z.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_StartSpinRangeZ", new Vector2(emitter.startSpinRange.x.min, emitter.startSpinRange.x.max));
            }
            if (emitter.spinsPerSecondRange != null)
            {
                material.SetVector("_SpinsPerSecondRangeX", new Vector2(emitter.spinsPerSecondRange.y.min, emitter.spinsPerSecondRange.y.max));
                material.SetVector("_SpinsPerSecondRangeY", new Vector2(emitter.spinsPerSecondRange.z.min, emitter.spinsPerSecondRange.z.max)); //TODO verify x-y-z convertion between unity and unreal 
                material.SetVector("_SpinsPerSecondRangeZ", new Vector2(emitter.spinsPerSecondRange.x.min, emitter.spinsPerSecondRange.x.max));
            }
        }


        // Velocity
        if (emitter.startVelocityRange != null)
        {
            // if (-emitter.startVelocityRange.x.max != emitter.startVelocityRange.x.min) // Can't understand the rule when min = -x and max = x....
            //  {
            material.SetVector("_StartVelocityRangeX", new Vector2(-emitter.startVelocityRange.x.max / 52.5f, -emitter.startVelocityRange.x.min / 52.5f));
            //  }
            //  if (-emitter.startVelocityRange.y.max != emitter.startVelocityRange.y.min) // Can't understand the rule when min = -x and max = x....
            //  {
            material.SetVector("_StartVelocityRangeY", new Vector2(emitter.startVelocityRange.z.max / 52.5f, emitter.startVelocityRange.z.min / 52.5f));
            //  }
            //  if (-emitter.startVelocityRange.z.max != emitter.startVelocityRange.z.min) // Can't understand the rule when min = -x and max = x....
            //  {
            material.SetVector("_StartVelocityRangeZ", new Vector2(emitter.startVelocityRange.y.max / 52.5f, emitter.startVelocityRange.y.min / 52.5f));
            //  }
        }

        //Acceleration
        material.SetVector("_Acceleration", new Vector3(emitter.acceleration.x, emitter.acceleration.z, emitter.acceleration.y) / 52.5f);

        // SizeRange
        if (emitter.velocityLossRange != null)
        {
            material.SetVector("_VelocityLossRangeX", new Vector2(emitter.velocityLossRange.x.min, emitter.velocityLossRange.x.max));
            material.SetVector("_VelocityLossRangeY", new Vector2(emitter.velocityLossRange.z.min, emitter.velocityLossRange.z.max)); //TODO verify x-y-z convertion between unity and unreal 
            material.SetVector("_VelocityLossRangeZ", new Vector2(emitter.velocityLossRange.y.min, emitter.velocityLossRange.y.max));
        }


        if (emitter.getVelocityDirectionFrom != null && emitter.getVelocityDirectionFrom.Length > 0)
        {
            switch (emitter.getVelocityDirectionFrom)
            {
                case "PTVD_None":
                    material.SetFloat("_GetVelocityDirectionFrom", 0);
                    break;

                case "PTVD_OwnerAndStartPosition":
                    material.SetFloat("_GetVelocityDirectionFrom", 1);
                    break;

                case "PTVD_StartPositionAndOwner":
                    material.SetFloat("_GetVelocityDirectionFrom", 2);
                    break;

                case "PTVD_AddRadial":
                    Debug.LogError("Mode still not supported.");
                    material.SetFloat("_GetVelocityDirectionFrom", 3);
                    break;
            }

        }


        // Location
        if (emitter.StartLocationShape != null && emitter.StartLocationShape.Length > 0)
        {
            switch (emitter.StartLocationShape)
            {
                case "PTLS_Box":
                    material.SetFloat("_StartLocationShape", 0);
                    break;
                case "PTLS_Sphere":
                    Debug.LogError("Mode still not supported.");
                    material.SetFloat("_StartLocationShape", 1);
                    break;
                case "PTLS_Polar":
                    material.SetFloat("_StartLocationShape", 2);
                    break;
                case "PTLS_All":
                    material.SetFloat("_StartLocationShape", 3);
                    break;
            }
        }

        material.SetVector("_StartLocationOffset", FromUnrealToUnityShader(emitter.startLocationOffset));

        if (emitter.startLocationRange != null)
        {
            material.SetVector("_StartLocationRangeX", new Vector2(-emitter.startLocationRange.x.max / 52.5f, -emitter.startLocationRange.x.min / 52.5f));
            material.SetVector("_StartLocationRangeY", new Vector2(emitter.startLocationRange.z.min / 52.5f, emitter.startLocationRange.z.max / 52.5f));
            material.SetVector("_StartLocationRangeZ", new Vector2(emitter.startLocationRange.y.min / 52.5f, emitter.startLocationRange.y.max / 52.5f));
        }

        if (emitter.sphereRadiusRange != null)
        {
            material.SetVector("_SphereRadiusRange", new Vector2(emitter.sphereRadiusRange.min / 52.5f, emitter.sphereRadiusRange.max / 52.5f));
        }


        if (emitter.startLocationPolarRange != null)
        {
            material.SetVector("_StartLocationPolarRangeX", new Vector2(emitter.startLocationPolarRange.x.min, emitter.startLocationPolarRange.x.max)); // Degrees
            material.SetVector("_StartLocationPolarRangeY", new Vector2(emitter.startLocationPolarRange.y.min, emitter.startLocationPolarRange.y.max)); // Degrees
            material.SetVector("_StartLocationPolarRangeZ", new Vector2(emitter.startLocationPolarRange.z.min / 52.5f, emitter.startLocationPolarRange.z.max / 52.5f)); // Y Offset
        }

        if (spriteEmitter)
        {
            if (emitter.useDirectionAs == null || emitter.useDirectionAs == "PTDU_None") // Billboard
            {
                material.SetFloat("_Billboard", 1f);
            }
            /*
                PTDU_NORMAL -> Parallel to the floor
                PTDU_UP -> Perpandicular to the floor (Y rotation still facing camera)
            */
            else if (emitter.useDirectionAs.ToUpper() == "PTDU_NORMAL") // Use parent rotation
            {
                material.SetFloat("_UseDirectionAs", 1);
            }
            else
            {
                // FOR NOW USE THE SAME VALUE FOR ANY OTHER TYPE
                material.SetFloat("_UseDirectionAs", 2);
            }

            if (emitter.projectionNormal != null)
            {
                material.SetVector("_ProjectionNormal", new Vector3(emitter.projectionNormal.y, emitter.projectionNormal.z, emitter.projectionNormal.x));
            }
        }


        return material;
    }
    private static Vector3 FromUnrealToUnityShader(Vector3 unrealVec)
    {
        return new Vector3(-unrealVec.x, unrealVec.z, unrealVec.y) / 52.5f;

    }
}
#endif
