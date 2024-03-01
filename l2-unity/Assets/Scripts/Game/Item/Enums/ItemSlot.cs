public enum ItemSlot : byte {
    none = 0,
    head = 1,
    chest = 2,
    legs = 3,
    fullarmor = 4,
    gloves = 5,
    feet = 6,
    lhand = 7,
    rhand = 8,
    lrhand = 9,
    rfinger = 10,
    lfinger = 11,
    lear = 12,
    rear = 13,
    neck = 14,
    underwear = 15,
    ring = 16,
    earring = 17
}

public class ItemSlotParser {
    public static ItemSlot ParseBodyPart(string bodyPart) {
        switch (bodyPart) {
            case "artifact_a1":
                return ItemSlot.chest;
            case "artifact_a2":
                return ItemSlot.legs;
            case "artifact_a3":
                return ItemSlot.feet;
            case "head":
                return ItemSlot.head;
            case "artifactbook":
                return ItemSlot.gloves;
            case "rfinger":
                return ItemSlot.rfinger;
            case "lfinger":
                return ItemSlot.lfinger;
            case "rear":
                return ItemSlot.rear;
            case "lear":
                return ItemSlot.lear;
            case "neck":
                return ItemSlot.neck;
            case "onepiece":
                return ItemSlot.fullarmor;
            case "artifact_a7":
                return ItemSlot.rhand;
            case "lrhand":
                return ItemSlot.lrhand;
            default:
                return ItemSlot.feet;
        }
    }
}