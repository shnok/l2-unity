using UnityEngine;

public class UserStateWalk : UserStateAction
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        if (!_enabled)
        {
            return;
        }

        _lastNormalizedTime = 0;
        SetBool("walk", true, false);
        foreach (var ratio in _audioHandler.WalkStepRatios)
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

        SetBool("walk", true, false);

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            foreach (var ratio in _audioHandler.WalkStepRatios)
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
        else if (_entity.Running)
        {
            SetBool("run", true, true);
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
