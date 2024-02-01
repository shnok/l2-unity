using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private int _currentEventPriority = -1;
    [SerializeField] private EventReference _currentMusicEvent;
    [SerializeField] private Dictionary<EventReference, EventInstance> _musicInstances;
    public int CurrentEventPriority { get { return _currentEventPriority; } }

    private static MusicManager _instance;
    public static MusicManager Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }

        _musicInstances = new Dictionary<EventReference, EventInstance>();
    }

    public void PlayMusic(EventReference musicEvent, int priority) {   
        if(priority <= _currentEventPriority) {
            return;
        }

        _currentEventPriority = priority;
        _currentMusicEvent = musicEvent;

        if(_musicInstances.ContainsKey(musicEvent)) {
            _musicInstances[musicEvent].start();
        } else {
            EventInstance musicInstance = RuntimeManager.CreateInstance(musicEvent);
            _musicInstances.Add(musicEvent, musicInstance);
            musicInstance.start();
        }

        Debug.Log("Playing music " + musicEvent + " with priority " + priority);
    }

    public void StopMusic(EventReference musicEvent) {
        if(_musicInstances.ContainsKey(musicEvent)) {
            _musicInstances[musicEvent].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }             
    }

    public void ResetPriority() {
        _currentEventPriority = -1;
    }
}
