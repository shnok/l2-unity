
using UnityEngine;

public class MagicSkillCanceledPacket : ServerPacket
{
    public int ObjectId { get; private set; }

    public MagicSkillCanceledPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();

        Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        return $"MagicSkillCanceledPacket:\n" +
               $"  ObjectId: {ObjectId}";
    }
}
