using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAttack : PlayerStateBase {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
       // animator.GetComponentInChildren<Xft.XWeaponTrail>(true).Deactivate();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!owned)
            return;

        if(InputManager.GetInstance().Attack() && !animator.GetNextAnimatorStateInfo(0).IsName("Attack1") && pc.controller.isGrounded && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))) {   
            SetBool("Attack", true);         
            pc.LookForward("camera");   
        }

        if(InputManager.GetInstance().HolsterWeapons()) {
            SetBool("TakeWeapon", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

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
