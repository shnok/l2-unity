using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public Dictionary<string, EventReference> uiSounds = new Dictionary<string, EventReference>();
    public Dictionary<string, EventReference> stepSounds = new Dictionary<string, EventReference>();

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
        SetReferences();
        SetBuses();
    }

    private void SetBuses() {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        UIBus = RuntimeManager.GetBus("bus:/UI");
        ambientBus = RuntimeManager.GetBus("bus:/Ambient");
    }

    private void SetReferences() {
        uiSounds.Add("click_01", RuntimeManager.PathToEventReference("event:/InterfaceSound/click_01"));
        uiSounds.Add("window_close", RuntimeManager.PathToEventReference("event:/InterfaceSound/window_close"));
        uiSounds.Add("window_open", RuntimeManager.PathToEventReference("event:/InterfaceSound/window_open"));
        
        stepSounds.Add("default_run", RuntimeManager.PathToEventReference("event:/StepSound/default_run"));
        stepSounds.Add("dirt_run", RuntimeManager.PathToEventReference("event:/StepSound/dirt_run"));
        stepSounds.Add("wood_run", RuntimeManager.PathToEventReference("event:/StepSound/wood_run"));
        stepSounds.Add("stone_run", RuntimeManager.PathToEventReference("event:/StepSound/stone_run"));
    }

    private void Update() {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);
        UIBus.setVolume(UIVolume);
        ambientBus.setVolume(ambientVolume);
    }

    public void PlayUISound(string soundName) {
        EventReference sound;
        if(uiSounds.TryGetValue(soundName, out sound)) {
            PlaySound(sound);
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
        Debug.Log(eventKey);

        EventReference sound;
        if(stepSounds.TryGetValue(eventKey, out sound)) {
            PlaySound(sound, position);
        }
    }

    public void PlaySound(EventReference sound, Vector3 postition) {
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlaySound(EventReference sound) {
        RuntimeManager.PlayOneShot(sound);
    }
}
