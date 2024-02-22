using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationController)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]

public class UserEntity : Entity {

    protected override void Initialize() {
        base.Initialize();
    }

    protected override void OnDeath() {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)PlayerAnimationEvent.death, 1f, true);
    }


    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = _networkAnimationReceive.transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = _networkAnimationReceive.transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = _networkAnimationReceive.transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            _networkAnimationReceive.SetBool("atk01" + WeaponType, true);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            _networkAnimationReceive.SetBool("atk01" + WeaponType, false);
            if(!_networkCharacterControllerReceive.IsMoving()) {
                _networkAnimationReceive.SetBool("atkwait" + WeaponType, true);
            }
        }

        return true;
    }

    protected override void OnHit(bool criticalHit) {
        base.OnHit(criticalHit);
    }

    public override float UpdateMAtkSpeed(int mAtkSpd) {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        _networkAnimationReceive.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd) {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        _networkAnimationReceive.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateSpeed(int speed) {
        float converted = base.UpdateSpeed(speed);
        _networkAnimationReceive.SetMoveSpeed(converted);

        return converted;
    }
}