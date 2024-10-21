using UnityEngine;

public class HumanoidStateAtk : HumanoidStateBase
{
    private float _lastNormalizedTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        AnimationController.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool(HumanoidAnimType.wait, false);
        SetBool(HumanoidAnimType.atkwait, false);
        SetBool(HumanoidAnimType.atk01, false);

        PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
        // PlaySoundAtRatio(ItemSoundEvent.sword_small, AudioHandler.SwishRatio);

        _lastNormalizedTime = 0;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atk01, false);
        // if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        // {
        //     _lastNormalizedTime = stateInfo.normalizedTime;
        //     PlaySoundAtRatio(EntitySoundEvent.Atk_1H, AudioHandler.AtkRatio);
        //     // PlaySoundAtRatio(ItemSoundEvent.sword_small, AudioHandler.SwishRatio);
        // }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}