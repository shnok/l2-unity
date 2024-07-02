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
    [SerializeField] private float _clipLengthSeconds = 0;
    [SerializeField] private bool _isSound3D = true;

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

    private void Awake() {
        if(!_eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        _eventDescription.getLength(out lengthMs);
        _clipLengthSeconds = lengthMs / 1000f;
        _clipLengthSeconds = _clipLengthSeconds / _soundPitch;

        _eventDescription.is3D(out _isSound3D);
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

        if (_loop) {
            StopCoroutine(StartPlayLoop());
            StartCoroutine(StartPlayLoop());
            return;
        }

        if (_isSound3D && ShouldStop()) {
            Stop();
        } else if (ShouldPlayEvent()) {
            PlayInstance();
        }
    }


    IEnumerator StartPlayLoop() {
        while(true) {
            if (_isSound3D && ShouldStop()) {
                Stop();
                yield break;
            }
            if(ShouldPlayEvent()) {
                PlayInstance();
            }
            yield return new WaitForSeconds(_clipLengthSeconds * 0.99f); // 1% for error margin
            if (_loopDelaySeconds > 0) {
                yield return new WaitForSeconds(_loopDelaySeconds);
            }
        }
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

    private bool ShouldStop() {
        if(ThirdPersonListener.Instance.Player == null) {
            return true;
        }

        float distance = Vector3.Distance(transform.position, ThirdPersonListener.Instance.Player.transform.position);
        return distance > MaxDistance;
    }

    private void PlayInstance() {
        if (!_instance.isValid()) {
            _instance.clearHandle();
        }

        // Let previous oneshot instances play out
        if (_instance.isValid()) {
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
        StopCoroutine(StartPlayLoop());
        StopInstance();
    }

    private void StopInstance() {
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
