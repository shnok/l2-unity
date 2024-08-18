public class RequestInventoryOpenPacket : ClientPacket
{
    public RequestInventoryOpenPacket() : base((byte)GameClientPacketType.RequestInventoryOpen) {
        BuildPacket();
    }
}
