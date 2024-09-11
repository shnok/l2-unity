using UnityEngine;

public class PlayerStateWait : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        if (!_enabled)
        {
            return;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enabled)
        {
            return;
        }

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
        if (!_enabled)
        {
            return;
        }

        SetBool("wait", true, false, false);
    }
}
