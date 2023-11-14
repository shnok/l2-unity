using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttackState : PlayerStateBase {
    public float start;
    public float elapsed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        start = Time.time;
        pc.canMove = false;
        animator.SetBool("Attack", false);
        animator.SetBool("ExitAttack", false);
        animator.SetBool("ForceExitAttack", false);
      //  animator.GetComponentInChildren<Xft.XWeaponTrail>(true).Activate();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(!owned)
            return;

        elapsed = Time.time - start;
        if(elapsed >= 0.3f) {
            if(InputManager.GetInstance().Attack()) {
                SetBool("Attack", true);
                if(!animator.GetNextAnimatorStateInfo(0).IsName("Attack1") && 
                    !animator.GetNextAnimatorStateInfo(0).IsName("Attack2") && 
                    !animator.GetNextAnimatorStateInfo(0).IsName("Attack3")) {
                    pc.LookForward("camera");
                }
            }
            if(elapsed >= 0.5f) {
                pc.canMove = true;
                if(elapsed >= 1f || animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) {
                    SetBool("ExitAttack", true);
                }
                if(animator.GetNextAnimatorStateInfo(0).IsName("Jump")) {
                    SetBool("ForceExitAttack", true);
                }
            }
        } else {
            pc.canMove = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Attack", false);
        animator.SetBool("ExitAttack", false);
        animator.SetBool("ForceExitAttack", false);
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
