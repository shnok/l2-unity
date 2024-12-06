public class RequestCharRestorePacket : ClientPacket
{
    public RequestCharRestorePacket(int slot) : base((byte)GameClientPacketType.RequestCharRestore)
    {
        WriteI(slot);
        BuildPacket();
    }
}
