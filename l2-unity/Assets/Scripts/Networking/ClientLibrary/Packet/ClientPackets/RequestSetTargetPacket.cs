public class RequestSetTargetPacket : ClientPacket {
    public RequestSetTargetPacket(int targetId) : base((byte)ClientPacketType.RequestSetTarget) {
        WriteI(targetId);
        BuildPacket();
    }
}
