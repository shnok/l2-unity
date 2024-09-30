using System.Collections;
using UnityEngine;

public class HumanoidAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] protected CharacterModelSound _race;
    [SerializeField] protected SurfaceDetector _surfaceDetector;

    protected override void Initialize()
    {
        base.Initialize();
        _surfaceDetector = GetComponent<SurfaceDetector>();
    }

    public void PlaySound(EntitySoundEvent soundEvent)
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

    public void PlaySoundAtRatio(EntitySoundEvent soundEvent, float ratio)
    {
        if (ratio == 0)
        {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(EntitySoundEvent soundEvent, float ratio)
    {
        while ((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio)
        {
            yield return null;
        }

        PlaySound(soundEvent);
    }

    private void PlayStepSound()
    {
        string currentSurface = _surfaceDetector.GetSurfaceTag();
        AudioManager.Instance.PlayStepSound(currentSurface, transform.position);
    }
}
