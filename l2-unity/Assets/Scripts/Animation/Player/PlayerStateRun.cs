using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerStateAction {
    private bool _hasStarted = false;
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        _hasStarted = true;
        _lastNormalizedTime = 0;

        foreach (var ratio in _audioHandler.RunStepRatios) {
            _audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
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

        if (ShouldAttack()) {
            return;
        }
        if (ShouldJump(true)) {
            return;
        }
        if (ShouldSit()) {
            return;
        }
        if (ShouldAtkWait()) {
            return;
        }
        if (ShouldIdle()) {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("run" + _weaponType.ToString(), false, false);
    }
}
