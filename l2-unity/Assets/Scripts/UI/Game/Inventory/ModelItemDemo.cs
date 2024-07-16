using System.Collections.Generic;
using UnityEngine;

public class ModelItemDemo
{
    private Texture2D _icon;
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
            return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/armor_leather_helmet_i00");
        }
        else if (_demoL2JItem.Type2 == 0)
        {
            Dictionary<int, Weapongrp> weapon = WeapongrpTable.Instance.WeaponGrps;
            if (weapon.ContainsKey(_item_id))
            {
                Weapongrp weapongrp = weapon[_item_id];
                return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/weapon_small_sword_i00");
            }
            else
            {
                Debug.Log("ModelItemDemo>> not found set default icon " + _demoL2JItem.ItemId);
                return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/NOIMAGE");
            }
        }
        else if (_demoL2JItem.Type2 == 4)
        {
            return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/etc_soulshot_none_for_rookie_i00");
        }
        else
        {
            return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/NOIMAGE");
        }

        //return Resources.Load<Texture2D>("Data/UI/Window/Inventory/demo_image_row/NOIMAGE");

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