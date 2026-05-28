using System.Collections.Generic;

public class RequestSellItemPacket : ClientPacket
{
    public RequestSellItemPacket(int listId, List<Product> products) : base((byte)GameClientPacketType.RequestSellItem)
    {
        WriteI(listId); // npc id + 1000000
        WriteI(products.Count);

        products.ForEach((p) =>
        {
            WriteI(p.ObjectId);
            WriteI(p.ItemId);
            WriteI(p.Count);
        });
        BuildPacket();
    }
}