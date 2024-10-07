using UnityEngine;

public class PlayerStateRun : PlayerStateAction
{
    private bool _hasStarted = false;
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _hasStarted = true;
        _lastNormalizedTime = 0;

        foreach (var ratio in AudioHandler.RunStepRatios)
        {
            AudioHandler.PlaySoundAtRatio(EntitySoundEvent.Step, ratio);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

        if (ShouldDie())
        {
            return;
        }

        if (ShouldAttack())
        {
            SetBool(HumanoidAnimType.atk01, true, false);
            return;
        }

        if (ShouldJump(true))
        {
            return;
        }

        if (ShouldWalk())
        {
            return;
        }

        if (ShouldSit())
        {
            return;
        }

        if (ShouldAtkWait())
        {
            return;
        }

        if (ShouldIdle())
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.run, false, false);
    }
}
