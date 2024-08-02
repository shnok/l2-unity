using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractItem
{
    [SerializeField] protected int _id;
    [SerializeField] protected string _icon;
    [SerializeField] protected ItemName _itemName;
    [SerializeField] protected ItemStatData _itemStatData;
    [SerializeField] protected Abstractgrp _itemgrp;

    public int Id { get { return _id; }}
    public string Icon { get { return _icon; } }
    public ItemName ItemName { get { return _itemName; } }
    public ItemStatData ItemStatData { get { return _itemStatData; } }
    public Abstractgrp Itemgrp { get { return _itemgrp; } }

    public AbstractItem(int id, ItemName itemName, ItemStatData itemStatData, Abstractgrp itemgrp, string icon) {
        _id = id;
        _itemName = itemName;
        _itemStatData = itemStatData;
        _icon = icon;
        _itemgrp = itemgrp;
    }
}
