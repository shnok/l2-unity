using UnityEngine;

public class HumanoidStateJump : HumanoidStateBase
{
    private float _lastNormalizedTime = 0;
    private bool _wasRunning = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        if (_animator.GetBool("run_jump") == true)
        {
            _wasRunning = true;
        }

        SetBool("jump", false, false);
        SetBool("run_jump", false, false);
        _audioHandler.PlaySound(EntitySoundEvent.Jump_1);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("jump", false, false);
        SetBool("run_jump", false, false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            if (_wasRunning)
            {
                SetBool("run", true, true);
            }
            else
            {
                SetBool("wait", true, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _audioHandler.PlaySound(EntitySoundEvent.Step);
        _audioHandler.PlaySound(EntitySoundEvent.Step);
    }
}