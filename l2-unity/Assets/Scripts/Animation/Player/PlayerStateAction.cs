using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAction : PlayerStateBase
{
    protected void VerifySit() {
        if (InputManager.Instance.IsInputPressed(InputType.Sit)) {
            CameraController.Instance.StickToBone = true;
            PlayerController.Instance.SetCanMove(false);
            SetBool("sit", true);
        }
    }

    protected void VerifyJump(bool run) {
        if (InputManager.Instance.IsInputPressed(InputType.Jump)) {
            CameraController.Instance.StickToBone = true;
            if (run) {
                SetBool("run_jump", true);
            } else {
                SetBool("jump", true);
            }
        }
    }

    protected void VerifyAttack() {
        PlayerCombatController.Instance.VerifyAttackInput();
    }

    protected void VerifyRun() {
        if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToTarget) && PlayerController.Instance.CanMove) {
            SetBool("run_" + _weaponType, true);
        }
    }

    protected void VerifyIdle() {
        if (!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToTarget || !PlayerController.Instance.CanMove) {
            if(PlayerEntity.Instance.AttackTarget == null) {
                SetBool("wait_" + _weaponType, true);
            }
        }
    }
}
