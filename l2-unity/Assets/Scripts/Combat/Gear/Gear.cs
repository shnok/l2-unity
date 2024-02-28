using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour {
    protected NetworkAnimationController _networkAnimationReceive;
    protected Entity _entity;

    [Header("Weapons")]
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

    private void Awake() {
        _weaponAnim = WeaponTypeParser.GetWeaponAnim(WeaponType.hand);
        _entity = GetComponent<Entity>();
        Initialize();
    }

    protected virtual void Initialize() {
        TryGetComponent(out _networkAnimationReceive);
    }

    public virtual void EquipWeapon(int weaponId, bool leftSlot) {
        UnequipWeapon(leftSlot);
        if (weaponId == 0) {
            return;
        }

        Debug.LogWarning("Equip weapon");

        // Loading from database
        Weapon weapon = ItemTable.Instance.GetWeapon(weaponId);
        if (weapon == null) {
            Debug.LogWarning($"Could find weapon {weaponId} in DB for entity {_entity.Identity.Id}.");
            return;
        }

        GameObject weaponPrefab = ModelTable.Instance.GetWeapon(weaponId);
        if (weaponPrefab == null) {
            Debug.LogWarning($"Could load prefab for {weaponId} in DB for entity {_entity.Identity.Id}.");
            return;
        }

        // Updating weapon type
        if (leftSlot) {
            _leftHandType = weapon.WeaponType;
        } else {
            _rightHandType = weapon.WeaponType;
        }

        _weaponAnim = WeaponTypeParser.GetWeaponAnim(weapon.WeaponType);

        // Instantiating weapon
        GameObject go = GameObject.Instantiate(weaponPrefab);
        go.SetActive(false);
        go.transform.name = "weapon";

        if (weapon.WeaponType == WeaponType.none) {
            go.transform.SetParent(GetShieldBone(), false);
        } else if (weapon.WeaponType == WeaponType.bow) {
            go.transform.SetParent(GetLeftHandBone(), false);
        } else if (leftSlot) {
            go.transform.SetParent(GetLeftHandBone(), false);
        } else {
            go.transform.SetParent(GetRightHandBone(), false);
        }

        go.SetActive(true);
    }

    protected virtual Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = _networkAnimationReceive.transform.FindRecursive("Bow Bone");
        }
        return _leftHandBone;
    }

    protected virtual Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = _networkAnimationReceive.transform.FindRecursive("Sword Bone");
        }
        return _rightHandBone;
    }

    protected virtual Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = _networkAnimationReceive.transform.FindRecursive("Shield Bone");
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
