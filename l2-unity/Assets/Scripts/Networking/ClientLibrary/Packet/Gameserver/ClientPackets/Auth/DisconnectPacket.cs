public class DisconnectPacket : ClientPacket
{
    public DisconnectPacket() : base((byte)GameClientPacketType.Disconnect)
    {
        WriteB(0);
        BuildPacket();
    }
}