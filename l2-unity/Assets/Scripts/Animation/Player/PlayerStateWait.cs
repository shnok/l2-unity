using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWait : PlayerStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        VerifyRun();

        VerifyJump(false);

        VerifySit();

        VerifyAttack();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("wait_" + _weaponType, false);
    }
}
