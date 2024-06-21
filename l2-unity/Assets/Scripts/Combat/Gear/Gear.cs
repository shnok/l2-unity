using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour {
    protected NetworkAnimationController _networkAnimationReceive;
    protected int _ownerId;
    protected CharacterRaceAnimation _raceId;

    [Header("Weapons")]
    [Header("Meta")]
    [SerializeField] private Weapon _rightHandWeapon;
    [SerializeField] private Weapon _leftHandWeapon;
    [Header("Models")]
    [Header("Right hand")]
    [SerializeField] private WeaponType _rightHandType;
    [SerializeField] protected Transform _rightHandBone;
    [SerializeField] protected Transform _rightHand;
    [Header("LeftHand")]
    [SerializeField] private WeaponType _leftHandType;
    [SerializeField] protected Transform _leftHandBone;
    [SerializeField] protected Transform _shieldBone;
    [SerializeField] protected Transform _leftHand;
    [SerializeField] protected string _weaponAnim;

    public WeaponType WeaponType { get { return _leftHandType != WeaponType.none ? _leftHandType : _rightHandType; } }
    public string WeaponAnim { get { return _weaponAnim; } }
    public int OwnerId { get { return _ownerId; } set { _ownerId = value; } }
    public CharacterRaceAnimation RaceId { get { return _raceId; } set { _raceId = value; } }

    public virtual void Initialize(int ownderId, CharacterRaceAnimation raceId) {
        TryGetComponent(out _networkAnimationReceive);
        _ownerId = ownderId;
        _raceId = raceId;
    }

    public virtual void EquipWeapon(int weaponId, bool leftSlot) {
        UnequipWeapon(leftSlot);
        if (weaponId == 0) {
            return;
        }

        Debug.LogWarning("Equip weapon: " + weaponId);

        // Loading from database
        Weapon weapon = ItemTable.Instance.GetWeapon(weaponId);
        if (weapon == null) {
            Debug.LogWarning($"Could find weapon {weaponId} in DB for entity {_ownerId}.");
            return;
        }

        GameObject weaponPrefab = ModelTable.Instance.GetWeaponById(weaponId);
        if (weaponPrefab == null) {
            Debug.LogWarning($"Could load prefab for {weaponId} in DB for entity {_ownerId}.");
            return;
        }

        // Updating weapon type
        if (leftSlot) {
            _leftHandWeapon = weapon;
            _leftHandType = weapon.Weapongrp.WeaponType;
        } else {
            _rightHandWeapon = weapon;
            _rightHandType = weapon.Weapongrp.WeaponType;
        }

        UpdateWeaponType(weapon.Weapongrp.WeaponType);

        // Instantiating weapon
        GameObject go = GameObject.Instantiate(weaponPrefab);
        go.SetActive(false);
        go.transform.name = "weapon";

        if (weapon.Weapongrp.WeaponType == WeaponType.none) {
            go.transform.SetParent(GetShieldBone(), false);
        } else if (weapon.Weapongrp.WeaponType == WeaponType.bow) {
            go.transform.SetParent(GetLeftHandBone(), false);
        } else if (leftSlot) {
            go.transform.SetParent(GetLeftHandBone(), false);
        } else {
            go.transform.SetParent(GetRightHandBone(), false);
        }

        go.SetActive(true);
    }

    private void UpdateWeaponType(WeaponType weaponType) {
        _weaponAnim = WeaponTypeParser.GetWeaponAnim(weaponType);
        UpdateIdleAnimation();
    }

    protected virtual void UpdateIdleAnimation() {
        if(_networkAnimationReceive != null) {
            _networkAnimationReceive.SetBool("wait_" + _weaponAnim, true);
        }
    }

    protected virtual Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Bow Bone");
        }
        return _leftHandBone;
    }

    protected virtual Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Sword Bone");
        }
        return _rightHandBone;
    }

    protected virtual Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield Bone");
        }
        return _shieldBone;
    }

    protected virtual void UnequipWeapon(bool leftSlot) {
        Transform weapon;
        if (leftSlot) {
            weapon = GetLeftHandBone().Find("weapon");
        } else {
            weapon = GetRightHandBone().Find("weapon");
        }

        if (weapon != null) {
            Debug.LogWarning("Unequip weapon");
            Destroy(weapon);
        }
    }
}
