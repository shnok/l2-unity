using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationAudioHandler : BaseAnimationAudioHandler
{
    [SerializeField] private CharacterRace _race;

    protected override void Initialize() {
        base.Initialize();
        if(_race == CharacterRace.Default) {
            string[] raceParts = _npcClassName.Split("_");
            if (raceParts.Length > 1) {
                string raceName = raceParts[raceParts.Length - 1];
                _race = CharacterRaceParser.ParseRace(raceName);
            }
        }
    }

    public void PlaySound(CharacterSoundEvent soundEvent) {
        AudioManager.Instance.PlayCharacterSound(soundEvent, _race, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        CharacterSoundEvent soundEvent = (CharacterSoundEvent)type;
        AudioManager.Instance.PlayCharacterSound(soundEvent, _race, transform.position);
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
