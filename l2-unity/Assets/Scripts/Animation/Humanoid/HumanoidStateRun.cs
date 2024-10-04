using UnityEngine;

public class HumanoidStateRun : HumanoidStateAction
{
    private bool _hasStarted = false;
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _hasStarted = true;
        _lastNormalizedTime = 0;
        SetBool(HumanoidAnimType.run, false);
        foreach (var ratio in _audioHandler.RunStepRatios)
        {
            _audioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.run, false);

        if (_hasStarted && (stateInfo.normalizedTime % 1) < 0.5f)
        {
            if (RandomUtils.ShouldEventHappen(_audioHandler.RunBreathChance))
            {
                _audioHandler.PlaySound(EntitySoundEvent.Breath);
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
            foreach (var ratio in _audioHandler.RunStepRatios)
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
        else if (!_entity.Running)
        {
            SetBool(HumanoidAnimType.walk, true);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
