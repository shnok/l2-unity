using UnityEngine;

public class HumanoidStateWait : HumanoidStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.wait, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsDead())
        {
            SetBool(HumanoidAnimType.death, true);
            return;
        }

        if (IsMoving())
        {
            if (Entity.Running)
            {
                SetBool(HumanoidAnimType.run, true);
            }
            else
            {
                SetBool(HumanoidAnimType.walk, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}