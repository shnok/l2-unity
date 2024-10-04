using UnityEngine;

public class HumanoidStateWalk : HumanoidStateAction
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _lastNormalizedTime = 0;
        SetBool(HumanoidAnimType.walk, false);
        foreach (var ratio in _audioHandler.WalkStepRatios)
        {
            _audioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.walk, false);

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in _audioHandler.WalkStepRatios)
            {
                _audioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
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
                SetBool(HumanoidAnimType.atkwait, true);
                return;
            }
            SetBool(HumanoidAnimType.wait, true);
        }
        else if (_entity.Running)
        {
            SetBool(HumanoidAnimType.run, true);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
