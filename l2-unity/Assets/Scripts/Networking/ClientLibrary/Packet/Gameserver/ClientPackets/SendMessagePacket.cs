using UnityEditor;
using UnityEngine;

public class SendMessagePacket : ClientPacket
{
    public SendMessagePacket(string text, MessageType messageType, string pmTarget) : base((byte)GameClientPacketType.SendMessage)
    {
        WriteS(text.Substring(0, Mathf.Min(100, text.Length)));
        WriteI((int)messageType);

        if (messageType == MessageType.TELL)
        {
            WriteS(pmTarget == null ? "" : pmTarget);
        }

        BuildPacket();
    }
}