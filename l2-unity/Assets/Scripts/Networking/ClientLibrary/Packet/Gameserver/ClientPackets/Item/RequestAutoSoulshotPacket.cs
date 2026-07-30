public class RequestAutoSoulshotPacket : ClientPacket
{
    public RequestAutoSoulshotPacket(int itemId, bool activate) : base((byte)GameClientPacketType.DoubleOPCode)
    {
        WriteB((byte)GameClientPacketDoubleType.RequestAutoSoulshot);
        WriteB(0);
        WriteI(itemId);
        WriteI(activate ? 1 : 0);
        BuildPacket();
    }
}