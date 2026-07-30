public class ShortcutDeletePacket : ServerPacket
{
    public int Slot { get; private set; }

    public ShortcutDeletePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Slot = ReadI();
        ReadI();
    }
}