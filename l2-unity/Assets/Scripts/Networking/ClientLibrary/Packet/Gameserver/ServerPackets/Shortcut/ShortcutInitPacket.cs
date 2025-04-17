using System.Collections.Generic;
using UnityEngine;

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


            Shortcut shortcut = new Shortcut(slot % 12, slot / 12, type, id, level);
            Shortcuts.Add(shortcut);
        }

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("ShortcutInitPacket:");
        sb.AppendLine($"  Shortcut Count: {Shortcuts.Count}");

        for (int i = 0; i < Shortcuts.Count; i++)
        {
            var sc = Shortcuts[i];
            sb.AppendLine($"  [{i}] Slot: ({sc.Slot} Type: {sc.Type}, Id: {sc.Id}, Level: {sc.Level}");
        }

        return sb.ToString();
    }
}