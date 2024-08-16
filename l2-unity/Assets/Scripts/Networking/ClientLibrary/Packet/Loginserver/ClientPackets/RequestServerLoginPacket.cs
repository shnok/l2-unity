public class RequestServerLoginPacket : ClientPacket {
    public RequestServerLoginPacket(int serverId, int sessionKey1, int sessionKey2) : base((byte)LoginClientPacketType.RequestServerLogin) {

        WriteI(sessionKey1);
        WriteI(sessionKey2);
        WriteI(serverId);

        BuildPacket();
    }
}