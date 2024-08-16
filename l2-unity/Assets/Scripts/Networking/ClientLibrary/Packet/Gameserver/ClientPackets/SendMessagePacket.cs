public class SendMessagePacket : ClientPacket {
    public SendMessagePacket(string text) : base((byte)GameClientPacketType.SendMessage) {
        WriteS(text);
        BuildPacket();
    }
}