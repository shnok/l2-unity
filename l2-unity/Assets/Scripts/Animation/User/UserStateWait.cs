using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateWait : UserStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        SetBool("wait_" + _weaponAnim, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("wait_" + _weaponAnim, false);

        if (IsMoving()) {
            SetBool("run_" + _weaponAnim, true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}