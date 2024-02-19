using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity {

    private static PlayerEntity _instance;
    public static PlayerEntity Instance { get => _instance; }

    protected override void Initialize() {
        base.Initialize();

        if (_instance == null) {
            _instance = this;
        }
    }

    protected override void LookAtTarget() { }

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

    protected override void OnDeath() {
        base.OnDeath();
        Debug.Log("Player on death _networkAnimationReceive:" + _networkAnimationReceive);
        PlayerAnimationController.Instance.SetAnimationProperty((int)PlayerAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit) {
        base.OnHit(criticalHit);
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
            PlayerAnimationController.Instance.SetAnimationProperty((int)PlayerAnimationEvent.Atk011HS, 1f);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            PlayerController.Instance.StopLookAt();
            PlayerAnimationController.Instance.SetAnimationProperty((int)PlayerAnimationEvent.AtkWait1HS, 1f);
        }

        return true;
    }

    public override float UpdateMAtkSpeed(int mAtkSpd) {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        PlayerAnimationController.Instance.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd) {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        PlayerAnimationController.Instance.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateSpeed(int speed) {
        float converted = base.UpdateSpeed(speed);
        PlayerAnimationController.Instance.SetMoveSpeed(converted);
        PlayerController.Instance.DefaultSpeed = converted;

        return converted;
    }

    public void OnActionFailed(PlayerAction action) {
        switch (action) {
            case PlayerAction.SetTarget:
                TargetManager.Instance.ClearTarget();
                break;
            case PlayerAction.AutoAttack:
                PlayerCombatController.Instance.OnAutoAttackFailed();
                break;
        }
    }
}