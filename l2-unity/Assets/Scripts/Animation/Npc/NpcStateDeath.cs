using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateDeath : NpcStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        PlaySoundAtRatio(CharacterSoundEvent.Death, audioHandler.DeathRatio);
        PlaySoundAtRatio(CharacterSoundEvent.Fall, audioHandler.FallRatio);
        animator.SetBool("death", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
