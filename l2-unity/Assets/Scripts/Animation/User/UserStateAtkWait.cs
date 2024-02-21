using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateWaitAtk : UserStateAction {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        _cancelAction = false;

        SetBool("atkwait" + _weaponType.ToString(), false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }
        SetBool("atkwait" + _weaponType.ToString(), false);

        if (IsMoving()) {
            SetBool("run" + _weaponType.ToString(), true);
        }

        if (!_cancelAction) {
            if (!ShouldAtkWait()) {
                Debug.Log("Wait now");
                SetBool("wait" + _weaponType.ToString(), true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }


        SetBool("atkwait" + _weaponType.ToString(), false);
    }
}