using System.Collections.Generic;
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

    [Header("Bodyparts")]
    [SerializeField] private GameObject _headBone; // Used in some cases when the hair model doesnt have an armature
    [SerializeField] private GameObject[] _hairs;
    [SerializeField] private GameObject _face;
    [SerializeField] private bool _bodyReady;

    public GameObject[] Hairs { get => _hairs; set => _hairs = value; }
    public GameObject Face { get => _face; set => _face = value; }
    public GameObject HeadBone { get => _headBone; set => _headBone = value; }

    [Header("Trail")]
    [SerializeField] private ParticleSystem _weaponTrail;

    public override void Initialize(int ownderId)
    {
        base.Initialize(ownderId);

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

        if (_headBone == null)
        {
            Debug.LogWarning($"[{transform.name}] Headbone was not assigned, please pre-assign it to avoid unecessary load.");
            _headBone = transform.GetChild(0)
                .GetChild(0)
                .GetChild(0)
                .GetChild(0)
                .GetChild(0)
                .GetChild(0)
                .GetChild(2)
                .GetChild(0)
                .GetChild(0)
                .GetChild(0).gameObject;
        }

        if (_weaponTrail == null)
        {
            Debug.LogWarning($"[{transform.name}] WeaponTrail was not assigned, please pre-assign it to avoid unecessary load.");
            Transform _ = _rightHandBone.Find("weapon_trail");
            _weaponTrail = _?.GetComponent<ParticleSystem>();

            if (_weaponTrail == null)
            {
                Debug.LogError($"[{transform.name}] WeaponTrail is null!");
            }
            else
            {
                StopTrail();
            }
        }
        else
        {
            StopTrail();
        }
    }

    public override void StartTrail()
    {
        // Debug.Log("Start weapon trail");
        _weaponTrail.gameObject.SetActive(true);
        _weaponTrail.Play();
    }

    public override void StopTrail()
    {
        // Debug.Log("Stop weapon trail");
        _weaponTrail.Stop();
        _weaponTrail.gameObject.SetActive(false);
    }

    public override void EquipWeapon(int weaponId, Weapon weapon, bool leftSlot)
    {
        base.EquipWeapon(weaponId, weapon, leftSlot);
        UpdateTrailScale();
    }

    private void UpdateTrailScale()
    {
        float scale = 0;
        float offset = 0;
        switch (_weaponAnim)
        {
            case WeaponAnimType._1HS:
                scale = 0.8f;
                offset = -0.003f;
                break;
            case WeaponAnimType._2HS:
                scale = 1.5f;
                offset = -0.004f;
                break;
            case WeaponAnimType.pole:
                scale = 2.5f;
                offset = -0.005f;
                break;
            default:
                break;
        }

        _weaponTrail.transform.localScale = Vector3.one * 0.1f * scale * _weaponSizeRatio;
        _weaponTrail.transform.localPosition = new Vector3(offset, 0, 0);
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
            case ItemSlot.SLOT_CHEST:
                return itemId == _torsoMeta.Id;
            case ItemSlot.SLOT_FULL_ARMOR:
                return itemId == _fullarmorMeta.Id;
            case ItemSlot.SLOT_LEGS:
                return itemId == _legsMeta.Id;
            case ItemSlot.SLOT_GLOVES:
                return itemId == _glovesMeta.Id;
            case ItemSlot.SLOT_FEET:
                return itemId == _bootsMeta.Id;
        }

        return true;
    }

    public override void EquipAllArmors(Appearance apr)
    {
        PlayerAppearance appearance = (PlayerAppearance)apr;
        if (appearance.Chest != 0)
        {
            EquipArmor(appearance.Chest, ItemSlot.SLOT_CHEST);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.SLOT_CHEST);
        }

        if (appearance.Legs != 0)
        {
            EquipArmor(appearance.Legs, ItemSlot.SLOT_LEGS);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.SLOT_LEGS);
        }

        if (appearance.Gloves != 0)
        {
            EquipArmor(appearance.Gloves, ItemSlot.SLOT_GLOVES);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.SLOT_GLOVES);
        }

        if (appearance.Feet != 0)
        {
            EquipArmor(appearance.Feet, ItemSlot.SLOT_FEET);
        }
        else
        {
            EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.SLOT_FEET);
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

        ModelTable.L2ArmorPiece armorPiece = ModelTable.Instance.GetArmorPiece(armor, _referenceHolder.Entity.RaceId);
        if (armorPiece == null)
        {
            Debug.LogWarning($"Can't find armor {itemId} for race {_referenceHolder.Entity.RaceId} in slot {slot} in ModelTable");
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
            case ItemSlot.SLOT_CHEST:
                if (_torso != null)
                {
                    DestroyImmediate(_torso);
                    _torsoMeta = null;
                }
                if (_fullarmor != null)
                {
                    DestroyImmediate(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.SLOT_LEGS);
                }
                _torso = armorPiece;
                _torsoMeta = armor;
                _torso.transform.SetParent(_bodypartsContainer.transform, false);
                break;
            case ItemSlot.SLOT_FULL_ARMOR:
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
            case ItemSlot.SLOT_LEGS:
                if (_legs != null)
                {
                    DestroyImmediate(_legs);
                    _legsMeta = null;
                }
                if (_fullarmor != null)
                {
                    DestroyImmediate(_fullarmor);
                    _fullarmorMeta = null;
                    EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.SLOT_CHEST);
                }
                _legs = armorPiece;
                _legs.transform.SetParent(_bodypartsContainer.transform, false);
                _legsMeta = armor;
                break;
            case ItemSlot.SLOT_GLOVES:
                if (_gloves != null)
                {
                    DestroyImmediate(_gloves);
                    _glovesMeta = null;
                }
                _gloves = armorPiece;
                _gloves.transform.SetParent(_bodypartsContainer.transform, false);
                _glovesMeta = armor;
                break;
            case ItemSlot.SLOT_FEET:
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

    public override void UpdateAppearance(Appearance baseOldAppearance, Appearance baseNewAppearance)
    {
        base.UpdateAppearance(baseOldAppearance, baseNewAppearance);

        PlayerAppearance newAppearance = (PlayerAppearance)baseNewAppearance;
        PlayerAppearance oldAppearance = new PlayerAppearance();

        if (!_bodyReady)
        {
            _hairs = new GameObject[3];
        }
        else
        {
            oldAppearance = (PlayerAppearance)baseOldAppearance;
        }

        // Debug.LogWarning($"UpdateAppearance: OldFace:{poldAppearance.Face} NewFace:{pnewAppearance.Face}");

        CharacterModelType raceId = _referenceHolder.Entity.RaceId;

        if (oldAppearance.ShouldUpdateHair(newAppearance) || !_bodyReady)
        {
            // Update hairstyle
            GameObject hair1 = ModelTable.Instance.GetHair(raceId, newAppearance.HairStyle, newAppearance.HairColor, false);
            GameObject hair2 = ModelTable.Instance.GetHair(raceId, newAppearance.HairStyle, newAppearance.HairColor, true);

            if (_hairs[(int)HairType.AH] != null)
            {
                Destroy(_hairs[(int)HairType.AH]);
            }
            if (_hairs[(int)HairType.BH] != null)
            {
                Destroy(_hairs[(int)HairType.BH]);
            }

            _hairs[(int)HairType.AH] = hair1;
            _hairs[(int)HairType.BH] = hair2;

            hair1?.transform.SetParent(_bodypartsContainer.transform, false);

            if (hair2 != null)
            {
                if (hair2.CompareTag("Hair")) // this hair doesnt have an armature and needs to be placed manually under the head bone
                {

                    Vector3 origin = hair2.transform.localPosition;
                    Vector3 originEuler = hair2.transform.eulerAngles;

                    hair2.transform.SetParent(_headBone.transform);

                    hair2.transform.localPosition = origin;
                    hair2.transform.localEulerAngles = originEuler;
                }
                else
                {
                    hair2.transform.SetParent(_bodypartsContainer.transform, false);
                }
            }
        }

        if (oldAppearance.ShouldUpdateFace(newAppearance) || !_bodyReady)
        {
            if (_face != null)
            {
                Destroy(_face);
            }

            // Update face
            GameObject face = ModelTable.Instance.GetFace(raceId, newAppearance.Face);
            if (face != null)
            {
                _face = face;
                _face.transform.SetParent(_bodypartsContainer.transform, false);
                _face.transform.localScale = Vector3.one;
                _face.transform.localPosition = Vector3.zero;
            }
        }

        if (oldAppearance.ShouldUpdateArmors(newAppearance) || !_bodyReady)
        {
            EquipAllArmors(newAppearance);
        }
        _bodyReady = true;

        _skinnedMeshSync.SyncMesh();
    }
}