public class ShortcutInitPacket : ServerPacket
{
    public ShortcutInitPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {

    }
}