using System.Collections.Generic;

public class RequestBuyItemPacket : ClientPacket
{
    public RequestBuyItemPacket(int listId, List<Product> products) : base((byte)GameClientPacketType.RequestBuyItem)
    {
        WriteI(listId);
        WriteI(products.Count);

        products.ForEach((p) =>
        {
            WriteI(p.ItemId);
            WriteI(p.Count);
        });
        BuildPacket();
    }
}