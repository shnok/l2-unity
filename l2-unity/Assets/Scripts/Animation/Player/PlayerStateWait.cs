using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWait : PlayerStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (ShouldAttack()) {
            return;
        }
        if (ShouldRun()) {
            return;
        }
        if(ShouldJump(false)) {
            return;
        }
        if(ShouldSit()) {
            return;
        }
        if (ShouldAtkWait()) {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("wait_" + _weaponType, false);
    }
}
