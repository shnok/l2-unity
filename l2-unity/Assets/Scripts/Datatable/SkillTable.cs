using System.Collections.Generic;
using UnityEngine;

public class SkillTable
{
    private static SkillTable _instance;
    public static SkillTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillTable();
            }

            return _instance;
        }
    }

    [SerializeField] private List<int> _skillToLoad = new List<int>();

    public Dictionary<int, Skillgrp> Skills { get; private set; }
    public Dictionary<int, SkillNameData> SkillNames { get; private set; }
    public Dictionary<int, L2SkillEffectEmitter> SkillEffects { get; private set; }

    private bool _loadAll = false;

    public void Initialize()
    {
        FillDataToLoad();
    }

    public void CacheItems()
    {
        CacheWeapons();
        CacheArmors();
        CacheEtcItems();
    }

    private void FillDataToLoad()
    {
        _skillToLoad = new List<int> { 16, 56, 3, 1216, 1177, 1012, 1011, 1168, 4345, 1040, 1027, 1015, 1147, 1164, 91, 77, 70, 29, 1090, 1100, 1097, 1010, 1095 };
    }


    // Mortal Blow 16
    // Power Shot 56
    // Power Strike 3
    // Self Heal 1216
    // Wind Strike 1177
    // Cure Poison 1012
    // Heal 1011
    // Curse: Poison 1168
    // Might 4345
    // Shield 1040
    // Group Heal 1027 
    // Battle Heal 1015 
    // Vampiric Touch 1147 
    // Curse: Weakness 1164 
    // Defense Aura 91 
    // Attack Aura 77
    // Drain Health 70
    // Iron Punch 29
    // Life Drain 1090
    // Chill Flame 1100 
    // Dreaming Spirit 1097 
    // Soul Shield 1010
    // Venom 1095 

    /*


    Soulshots (cast):
    2039 NG
    2150 D
    2151 C
    2152 B
    2153 A
    2154 S

    */

    public bool ShouldLoadSkill(int id)
    {
        if (_loadAll) return true;

        FillDataToLoad();

        if (_skillToLoad.Count == 0)
        {
            return true;
        }

        if (_skillToLoad.Contains(id))
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
