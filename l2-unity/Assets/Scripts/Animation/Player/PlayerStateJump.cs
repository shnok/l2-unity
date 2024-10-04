using UnityEngine;

public class PlayerStateJump : PlayerStateAction
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _lastNormalizedTime = 0;

        SetBool(HumanoidAnimType.jump, false, false);
        SetBool(HumanoidAnimType.runjump, false, false);
        _audioHandler.PlaySound(EntitySoundEvent.Jump_1);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ShouldDie())
        {
            return;
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;

            if (ShouldRun())
            {
                return;
            }

            if (ShouldWalk())
            {
                return;
            }

            SetBool(HumanoidAnimType.wait, true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraController.Instance.StickToBone = false;
        _audioHandler.PlaySound(EntitySoundEvent.Step);
        _audioHandler.PlaySound(EntitySoundEvent.Step);
    }
}