using UnityEngine;

public class PlayerStateDeadWait : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        //PlayerController.Instance.SetCanMove(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ShouldIdle())
        {

        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // PlayerController.Instance.SetCanMove(true);
    }
}
