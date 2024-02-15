using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerStateBase {
    private bool _hasStarted = false;
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        _hasStarted = true;
        _lastNormalizedTime = 0;

        foreach (var ratio in _audioHandler.RunStepRatios) {
            _audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
        }
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
            PlayerController.Instance.SetCanMove(false);
            SetBool("sit", true);
        }


        if (_hasStarted && (stateInfo.normalizedTime % 1) < 0.5f) {
            if (RandomUtils.ShouldEventHappen(_audioHandler.RunBreathChance)) {
                _audioHandler.PlaySound(CharacterSoundEvent.Breath);
            }
            _hasStarted = false;
        }
        if ((stateInfo.normalizedTime % 1) >= 0.90f) {
            _hasStarted = true;
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in _audioHandler.RunStepRatios) {
                _audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("run_" + _weaponType, false);
    }
}
