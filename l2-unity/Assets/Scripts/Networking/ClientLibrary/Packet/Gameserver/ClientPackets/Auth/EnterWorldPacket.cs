public class EnterWorldPacket : ClientPacket
{
    public EnterWorldPacket() : base((byte)GameClientPacketType.EnterWorld)
    {
        BuildPacket();
    }
}