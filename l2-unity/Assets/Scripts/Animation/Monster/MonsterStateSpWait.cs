using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateWait : MonsterStateBase
{
    public int playBreatheSoundChancePercent = 100;
    bool started = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if(RandomUtils.ShouldEventHappen(playBreatheSoundChancePercent)) {
            audioHandler.PlaySound(MonsterSoundEvent.Wait);
        }
        SetBool("spwait", false);
        started = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if((stateInfo.normalizedTime % 1) >= 0.90f && started) {
            started = false;
            SetBool("wait", true);       
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }
}
