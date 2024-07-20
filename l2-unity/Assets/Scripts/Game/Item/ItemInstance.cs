using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstance
{
    [SerializeField] AbstractItem _itemData;
    [SerializeField] private int _objectId;
    [SerializeField] private int _itemId;
    [SerializeField] private byte _location;
    [SerializeField] private int _slot;
    [SerializeField] private int _count;
    [SerializeField] private byte _category;
    [SerializeField] private bool _equipped;
    [SerializeField] private int _bodyPart;
    [SerializeField] private long _remainingTime;

    public AbstractItem ItemData { get { return _itemData; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _itemId; } }
    public byte Location { get { return _location; } }
    public bool Equipped { get { return _equipped; } }
    public int Slot { get { return _slot; } }
    public int Count { get { return _count; } }
    public byte Category { get { return _category; } }
    public int BodyPart { get { return _bodyPart; } }
    public long RemainingTime { get { return _remainingTime; } }

    public ItemInstance(int objectId, int itemId, byte location, int slot, int count, byte category, bool equipped, int bodyPart, long remainingTime) {
        _objectId = objectId;
        _itemId = itemId;
        _location = location;
        _slot = slot;
        _count = count;
        _category = category;
        _equipped = equipped;
        _bodyPart = bodyPart;
        _remainingTime = remainingTime;

        if (_category == 0) {
            _itemData = ItemTable.Instance.GetWeapon(_itemId);
        } else if (_category == 1 || _category == 2) {
            _itemData = ItemTable.Instance.GetArmor(_itemId);
        } else {
            _itemData = ItemTable.Instance.GetEtcItem(_itemId);
        }
    }

    // Packet data

    //writeI(item.getObjectId()); // ObjectId
    //writeI(item.getItem().getId()); // ItemId
    //writeB((byte) item.getLocation());  EQUIPPED((byte) 1),INVENTORY((byte) 2),WAREHOUSE((byte) 3);
    //writeI(item.getSlot()); // Slot
    //writeL(item.getCount()); // Quantity
    //writeB(item.getCategory()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
    //writeB(item.getEquipped()); // Equipped : 00-No, 01-yes
    //writeI(item.getItem().getBodyPart().getValue());
    //writeI(item.getEnchant()); // Enchant level
    //writeL(item.getTime());
}
