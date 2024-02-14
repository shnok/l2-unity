using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] private CharacterRace _playerClassName = CharacterRace.FDElf;
    private SurfaceDetector _surfaceDetector;

    private void Start() {
        _surfaceDetector = transform.GetComponentInParent<SurfaceDetector>(true);    
    }

    public void PlayRunStepSound() {
        if(_surfaceDetector.GetSurfaceTag() != null) {
            AudioManager.Instance.PlayStepSound(_surfaceDetector.GetSurfaceTag(), transform.position);
        }
    }

    public void PlaySound(CharacterSoundEvent soundEvent) {
        AudioManager.Instance.PlayCharacterSound(soundEvent, _playerClassName, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        CharacterSoundEvent soundEvent = (CharacterSoundEvent)type;
        AudioManager.Instance.PlayCharacterSound(soundEvent, _playerClassName, transform.position);
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        if (ratio == 0) {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(CharacterSoundEvent soundEvent, float ratio) {
        while ((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }
}
