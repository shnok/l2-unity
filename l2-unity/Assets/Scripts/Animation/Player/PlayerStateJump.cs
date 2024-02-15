using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerStateBase {
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _lastNormalizedTime = 0;
        LoadComponents(animator);
        SetBool("jump", false);
        SetBool("run_jump", false);
        _audioHandler.PlaySound(CharacterSoundEvent.Jump_1);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToTarget) && PlayerController.Instance.CanMove) {
                SetBool("run_" + _weaponType, true);
            } else {
                SetBool("wait_" + _weaponType, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CameraController.Instance.StickToBone = false;
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
    }
}