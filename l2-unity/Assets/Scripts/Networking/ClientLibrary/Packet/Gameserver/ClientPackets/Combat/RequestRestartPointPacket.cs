public class RequestRestartPointPacket : ClientPacket
{

    public RequestRestartPointPacket(int restartPoint) : base((byte)GameClientPacketType.RequestRestartPoint)
    {
        WriteI(restartPoint);
        BuildPacket();
    }
}
