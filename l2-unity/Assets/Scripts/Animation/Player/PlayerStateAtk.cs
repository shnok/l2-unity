using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAtk : PlayerStateAction {
    private float _lastNormalizedTime;
    private bool moved;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        SetBool("atk01_" + _weaponType, false);
        PlayerController.Instance.SetCanMove(false);
        LoadComponents(animator);
        PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
        _lastNormalizedTime = 0;
        PlayerCombatController.Instance.AutoAttacking = true;
        moved = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Check if the state has looped (re-entered)

        if(!moved) {
            if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
                // This block will be executed once when the state is re-entered after completion
                _lastNormalizedTime = stateInfo.normalizedTime;
                PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
                PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
            }

            if ((stateInfo.normalizedTime % 1) <= 0.50f) {
                PlayerController.Instance.SetCanMove(false);
            } else {
                if ((InputManager.Instance.IsInputPressed(InputType.Move) || PlayerController.Instance.RunningToDestination)) {
                    PlayerController.Instance.SetCanMove(true);
                }
                moved = VerifyRun();
            }
        }

        VerifyAttack();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.LogWarning("Cancel autoattack");
        PlayerCombatController.Instance.AutoAttacking = false;
        PlayerEntity.Instance.StopAutoAttacking();
        PlayerController.Instance.SetCanMove(true);
    }
}