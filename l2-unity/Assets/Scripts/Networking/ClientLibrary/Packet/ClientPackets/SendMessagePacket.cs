using System;
using System.Text;

public class SendMessagePacket : ClientPacket {
    private string _text;

    public SendMessagePacket(string text) : base((byte)ClientPacketType.SendMessage) {
        SetText(text);
        WriteS(text);
        BuildPacket();
    }

    public void SetText(string text) {
        _text = text;
    }
}