using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Weapon : Item
{
    [SerializeField] protected WeaponType _weaponType; 
    [SerializeField] protected byte _shoulshot;
    [SerializeField] protected byte _spiritshot;
    [SerializeField] protected int _shieldDef;
    [SerializeField] protected int _shieldDefRate;
    [SerializeField] protected int _mpConsume;
    [SerializeField] protected int _atkSpd;
    [SerializeField] protected int _pAtk;
    [SerializeField] protected int _mAtk;
    [SerializeField] protected int _critical;

    public WeaponType WeaponType { get { return _weaponType; } }

    public Weapon(
        int id, 
        string name, 
        ItemSlot slot, 
        bool crystallizable, 
        int weight, 
        byte soulshot,
        byte spiritshot, 
        ItemMaterial material, 
        ItemGrade grade, 
        int pAtk, 
        int rndDmg, 
        WeaponType weaponType, 
        int critical, 
        int hitModify, 
        int avoidNodify, 
        int shieldDef, 
        int shieldDefRate, 
        int atkSpeed, 
        int mpConsume, 
        int mAtk, 
        int duration, 
        int price, 
        int crystalCount, 
        bool sellable, 
        bool droppable, 
        bool destroyable, 
        bool tradeable)
        : base(id, name, "nice weapon",slot, weight, price, material, grade, duration, crystallizable, crystalCount, sellable, droppable, tradeable, destroyable) {
        _weaponType = weaponType;
        _atkSpd = atkSpeed;
        _pAtk = pAtk;
        _mAtk = mAtk;
        _critical = critical;
        _shoulshot = soulshot;
        _spiritshot = spiritshot;
        _shieldDef = shieldDef;
        _shieldDefRate = shieldDefRate;
        _mpConsume = mpConsume;
        _prefab = (GameObject) Resources.Load(Path.Combine("Data", "Animations", "LineageWeapons", _clientName + "_m00_wp", _clientName + "_m00_wp_prefab"));
        if(_prefab == null) {
            Debug.LogWarning($"Could not load prefab for item {_name}.");
        }
    }
}
