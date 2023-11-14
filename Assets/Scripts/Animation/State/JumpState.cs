using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerStateBase {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    bool jumping;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        jumping = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!owned)
            return;

        if(!jumping && !pc.controller.isGrounded) {
            jumping = true;
        }

        if(pc.controller.isGrounded && jumping) {
           SetBool("Jump", false);
        }

        if(InputManager.GetInstance().AxisPressed()) {
            SetBool("Moving", true);
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
