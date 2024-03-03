using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGear : Gear
{
    protected SkinnedMeshSync _skinnedMeshSync;

    [Header("Armors")]
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _torso;
    [SerializeField] private GameObject _fullarmor;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _gloves;
    [SerializeField] private GameObject _boots;

    protected override void Initialize() {
        base.Initialize();

        if(this is PlayerGear) {
            _container = this.gameObject;
        } else {
            _container = transform.GetChild(0).gameObject;
        }

        _skinnedMeshSync = _container.GetComponentInChildren<SkinnedMeshSync>();
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

    public void EquipArmor(int itemId, ItemSlot slot) {
        ModelTable.L2ArmorPiece armorPiece = ModelTable.Instance.GetArmorPieceByItemId(itemId, _entity.RaceId);
        if (armorPiece == null) {
            Debug.LogWarning($"Can't find armor {itemId} for race {_entity.RaceId} in slot {slot} in ModelTable");
            return;
        }

        GameObject mesh = Instantiate(armorPiece.baseArmorModel);
        mesh.GetComponentInChildren<SkinnedMeshRenderer>().material = armorPiece.material;

        SetArmorPiece(mesh, slot);
    }

    private void SetArmorPiece(GameObject armorPiece, ItemSlot slot) {
        switch (slot) {
            case ItemSlot.chest:
                if (_torso != null) {
                    Destroy(_torso);
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
                }
                _torso = armorPiece;
                _torso.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.fullarmor:
                if (_torso != null) {
                    Destroy(_torso);
                }
                if (_legs != null) {
                    Destroy(_legs);
                }
                _fullarmor = armorPiece;
                _fullarmor.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.legs:
                if (_legs != null) {
                    Destroy(_legs);
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
                }
                _legs = armorPiece;
                _legs.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.gloves:
                if (_gloves != null) {
                    Destroy(_gloves);
                }
                _gloves = armorPiece;
                _gloves.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.feet:
                if (_boots != null) {
                    Destroy(_boots);
                }
                _boots = armorPiece;
                _boots.transform.SetParent(_container.transform, false);
                break;
        }

        _skinnedMeshSync.SyncMesh();
    }
}
