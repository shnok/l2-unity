public class RequestCancelPacket : ClientPacket
{
    public RequestCancelPacket(bool cancelCast) : base((byte)GameClientPacketType.RequestCancel)
    {
        WriteB(0);
        WriteB(cancelCast ? (byte)0 : (byte)1);
        BuildPacket();
    }
}
