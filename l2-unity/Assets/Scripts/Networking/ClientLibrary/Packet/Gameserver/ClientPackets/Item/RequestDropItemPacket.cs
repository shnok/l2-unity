public class RequestDropItemPacket : ClientPacket
{
    public RequestDropItemPacket(int objectId, int count) : base((byte)GameClientPacketType.RequestDropItem)
    {
        WriteI(objectId);
        WriteI(count);
        BuildPacket();
    }
}