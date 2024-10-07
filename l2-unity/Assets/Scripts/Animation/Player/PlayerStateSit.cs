using UnityEngine;

public class PlayerStateSit : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.sit, false, false);
        AudioHandler.PlaySound(EntitySoundEvent.Sitdown);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.sit, false, false);

        if (ShouldDie())
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
