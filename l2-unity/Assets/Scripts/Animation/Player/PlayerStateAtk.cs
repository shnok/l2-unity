using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAtk : PlayerStateAction {
    private float _lastNormalizedTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        SetBool("atk01_" + _weaponType, false);
        PlayerController.Instance.SetCanMove(false);
        LoadComponents(animator);
        PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
        PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Check if the state has looped (re-entered)
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            // This block will be executed once when the state is re-entered after completion
            _lastNormalizedTime = stateInfo.normalizedTime;
            PlaySoundAtRatio(CharacterSoundEvent.Atk_1H, _audioHandler.AtkRatio);
            PlaySoundAtRatio(ItemSoundEvent.sword_small, _audioHandler.SwishRatio);
        }

        if ((stateInfo.normalizedTime % 1) <= 0.50f) {
            PlayerController.Instance.SetCanMove(false);
        } else {
            PlayerController.Instance.SetCanMove(true);
            VerifyRun();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        PlayerController.Instance.SetCanMove(true);
    }
}