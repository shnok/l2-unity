using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Armor : Item {
    [SerializeField] protected ArmorType _armorType;
    [SerializeField] protected int _pDef;
    [SerializeField] protected int _mDef;
    [SerializeField] protected int _mpBonus;

    public ArmorType ArmorType { get { return _armorType; } }

    public Armor(
        int id,
        string name,
        ItemSlot slot,
        bool crystallizable,
        ArmorType armorType,
        int weight,
        ItemMaterial material,
        ItemGrade grade,
        int avoidNodify,
        int duration,
        int pDef,
        int mDef,
        int mpBonus,
        int price,
        int crystalCount,
        bool sellable,
        bool droppable,
        bool destroyable,
        bool tradeable)
        : base(id, name, "nice armor", slot, weight, price, material, grade, duration, crystallizable, crystalCount, sellable, droppable, tradeable, destroyable) {
        _armorType = armorType;
        _pDef = pDef;
        _mDef = mDef;
        _mpBonus = mpBonus;
        /*_prefab = (GameObject)Resources.Load(Path.Combine("Data", "Animations", "LineageWeapons", _clientName + "_m00_wp", _clientName + "_m00_wp_prefab"));
        if (_prefab == null) {
            Debug.LogWarning($"Could not load prefab for item {_name}.");
        }*/
    }
}
