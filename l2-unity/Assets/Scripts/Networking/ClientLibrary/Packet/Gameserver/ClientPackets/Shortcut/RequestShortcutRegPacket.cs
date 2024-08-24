public class RequestShortcutRegPacket : ClientPacket
{
    public RequestShortcutRegPacket(int type, int id, int slot) : base((byte)GameClientPacketType.RequestShortcutReg)
    {
        WriteI(type);
        WriteI(slot);
        WriteI(id);
        WriteI(1);

        BuildPacket();
    }
}