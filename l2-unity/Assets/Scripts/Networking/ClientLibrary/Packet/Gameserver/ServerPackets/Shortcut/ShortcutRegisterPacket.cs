public class ShortcutRegisterPacket : ServerPacket
{
    public ShortcutRegisterPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {

    }
}