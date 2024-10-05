using UnityEngine;

public class MonsterStateDead : MonsterStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        PlaySoundAtRatio(EntitySoundEvent.Death, AudioHandler.DeathRatio);
        PlaySoundAtRatio(EntitySoundEvent.Fall, AudioHandler.FallRatio);
        SetBool(MonsterAnimationEvent.death, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //SetBool(MonsterAnimationEvent.death, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
