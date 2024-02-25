using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAtkWait : PlayerStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if (ShouldAttack()) {
            return;
        }

        if (ShouldRun()) {
            return;
        }

        if (ShouldJump(false)) {
            return;
        }

        if (ShouldSit()) {
            return;
        }

        if(ShouldAtkWait()) {
            return;
        }

        if (ShouldIdle()) {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("atkwait_" + _weaponAnim, false, false);
    }
}