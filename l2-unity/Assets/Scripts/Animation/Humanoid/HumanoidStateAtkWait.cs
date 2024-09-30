using UnityEngine;

public class HumanoidStateWaitAtk : HumanoidStateAction
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _cancelAction = false;

        SetBool("atkwait", true, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("atkwait", true, false);

        if (IsMoving())
        {
            SetBool("run", true, true);
        }

        if (!_cancelAction)
        {
            if (!ShouldAtkWait())
            {
                Debug.Log("Wait now");
                SetBool("wait", true, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("atkwait", true, false);
    }
}