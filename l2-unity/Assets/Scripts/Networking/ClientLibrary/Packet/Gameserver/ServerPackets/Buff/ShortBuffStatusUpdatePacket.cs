using UnityEngine;

public class ShortBuffStatusUpdatePacket : ServerPacket
{
    public int SkillId { get; private set; }
    public int SkillLvl { get; private set; }
    public int Duration { get; private set; }

    public ShortBuffStatusUpdatePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        SkillId = ReadI();
        SkillLvl = ReadI();
        Duration = ReadI();
        Debug.LogError($"Received ShortBuffStatusUpdatePacket id: {SkillId}, duration: {Duration}");
    }
}