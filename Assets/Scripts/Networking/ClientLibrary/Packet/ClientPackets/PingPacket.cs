public class PingPacket : ClientPacket {
    public PingPacket() : base((byte)ClientPacketType.Ping) {
        SetData(new byte[] { (byte)ClientPacketType.Ping, 0x02});
    }
}