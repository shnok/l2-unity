using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateJump : UserStateBase {
    private float _lastNormalizedTime = 0;
    private bool _wasRunning = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        if(_animator.GetBool("run_jump") == true) {
            _wasRunning = true;
        }

        SetBool("jump", false);
        SetBool("run_jump", false);
        _audioHandler.PlaySound(CharacterSoundEvent.Jump_1);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("jump", false);
        SetBool("run_jump", false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            if(_wasRunning) {
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

        _audioHandler.PlaySound(CharacterSoundEvent.Step);
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
    }
}