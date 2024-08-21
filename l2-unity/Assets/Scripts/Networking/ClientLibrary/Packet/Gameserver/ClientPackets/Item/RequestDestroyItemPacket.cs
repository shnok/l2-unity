public class RequestDestroyItemPacket : ClientPacket
{
    public RequestDestroyItemPacket(int objectId, int count) : base((byte)GameClientPacketType.RequestDestroyItem)
    {
        WriteI(objectId);
        WriteI(count);
        BuildPacket();
    }
}