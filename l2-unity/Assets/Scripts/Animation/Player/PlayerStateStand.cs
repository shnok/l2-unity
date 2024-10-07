using UnityEngine;

public class PlayerStateStand : PlayerStateAction
{
    // private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.stand, false, false);
        // _lastNormalizedTime = 0;
        AudioHandler.PlaySound(EntitySoundEvent.Standup);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.stand, false, false);

        if (ShouldDie())
        {
            return;
        }

        if (ShouldIdle())
        {
            return;
        }

        // if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        // {
        //     _lastNormalizedTime = stateInfo.normalizedTime;
        //     SetBool("wait", true, true);
        // }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CameraController.Instance.StickToBone = false;
        //  PlayerController.Instance.SetCanMove(true);
    }
}
