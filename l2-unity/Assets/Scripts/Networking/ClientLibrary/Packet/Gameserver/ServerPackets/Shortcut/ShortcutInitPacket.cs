using System.Collections.Generic;

public class ShortcutInitPacket : ServerPacket
{
    public List<Shortcut> Shortcuts { get; private set; }

    public ShortcutInitPacket(byte[] d) : base(d)
    {
        Shortcuts = new List<Shortcut>();
        Parse();
    }

    public override void Parse()
    {
        int count = ReadI();

        for (int i = 0; i < count; i++)
        {
            int type = ReadI();
            int slot = ReadI();
            int id = ReadI();
            int level = -1;

            if (type == Shortcut.TYPE_SKILL)
            {
                level = ReadI();
            }

            Shortcut shortcut = new Shortcut(slot % 12, slot / 12, type, id, level);
            Shortcuts.Add(shortcut);
        }
    }
}