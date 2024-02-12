using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[RequireComponent(typeof(MonsterEntity), typeof(Animator))]
public class MonsterAnimationAudioHandler : BaseAnimationAudioHandler
{
    public void PlaySound(MonsterSoundEvent soundEvent) {
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcClassName, transform.position);
    }

    public void PlaySoundFromAnimationClip(int type) {
        MonsterSoundEvent soundEvent = (MonsterSoundEvent) type;
        AudioManager.Instance.PlayMonsterSound(soundEvent, _npcClassName, transform.position);
    }

    public void PlaySoundAtRatio(MonsterSoundEvent soundEvent, float ratio) {
        if(ratio == 0) {
            PlaySound(soundEvent);
            return;
        }

        StartCoroutine(PlaySoundAtRatioCoroutine(soundEvent, ratio));
    }

    public IEnumerator PlaySoundAtRatioCoroutine(MonsterSoundEvent soundEvent, float ratio) {
        while((_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) < ratio) {
            yield return null;
        }

        PlaySound(soundEvent);
    }
}
