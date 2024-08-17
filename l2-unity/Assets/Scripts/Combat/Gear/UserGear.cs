using UnityEngine;

public class UserGear : Gear
{
    protected SkinnedMeshSync _skinnedMeshSync;
    [Header("Armors")]
    [Header("Meta")]
    [SerializeField] private Armor _torsoMeta;
    [SerializeField] private Armor _fullarmorMeta;
    [SerializeField] private Armor _legsMeta;
    [SerializeField] private Armor _glovesMeta;
    [SerializeField] private Armor _bootsMeta;
    [Header("Models")]
    [SerializeField] private GameObject _container;
    [SerializeField] private GameObject _torso;
    [SerializeField] private GameObject _fullarmor;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _gloves;
    [SerializeField] private GameObject _boots;

    public override void Initialize(int ownderId, CharacterRaceAnimation raceId) {
        base.Initialize(ownderId, raceId);

        if(this is PlayerGear) {
            _container = this.gameObject;
        } else {
            _container = transform.GetChild(0).gameObject;
        }

        _skinnedMeshSync = _container.GetComponentInChildren<SkinnedMeshSync>();
    }

    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public bool IsArmorAlreadyEquipped(int itemId, ItemSlot slot) {
        //Debug.Log($"IsArmorAlreadyEquipped ({itemId},{slot})");
        
        switch(slot) {
            case ItemSlot.chest:
                return itemId == _torsoMeta.Id;
            case ItemSlot.fullarmor:
                return itemId == _fullarmorMeta.Id;
            case ItemSlot.legs:
                return itemId == _legsMeta.Id;
            case ItemSlot.gloves:
                return itemId == _glovesMeta.Id;
            case ItemSlot.feet:
                return itemId == _bootsMeta.Id;
        }

        return true;
    }

    public void EquipArmor(int itemId, ItemSlot slot) {
        if(IsArmorAlreadyEquipped(itemId, slot)) {
            Debug.Log($"Item {itemId} is already equipped in slot {slot}.");
            return;
        }

        Armor armor = ItemTable.Instance.GetArmor(itemId);
        if (armor == null) {
            Debug.LogWarning($"Can't find armor {itemId} in ItemTable");
            return;
        }

        ModelTable.L2ArmorPiece armorPiece = ModelTable.Instance.GetArmorPiece(armor, _raceId);
        if (armorPiece == null) {
            Debug.LogWarning($"Can't find armor {itemId} for race {_raceId} in slot {slot} in ModelTable");
            return;
        }

        GameObject mesh = Instantiate(armorPiece.baseArmorModel);
        mesh.GetComponentInChildren<SkinnedMeshRenderer>().material = armorPiece.material;

        Debug.LogWarning("Instantiate new armor piece");

        SetArmorPiece(armor, mesh, slot);
    }

    private void SetArmorPiece(Armor armor, GameObject armorPiece, ItemSlot slot) {
        switch (slot) {
            case ItemSlot.chest:
                if (_torso != null) {
                    Destroy(_torso);
                    _torsoMeta = null;
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
                }
                _torso = armorPiece;
                _torsoMeta = armor;
                _torso.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.fullarmor:
                if (_torso != null) {
                    Destroy(_torso);
                    _torsoMeta = null;
                }
                if (_legs != null) {
                    Destroy(_legs);
                    _legsMeta = null;
                }
                _fullarmor = armorPiece;
                _fullarmorMeta = armor;
                _fullarmor.transform.SetParent(_container.transform, false);
                break;
            case ItemSlot.legs:
                if (_legs != null) {
                    Destroy(_legs);
                    _legsMeta = null;
                }
                if (_fullarmor != null) {
                    Destroy(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
                }
                _legs = armorPiece;
                _legs.transform.SetParent(_container.transform, false);
                _legsMeta = armor;
                break;
            case ItemSlot.gloves:
                if (_gloves != null) {
                    Destroy(_gloves);
                    _glovesMeta = null;
                }
                _gloves = armorPiece;
                _gloves.transform.SetParent(_container.transform, false);
                _glovesMeta = armor;
                break;
            case ItemSlot.feet:
                if (_boots != null) {
                    Destroy(_boots);
                    _bootsMeta = null;
                }
                _boots = armorPiece;
                _boots.transform.SetParent(_container.transform, false);
                _bootsMeta = armor;
                break;
        }

        _skinnedMeshSync.SyncMesh();
    }
}
