using System.Numerics;

public class RequestActionPacket : ClientPacket
{
    public RequestActionPacket(int objectId) : base((byte)GameClientPacketType.RequestAction)
    {
        WriteI(objectId);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteB(0);
        BuildPacket();
    }
}
