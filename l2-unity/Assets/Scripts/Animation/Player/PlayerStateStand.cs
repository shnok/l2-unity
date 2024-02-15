using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateStand : PlayerStateBase {
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        SetBool("stand", false);
        _lastNormalizedTime = 0;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f) {
            _lastNormalizedTime = stateInfo.normalizedTime;
            SetBool("wait_" + _weaponType, true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CameraController.Instance.StickToBone = false;
        PlayerController.Instance.CanMove = true;
    }
}
