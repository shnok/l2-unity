using UnityEngine;

public class UserGear : HumanoidGear
{
    [Header("References")]
    [SerializeField] private SkinnedMeshSync _skinnedMeshSync;
    [SerializeField] private GameObject _bodypartsContainer;

    [Header("Armors")]
    [Header("Meta")]
    [SerializeField] private Armor _torsoMeta;
    [SerializeField] private Armor _fullarmorMeta;
    [SerializeField] private Armor _legsMeta;
    [SerializeField] private Armor _glovesMeta;
    [SerializeField] private Armor _bootsMeta;

    [Header("Models")]
    [SerializeField] private GameObject _torso;
    [SerializeField] private GameObject _fullarmor;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _gloves;
    [SerializeField] private GameObject _boots;


    public override void Initialize(int ownderId, CharacterModelType raceId)
    {
        base.Initialize(ownderId, raceId);

        if (_bodypartsContainer == null)
        {
            Debug.LogWarning($"[{transform.name}] bodypartsContainer was not assigned, please pre-assign it to avoid unecessary load.");
            _bodypartsContainer = transform.GetChild(0).GetChild(1).gameObject;
        }

        if (_skinnedMeshSync == null)
        {
            Debug.LogWarning($"[{transform.name}] SkinnedMeshSync was not assigned, please pre-assign it to avoid unecessary load.");
            _skinnedMeshSync = _bodypartsContainer.GetComponentInChildren<SkinnedMeshSync>();
        }
    }

    protected override Transform GetLeftHandBone()
    {
        if (_leftHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Left hand bone was not assigned, please pre-assign it to avoid unecessary load.");
            _leftHandBone = transform.FindRecursive("Weapon_L_Bone");
        }
        return _leftHandBone;
    }

    protected override Transform GetRightHandBone()
    {
        if (_rightHandBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Right hand bone was not assigned, please pre-assign it to avoid unecessary load.");
            _rightHandBone = transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone()
    {
        if (_shieldBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Shield bone was not assigned, please pre-assign it to avoid unecessary load.");
            _shieldBone = transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public bool IsArmorAlreadyEquipped(int itemId, ItemSlot slot)
    {
        //Debug.Log($"IsArmorAlreadyEquipped ({itemId},{slot})");

        switch (slot)
        {
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

    public override void EquipAllArmors(Appearance apr)
    {
        PlayerAppearance appearance = (PlayerAppearance)apr;
        if (appearance.Chest != 0)
        {
            EquipArmor(appearance.Chest, ItemSlot.chest);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
        }

        if (appearance.Legs != 0)
        {
            EquipArmor(appearance.Legs, ItemSlot.legs);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
        }

        if (appearance.Gloves != 0)
        {
            EquipArmor(appearance.Gloves, ItemSlot.gloves);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.gloves);
        }

        if (appearance.Feet != 0)
        {
            EquipArmor(appearance.Feet, ItemSlot.feet);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.feet);
        }
    }

    public void EquipArmor(int itemId, ItemSlot slot)
    {
        if (IsArmorAlreadyEquipped(itemId, slot))
        {
            Debug.Log($"Item {itemId} is already equipped in slot {slot}.");
            return;
        }

        Armor armor = ItemTable.Instance.GetArmor(itemId);
        if (armor == null)
        {
            Debug.LogWarning($"Can't find armor {itemId} in ItemTable");
            return;
        }

        ModelTable.L2ArmorPiece armorPiece = ModelTable.Instance.GetArmorPiece(armor, _raceId);
        if (armorPiece == null)
        {
            Debug.LogWarning($"Can't find armor {itemId} for race {_raceId} in slot {slot} in ModelTable");
            return;
        }

        GameObject mesh = Instantiate(armorPiece.baseArmorModel);
        mesh.GetComponentInChildren<SkinnedMeshRenderer>().material = armorPiece.material;

        SetArmorPiece(armor, mesh, slot);
    }

    private void SetArmorPiece(Armor armor, GameObject armorPiece, ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.chest:
                if (_torso != null)
                {
                    DestroyImmediate(_torso);
                    _torsoMeta = null;
                }
                if (_fullarmor != null)
                {
                    DestroyImmediate(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
                }
                _torso = armorPiece;
                _torsoMeta = armor;
                _torso.transform.SetParent(_bodypartsContainer.transform, false);
                break;
            case ItemSlot.fullarmor:
                if (_torso != null)
                {
                    DestroyImmediate(_torso);
                    _torsoMeta = null;
                }
                if (_legs != null)
                {
                    DestroyImmediate(_legs);
                    _legsMeta = null;
                }
                _fullarmor = armorPiece;
                _fullarmorMeta = armor;
                _fullarmor.transform.SetParent(_bodypartsContainer.transform, false);
                break;
            case ItemSlot.legs:
                if (_legs != null)
                {
                    DestroyImmediate(_legs);
                    _legsMeta = null;
                }
                if (_fullarmor != null)
                {
                    DestroyImmediate(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
                }
                _legs = armorPiece;
                _legs.transform.SetParent(_bodypartsContainer.transform, false);
                _legsMeta = armor;
                break;
            case ItemSlot.gloves:
                if (_gloves != null)
                {
                    DestroyImmediate(_gloves);
                    _glovesMeta = null;
                }
                _gloves = armorPiece;
                _gloves.transform.SetParent(_bodypartsContainer.transform, false);
                _glovesMeta = armor;
                break;
            case ItemSlot.feet:
                if (_boots != null)
                {
                    DestroyImmediate(_boots);
                    _bootsMeta = null;
                }
                _boots = armorPiece;
                _boots.transform.SetParent(_bodypartsContainer.transform, false);
                _bootsMeta = armor;
                break;
        }

        _skinnedMeshSync.SyncMesh();
    }
}