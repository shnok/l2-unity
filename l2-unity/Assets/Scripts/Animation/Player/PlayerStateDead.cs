using UnityEngine;

public class PlayerStateDead : PlayerStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.death, false, false);
        //PlayerController.Instance.SetCanMove(false);
        PlaySoundAtRatio(EntitySoundEvent.Death, AudioHandler.DeathRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.death, false, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
