using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    public int currentEventPriority = -1;
    public EventReference currentMusicEvent;
    Dictionary<EventReference, EventInstance> musicInstances = new Dictionary<EventReference, EventInstance>();

    public void PlayMusic(EventReference musicEvent, int priority) {   
        if(priority <= currentEventPriority) {
            return;
        }

        currentEventPriority = priority;
        currentMusicEvent = musicEvent;

        if(musicInstances.ContainsKey(musicEvent)) {
            musicInstances[musicEvent].start();
        } else {
            EventInstance musicInstance = RuntimeManager.CreateInstance(musicEvent);
            musicInstances.Add(musicEvent, musicInstance);
            musicInstance.start();
        }

        Debug.Log("Playing music " + musicEvent + " with priority " + priority);
    }

    public void StopMusic(EventReference musicEvent) {
        if(musicInstances.ContainsKey(musicEvent)) {
            musicInstances[musicEvent].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }             
    }

    public void ResetPriority() {
        currentEventPriority = -1;
    }
}
