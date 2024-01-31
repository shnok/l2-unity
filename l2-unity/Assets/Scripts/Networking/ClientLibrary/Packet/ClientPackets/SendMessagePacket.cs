using System;
using System.Text;

public class SendMessagePacket : ClientPacket {
    public SendMessagePacket(string text) : base((byte)ClientPacketType.SendMessage) {
        WriteS(text);
        BuildPacket();
    }
}