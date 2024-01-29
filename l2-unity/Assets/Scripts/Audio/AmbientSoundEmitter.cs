using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundEmitter : EventHandler {
    public EventReference EventReference;
    protected FMOD.Studio.EventDescription eventDescription;
    protected FMOD.Studio.EventInstance instance;
    public FMOD.Studio.EventDescription EventDescription { get { return eventDescription; } }
    public FMOD.Studio.EventInstance EventInstance { get { return instance; } }

    public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
    public EmitterGameEvent StopEvent = EmitterGameEvent.None;
    public AmbientSoundType ambientSoundType;
    public bool AllowFadeout = true;
    public bool OverrideAttenuation = false;
    private float clipLengthSeconds = 0;
    public float loopDelaySeconds = 1;
    public float playChancePercent = 100;
    public float soundPitch = 0;
    public bool loop = true;
    public float OverrideMinDistance = -1.0f;
    public float OverrideMaxDistance = -1.0f;

    private void Awake() {
        if(!eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        eventDescription.getLength(out lengthMs);
        clipLengthSeconds = lengthMs / 1000f;
    }

    private float MaxDistance {
        get {
            if(OverrideAttenuation) {
                return OverrideMaxDistance;
            }

            if(!eventDescription.isValid()) {
                Lookup();
            }

            float minDistance, maxDistance;
            eventDescription.getMinMaxDistance(out minDistance, out maxDistance);
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
        eventDescription = RuntimeManager.GetEventDescription(EventReference);
    }

    public void Play() {
        if(EventReference.IsNull) {
            return;
        }

        if(!eventDescription.isValid()) {
            Lookup();
        }

        if(loop) {
            StartCoroutine("StartPlayLoop");
        }

        if(!ShouldPlayEvent()) {
            return;
        }

        bool is3D;
        eventDescription.is3D(out is3D);

        if(is3D && Settings.Instance.StopEventsOutsideMaxDistance) {
            UpdatePlayingStatus(true);
        } else {
            PlayInstance();
        }
    }

    IEnumerator StartPlayLoop() {
        yield return new WaitForSeconds(clipLengthSeconds);
        yield return new WaitForSeconds(loopDelaySeconds);
        Play();
    }

    private bool ShouldPlayEvent() {
        if(ambientSoundType == AmbientSoundType.AST1_Day && WorldClock.Instance.IsNightTime()) {
            return false;
        }
        if(ambientSoundType == AmbientSoundType.AST1_Night && !WorldClock.Instance.IsNightTime()) {
            return false;
        }
        if(Random.Range(1, 100) <= playChancePercent) {
            return true;
        }

        return false;
    }

    private void PlayInstance() {
        if(!instance.isValid()) {
            instance.clearHandle();
        }

        // Let previous oneshot instances play out
        if(instance.isValid()) {
            instance.release();
            instance.clearHandle();
        }

        bool is3D;
        eventDescription.is3D(out is3D);

        if(!instance.isValid()) {
            eventDescription.createInstance(out instance);

            // Only want to update if we need to set 3D attributes
            if(is3D) {
                instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            }
        }

        if(is3D && OverrideAttenuation) {
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
        }

        instance.setPitch(soundPitch);
        instance.start();
    }

    public void Stop() {
        StopInstance();
    }

    private void StopInstance() {
        StopCoroutine("StartPlayLoop");
        if(instance.isValid()) {
            instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            if(!AllowFadeout) {
                instance.clearHandle();
            }
        }
    }

    public bool IsPlaying() {
        if(instance.isValid()) {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }

    protected override void HandleGameEvent(EmitterGameEvent gameEvent) {
        if(PlayEvent == gameEvent) {
            Play();
        }
        if(StopEvent == gameEvent) {
            Stop();
        }
    }
}
