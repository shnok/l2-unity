using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateSpWait : MonsterStateBase
{
    public int playWaitSoundChancePercent = 15;
    public int playSpWaitChancePercent = 20;
    private bool hasStarted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        hasStarted = true;
        SetBool("wait", false);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(hasStarted && (stateInfo.normalizedTime % 1) < 0.5f) {
            SetBool("wait", false);

            if(RandomUtils.ShouldEventHappen(playWaitSoundChancePercent)) {
                audioHandler.PlaySound(MonsterSoundEvent.Breathe);
            }
            hasStarted = false;
        }

        if(!hasStarted && (stateInfo.normalizedTime % 1) >= 0.90f) {
            if(RandomUtils.ShouldEventHappen(playSpWaitChancePercent)) {
                SetBool("spwait", true);
            }
            hasStarted = true;
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
