public class SkillRequirement
{
    public SkillRequirementType Type;
    public int ItemId;
    public int Count;
    public int Unk;

    public SkillRequirement(SkillRequirementType type, int itemId, int count, int unk)
    {
        Type = type;
        ItemId = itemId;
        Count = count;
        Unk = unk;
    }
}

public enum SkillRequirementType
{
    Clan = 1,
    Fishing = 4,
    General = 99,
}