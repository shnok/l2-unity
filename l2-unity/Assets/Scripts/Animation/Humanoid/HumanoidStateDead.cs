using UnityEngine;

public class HumanoidStateDead : HumanoidStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.death, false);
        PlaySoundAtRatio(EntitySoundEvent.Death, AudioHandler.DeathRatio);
        PlaySoundAtRatio(EntitySoundEvent.Fall, AudioHandler.DeathRatio);

        AnimController.ClearAnimParams();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

