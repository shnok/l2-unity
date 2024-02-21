using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateSit : UserStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        SetBool("sit", false);
        _audioHandler.PlaySound(CharacterSoundEvent.Sitdown);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("sit", false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }
}
