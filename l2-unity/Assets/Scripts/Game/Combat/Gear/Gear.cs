using System;
using UnityEngine;

public class Gear : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _referenceHolder;

    protected int _ownerId;
    protected CharacterModelType _raceId;

    [Header("Bones")]
    [SerializeField] protected Transform _rightHandBone;
    [SerializeField] protected Transform _leftHandBone;
    [SerializeField] protected Transform _shieldBone;

    [Header("Weapons")]
    [Header("Meta")]
    [SerializeField] private Weapon _rightHandWeapon;
    [SerializeField] private Weapon _leftHandWeapon;
    [SerializeField] protected float _weaponSizeRatio;
    [Header("Models")]
    [Header("Right hand")]
    [SerializeField] private WeaponType _rightHandType;
    [SerializeField] protected Transform _rightHand;
    [SerializeField] protected Transform _arrow;
    [Header("LeftHand")]
    [SerializeField] private WeaponType _leftHandType;
    [SerializeField] protected Transform _leftHand;

    protected NewBaseAnimationController AnimationController { get { return _referenceHolder.NewAnimationController; } }
    public WeaponType WeaponType { get { return _leftHandType != WeaponType.none ? _leftHandType : _rightHandType; } }
    public int OwnerId { get { return _ownerId; } set { _ownerId = value; } }
    public CharacterModelType RaceId { get { return _raceId; } set { _raceId = value; } }

    public Transform RightHandBone { get { return _rightHandBone; } }
    public Transform LeftHandBone { get { return _leftHandBone; } }
    public Transform Arrow { get { return _arrow; } }

    public virtual void Initialize(int ownderId, CharacterModelType raceId)
    {
        if (_referenceHolder == null)
        {
            TryGetComponent(out _referenceHolder);
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
        }

        _ownerId = ownderId;
        _raceId = raceId;

        GetLeftHandBone();
        GetRightHandBone();
    }

    // TODO: PRE-ASSIGN BONES IN PREFAB TO AVOID CPU LOAD

    public bool IsWeaponAlreadyEquipped(int itemId, bool leftSlot)
    {
        Debug.Log($"IsWeaponAlreadyEquipped ({itemId},{leftSlot})");

        if (leftSlot)
        {
            if (_leftHandWeapon == null)
            {
                Debug.Log("Left hand metadata is null, weapon not equiped.");
                return false;
            }

            bool idMatch = itemId == _leftHandWeapon.Id;
            if (!idMatch)
            {
                Debug.Log("Left hand weapon id did not match, weapon not equiped.");
            }

            return idMatch;
        }
        else
        {
            if (_rightHandWeapon == null)
            {
                Debug.Log("Right hand metadata is null, weapon not equiped.");
                return false;
            }

            bool idMatch = itemId == _rightHandWeapon.Id;
            if (!idMatch)
            {
                Debug.Log("Right hand weapon id did not match, weapon not equiped.");
            }

            return idMatch;
        }
    }

    public virtual void EquipAllWeapons(Appearance appearance)
    {
        if (appearance.RHand != 0)
        {
            // Loading from table
            Weapon weapon = ItemTable.Instance.GetWeapon(appearance.RHand);
            if (weapon == null)
            {
                Debug.LogWarning($"Could find weapon {appearance.RHand} in DB for entity {_ownerId}.");
                return;
            }

            if (weapon.Weapongrp.WeaponType == WeaponType.bow)
            {
                appearance.LHand = appearance.RHand;
                appearance.RHand = 0;
                UnequipWeapon(false);
            }
            else
            {
                EquipWeapon(appearance.RHand, weapon, false);
            }
        }
        else
        {
            UnequipWeapon(false);
        }


        if (appearance.LHand != 0)
        {
            // Loading from table
            Weapon weapon = ItemTable.Instance.GetWeapon(appearance.LHand);
            if (weapon == null)
            {
                Debug.LogWarning($"Could find weapon {appearance.LHand} in DB for entity {_ownerId}.");
                return;
            }

            EquipWeapon(appearance.LHand, weapon, true);
        }
        else
        {
            UnequipWeapon(true);
        }
    }

    public virtual void EquipArrow()
    {
        Debug.Log($"[{transform.name}] Equip arrow");
        GameObject arrowPrefab = ModelTable.Instance.GetItemModelById(17);
        if (arrowPrefab == null)
        {
            Debug.LogWarning($"Could not load arrow prefab in DB for entity {_ownerId}.");
            return;
        }

        GameObject go = GameObject.Instantiate(arrowPrefab);
        go.SetActive(false);
        go.transform.name = "arrow";

        _arrow = go.transform;
        _arrow.parent = RightHandBone;
        _arrow.localPosition = new Vector3(-0.0005f, 0, 0);
        _arrow.localRotation = new Quaternion(0, 0, 0, 0);
        _arrow.localScale = Vector3.one * GetWeaponSizeRatio();
    }

    private float GetWeaponSizeRatio()
    {
        if (_weaponSizeRatio == 0)
        {
            float collisionHeight = _referenceHolder.Entity.Appearance.CollisionHeight;
            float ratio = 1 + (collisionHeight - 0.45f) / 0.45f;

            // Debug.Log("WeaponSizeRatio: " + ratio);

            _weaponSizeRatio = ratio;
        }

        return _weaponSizeRatio;
    }

    public virtual void UnEquipArrow()
    {
        if (_arrow == null)
        {
            return;
        }

        Debug.Log($"[{transform.name}] Unequip arrow");
        GameObject.DestroyImmediate(_arrow.gameObject);
    }

    public virtual void ShowArrow()
    {
        if (_arrow == null)
        {
            EquipArrow();
        }

        // Debug.Log($"[{transform.name}] Show arrow");
        _arrow?.gameObject.SetActive(true);
    }

    public virtual void HideArrow()
    {
        // Debug.Log($"[{transform.name}] Hide arrow");
        _arrow?.gameObject.SetActive(false);
    }

    public virtual void EquipAllArmors(Appearance appearance) { }

    public virtual void EquipWeapon(int weaponId, Weapon weapon, bool leftSlot)
    {
        if (weaponId == 0)
        {
            return;
        }

        WeaponType weaponType = weapon.Weapongrp.WeaponType;
        if (IsWeaponAlreadyEquipped(weaponId, leftSlot))
        {
            Debug.Log($"Weapon {weaponId} of type {weaponType} is already equipped in {(leftSlot ? "left" : "right")} slot.");
            return;
        }
        else
        {
            Debug.Log($"Weapon {weaponId} of type {weaponType} was not equipped in {(leftSlot ? "left" : "right")} slot.");
        }

        UnequipWeapon(leftSlot);

        GameObject weaponPrefab = ModelTable.Instance.GetWeaponById(weaponId);
        if (weaponPrefab == null)
        {
            Debug.LogWarning($"Could load prefab for {weaponId} in DB for entity {_ownerId}.");
            return;
        }

        // Updating weapon type
        if (leftSlot)
        {
            _leftHandWeapon = weapon;
            _leftHandType = weapon.Weapongrp.WeaponType;
        }
        else
        {
            _rightHandWeapon = weapon;
            _rightHandType = weapon.Weapongrp.WeaponType;
        }

        if (weapon.Weapongrp.WeaponType != WeaponType.none)
        { // Do not update for shields
            UpdateWeaponType(weapon.Weapongrp.WeaponType);
        }

        // Instantiating weapon
        GameObject go = GameObject.Instantiate(weaponPrefab);
        go.SetActive(false);
        go.transform.name = "weapon";

        if (weapon.Weapongrp.WeaponType == WeaponType.none)
        {
            go.transform.SetParent(GetShieldBone(), false);
        }
        else if (weapon.Weapongrp.WeaponType == WeaponType.bow)
        {
            go.transform.SetParent(GetLeftHandBone(), false);
        }
        else if (leftSlot)
        {
            go.transform.SetParent(GetLeftHandBone(), false);
        }
        else
        {
            go.transform.SetParent(GetRightHandBone(), false);
        }

        go.SetActive(true);

        go.transform.localScale *= GetWeaponSizeRatio();

        if (weaponType == WeaponType.bow)
        {
            EquipArrow();
        }
    }

    protected virtual void UpdateWeaponType(WeaponType weaponType) { }

    public virtual void UpdateWeaponAnim(WeaponAnimType value) { }

    protected virtual Transform GetLeftHandBone()
    {
        if (_leftHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign bones to avoid unecessary load.");
            _leftHandBone = transform.FindRecursive("Bow Bone");
        }

        if (_leftHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign bones to avoid unecessary load.");
            _leftHandBone = transform.FindRecursive("bow_bone");
        }

        if (_leftHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign bones to avoid unecessary load.");
            _leftHandBone = transform.FindRecursive("Sword Bone01");
        }
        return _leftHandBone;
    }

    protected virtual Transform GetRightHandBone()
    {
        if (_rightHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign bones to avoid unecessary load.");
            _rightHandBone = transform.FindRecursive("Sword Bone");
        }
        return _rightHandBone;
    }

    protected virtual Transform GetShieldBone()
    {
        if (_shieldBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign bones to avoid unecessary load.");
            _shieldBone = transform.FindRecursive("Shield Bone");
        }
        return _shieldBone;
    }

    public virtual void UnequipWeapon(bool leftSlot)
    {
        Transform weaponBone = leftSlot ? GetLeftHandBone() : GetRightHandBone();
        if (weaponBone == null)
        {
            return;
        }

        Transform weapon = weaponBone.Find("weapon") ?? (leftSlot ? GetShieldBone().Find("weapon") : null);

        if (weapon != null)
        {
            Debug.Log("Unequip weapon: " + weapon);
            Destroy(weapon.gameObject);

            if (WeaponType == WeaponType.bow)
            {
                UnEquipArrow();
            }

            if (leftSlot)
            {
                _leftHandWeapon = null;
                _leftHandType = WeaponType.hand;
                UpdateWeaponAnim(WeaponAnimParser.GetWeaponAnim(_rightHandType == WeaponType.none ? WeaponType.hand : _rightHandType));
            }
            else
            {
                _rightHandWeapon = null;
                _rightHandType = WeaponType.hand;
                UpdateWeaponAnim(WeaponAnimParser.GetWeaponAnim(_leftHandType == WeaponType.none ? WeaponType.hand : _leftHandType));
            }
        }
    }

    public virtual void StartTrail()
    {
    }

    public virtual void StopTrail()
    {
    }
}
