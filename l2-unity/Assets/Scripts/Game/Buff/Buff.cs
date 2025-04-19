public class Buff
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public int Level { get; set; }
    public int Duration { get; set; }
    public BuffType Type { get; set; }
    public float StartTime { get; set; }

    public Buff(string name, string description, string icon, int level, int duration, BuffType type, float startTime)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Level = level;
        Duration = duration;
        Type = type;
        StartTime = startTime;
    }

    public static BuffType ResolveBuffType(int skillId, int skillLvl)
    {
        Skillgrp skill = SkillgrpTable.Instance.GetSkill(skillId, skillLvl);
        return ResolveBuffType(skill);
    }

    public static BuffType ResolveBuffType(Skillgrp skill)
    {
        return skill.IconType switch
        {
            SkillType.Special or SkillType.AbilityPassive => BuffType.Special,
            SkillType.CraftAndItems or SkillType.Buff when skill.OperateType is SkillOperateType.Special => BuffType.Special,
            // SkillIconType.Buff when skill.IsMagic == IsMagicType.SongOrDances => BuffType.DanceSong, // this is for later than interlude
            SkillType.Buff or SkillType.CraftAndItems or SkillType.Physical => BuffType.Normal,
            SkillType.Toggle => BuffType.Toggle,
            SkillType.Debuff => BuffType.Debuff,
            _ => BuffType.Normal
        };
    }
}

