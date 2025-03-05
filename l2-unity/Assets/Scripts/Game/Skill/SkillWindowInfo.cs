using System.Collections.Generic;

public class SkillWindowInfo
{
    public int SkillId { get; private set; }
    public int Level { get; }
    public string Name { get; }
    public string Desc { get; }
    public string Icon { get; }
    public string IconPanel { get; }
    public int MpCost { get; }
    public int SpCost { get; }
    public int Range { get; }
    public bool IsMagic { get; }
    public SkillType Type { get; }
    public SkillRequirement[] SkillRequirement { get; }

    public SkillWindowInfo(int skillId, int lvl, int spCost, SkillRequirement[] skillRequirements)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, lvl);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, lvl);
        SkillId = skillId;
        Level = skillNameData.Level;
        Name = skillNameData.Name;
        Desc = skillNameData.Desc;
        Icon = skillgrp.Icon;
        IconPanel = skillgrp.IconPanel;
        MpCost = skillgrp.MpConsume;
        SpCost = spCost;
        Range = skillgrp.CastRange;
        IsMagic = skillgrp.IsMagic != IsMagicType.None;
        Type = skillgrp.IconType;
        SkillRequirement = skillRequirements;
    }
    
    public SkillWindowInfo(int skillId, int lvl) 
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, lvl);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, lvl);
        SkillId = skillId;
        Level = skillNameData.Level;
        Name = skillNameData.Name;
        Desc = skillNameData.Desc;
        Icon = skillgrp.Icon;
        IconPanel = skillgrp.IconPanel;
        MpCost = skillgrp.MpConsume;
        Range = skillgrp.CastRange;
        IsMagic = skillgrp.IsMagic != IsMagicType.None;
        Type = skillgrp.IconType;
    }
}