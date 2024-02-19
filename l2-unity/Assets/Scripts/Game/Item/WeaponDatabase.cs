using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    private static WeaponDatabase _instance;
    public static WeaponDatabase Instance { get { return _instance; } }

    private Dictionary<int, Weapon> _weapons = new Dictionary<int, Weapon>();
    void Awake() {
        if(_instance == null) { _instance = this; }

        _weapons.Add(6, new Weapon(6, "Apprentice's Wand", "Nice wand", 1350, 0, true, ItemMaterial.Steel, WeaponType._1HS, 379, 5, 7, 4));
        _weapons.Add(20, new Weapon(20, "Buckler", "Nice shield", 1410, 2780, true, ItemMaterial.Wood, WeaponType._shield, 0, 0, 0, 0));
        _weapons.Add(14, new Weapon(14, "Bow", "Nice bow", 1930, 12500, true, ItemMaterial.Wood, WeaponType._bow, 293, 23, 9, 12));
        _weapons.Add(177, new Weapon(177, "Mage Staff", "Nice staff", 1050, 244000, true, ItemMaterial.Wood, WeaponType._2HS, 325, 30, 28, 4));
        _weapons.Add(2369, new Weapon(2369, "Squire's Sword", "Nice sword", 1600, 0, true, ItemMaterial.Steel, WeaponType._1HS, 379, 6, 5, 8));
    }

    public Weapon GetWeapon(int id) {
        Weapon weapon;
        _weapons.TryGetValue(id, out weapon);

        return weapon;
    }
}
