public class LoadWorldPacket : ClientPacket {
    public LoadWorldPacket() : base((byte)GameClientPacketType.LoadWorld) {
       SetData(new byte[] { (byte)GameClientPacketType.LoadWorld, 0x02});
       BuildPacket();
    }
}