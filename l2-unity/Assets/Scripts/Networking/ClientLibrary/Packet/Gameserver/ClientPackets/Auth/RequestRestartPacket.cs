public class RequestRestartPacket : ClientPacket
{
    public RequestRestartPacket() : base((byte)GameClientPacketType.RequestRestart)
    {
        WriteB(0);
        BuildPacket();
    }
}
