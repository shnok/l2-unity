public class RequestAutoAttackPacket : ClientPacket
{
    public RequestAutoAttackPacket(int objectId) : base((byte)GameClientPacketType.RequestAutoAttack)
    {
        WriteI(objectId);
        BuildPacket();
    }
}
