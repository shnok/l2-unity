using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateRun : NpcStateBase {
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        _lastNormalizedTime = 0;
        foreach (var ratio in audioHandler.RunStepRatios) {
            audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in audioHandler.RunStepRatios) {
                audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
