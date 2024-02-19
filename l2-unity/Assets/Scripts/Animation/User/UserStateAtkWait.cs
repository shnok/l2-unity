using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateWaitAtk : UserStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //TODO ADD ATKWAIT TIMEOUT
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("atkwait" + _weaponType.ToString(), false);
    }
}