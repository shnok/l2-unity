using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationController)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]

public class UserEntity : Entity {

    public override void Initialize() {
        base.Initialize();

        EquipAllArmors();
    }

    private void EquipAllArmors() {
        PlayerAppearance appearance = (PlayerAppearance)_appearance;
        if (appearance.Chest != 0) {
            ((UserGear)_gear).EquipArmor(appearance.Chest, ItemSlot.chest);
        } else {
            ((UserGear)_gear).EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
        }

        if (appearance.Legs != 0) {
            ((UserGear)_gear).EquipArmor(appearance.Legs, ItemSlot.legs);
        } else {
            ((UserGear)_gear).EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
        }

        if (appearance.Gloves != 0) {
            ((UserGear)_gear).EquipArmor(appearance.Gloves, ItemSlot.gloves);
        } else {
            ((UserGear)_gear).EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.gloves);
        }

        if (appearance.Feet != 0) {
            ((UserGear)_gear).EquipArmor(appearance.Feet, ItemSlot.feet);
        } else {
            ((UserGear)_gear).EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.feet);
        }
    }

    protected override void OnDeath() {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)PlayerAnimationEvent.death, 1f, true);
    } 

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            _networkAnimationReceive.SetBool("atk01_" + _gear.WeaponAnim, true);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            _networkAnimationReceive.SetBool("atk01_" + _gear.WeaponAnim, false);
            if(!_networkCharacterControllerReceive.IsMoving()) {
                _networkAnimationReceive.SetBool("atkwait_" + _gear.WeaponAnim, true);
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