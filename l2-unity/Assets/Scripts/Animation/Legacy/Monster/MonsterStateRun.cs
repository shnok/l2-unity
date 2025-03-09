using UnityEngine;

public class MonsterStateRun : MonsterStateAction
{
    private float _lastNormalizedTime = 0;

    private bool _hasStarted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _hasStarted = true;
        _lastNormalizedTime = 0;
        SetBool(MonsterAnimationEvent.run, false);
        foreach (var ratio in AudioHandler.RunStepRatios)
        {
            AudioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.run, false);

        if (IsDead())
        {
            SetBool(MonsterAnimationEvent.death, true);
            return;
        }

        if (_hasStarted && (stateInfo.normalizedTime % 1) < 0.5f)
        {
            if (RandomUtils.ShouldEventHappen(AudioHandler.RunBreathChance))
            {
                AudioHandler.PlaySound(EntitySoundEvent.Breath);
            }
            _hasStarted = false;
        }
        if ((stateInfo.normalizedTime % 1) >= 0.90f)
        {
            _hasStarted = true;
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in AudioHandler.RunStepRatios)
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
        else if (!Entity.Running)
        {
            SetBool(MonsterAnimationEvent.walk, true);
            return;
        }
    }
}
