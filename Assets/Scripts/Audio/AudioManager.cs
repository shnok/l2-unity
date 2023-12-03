using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public Dictionary<string, EventReference> uiSounds = new Dictionary<string, EventReference>();

    public static AudioManager instance;

    public static AudioManager GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }


    void Start()
    {
        SetReferences();
    }

    void SetReferences() {
        uiSounds.Add("click_01", RuntimeManager.PathToEventReference("event:/InterfaceSound/click_01"));
        uiSounds.Add("window_close", RuntimeManager.PathToEventReference("event:/InterfaceSound/window_close"));
        uiSounds.Add("window_open", RuntimeManager.PathToEventReference("event:/InterfaceSound/window_open"));
    }

    public void PlaySound3D(EventReference sound, Vector3 postition) {
        RuntimeManager.PlayOneShot(sound, postition);
    }

    public void PlayUISound(string soundName) {
        EventReference sound;
        if(uiSounds.TryGetValue(soundName, out sound)) {
            PlaySound(sound);
        }
    }
        

    public void PlaySound(EventReference sound) {
        RuntimeManager.PlayOneShot(sound);
    }
}
