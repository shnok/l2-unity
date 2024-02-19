using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateJump : UserStateBase {
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        SetBool("jump", false);
        SetBool("run_jump", false);
        _audioHandler.PlaySound(CharacterSoundEvent.Jump_1);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("jump", false);
        SetBool("run_jump", false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            SetBool("wait" + _weaponType.ToString(), true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
        _audioHandler.PlaySound(CharacterSoundEvent.Step);
    }
}