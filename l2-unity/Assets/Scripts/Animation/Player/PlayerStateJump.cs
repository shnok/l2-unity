using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerStateBase {
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        _lastNormalizedTime = 0;

        SetBool("jump", false, false);
        SetBool("run_jump", false, false);
        _audioHandler.PlaySound(CharacterSoundEvent.Jump_1);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination) && PlayerController.Instance.CanMove) {
                SetBool("run_" + _weaponAnim, true);
            } else {
                SetBool("wait_" + _weaponAnim, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        CameraController.Instance.StickToBone = false;
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
    }
}