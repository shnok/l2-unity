using System.Collections;
using UnityEngine;

public class HumanoidAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] protected CharacterModelSound _race;
    [SerializeField] protected SurfaceDetector _surfaceDetector;

    protected override void Initialize()
    {
        base.Initialize();
        if (_surfaceDetector == null)
        {
            Debug.LogWarning($"[{transform.name}] SurfaceDetector was not assigned, please pre-assign it to avoid unecessary load.");
            _surfaceDetector = GetComponent<SurfaceDetector>();
        }
    }

    public override void PlaySound(EntitySoundEvent soundEvent)
    {
        if (soundEvent == EntitySoundEvent.Step)
        {
            PlayStepSound();
            return;
        }

        AudioManager.Instance.PlayCharacterSound(soundEvent, _race, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type)
    {
        EntitySoundEvent soundEvent = (EntitySoundEvent)type;
        AudioManager.Instance.PlayCharacterSound(soundEvent, _race, transform.position);
    }

    private void PlayStepSound()
    {
        string currentSurface = _surfaceDetector.GetSurfaceTag();
        AudioManager.Instance.PlayStepSound(currentSurface, transform.position);
    }
}
