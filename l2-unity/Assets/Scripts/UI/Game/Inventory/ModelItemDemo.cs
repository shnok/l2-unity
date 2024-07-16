using System.Collections.Generic;
using UnityEngine;

public class ModelItemDemo
{
    private ItemName _itemName;
    private ItemStatData _statItem;
    private int _item_id;
    private DemoL2JItem _demoL2JItem;

    public ModelItemDemo(DemoL2JItem demoL2j)
    {
        _item_id = demoL2j.ItemId;
        _itemName = ItemNameTable.Instance.GetItemName(_item_id);
        _statItem = ItemStatDataTable.Instance.GetItemStatData(_item_id);
        this._demoL2JItem = demoL2j;
    }

    public DemoL2JItem GetDemoL2J()
    {
        return _demoL2JItem;
    }

    public int item_id()
    {
        return _item_id;
    }


    //public static final int TYPE2_WEAPON = 0;
    //public static final int TYPE2_SHIELD_ARMOR = 1;
    //public static final int TYPE2_ACCESSORY = 2;
    //public static final int TYPE2_QUEST = 3;
    //public static final int TYPE2_MONEY = 4;
    //public static final int TYPE2_OTHER = 5;
    public Texture2D Icon()
    {
        if(_demoL2JItem.Type2 == 1)
        {
            Armor armor = ItemTable.Instance.GetArmor(_item_id);
            if(armor != null) return IconManager.Instance.LoadTextureByName(armor.Armorgrp.Icon);
        }
        else if (_demoL2JItem.Type2 == 0)
        {
            Weapon weapon = ItemTable.Instance.GetWeapon(_item_id);
            if (weapon != null) return IconManager.Instance.LoadTextureByName(weapon.Weapongrp.Icon);
        }
        else if (_demoL2JItem.Type2 == 4)
        {
            var etcitem = EtcItemgrpTable.Instance.EtcItemGrps[_item_id];
            if(etcitem != null) return IconManager.Instance.LoadTextureByName(etcitem.Icon);
        }

        return IconManager.Instance.LoadTextureByName("NOIMAGE");
    }

    public string Name()
    {
        return _itemName.Name;
    }

    public int Slot()
    {
        return _demoL2JItem.Slot;
    }

    public int Type2()
    {
       return _demoL2JItem.Type2;
    }

    public int Equipped()
    {
        return _demoL2JItem.Equipped;
    }

    public int Location()
    {
        return _demoL2JItem.Location;
    }
}