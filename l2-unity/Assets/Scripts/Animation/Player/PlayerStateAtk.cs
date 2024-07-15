using System.Collections;
using System.Collections.Generic;
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

        //if(TargetManager.Instance.HasAttackTarget()) {
            PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
            PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
         //   PlayerController.Instance.SetCanMove(false);
          //  PlayerCombatController.Instance.AutoAttacking = true;
       // }

        _lastNormalizedTime = 0;
        moved = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if (!moved) {
            if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
                _lastNormalizedTime = stateInfo.normalizedTime;
                PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
                PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
            }

            if ((stateInfo.normalizedTime % 1) <= 0.50f) {
               // PlayerController.Instance.SetCanMove(false);
            } else { 
                //if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination)) {
                //    PlayerController.Instance.SetCanMove(true);
                //}
              //  moved = ShouldRun();
            }

            if ((stateInfo.normalizedTime % 1) >= 0.90f) {
                //if (!TargetManager.Instance.HasAttackTarget() || TargetManager.Instance.AttackTarget.Data.Entity.IsDead()) {
                //    PlayerEntity.Instance.StopAutoAttacking();
                //}
            }
        }

        if (ShouldAttack()) {
            return;
        }

        if (ShouldIdle()) {
            return;
        }

        // ShouldAttack();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_enabled) {
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) {
            Debug.Log("Exiting atk state. Next tag: " + animator.GetCurrentAnimatorStateInfo(0).tagHash);
           // if(PlayerCombatController.Instance.AutoAttacking) {
           //     Debug.LogWarning("Exited attack animation but client is still attacking!");
                //PlayerCombatController.Instance.AutoAttacking = false;
                //PlayerEntity.Instance.StopAutoAttacking();
                //PlayerController.Instance.SetCanMove(true);
           // }
        }
    }
}