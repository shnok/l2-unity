public class UseItemPacket : ClientPacket {
    public UseItemPacket(int itemObjectId) : base((byte)GameClientPacketType.UseItem) {
        WriteI(itemObjectId);
        BuildPacket();
    }
}
