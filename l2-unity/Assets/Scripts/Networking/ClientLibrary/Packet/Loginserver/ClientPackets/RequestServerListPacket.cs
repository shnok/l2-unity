public class RequestServerListPacket : ClientPacket {
    public RequestServerListPacket(int sessionKey1, int sessionKey2) : base((byte)LoginClientPacketType.RequestServerList) {

        WriteI(sessionKey1);
        WriteI(sessionKey2);

        BuildPacket();
    }
}