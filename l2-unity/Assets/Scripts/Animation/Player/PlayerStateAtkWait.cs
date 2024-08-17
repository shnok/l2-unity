using UnityEngine;

public class PlayerStateAtkWait : PlayerStateAction {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        SetBool("atkwait", true, false, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if (ShouldDie()) {
            return;
        }

        if (ShouldAttack()) {
            SetBool("atk01", true, true, false);
            return;
        }

        if (ShouldRun()) {
            return;
        }

        if (ShouldJump(false)) {
            return;
        }

        //if (ShouldSit()) {
        //    return;
        //}

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

        SetBool("atkwait", true, false, false);
    }
}