using System.Collections.Generic;
using UnityEngine;

public class ItemTable
{
    private static ItemTable _instance;
    public static ItemTable Instance
    {
        get
        {
            if (_instance == null)
            {
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
    private Dictionary<int, EtcItem> _etcItems = new Dictionary<int, EtcItem>();

    public Dictionary<int, Weapon> Weapons { get { return _weapons; } }
    public Dictionary<int, Armor> Armors { get { return _armors; } }
    public Dictionary<int, EtcItem> EtcItems { get { return _etcItems; } }

    public static int NAKED_CHEST = 21;
    public static int NAKED_LEGS = 28;
    public static int NAKED_GLOVES = 48;
    public static int NAKED_BOOTS = 35;
    private bool _loadAll = false;

    public void Initialize()
    {
        FillDataToLoad();
    }

    // private void OnDestroy()
    // {
    //     _weapons.Clear();
    //     _armors.Clear();
    //     _instance = null;
    // }

    public void CacheItems()
    {
        CacheWeapons();
        CacheArmors();
        CacheEtcItems();
    }

    private void FillDataToLoad()
    {
        weaponsToLoad = new List<int> { 1, 2, 3, 4, 5, 6, 7, 10, 14, 89, 102, 129, 177, 188, 156, 275, 2369, 2370, 5284, 20 };
        armorsToLoad = new List<int> { NAKED_CHEST, NAKED_LEGS, NAKED_GLOVES, NAKED_BOOTS, 425, 461, 1146, 1147, 45, 118, 112, 116 };
        itemsToLoad = new List<int>() { 1835, 3947, 2509, 57, 736 };
    }

    public bool ShouldLoadItem(int id)
    {
        if (_loadAll) return true;

        if (weaponsToLoad.Count == 0 && armorsToLoad.Count == 0 && itemsToLoad.Count == 0)
        {
            return true;
        }

        if (weaponsToLoad.Contains(id))
        {
            return true;
        }
        if (armorsToLoad.Contains(id))
        {
            return true;
        }
        if (itemsToLoad.Contains(id))
        {
            return true;
        }

        return false;
    }

    private void CacheWeapons()
    {
        foreach (KeyValuePair<int, Weapongrp> kvp in WeapongrpTable.Instance.WeaponGrps)
        {
            if (_armors.ContainsKey(kvp.Key))
            {
                continue;
            }

            Weapongrp weapongrp = kvp.Value;
            ItemName itemName = ItemNameTable.Instance.GetItemName(kvp.Key);
            ItemStatData itemStatData = ItemStatDataTable.Instance.GetItemStatData(kvp.Key);

            Weapon weapon = new Weapon(kvp.Key, itemName, itemStatData, weapongrp);

            if (!_weapons.ContainsKey(kvp.Key))
            {
                _weapons.Add(kvp.Key, weapon);
            }

        }
    }

    private void CacheArmors()
    {
        foreach (KeyValuePair<int, Armorgrp> kvp in ArmorgrpTable.Instance.ArmorGrps)
        {
            if (_armors.ContainsKey(kvp.Key))
            {
                continue;
            }

            Armorgrp armorgrp = kvp.Value;
            ItemName itemName = ItemNameTable.Instance.GetItemName(kvp.Key);
            ItemStatData itemStatData = ItemStatDataTable.Instance.GetItemStatData(kvp.Key);

            Armor armor = new Armor(kvp.Key, itemName, itemStatData, armorgrp);
            _armors.Add(kvp.Key, armor);
        }
    }

    private void CacheEtcItems()
    {
        foreach (KeyValuePair<int, EtcItemgrp> kvp in EtcItemgrpTable.Instance.EtcItemGrps)
        {
            if (_etcItems.ContainsKey(kvp.Key))
            {
                continue;
            }

            EtcItemgrp itemgrp = kvp.Value;
            ItemName itemName = ItemNameTable.Instance.GetItemName(kvp.Key);
            ItemStatData itemStatData = ItemStatDataTable.Instance.GetItemStatData(kvp.Key);

            EtcItem item = new EtcItem(kvp.Key, itemName, itemStatData, itemgrp);
            _etcItems.Add(kvp.Key, item);
        }
    }

    public Weapon GetWeapon(int id)
    {
        Weapon weapon;
        _weapons.TryGetValue(id, out weapon);

        return weapon;
    }

    public Armor GetArmor(int id)
    {
        Armor armor;
        _armors.TryGetValue(id, out armor);

        return armor;
    }

    public EtcItem GetEtcItem(int id)
    {
        EtcItem item;
        _etcItems.TryGetValue(id, out item);

        return item;
    }

    public AbstractItem GetItem(int id)
    {
        if (_weapons.TryGetValue(id, out Weapon weapon))
        {
            return weapon;
        }
        if (_armors.TryGetValue(id, out Armor armor))
        {
            return armor;
        }
        if (_etcItems.TryGetValue(id, out EtcItem etcItem))
        {
            return etcItem;
        }

        return null;
    }
}
