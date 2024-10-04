using UnityEngine;

public class HumanoidStateWaitAtk : HumanoidStateAction
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _cancelAction = false;

        SetBool(HumanoidAnimType.atkwait, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atkwait, false);

        if (IsMoving())
        {
            SetBool(HumanoidAnimType.run, true);
        }

        if (!_cancelAction)
        {
            if (!ShouldAtkWait())
            {
                Debug.Log("Wait now");
                SetBool(HumanoidAnimType.wait, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atkwait, false);
    }
}