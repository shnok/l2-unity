using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundEmitter : EventHandler {
    private FMOD.Studio.EventDescription _eventDescription;
    private FMOD.Studio.EventInstance _instance;

    [SerializeField] private EventReference _eventReference;
    [SerializeField] private EmitterGameEvent _playEvent = EmitterGameEvent.None;
    [SerializeField] private EmitterGameEvent _stopEvent = EmitterGameEvent.None;
    [SerializeField] private AmbientSoundType _ambientSoundType;
    [SerializeField] private bool _allowFadeout = true;
    [SerializeField] private bool _overrideAttenuation = false;
    [SerializeField] private float _loopDelaySeconds = 1;
    [SerializeField] private float _playChancePercent = 100;
    [SerializeField] private float _soundPitch = 0;
    [SerializeField] private bool _loop = true;
    [SerializeField] private float _overrideMinDistance = -1.0f;
    [SerializeField] private float _overrideMaxDistance = -1.0f;

    public EventReference EventReference { set { _eventReference = value; } }
    public EmitterGameEvent PlayEvent { set { _playEvent = value; } }
    public EmitterGameEvent StopEvent { set { _stopEvent = value; } }
    public AmbientSoundType AmbientSoundType { set { _ambientSoundType = value; } }
    public bool AllowFadeout { set { _allowFadeout = value; } }
    public bool OverrideAttenuation { set { _overrideAttenuation = value; } }
    public float LoopDelaySeconds { set { _loopDelaySeconds = value; } }
    public float PlayChancePercent { set { _playChancePercent = value; } }
    public float SoundPitch { set { _soundPitch = value; } }
    public bool Loop { set { _loop = value; } }
    public float OverrideMinDistance { set { _overrideMinDistance = value; } }
    public float OverrideMaxDistance { set { _overrideMaxDistance = value; } }

    private float _clipLengthSeconds = 0;

    private void Awake() {
        if(!_eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        _eventDescription.getLength(out lengthMs);
        _clipLengthSeconds = lengthMs / 1000f;
    }

    private float MaxDistance {
        get {
            if(_overrideAttenuation) {
                return _overrideMaxDistance;
            }

            if(!_eventDescription.isValid()) {
                Lookup();
            }

            float minDistance, maxDistance;
            _eventDescription.getMinMaxDistance(out minDistance, out maxDistance);
            return maxDistance;
        }
    }

    private void UpdatePlayingStatus(bool force = false) {
        // If at least one listener is within the max distance, ensure an event instance is playing
        bool playInstance = StudioListener.DistanceSquaredToNearestListener(transform.position) <= (MaxDistance * MaxDistance);

        if(force || playInstance != IsPlaying()) {
            if(playInstance) {
                PlayInstance();
            } else {
                StopInstance();
            }
        }
    }

    private void Lookup() {
        _eventDescription = RuntimeManager.GetEventDescription(_eventReference);
    }

    public void Play() {
        if(_eventReference.IsNull) {
            return;
        }

        if(!_eventDescription.isValid()) {
            Lookup();
        }

        if(_loop) {
            StartCoroutine("StartPlayLoop");
        }

        if(!ShouldPlayEvent()) {
            return;
        }

        bool is3D;
        _eventDescription.is3D(out is3D);

        if(is3D && Settings.Instance.StopEventsOutsideMaxDistance) {
            UpdatePlayingStatus(true);
        } else {
            PlayInstance();
        }
    }

    IEnumerator StartPlayLoop() {
        yield return new WaitForSeconds(_clipLengthSeconds);
        yield return new WaitForSeconds(_loopDelaySeconds);
        Play();
    }

    private bool ShouldPlayEvent() {
        if(_ambientSoundType == AmbientSoundType.AST1_Day && WorldClock.Instance.IsNightTime()) {
            return false;
        }
        if(_ambientSoundType == AmbientSoundType.AST1_Night && !WorldClock.Instance.IsNightTime()) {
            return false;
        }
        if(Random.Range(1, 100) <= _playChancePercent) {
            return true;
        }

        return false;
    }

    private void PlayInstance() {
        if(!_instance.isValid()) {
            _instance.clearHandle();
        }

        // Let previous oneshot instances play out
        if(_instance.isValid()) {
            _instance.release();
            _instance.clearHandle();
        }

        bool is3D;
        _eventDescription.is3D(out is3D);

        if(!_instance.isValid()) {
            _eventDescription.createInstance(out _instance);

            // Only want to update if we need to set 3D attributes
            if(is3D) {
                _instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            }
        }

        if(is3D && _overrideAttenuation) {
            _instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, _overrideMinDistance);
            _instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, _overrideMaxDistance);
        }

        _instance.setPitch(_soundPitch);
        _instance.start();
    }

    public void Stop() {
        StopInstance();
    }

    private void StopInstance() {
        StopCoroutine("StartPlayLoop");
        if(_instance.isValid()) {
            _instance.stop(_allowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
            _instance.release();
            if(!_allowFadeout) {
                _instance.clearHandle();
            }
        }
    }

    public bool IsPlaying() {
        if(_instance.isValid()) {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            _instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }

    protected override void HandleGameEvent(EmitterGameEvent gameEvent) {
        if(_playEvent == gameEvent) {
            Play();
        }
        if(_stopEvent == gameEvent) {
            Stop();
        }
    }
}
