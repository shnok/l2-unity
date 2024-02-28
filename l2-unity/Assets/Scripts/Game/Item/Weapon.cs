using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Weapon : Item{
    [SerializeField] private WeaponType _weaponType; 
    [SerializeField] private byte _shoulshot;
    [SerializeField] private byte _spiritshot;
    [SerializeField] private int _shieldDef;
    [SerializeField] private int _shieldDefRate;
    [SerializeField] private int _mpConsume;
    [SerializeField] private int _atkSpd;
    [SerializeField] private int _pAtk;
    [SerializeField] private int _mAtk;
    [SerializeField] private int _critical;

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
    }
}
