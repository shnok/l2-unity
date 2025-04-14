public class BuffEffect
{
    public int SkillId { get; }
    public int SkillLvl { get; }
    public int Duration { get; }

    public BuffEffect(int skillId, int skillLvl, int duration)
    {
        SkillId = skillId;
        SkillLvl = skillLvl;
        Duration = duration;
    }
}