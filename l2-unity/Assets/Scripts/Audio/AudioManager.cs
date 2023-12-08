using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;
    [Range(0, 1)]
    public float UIVolume = 1;
    [Range(0, 1)]
    public float ambientVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus SFXBus;
    private Bus UIBus;
    private Bus ambientBus;

    public static AudioManager instance;

    public static AudioManager GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
        SetBuses();
    }

    private void SetBuses() {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        UIBus = RuntimeManager.GetBus("bus:/UI");
        ambientBus = RuntimeManager.GetBus("bus:/Ambient");
    }
    private void Update() {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);
        UIBus.setVolume(UIVolume);
        ambientBus.setVolume(ambientVolume);
    }

    public void PlayMonsterSound(MonsterSoundEvent monsterSoundEvent, string npcClassName, Vector3 position) {
        string eventKey = monsterSoundEvent.ToString().ToLower();
       // Debug.Log("event:/MonSound/" + npcClassName + "/" + eventKey);
        EventReference er = RuntimeManager.PathToEventReference("event:/MonSound/" + npcClassName + "/" + eventKey);
        if(!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlayUISound(string soundName) {
        EventReference er = RuntimeManager.PathToEventReference("event:/InterfaceSound/" + soundName);
        if(!er.IsNull) {
            PlaySound(er);
        }
    }

    public void PlayStepSound(string surfaceTag, Vector3 position) {
        string eventKey;
        surfaceTag = surfaceTag.ToLower();

        switch(surfaceTag) {
            case "dirt":
                eventKey = surfaceTag + "_run";
                break;
            case "stone":
                eventKey = surfaceTag + "_run";
                break;
            case "wood":
                eventKey = surfaceTag + "_run";
                break;
            default:
                eventKey = "default_run";
                break;

        }

        EventReference er = RuntimeManager.PathToEventReference("event:/StepSound/" + eventKey);
        if(!er.IsNull) {
            PlaySound(er, position);
        }
    }

    public void PlaySound(EventReference sound, Vector3 postition) {
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound) {
        RuntimeManager.PlayOneShot(sound);
    }
}
