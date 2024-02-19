using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Weapon : Item
{
    [SerializeField] protected WeaponType _weaponType; 
    [SerializeField] protected int _atkSpd;
    [SerializeField] protected int _pAtk;
    [SerializeField] protected int _mAtk;
    [SerializeField] protected int _critical;

    public WeaponType WeaponType { get { return _weaponType; } }

    public Weapon(int id, string name, string description, int weight, int price, bool droppable, ItemMaterial material, WeaponType weaponType, int atkSpd, int pAtk, int mAtk, int critical) 
        : base(id, name, description, weight, price, droppable, material) {
        _weaponType = weaponType;
        _atkSpd = atkSpd;
        _pAtk = pAtk;
        _mAtk = mAtk;
        _critical = critical;
        _prefab = (GameObject) Resources.Load(Path.Combine("Data", "Animations", "LineageWeapons", _clientName + "_m00_wp", _clientName + "_m00_wp_prefab"));
        if(_prefab == null) {
            Debug.LogWarning($"Could not load prefab for item {_name}.");
        }
    }
}
