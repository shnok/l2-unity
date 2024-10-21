using System;
using UnityEngine;

public class PlayerStateAtk : PlayerStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        PlayerAnimationController.Instance.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool(HumanoidAnimType.atkwait, false, false);
        SetBool(HumanoidAnimType.atk01, false, false);

        PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
        // PlaySoundAtRatio(EntitySoundEvent.Atk_1H, AudioHandler.AtkRatio);
        // PlaySoundAtRatio(ItemSoundEvent.sword_small, AudioHandler.SwishRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atkwait, false, false);
        SetBool(HumanoidAnimType.atk01, false, false);

        if (ShouldDie())
        {
            return;
        }

        if (ShouldAttack())
        {
            return;
        }

        if (ShouldRun())
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

        if (ShouldIdle())
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}