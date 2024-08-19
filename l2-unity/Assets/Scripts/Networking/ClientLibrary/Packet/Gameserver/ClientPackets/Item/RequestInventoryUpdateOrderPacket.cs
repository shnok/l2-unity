using System.Collections.Generic;

public class RequestInventoryUpdateOrderPacket : ClientPacket {
    public RequestInventoryUpdateOrderPacket(int objectId, int slot) : base((byte)GameClientPacketType.RequestInventoryUpdateOrder) {
        WriteI(1);
        WriteI(objectId);
        WriteI(slot);

        BuildPacket();
    }

    public RequestInventoryUpdateOrderPacket(List<InventoryOrder> orders) : base((byte)GameClientPacketType.RequestInventoryUpdateOrder) {
        WriteI(orders.Count);
        orders.ForEach((order) => {
            WriteI(order.ObjectId);
            WriteI(order.Slot);
        });

        BuildPacket();
    }
}
