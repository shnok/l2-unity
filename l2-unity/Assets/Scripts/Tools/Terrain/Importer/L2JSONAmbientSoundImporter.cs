#if (UNITY_EDITOR) 
using FMODUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AmbientDataContainer {
    public AmbientSound[] data;
}

public class L2JSONAmbientSoundImporter : AssetImporter {

    // umodel export folder
    static string dataFolder = @"D:\Stock\Projects\L2-Unity\umodel_win32\export";
    // copy ambient sounds to
    static string scriptExportFolder = @"D:\Stock\Projects\L2-Unity\umodel_win32\export\unityexport";

    [MenuItem("Shnok/[AmbientSound] Import sounds")]
    static void ImportSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            ImportAmbientSoundFiles(fileToProcess, dataFolder);
        }
    }

    [MenuItem("Shnok/[AmbientSound] Build ambient sounds")]
    static void BuildSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if(!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            BuildAmbientSounds(fileToProcess);
        }
    }

    private static void BuildAmbientSounds(string fileToProcess) {
        AmbientSound[] sounds = ParseAmbientSoundFile(fileToProcess);
        Debug.Log(sounds.Length);

        GameObject container = new GameObject(Path.GetFileNameWithoutExtension(fileToProcess));

        foreach(AmbientSound ambientSound in sounds) {
            Debug.Log(ambientSound);

            GameObject soundObject = BuildAmbientSoundGameObject(ambientSound);
            soundObject.transform.parent = container.transform;
        }
        Debug.Log($"Need to {sounds.Length} load ambient sounds.");
    }

    private static GameObject BuildAmbientSoundGameObject(AmbientSound ambientSound) {

        GameObject go = new GameObject(ambientSound.name);

        // collider
        SphereCollider collider = go.AddComponent<SphereCollider>();
        float soundRadius = ambientSound.soundRadius / 52.5f * 14f;
        collider.radius = soundRadius;
        collider.isTrigger = true;

        go.transform.position = VectorUtils.ConvertToUnity(ambientSound.position);
        go.layer = LayerMask.NameToLayer("AmbientSound");

        // event
        AmbientSoundEmitter emitter = go.AddComponent<AmbientSoundEmitter>();
        string[] parts = ambientSound.ambientSoundName.Split(".");
        string eventName = $"event:/AmbSound/{parts[0]}/{parts[2]}";
        EventReference er = RuntimeManager.PathToEventReference(eventName);

        emitter.EventReference = er;
        emitter.OverrideAttenuation = true;
        emitter.OverrideMinDistance = soundRadius * 0.05f;
        emitter.OverrideMaxDistance = soundRadius;
        emitter.Loop = true;
        emitter.PlayEvent = EmitterGameEvent.TriggerEnter;
        emitter.StopEvent = EmitterGameEvent.TriggerExit;
        emitter.CollisionTag = "Player";
        emitter.AllowFadeout = true;
        if(ambientSound.ambientRandom == 0) {
            emitter.PlayChancePercent = 100;
            emitter.LoopDelaySeconds = 0;
        } else {
            emitter.PlayChancePercent = ambientSound.ambientRandom;
            emitter.LoopDelaySeconds = 3;
        }

        // from UE pitch to FMOD
        float uePitch = ambientSound.soundPitch;
        float fmodPitch = uePitch / 100f + 1f;
        emitter.SoundPitch = fmodPitch;

        AmbientSoundType ambientSoundType = (AmbientSoundType)Enum.Parse(typeof(AmbientSoundType), ambientSound.ambientSoundType);
        emitter.AmbientSoundType = ambientSoundType;
        emitter.enabled = true;

        return go;
    }

    private static void ImportAmbientSoundFiles(string fileToProcess, string dataFolder) {
        AmbientSound[] sounds = ParseAmbientSoundFile(fileToProcess);
        Debug.Log(sounds.Length);

        List<string> soundNames = new List<string>();
        foreach(AmbientSound sound in sounds) {
            string[] parts = sound.ambientSoundName.Split(".");
            soundNames.Add(parts[0].Trim() + "." + parts[2].Trim());
        }

        soundNames = soundNames.Distinct().ToList();
        foreach(string soundname in soundNames) {
            Debug.Log(soundname);
            string[] parts = soundname.Split(".");
            string fileToCopy = Path.Combine(dataFolder, parts[0], "Sound", parts[1] + ".wav");
            Debug.Log(fileToCopy);
            if(!File.Exists(fileToCopy)) {
                Debug.LogError($"File missing at {fileToCopy}");
            } else {
                string outputFolder = Path.Combine(scriptExportFolder, parts[0]);
                string outputFile = Path.Combine(outputFolder, parts[1] + ".wav");
                if(!Directory.Exists(outputFolder)) {
                    Directory.CreateDirectory(outputFolder);
                }
                File.Copy(fileToCopy, outputFile, true);
            }
        }
        Debug.Log($"Need to {soundNames.Count} load ambient sounds.");
    }

    public static AmbientSound[] ParseAmbientSoundFile(string path) {
        try {
            string json = File.ReadAllText(path);
            AmbientDataContainer sounds = JsonUtility.FromJson<AmbientDataContainer>(json);
            return sounds.data;

        } catch(IOException e) {
            Debug.LogError("An error occurred: " + e.Message);
        }

        return null;
    }
}
#endif