using UnityEngine;

public class PlayerStateAtk : PlayerStateAction {
    private float _lastNormalizedTime;
    private bool moved;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0) {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        PlayerAnimationController.Instance.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool("atkwait_" + _weaponAnim, false, false);
        SetBool("atk01_" + _weaponAnim, false, false);

        PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);

        _lastNormalizedTime = 0;
        moved = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        SetBool("atkwait_" + _weaponAnim, false, false);
        SetBool("atk01_" + _weaponAnim, false, false);

        if(ShouldDie()) {
            return;
        }

        if (ShouldAttack()) {
            return;
        }

        if (ShouldRun()) {
            return;
        }

        if (ShouldIdle()) {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }
    }
}