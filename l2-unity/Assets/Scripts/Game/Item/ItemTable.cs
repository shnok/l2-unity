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

    private Dictionary<int, Weapon> _weapons = new Dictionary<int, Weapon>();
    private Dictionary<int, Armor> _armors = new Dictionary<int, Armor>();
    public Dictionary<int, Weapon> Weapons { get { return _weapons; } }
    public Dictionary<int, Armor> Armors { get { return _armors; } }

    public void Initialize() {
        CacheWeapons();
        CacheArmors();
    }

    private void OnDestroy() {
        _weapons.Clear();
        _armors.Clear();
        _instance = null;
    }

    private void CacheWeapons() {
        _weapons.Add(1, new Weapon(1, "Short Sword", ItemSlot.rhand, false, 1600, 1, 1, ItemMaterial.steel, ItemGrade.none, 8, 10, WeaponType.sword, 8, 0, 0, 0, 0, 379, 0, 6, -1, 768, 0, true, true, true, true));
        _weapons.Add(2, new Weapon(2, "Long Sword", ItemSlot.rhand, false, 1560, 2, 2, ItemMaterial.fine_steel, ItemGrade.none, 24, 10, WeaponType.sword, 8, 0, 0, 0, 0, 379, 0, 17, -1, 136000, 0, true, true, true, true));
        _weapons.Add(3, new Weapon(3, "Broadsword", ItemSlot.rhand, false, 1590, 1, 1, ItemMaterial.steel, ItemGrade.none, 11, 10, WeaponType.sword, 8, 0, 0, 0, 0, 379, 0, 9, -1, 12500, 0, true, true, true, true));
        _weapons.Add(4, new Weapon(4, "Club", ItemSlot.rhand, false, 1870, 1, 1, ItemMaterial.wood, ItemGrade.none, 8, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 6, -1, 768, 0, true, true, true, true));
        _weapons.Add(5, new Weapon(5, "Mace", ItemSlot.rhand, false, 1880, 1, 1, ItemMaterial.steel, ItemGrade.none, 11, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 9, -1, 12500, 0, true, true, true, true));
        _weapons.Add(6, new Weapon(6, "Apprentice's Wand", ItemSlot.rhand, false, 1350, 1, 1, ItemMaterial.steel, ItemGrade.none, 5, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 7, -1, 138, 0, false, false, true, false));
        _weapons.Add(7, new Weapon(7, "Apprentice's Rod", ItemSlot.rhand, false, 1330, 1, 1, ItemMaterial.wood, ItemGrade.none, 6, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 8, -1, 768, 0, true, true, true, true));
        _weapons.Add(10, new Weapon(10, "Dagger", ItemSlot.rhand, false, 1160, 1, 1, ItemMaterial.steel, ItemGrade.none, 5, 5, WeaponType.dagger, 12, -3, 0, 0, 0, 433, 0, 5, -1, 138, 0, false, false, true, false));
        _weapons.Add(14, new Weapon(14, "Bow", ItemSlot.lrhand, false, 1930, 1, 1, ItemMaterial.wood, ItemGrade.none, 23, 5, WeaponType.bow, 12, -3, 0, 0, 0, 293, 1, 9, -1, 12500, 0, true, true, true, true));
        _weapons.Add(20, new Weapon(20, "Buckler", ItemSlot.lhand, false, 1410, 0, 0, ItemMaterial.wood, ItemGrade.none, 0, 0, WeaponType.none, 0, 0, -8, 67, 20, 0, 0, 0, -1, 2780, 0, true, true, true, true));
        _weapons.Add(102, new Weapon(102, "Round Shield", ItemSlot.lhand, false, 1390, 0, 0, ItemMaterial.steel, ItemGrade.none, 0, 0, WeaponType.none, 0, 0, -8, 79, 20, 0, 0, 0, -1, 7110, 0, true, true, true, true));
        _weapons.Add(89, new Weapon(89, "Big Hammer", ItemSlot.rhand, true, 1710, 2, 2, ItemMaterial.fine_steel, ItemGrade.c, 107, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 61, -1, 2290000, 916, true, true, true, true));
        _weapons.Add(129, new Weapon(129, "Sword of Revolution", ItemSlot.rhand, true, 1450, 3, 3, ItemMaterial.fine_steel, ItemGrade.d, 79, 10, WeaponType.sword, 8, 0, 0, 0, 0, 379, 0, 47, -1, 1400000, 2545, true, true, true, true));
        _weapons.Add(188, new Weapon(177, "Mage Staff", ItemSlot.lrhand, false, 1050, 2, 2, ItemMaterial.wood, ItemGrade.none, 30, 20, WeaponType.bigblunt, 4, 4, 0, 0, 0, 325, 0, 28, -1, 244000, 0, true, true, true, true));
        _weapons.Add(156, new Weapon(156, "Hand Axe", ItemSlot.rhand, true, 1820, 2, 2, ItemMaterial.steel, ItemGrade.d, 40, 20, WeaponType.blunt, 4, 4, 0, 0, 0, 379, 0, 26, -1, 409000, 743, true, true, true, true));
        _weapons.Add(275, new Weapon(275, "Long Bow", ItemSlot.lrhand, true, 1830, 6, 2, ItemMaterial.steel, ItemGrade.d, 114, 5, WeaponType.bow, 12, -3, 0, 0, 0, 227, 4, 35, -1, 644000, 1170, true, true, true, true));
        _weapons.Add(2369, new Weapon(2369, "Squire's Sword", ItemSlot.rhand, false, 1600, 1, 1, ItemMaterial.steel, ItemGrade.none, 6, 10, WeaponType.sword, 8, 0, 0, 0, 0, 379, 0, 5, -1, 138, 0, false, false, true, false));
    }

    private void CacheArmors() {
        _armors.Add(425, new Armor(425, 1, "Apprentice's Tunic", ItemSlot.chest, false, ArmorType.magic, 2150, ItemMaterial.cloth, ItemGrade.none, 0, -1, 17, 0, 19, 26, 0, false, false, true, false));
        _armors.Add(461, new Armor(461, 1, "Apprentice's Stockings", ItemSlot.legs, false, ArmorType.magic, 1100, ItemMaterial.cloth, ItemGrade.none, 0, -1, 10, 0, 10, 6, 0, false, false, true, false));
        _armors.Add(1146, new Armor(1146, 1, "Squire's Shirt", ItemSlot.chest, false, ArmorType.light, 3301, ItemMaterial.cloth, ItemGrade.none, 0, -1, 33, 0, 0, 26, 0, false, false, true, false));
        _armors.Add(1147, new Armor(1147, 1, "Squire's Pants", ItemSlot.legs, false, ArmorType.light, 1750, ItemMaterial.cloth, ItemGrade.none, 0, -1, 20, 0, 0, 6, 0, false, false, true, false));
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
