using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemListPacket : AbstractItemPacket
{
    private ItemInstance[] _items;
    public ItemInstance[] Items { get { return _items; } }
    
    public InventoryItemListPacket(byte[] d) : base(d){
        Parse();
    }

    public override void Parse() {
        // writeB((byte) (showWindow ? 0x01 : 0x00));
        // writeI(items.size());
        bool openWindow = ReadB() == 1;
        int itemListSize = ReadI();

        Debug.Log("OpenWindow? " + openWindow);
        Debug.Log("itemListSize " + itemListSize);

        _items = new ItemInstance[itemListSize];
        for(int i = 0; i < itemListSize; i++) {
            _items[i] = ReadItem();
        }
    }
}
