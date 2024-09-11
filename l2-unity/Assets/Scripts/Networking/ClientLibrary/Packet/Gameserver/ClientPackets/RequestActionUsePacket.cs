public class RequestActionUsePacket : ClientPacket
{

    public RequestActionUsePacket(int action) : base((byte)GameClientPacketType.RequestActionUse)
    {
        WriteI(action);
        BuildPacket();
    }
}
