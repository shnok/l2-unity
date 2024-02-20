using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAction : PlayerStateBase
{
    protected bool ShouldSit() {
        if (InputManager.Instance.IsInputPressed(InputType.Sit)) {
            CameraController.Instance.StickToBone = true;
            PlayerController.Instance.SetCanMove(false);
            SetBool("sit", true);
            return true;
        }

        return false;
    }

    protected bool ShouldJump(bool run) {
        if (InputManager.Instance.IsInputPressed(InputType.Jump)) {
            CameraController.Instance.StickToBone = true;
            if (run) {
                SetBool("run_jump", true);
            } else {
                SetBool("jump", true);
            }
            return true;
        }

        return false;
    }

    protected bool ShouldAttack() {
        return PlayerCombatController.Instance.VerifyAttackInput();
    }

    protected bool ShouldRun() {
        if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) && PlayerController.Instance.CanMove) {
            SetBool("run" + _weaponType.ToString(), true);
            return true;
        }

        return false;
    }

    protected bool ShouldIdle() {
        if (!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToDestination || !PlayerController.Instance.CanMove) {
            if(PlayerEntity.Instance.AttackTarget == null) {
                SetBool("wait" + _weaponType.ToString(), true);
                return true;
            }
        }

        return false;
    }

    protected bool ShouldAtkWait() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if ((!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToDestination || !PlayerController.Instance.CanMove)
             && now - _entity.StopAutoAttackTime < 5000) {
            if (PlayerEntity.Instance.AttackTarget == null) {
                SetBool("atkwait" + _weaponType.ToString(), true, false);
                return true;
            }
        }

        return false;
    }
}