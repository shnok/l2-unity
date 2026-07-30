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

        switch (type)
        {
            case Shortcut.TYPE_ITEM:
                ReadI(); // CharacterType
                ReadI(); // SharedReuseGroup

                ReadI(); // Remaining
                ReadI(); // Reusedelay
                ReadI(); //Augment Id
                break;
            case Shortcut.TYPE_SKILL:
                level = ReadI();
                ReadB();
                ReadI(); // Character type
                break;
            default:
                ReadI(); // Character type
                break;
        }

        NewShortcut = new Shortcut(slot % 12, slot / 12, type, id, level);
    }
}