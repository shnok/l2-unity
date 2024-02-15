using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateSocial : NpcStateBase {
    bool started = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        audioHandler.PlaySound(CharacterSoundEvent.Agree);

        SetBool("spwait", false);
        started = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if((stateInfo.normalizedTime % 1) >= 0.90f && started) {
            started = false;
            SetBool("wait", true);       
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
