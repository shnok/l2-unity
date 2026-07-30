public class RequestAttackPacket : ClientPacket
{
    public RequestAttackPacket(int objectId) : base((byte)GameClientPacketType.RequestAttack)
    {
        WriteI(objectId);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteB(0);
        BuildPacket();
    }
}

