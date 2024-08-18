using UnityEngine;

public class UserStateWaitAtk : UserStateAction {

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        _cancelAction = false;

        SetBool("atkwait", true, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }
        SetBool("atkwait", true, false);

        if (IsMoving()) {
            SetBool("run", true, true);
        }

        if (!_cancelAction) {
            if (!ShouldAtkWait()) {
                Debug.Log("Wait now");
                SetBool("wait", true, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }


        SetBool("atkwait", true, false);
    }
}