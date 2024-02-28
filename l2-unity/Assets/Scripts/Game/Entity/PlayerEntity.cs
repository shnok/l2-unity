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

        EquipAllArmors();
    }

    void OnDestroy() {
        _instance = null;
    }

    private void EquipAllArmors() {
        PlayerAppearance appearance = (PlayerAppearance) _appearance;
        if(appearance.Chest != 0) {
            ((PlayerGear)_gear).EquipArmor(appearance.Chest);
        } else {
            ((PlayerGear)_gear).EquipArmor(0, ItemSlot.chest);
        }

        if (appearance.Legs != 0) {
            ((PlayerGear)_gear).EquipArmor(appearance.Legs);
        } else {
            ((PlayerGear)_gear).EquipArmor(0, ItemSlot.legs);
        }

        if (appearance.Gloves != 0) {
            ((PlayerGear)_gear).EquipArmor(appearance.Gloves);
        } else {
            ((PlayerGear)_gear).EquipArmor(0, ItemSlot.gloves);
        }

        if (appearance.Feet != 0) {
            ((PlayerGear)_gear).EquipArmor(appearance.Feet);
        } else {
            ((PlayerGear)_gear).EquipArmor(0, ItemSlot.feet);
        }
    }

    protected override void LookAtTarget() { }

    protected override void OnDeath() {
        base.OnDeath();
        Debug.Log("Player on death _networkAnimationReceive:" + _networkAnimationReceive);
        PlayerAnimationController.Instance.SetAnimationProperty((int)PlayerAnimationEvent.death, 1f, true);
    }

    protected override void OnHit(bool criticalHit) {
        base.OnHit(criticalHit);
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
            PlayerAnimationController.Instance.SetBool("atk01_" + _gear.WeaponAnim, true);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            PlayerController.Instance.StopLookAt();
            if(!PlayerController.Instance.IsMoving()) {
                PlayerAnimationController.Instance.SetBool("atkwait_" + _gear.WeaponAnim, true);
            }
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