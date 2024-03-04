using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGear : UserGear
{
    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = PlayerAnimationController.Instance.transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = PlayerAnimationController.Instance.transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = PlayerAnimationController.Instance.transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    protected override void UpdateIdleAnimation() {
        Debug.LogWarning("Update idle animation");
        PlayerAnimationController.Instance.SetBool("wait_" + _weaponAnim, true);
    }
}
