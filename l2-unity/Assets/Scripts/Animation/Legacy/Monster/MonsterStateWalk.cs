using UnityEngine;

public class MonsterStateWalk : MonsterStateAction
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _lastNormalizedTime = 0;
        SetBool(MonsterAnimationEvent.walk, false);
        foreach (var ratio in AudioHandler.WalkStepRatios)
        {
            AudioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.walk, false);

        if (IsDead())
        {
            SetBool(MonsterAnimationEvent.death, true);
            return;
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in AudioHandler.WalkStepRatios)
            {
                AudioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
            }
        }

        if (!IsMoving() && (stateInfo.normalizedTime) >= 0.10f)
        {
            if (IsAttacking())
            {
                return;
            }
            if (ShouldAtkWait())
            {
                SetBool(MonsterAnimationEvent.atkwait, true);
                return;
            }
            SetBool(MonsterAnimationEvent.wait, true);
        }
        else if (Entity.Running)
        {
            SetBool(MonsterAnimationEvent.run, true);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
