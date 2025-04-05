public class AcquireSkillLearnInfoPacket : ServerPacket
{
    public SkillRequirement[] Requirements;
    public AcquireSkillLearnInfoPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        int skillId = ReadI();
        int lvl = ReadI();
        int spCost = ReadI();
        int mode = ReadI();

        int skillRequirementsSize = ReadI();
        Requirements = new SkillRequirement[skillRequirementsSize];

        for (int i = 0; i < skillRequirementsSize; i++)
        {
            SkillRequirementType type = (SkillRequirementType)ReadI();
            int itemId = ReadI();
            int count = ReadI();
            int unk = ReadI();
            ReadI();
            Requirements[i] = new SkillRequirement(type, itemId, count, unk);
        }
    }
}
