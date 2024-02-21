using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateWait : UserStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        SetBool("wait" + _weaponType.ToString(), false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("wait" + _weaponType.ToString(), false);

        if (IsMoving()) {
            SetBool("run" + _weaponType.ToString(), true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}