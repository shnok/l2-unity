public class ProtocolVersionPacket : ClientPacket
{
    public ProtocolVersionPacket(int version) : base((byte)GameClientPacketType.ProtocolVersion) {
        WriteI(version);

        BuildPacket();
    }
}
