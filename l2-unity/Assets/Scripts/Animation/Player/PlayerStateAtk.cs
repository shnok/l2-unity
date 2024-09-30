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

        SetBool("atkwait", true, false, false);
        SetBool("atk01", true, false, false);

        PlaySoundAtRatio(EntitySoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("atkwait", true, false, false);
        SetBool("atk01", true, false, false);

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