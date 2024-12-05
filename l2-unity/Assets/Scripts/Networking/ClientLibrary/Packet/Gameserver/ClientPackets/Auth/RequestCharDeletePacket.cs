public class RequestCharDeletePacket : ClientPacket
{
    public RequestCharDeletePacket(int slot) : base((byte)GameClientPacketType.RequestCharDelete)
    {
        WriteI(slot);
        BuildPacket();
    }
}
