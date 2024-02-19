using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSitWait : PlayerStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (InputManager.Instance.IsInputPressed(InputType.Sit) || InputManager.Instance.IsInputPressed(InputType.Move)) {
            CameraController.Instance.StickToBone = true;
            PlayerController.Instance.SetCanMove(false);
            SetBool("stand", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }
}
