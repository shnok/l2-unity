public class PingPacket : ClientPacket {
    public PingPacket() : base((byte)GameClientPacketType.Ping) {
        SetData(new byte[] { (byte)GameClientPacketType.Ping, 0x02});
        BuildPacket();
    }
}