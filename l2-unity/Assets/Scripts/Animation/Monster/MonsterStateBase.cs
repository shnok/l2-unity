using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateBase : StateMachineBehaviour
{
    public MonsterAnimationAudioHandler audioHandler;
    public Animator animator;

    public void LoadComponents(Animator animator) {
        if(audioHandler == null) {
            audioHandler = animator.gameObject.GetComponent<MonsterAnimationAudioHandler>();
        }
        if(this.animator == null) {
            this.animator = animator;
        }
    }

    public void PlaySoundAtRatio(MonsterSoundEvent soundEvent, float ratio) {
        audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(string name, bool value) {
        if(animator.GetBool(name) != value) {
            animator.SetBool(name, value);
        }
    }
}
