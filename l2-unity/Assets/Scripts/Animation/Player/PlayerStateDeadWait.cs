using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDeadWait : PlayerStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        PlayerController.Instance.SetCanMove(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        PlayerController.Instance.SetCanMove(true);
    }
}
