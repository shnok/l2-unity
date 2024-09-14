public class ShortcutRegisterPacket : ServerPacket
{
    public Shortcut NewShortcut { get; private set; }

    public ShortcutRegisterPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int type = ReadI();
        int slot = ReadI();
        int id = ReadI();
        int level = -1;

        if (type == Shortcut.TYPE_SKILL)
        {
            level = ReadI();
        }

        NewShortcut = new Shortcut(slot % 12, slot / 12, type, id, level);
    }
}