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

        _networkAnimationController.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool("wait", true, false);
        SetBool("atkwait", true, false);
        SetBool("atk01", true, false);

        PlaySoundAtRatio(EntitySoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);

        _lastNormalizedTime = 0;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool("atk01", true, false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            PlaySoundAtRatio(EntitySoundEvent.Atk_1H, _audioHandler.AtkRatio);
            PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}