using UnityEngine;

public class HumanoidStateJump : HumanoidStateBase
{
    private float _lastNormalizedTime = 0;
    private bool _wasRunning = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        if (GetBool(HumanoidAnimType.runjump) == true)
        {
            _wasRunning = true;
        }

        SetBool(HumanoidAnimType.jump, false);
        SetBool(HumanoidAnimType.runjump, false);
        _audioHandler.PlaySound(EntitySoundEvent.Jump_1);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.jump, false);
        SetBool(HumanoidAnimType.runjump, false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            if (_wasRunning)
            {
                SetBool(HumanoidAnimType.run, true);
            }
            else
            {
                SetBool(HumanoidAnimType.wait, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _audioHandler.PlaySound(EntitySoundEvent.Step);
        _audioHandler.PlaySound(EntitySoundEvent.Step);
    }
}