using UnityEngine;

public class ItemInstance
{
    [SerializeField] AbstractItem _itemData;
    [SerializeField] private int _objectId;
    [SerializeField] private int _itemId;
    [SerializeField] private ItemLocation _location;
    [SerializeField] private int _slot;
    [SerializeField] private int _count;
    [SerializeField] private ItemType1 _type1;
    [SerializeField] private ItemType2 _type2;
    [SerializeField] private bool _equipped;
    [SerializeField] private ItemSlot _bodyPart;
    [SerializeField] private int _enchantLevel;
    [SerializeField] private long _remainingTime;
    [SerializeField] private int _lastChange;

    public AbstractItem ItemData { get { return _itemData; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _itemId; } }
    public ItemLocation Location { get { return _location; } }
    public bool Equipped { get { return _equipped; } }
    public int Slot { get { return _slot; } }
    public int Count { get { return _count; } }
    public ItemType1 Type1 { get { return _type1; } }
    public ItemType2 Type2 { get { return _type2; } }
    public ItemSlot BodyPart { get { return _bodyPart; } }
    public int EnchantLevel { get { return _enchantLevel; } }
    public long RemainingTime { get { return _remainingTime; } }
    public int LastChange { get { return _lastChange; } set { _lastChange = value; } }

    public ItemInstance(int objectId, int itemId, ItemLocation location, int slot, int count, ItemType1 type1, ItemType2 type2, bool equipped, ItemSlot bodyPart, int enchantLevel, long remainingTime)
    {
        _objectId = objectId;
        _itemId = itemId;
        _location = location;
        _slot = slot;
        _count = count;
        _type1 = type1;
        _type2 = type2;
        _equipped = equipped;
        _bodyPart = bodyPart;
        _remainingTime = remainingTime;
        _enchantLevel = enchantLevel;

        if (_type2 == ItemType2.TYPE2_WEAPON)
        {
            _itemData = ItemTable.Instance.GetWeapon(_itemId);
        }
        else if (_type2 == ItemType2.TYPE2_SHIELD_ARMOR || _type2 == ItemType2.TYPE2_ACCESSORY)
        {
            if (bodyPart == ItemSlot.SLOT_LR_HAND)
            {
                _itemData = ItemTable.Instance.GetWeapon(_itemId);
            }
            else
            {
                _itemData = ItemTable.Instance.GetArmor(_itemId);
            }
        }
        else
        {
            _itemData = ItemTable.Instance.GetEtcItem(_itemId);
        }

        //Debug.Log(this.ToString());
    }

    public void Update(ItemInstance newItem)
    {
        _location = newItem.Location;
        _slot = newItem.Slot;
        _count = newItem.Count;
        _remainingTime = newItem.RemainingTime;
        _enchantLevel = newItem.EnchantLevel;

        if (_equipped == false && newItem.Equipped == true
        || _equipped == true && newItem.Equipped == false)
        {
            AudioManager.Instance.PlayEquipSound(_itemData.Itemgrp.EquipSound);
        }

        _equipped = newItem.Equipped;
        _objectId = newItem.ObjectId;
        _bodyPart = newItem.BodyPart;
    }

    public override string ToString()
    {
        return $"New item: ServerId:{_objectId} ItemId:{_itemId} Location:{_location} Slot:{_slot} Count:{_count} " +
        $"Type1:{_type1} Type2:{_type2} Equipped:{_equipped} Bodypart:{_bodyPart} Change:{_lastChange}";
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
