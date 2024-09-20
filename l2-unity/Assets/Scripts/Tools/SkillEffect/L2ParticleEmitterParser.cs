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
            ParseParticleEmitterFile(fileToProcess);
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

                        if (line.StartsWith("Texture='"))
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
                            emitter.fadeOut = L2MetaDataUtils.ParseBool(line);
                            Debug.Log("UseSizeScale=" + emitter.fadeOut);
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
                    }

                    emitters.Add(emitter);
                }
            }
        }

        Debug.Log($"Parsed {emitters.Count} emitters.");

        return emitters;
    }
}
