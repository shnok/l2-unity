using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


//packet.writeD(item.getObjectId()); // ObjectId
//packet.writeD(item.getItem().getDisplayId()); // ItemId
//packet.writeD(item.getLocation()); // T1
//packet.writeQ(item.getCount()); // Quantity
//packet.writeH(item.getItem().getType2()); // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
//packet.writeH(item.getCustomType1()); // Filler (always 0)
//packet.writeH(item.getEquipped()); // Equipped : 00-No, 01-yes
//packet.writeD(item.getItem().getBodyPart()); // Slot : 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
//packet.writeH(item.getEnchant()); // Enchant level (pet level shown in control item)
//packet.writeH(item.getCustomType2()); // Pet name exists or not shown in control item
//packet.writeD(item.getAugmentationBonus());
//packet.writeD(item.getMana());
//packet.writeD(item.getTime());
//writeItemElementalAndEnchant(packet, item);

//Test Example 1
//Tauti LEATHER Helmet
//ObjectId 268465500
//ItemId 45573
//T1 34
//Quantity 1
//Type_2 1
//Filler 0
//Equipped 0
//Slot 64
//Enchant 0
//Pet name 0
//Augmentation 0
//Mana -1
//Time -9999
//location - number cell


//Test Example 2  Soulshot: No Grade for Beginners
//ObjectId 268473060
//ItemId 5789
//T1 5
//Quantity 4
//Type_2 4
//Filler 0
//Equipped 0
//Slot 0
//Enchant 0
//Pet name 0
//Augmentation 0
//Mana -1
//Time -9999
//location - number cell


//Dagger equip weapon 
//ObjectId 268463170
//ItemId 5615
//T1 5
//Quantity 0
//Type_2 0
//Filler 0
//Equipped 1
//Slot 128
//Enchant 0
//Pet name 0
//Augmentation 0
//Mana -1
//Time -9999
//location - number cell

public class DemoL2JItem
{
    public int ObjectId { get; set; }
    public int ItemId { get; set; }
    public int T1 { get; set; }
    public int Quantity { get; set; }
    public int Type2  { get; set; }
    public int Filler { get; set; }
    public int Equipped { get; set; }

    public int Location { get; set; }
    public int Slot { get; set; }

    public int Enchant { get; set; }
    public int CustomType2 { get; set; }

    public int AugmentationBonus { get; set; }

    public int Mana { get; set; }

    public int Time { get; set; }
}

