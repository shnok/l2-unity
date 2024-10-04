using UnityEngine;

public class PlayerStateSitWait : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.sitwait, false, false);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ShouldDie())
        {
            return;
        }

        if (ShouldStand())
        {
            return;
        }

        //if (InputManager.Instance.IsInputPressed(InputType.Sit) || InputManager.Instance.IsInputPressed(InputType.Move)) {
        //    CameraController.Instance.StickToBone = true;
        //    PlayerController.Instance.SetCanMove(false);
        //    SetBool("stand", true);
        //}
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
