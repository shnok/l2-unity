public class RequestShortcutDelPacket : ClientPacket
{
    public RequestShortcutDelPacket(int slot) : base((byte)GameClientPacketType.RequestShortcutDel)
    {
        WriteI(slot);

        BuildPacket();
    }
}
