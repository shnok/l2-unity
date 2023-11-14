using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterWeaponState : PlayerStateBase {
    public bool holsterApplied;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        holsterApplied = false;
        animator.SetBool("TakeWeapon", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!animator.IsInTransition(1) && animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.9f && !holsterApplied) {
          //  cc.HolsterWeapons();
            holsterApplied = true;
            animator.SetBool("TakeWeapon", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("TakeWeapon", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //Implement code that processes and affects root motion
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
