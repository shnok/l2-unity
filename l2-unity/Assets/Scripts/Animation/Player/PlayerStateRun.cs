using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (InputManager.Instance.IsInputPressed(InputType.Jump)) {
            CameraController.Instance.StickToBone = true;
            SetBool("run_jump", true);
        }
        if (!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToTarget || !PlayerController.Instance.CanMove) {
            SetBool("wait_" + _weaponType, true);
        }
        if (InputManager.Instance.IsInputPressed(InputType.Sit)) {
            CameraController.Instance.StickToBone = true;
            PlayerController.Instance.CanMove = false;
            SetBool("sit", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("run_" + _weaponType, false);
    }
}
