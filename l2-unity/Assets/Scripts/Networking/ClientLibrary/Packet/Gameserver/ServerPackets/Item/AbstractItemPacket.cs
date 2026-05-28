
using UnityEngine;

public abstract class AbstractItemPacket : ServerPacket
{
    protected AbstractItemPacket(byte[] d) : base(d)
    {
    }

    public ItemType1 Type1 { get; private set; }
    public int ObjectId { get; private set; }
    public int ItemId { get; private set; }
    public int Count { get; private set; }
    public int CustomType1 { get; private set; }
    public int CustomType2 { get; private set; }
    public int AugmentId { get; private set; }
    public ItemType2 Type2 { get; private set; }
    public bool Equipped { get; private set; }
    public ItemSlot BodyPart { get; private set; }
    public int EnchantLevel { get; private set; }
    public long RemainingTime { get; private set; }
    public ItemLocation ItemLocation { get; private set; }
    public int Slot { get; private set; }

    protected ItemInstance ReadItem()
    {
        Type1 = (ItemType1)ReadH();
        ObjectId = ReadI();
        ItemId = ReadI();
        Count = ReadI();
        Type2 = (ItemType2)ReadH();
        CustomType1 = ReadH(); //NOT USED
        Equipped = ReadH() == 1;
        BodyPart = (ItemSlot)ReadI();
        EnchantLevel = ReadH();
        CustomType2 = ReadH(); //NOT USED
        AugmentId = ReadI(); //NOT USED
        RemainingTime = ReadI();

        ItemLocation = ItemLocation.Inventory;
        if (Equipped)
        {
            ItemLocation = ItemLocation.Equipped;
        }

        // int slot = ReadI();
        // In interlude slot is not shared?
        Slot = ReadI();

        // Debug.LogWarning(ToString());

        return new ItemInstance(
            ObjectId, ItemId, ItemLocation, Slot, Count, Type1, Type2,
            Equipped, BodyPart, EnchantLevel, RemainingTime);
    }

    public override string ToString()
    {
        return $"Item Information:\n" +
               $"  Type1: {Type1}\n" +
               $"  Object ID: {ObjectId}\n" +
               $"  Item ID: {ItemId}\n" +
               $"  Slot: {Slot}\n" +
               $"  Count: {Count}\n" +
               $"  Type2: {Type2}\n" +
               $"  CustomType2: {CustomType1}\n" +
               $"  Equipped: {Equipped}\n" +
               $"  Body Part: {BodyPart}\n" +
               $"  Enchant Level: {EnchantLevel}\n" +
               $"  CustomType2: {CustomType2}\n" +
               $"  Augment Id: {AugmentId}\n" +
               $"  Remaining Time: {RemainingTime}\n" +
               $"  Item Location: {ItemLocation}\n";
    }
}
