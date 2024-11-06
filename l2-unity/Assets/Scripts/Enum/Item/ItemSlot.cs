public enum ItemSlot : int
{
    SLOT_NONE = 0,
    SLOT_UNDERWEAR = 0x0001,
    SLOT_R_EAR = 0x0002,
    SLOT_L_EAR = 0x0004,
    SLOT_NECK = 0x0008,
    SLOT_R_FINGER = 0x0010,
    SLOT_L_FINGER = 0x0020,
    SLOT_HEAD = 0x0040,
    SLOT_R_HAND = 0x0080,
    SLOT_LR_HAND = 0x0100,
    SLOT_GLOVES = 0x0200,
    SLOT_CHEST = 0x0400,
    SLOT_LEGS = 0x0800,
    SLOT_FEET = 0x1000,
    SLOT_L_HAND = 0x4000,
    SLOT_FULL_ARMOR = 0x8000,


    // not handled yet
    SLOT_BACK = 0x1000,
    SLOT_ALLWEAPON = 0x4080,
    SLOT_FACE = 0x010000,
    SLOT_ALLDRESS = 0x020000,
    SLOT_HAIR = 0x040000,
    SLOT_HAIRALL = 0x080000,
    SLOT_WOLF = 0x080000,
    SLOT_HATCHLING = 0x020000,
    SLOT_STRIDER = 0x020000,
    SLOT_BABYPET = 0x020000,
}

public class ItemSlotParser
{
    public static ItemSlot ParseBodyPart(string bodyPart)
    {
        switch (bodyPart)
        {
            case "artifact_a1":
                return ItemSlot.SLOT_CHEST;
            case "artifact_a2":
                return ItemSlot.SLOT_LEGS;
            case "artifact_a3":
                return ItemSlot.SLOT_FEET;
            case "head":
                return ItemSlot.SLOT_HEAD;
            case "artifactbook":
                return ItemSlot.SLOT_GLOVES;
            case "rfinger":
                return ItemSlot.SLOT_R_FINGER;
            case "lfinger":
                return ItemSlot.SLOT_L_FINGER;
            case "rear":
                return ItemSlot.SLOT_R_EAR;
            case "lear":
                return ItemSlot.SLOT_L_EAR;
            case "neck":
                return ItemSlot.SLOT_NECK;
            case "onepiece":
                return ItemSlot.SLOT_FULL_ARMOR;
            case "artifact_a7":
                return ItemSlot.SLOT_R_HAND;
            case "lrhand":
                return ItemSlot.SLOT_LR_HAND;
            default:
                return ItemSlot.SLOT_FEET;
        }
    }
}