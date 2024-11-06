using UnityEngine;

public class HumanoidStateDead : HumanoidStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        PlaySoundAtRatio(EntitySoundEvent.Death, AudioHandler.DeathRatio);
        PlaySoundAtRatio(EntitySoundEvent.Fall, AudioHandler.DeathRatio);

        AnimController.ClearAnimParams();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.death, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

