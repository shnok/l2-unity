public class RequestSetTargetPacket : ClientPacket
{
    public RequestSetTargetPacket(int targetId, bool shiftPressed) : base((byte)GameClientPacketType.RequestSetTarget)
    {
        WriteI(targetId);
        WriteB(shiftPressed ? (byte)1 : (byte)0);
        BuildPacket();
    }
}
