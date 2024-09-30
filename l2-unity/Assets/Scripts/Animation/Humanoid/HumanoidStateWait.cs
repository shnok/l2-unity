using UnityEngine;

public class HumanoidStateWait : HumanoidStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool("wait", true, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("wait", true, false);

        if (IsMoving())
        {
            if (_entity.Running)
            {
                SetBool("run", true, true);
            }
            else
            {
                SetBool("walk", true, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}