using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateAtkWait : MonsterStateBase
{
    private IEnumerator _exitStateCoroutine;
    private float _lastNormalizedTime = 0;
    [SerializeField] private float _timeoutAfterLoopCount = 3f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        audioHandler.PlaySoundAtRatio(MonsterSoundEvent.AtkWait, audioHandler.AtkWaitRatio);
        _lastNormalizedTime = 0;
        animator.SetBool("atkwait", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= _timeoutAfterLoopCount) {
            animator.SetBool("wait", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
