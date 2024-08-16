public class InventoryOrder {
    public int ObjectId {get;}
    public int Slot {get;}

    public InventoryOrder(int objectId, int slot) {
        ObjectId = objectId;
        Slot = slot;
    }
}