using UnityEngine;

public class HumanoidStateDead : HumanoidStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool("death", false, false);
        PlaySoundAtRatio(EntitySoundEvent.Death, _audioHandler.DeathRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("death", false, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}

