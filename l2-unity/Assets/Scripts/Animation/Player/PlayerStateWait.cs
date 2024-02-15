using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWait : PlayerStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (InputManager.Instance.IsInputPressed(InputType.Jump)) {
            CameraController.Instance.StickToBone = true;
            SetBool("jump", true);
        }
        if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToTarget) && PlayerController.Instance.CanMove) {
            SetBool("run_" + _weaponType, true);
        }
        if (InputManager.Instance.IsInputPressed(InputType.Sit)) {
            CameraController.Instance.StickToBone = true;
            PlayerController.Instance.SetCanMove(false);
            SetBool("sit", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("wait_" + _weaponType, false);
    }
}
