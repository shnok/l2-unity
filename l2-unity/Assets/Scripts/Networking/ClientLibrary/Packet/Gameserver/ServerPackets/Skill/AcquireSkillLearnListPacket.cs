using UnityEngine;

public class AcquireSkillLearnListPacket : ServerPacket
{
    public SkillWindowInfo[] Skills { get; private set; }
    public AcquireSkillLearnListPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        PacketSkillType type = (PacketSkillType)ReadI();
        int skillsSize = ReadI();
        Skills = new SkillWindowInfo[skillsSize];
        for (int i = 0; i < skillsSize; i++)
        {
            int skillId = ReadI();
            int lvl = ReadI();
            int minLvl = ReadI();
            int cost = ReadI();
            ReadI();
            Skills[i] = new SkillWindowInfo(skillId, lvl, cost, null);
        }

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("AcquireSkillListPacket:");
        sb.AppendLine($"  Skills Count: {Skills?.Length ?? 0}");

        if (Skills != null)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                var skill = Skills[i];
                sb.AppendLine($"  [{i}] SkillId: {skill.SkillId}, Level: {skill.Level}, Cost: {skill.MpCost}");
            }
        }

        return sb.ToString();
    }
}
