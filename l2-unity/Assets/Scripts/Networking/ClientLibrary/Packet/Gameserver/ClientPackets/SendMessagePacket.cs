using UnityEditor;
using UnityEngine;

public class SendMessagePacket : ClientPacket
{
    public SendMessagePacket(string text, L2MessageType messageType, string pmTarget) : base((byte)GameClientPacketType.SendMessage)
    {
        WriteS(text.Substring(0, Mathf.Min(100, text.Length)));
        WriteI((int)messageType);

        if (messageType == L2MessageType.TELL)
        {
            WriteS(pmTarget == null ? "" : pmTarget);
        }

        BuildPacket();
    }
}