using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractItem
{
    [SerializeField] protected int _id;
    [SerializeField] protected ItemName _itemName;
    [SerializeField] protected ItemStatData _itemStatData;

    public AbstractItem(int id, ItemName itemName, ItemStatData itemStatData) {
        _id = id;
        _itemName = itemName;
        _itemStatData = itemStatData;
    }
}
