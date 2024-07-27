using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemListPacket : AbstractItemPacket
{
    private ItemInstance[] _items;
    public ItemInstance[] Items { get { return _items; } }

    private bool _openWindow;
    public Boolean OpenWindow { get { return _openWindow; } }
    
    public InventoryItemListPacket(byte[] d) : base(d){
        Parse();
    }

    public override void Parse() {
        // writeB((byte) (showWindow ? 0x01 : 0x00));
        // writeI(items.size());
        _openWindow = ReadB() == 1;
        int itemListSize = ReadI();

        _items = new ItemInstance[itemListSize];
        for(int i = 0; i < itemListSize; i++) {
            _items[i] = ReadItem();
        }
    }
}
