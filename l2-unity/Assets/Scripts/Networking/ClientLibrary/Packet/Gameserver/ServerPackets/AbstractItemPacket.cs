using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractItemPacket : ServerPacket
{
    protected AbstractItemPacket(byte[] d) : base(d) {
    }
    

    /*
        protected void writeItem(ItemInfo item) {
        writeI(item.getObjectId()); // ObjectId
        writeI(item.getItem().getId()); // ItemId
        writeB((byte) item.getLocation()); // Inventory? Warehouse?
        writeI(item.getSlot()); // Slot
        writeL(item.getCount()); // Quantity
        writeB(item.getCategory()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
        writeB(item.getEquipped()); // Equipped : 00-No, 01-yes
        writeI(item.getItem().getBodyPart().getValue());
        writeI(item.getEnchant()); // Enchant level
        writeL(item.getTime());
    }
    */
    protected ItemInstance ReadItem() {
        int objectId = ReadI();
        int itemId = ReadI();
        ItemLocation itemLocation = (ItemLocation) ReadB();
        int slot = ReadI();
        int count = ReadI();
        ItemCategory itemCategory = (ItemCategory) ReadB();
        bool equipped = ReadB() == 1;
        ItemSlot bodyPart = (ItemSlot) ReadI();
        int enchantLevel = ReadI();
        long remainingTime = ReadL();

        return new ItemInstance(
            objectId, itemId, itemLocation, slot, count, itemCategory, 
            equipped, bodyPart, enchantLevel, remainingTime);
    }
}
