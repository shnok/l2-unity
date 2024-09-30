using UnityEngine;

public class PlayerStateWait : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ShouldDie())
        {
            return;
        }

        if (ShouldAttack())
        {
            SetBool("atk01", true, true, false);
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
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("wait", true, false, false);
    }
}
