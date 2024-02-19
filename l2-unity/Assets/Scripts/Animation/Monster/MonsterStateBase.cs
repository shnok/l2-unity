using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateBase : StateMachineBehaviour
{
    public MonsterAnimationAudioHandler audioHandler;
    public Animator animator;
    private Entity _entity;

    public void LoadComponents(Animator animator) {
        if(audioHandler == null) {
            audioHandler = animator.gameObject.GetComponent<MonsterAnimationAudioHandler>();
        }
        if(this.animator == null) {
            this.animator = animator;
        }
        if (_entity == null) {
            _entity = animator.gameObject.GetComponent<Entity>();
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
