public class AcquireSkillListPacket : ServerPacket
{
    public SkillWindowInfo[] Skills { get; private set; }
    public AcquireSkillListPacket(byte[] d) : base(d)
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
    }
}
