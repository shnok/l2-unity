using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTable
{
    private static ItemTable _instance;
    public static ItemTable Instance {
        get {
            if (_instance == null) {
                _instance = new ItemTable();
            }

            return _instance;
        }
    }

    [SerializeField] private List<int> weaponsToLoad = new List<int>();
    [SerializeField] private List<int> armorsToLoad = new List<int>();
    [SerializeField] private List<int> itemsToLoad = new List<int>();

    private Dictionary<int, Weapon> _weapons = new Dictionary<int, Weapon>();
    private Dictionary<int, Armor> _armors = new Dictionary<int, Armor>();
    public Dictionary<int, Weapon> Weapons { get { return _weapons; } }
    public Dictionary<int, Armor> Armors { get { return _armors; } }

    public static int NAKED_CHEST = 21;
    public static int NAKED_LEGS = 28;
    public static int NAKED_GLOVES = 48;
    public static int NAKED_BOOTS = 35;

    public void Initialize() {
        FillDataToLoad();
    }

    private void OnDestroy() {
        _weapons.Clear();
        _armors.Clear();
        _instance = null;
    }

    public void CacheItems() {
        CacheWeapons();
        CacheArmors();
    }

    private void FillDataToLoad() {
        weaponsToLoad = new List<int> { 1, 2, 3, 4, 5, 6, 7, 10, 14, 20, 89, 102, 129, 188, 156, 275, 2369 };
        armorsToLoad = new List<int> { NAKED_CHEST, NAKED_LEGS, NAKED_GLOVES, NAKED_BOOTS, 425, 461, 1146, 1147 };
        itemsToLoad = new List<int>();
    }

    public bool ShouldLoadItem(int id) {
        FillDataToLoad(); //TODO: Remove
        if (weaponsToLoad.Count == 0 && armorsToLoad.Count == 0 && itemsToLoad.Count == 0) {
            return true;
        }

        if (weaponsToLoad.Contains(id)) {
            return true;
        }
        if (armorsToLoad.Contains(id)) {
            return true;
        }
        if (itemsToLoad.Contains(id)) {
            return true;
        }

        return false;
    }

    private void CacheWeapons() {
        foreach (KeyValuePair<int, Weapongrp> kvp in WeapongrpTable.Instance.WeaponGrps) {
            if (_armors.ContainsKey(kvp.Key)) {
                continue;
            }

            Weapongrp weapongrp = kvp.Value;
            ItemName itemName = ItemNameTable.Instance.GetItemName(kvp.Key);
            ItemStatData itemStatData = ItemStatDataTable.Instance.GetItemStatData(kvp.Key);

            Weapon weapon = new Weapon(kvp.Key, itemName, itemStatData, weapongrp);
            _weapons.Add(kvp.Key, weapon);
        }
    }

    private void CacheArmors() {
        foreach (KeyValuePair<int, Armorgrp> kvp in ArmorgrpTable.Instance.ArmorGrps) {
            if (_armors.ContainsKey(kvp.Key)) {
                continue;
            }

            Armorgrp armorgrp = kvp.Value;
            ItemName itemName = ItemNameTable.Instance.GetItemName(kvp.Key);
            ItemStatData itemStatData = ItemStatDataTable.Instance.GetItemStatData(kvp.Key);

            Armor armor = new Armor(kvp.Key, itemName, itemStatData, armorgrp);
            _armors.Add(kvp.Key, armor);
        }
    }

    public Weapon GetWeapon(int id) {
        Weapon weapon;
        _weapons.TryGetValue(id, out weapon);

        return weapon;
    }

    public Armor GetArmor(int id) {
        Armor armor;
        _armors.TryGetValue(id, out armor);

        return armor;
    }
}
