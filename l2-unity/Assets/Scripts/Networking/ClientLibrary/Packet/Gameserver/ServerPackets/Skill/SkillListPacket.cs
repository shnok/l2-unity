// Default Player skill list on player log in
public class SkillListPacket : ServerPacket
{
    public SkillInfo[] Skills { get; private set; }

    public SkillListPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int skillSize = ReadI();
        Skills = new SkillInfo[skillSize];

        for (var i = 0; i < skillSize; i++)
        {
            bool isPassive = ReadI() == 1;
            int level = ReadI();
            int id = ReadI();
            bool isDisabled = ReadB() == 1;
            Skills[i] = new SkillInfo(id, level, isPassive, isDisabled);
        }
    }
}
