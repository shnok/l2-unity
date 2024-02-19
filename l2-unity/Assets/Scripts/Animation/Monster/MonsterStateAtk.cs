using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateAtk : MonsterStateBase
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        PlaySoundAtRatio(MonsterSoundEvent.Atk, audioHandler.AtkRatio);
        PlaySoundAtRatio(MonsterSoundEvent.Swish, audioHandler.SwishRatio);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Check if the state has looped (re-entered)
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            // This block will be executed once when the state is re-entered after completion
            _lastNormalizedTime = stateInfo.normalizedTime;
            PlaySoundAtRatio(MonsterSoundEvent.Atk, audioHandler.AtkRatio);
            PlaySoundAtRatio(MonsterSoundEvent.Swish, audioHandler.SwishRatio);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
