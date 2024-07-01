using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEquipId 
{
    public static int TYPE1_WEAPON_RING_EARRING_NECKLACE = 0;
    public static int TYPE1_SHIELD_ARMOR = 1;
    public static int TYPE1_ITEM_QUESTITEM_ADENA = 4;

    public static int TYPE2_WEAPON = 0;
    public static int TYPE2_SHIELD_ARMOR = 1;
    public static int TYPE2_ACCESSORY = 2;
    public static int TYPE2_QUEST = 3;
    public static int TYPE2_MONEY = 4;
    public static int TYPE2_OTHER = 5;

    public static int SLOT_NONE = 0x0000;
    public static int SLOT_UNDERWEAR = 0x0001;
    public static int SLOT_R_EAR = 0x0002;
    public static int SLOT_L_EAR = 0x0004;
    public static int SLOT_LR_EAR = 0x00006;
    public static int SLOT_NECK = 0x0008;
    public static int SLOT_R_FINGER = 0x0010;
    public static int SLOT_L_FINGER = 0x0020;
    public static int SLOT_LR_FINGER = 0x0030;
    public static int SLOT_HEAD = 0x0040;
    public static int SLOT_R_HAND = 0x0080;
    public static int SLOT_L_HAND = 0x0100;
    public static int SLOT_GLOVES = 0x0200;
    public static int SLOT_CHEST = 0x0400;
    public static int SLOT_LEGS = 0x0800;
    public static int SLOT_FEET = 0x1000;
    public static int SLOT_BACK = 0x2000;
    public static int SLOT_LR_HAND = 0x4000;
    public static int SLOT_FULL_ARMOR = 0x8000;
    public static int SLOT_HAIR = 0x010000;
    public static int SLOT_ALLDRESS = 0x020000;
    public static int SLOT_HAIR2 = 0x040000;
    public static int SLOT_HAIRALL = 0x080000;
    public static int SLOT_R_BRACELET = 0x100000;
    public static int SLOT_L_BRACELET = 0x200000;
    public static int SLOT_DECO = 0x400000;
    public static int SLOT_BELT = 0x10000000;
    public static int SLOT_WOLF = -100;
    public static int SLOT_HATCHLING = -101;
    public static int SLOT_STRIDER = -102;
    public static int SLOT_BABYPET = -103;
    public static int SLOT_GREATWOLF = -104;

    public static int SLOT_MULTI_ALLWEAPON = SLOT_LR_HAND | SLOT_R_HAND;

}
