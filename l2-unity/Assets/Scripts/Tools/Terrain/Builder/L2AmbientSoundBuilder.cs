#if UNITY_EDITOR
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class L2AmbientSoundBuilder : MonoBehaviour
{

    [MenuItem("Shnok/[AmbientSound] (JSON) Build ambient sounds")]
    static void BuildSoundsMenu() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "json";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            AmbientSound[] sounds = L2TerrainInfoParser.ParseAmbientSoundFile(fileToProcess);
            BuildAmbientSounds(sounds);
        }
    }

    [MenuItem("Shnok/9.1. [AmbientSound] (T3D) Build ambient sounds")]
    static void BuildSoundsMenuT3D() {
        string title = "Select ambient sound list";
        string directory = Path.Combine(Application.dataPath, "Data/Maps");
        string extension = "t3d";

        string fileToProcess = EditorUtility.OpenFilePanel(title, directory, extension);

        if (!string.IsNullOrEmpty(fileToProcess)) {
            Debug.Log("Selected file: " + fileToProcess);
            AmbientSound[] sounds = L2T3DInfoParser.ParseAmbientSounds(fileToProcess).ToArray();
            BuildAmbientSounds(sounds);
        }
    }

    private static void BuildAmbientSounds(AmbientSound[] sounds) {
        GameObject container = new GameObject("AmbientSounds");

        foreach (AmbientSound ambientSound in sounds) {
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

        go.transform.position = VectorUtils.ConvertPosToUnity(ambientSound.position);
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
        if (ambientSound.ambientRandom == 0) {
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
}
#endif
