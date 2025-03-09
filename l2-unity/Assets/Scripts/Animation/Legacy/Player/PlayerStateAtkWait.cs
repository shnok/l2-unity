using System;
using UnityEngine;

public class PlayerStateAtkWait : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.atkwait, false, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ShouldDie())
        {
            return;
        }

        if (!DidAttackTimeout() && ShouldAttack())
        {
            SetBool(HumanoidAnimType.atk01, true, false);
            return;
        }

        if (ShouldRun())
        {
            return;
        }

        if (ShouldWalk())
        {
            return;
        }

        if (ShouldJump(false))
        {
            return;
        }

        if (ShouldSit())
        {
            return;
        }

        if (ShouldAtkWait())
        {
            return;
        }

        if (ShouldIdle())
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atkwait, false, false);
    }
}