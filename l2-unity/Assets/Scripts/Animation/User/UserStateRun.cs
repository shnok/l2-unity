using UnityEngine;

public class UserStateRun : UserStateAction
{
    private bool _hasStarted = false;
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        if (!_enabled)
        {
            return;
        }

        _hasStarted = true;
        _lastNormalizedTime = 0;
        SetBool("run", true, false);
        foreach (var ratio in _audioHandler.RunStepRatios)
        {
            _audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enabled)
        {
            return;
        }

        SetBool("run", true, false);

        if (_hasStarted && (stateInfo.normalizedTime % 1) < 0.5f)
        {
            if (RandomUtils.ShouldEventHappen(_audioHandler.RunBreathChance))
            {
                _audioHandler.PlaySound(CharacterSoundEvent.Breath);
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
                _audioHandler.PlaySoundAtRatio(CharacterSoundEvent.Step, ratio);
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
                SetBool("atkwait", true, true);
                return;
            }
            SetBool("wait", true, true);
        }
        else if (!_entity.Running)
        {
            SetBool("walk", true, true);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
