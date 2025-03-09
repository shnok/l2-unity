using System;
using UnityEngine;

public class HumanoidStateCastThrow : HumanoidStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.magic_no_target, false);
        SetBool(HumanoidAnimType.magic_throw, false);
        SetBool(HumanoidAnimType.magic_shot, false);

        if (IsDead())
        {
            return;
        }

        if (IsMoving())
        {
            return;
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}