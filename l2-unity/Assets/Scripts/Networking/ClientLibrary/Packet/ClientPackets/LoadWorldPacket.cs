public class LoadWorldPacket : ClientPacket {
    public LoadWorldPacket() : base((byte)ClientPacketType.LoadWorld) {
       SetData(new byte[] { (byte)ClientPacketType.LoadWorld, 0x02});
    }
}