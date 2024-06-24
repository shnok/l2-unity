public class RequestSetTargetPacket : ClientPacket {
    public RequestSetTargetPacket(int targetId) : base((byte)GameClientPacketType.RequestSetTarget) {
        WriteI(targetId);
        BuildPacket();
    }
}
