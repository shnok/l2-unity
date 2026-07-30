// Default Player skill list on player log in
using UnityEngine;

public class SkillCoolTimePacket : ServerPacket
{
    public struct SkillCoolTimeInfo
    {
        public int SKillId { get; set; }
        public int SkillLevel { get; set; }
        public int SkillReuseTimeSec { get; set; }
        public int SkillCooldownRemainingTimeSec { get; set; }
    }

    public SkillCoolTimeInfo[] Cooltimes { get; private set; }

    public SkillCoolTimePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int skillSize = ReadI();
        Cooltimes = new SkillCoolTimeInfo[skillSize];

        for (var i = 0; i < skillSize; i++)
        {
            Cooltimes[i] = new SkillCoolTimeInfo
            {
                SKillId = ReadI(),
                SkillLevel = ReadI(),
                SkillReuseTimeSec = ReadI(),
                SkillCooldownRemainingTimeSec = ReadI()
            };
        }

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("SkillCoolTimePacket:");
        sb.AppendLine($"  Cooltime Skills Count: {Cooltimes?.Length ?? 0}");

        if (Cooltimes != null)
        {
            for (int i = 0; i < Cooltimes.Length; i++)
            {
                var info = Cooltimes[i];
                sb.AppendLine($"  [{i}] SkillId: {info.SKillId}, Level: {info.SkillLevel}, " +
                              $"ReuseTimeSec: {info.SkillReuseTimeSec}, RemainingTimeSec: {info.SkillCooldownRemainingTimeSec}");
            }
        }

        return sb.ToString();
    }
}
