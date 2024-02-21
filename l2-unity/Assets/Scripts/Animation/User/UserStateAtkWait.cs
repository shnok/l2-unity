using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateWaitAtk : UserStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if(IsAttacking()) {
            return;
        }

        if (ShouldAtkWait()) {
            return;
        }

        SetBool("wait" + _weaponType.ToString(), true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("atkwait" + _weaponType.ToString(), false);
    }
}