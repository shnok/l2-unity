using UnityEngine;

public class SkillWindowInfo
{
    public int SkillId { get; private set; }
    public int Level { get; }
    public string Name { get; }
    public string Desc { get; private set; }
    public string Icon { get; }
    public string IconPanel { get; }
    public int MpCost { get; }
    public int SpCost { get; }
    public int Range { get; }
    public SkillMagicType IsMagic { get; }
    public SkillType Type { get; }
    public float HitTime { get; }
    public float ReuseDelay { get; }
    public SkillRequirement[] SkillRequirement { get; set; }
    public string[] SkillDescParams { get; set; }

    public SkillWindowInfo(int skillId, int lvl, int spCost, SkillRequirement[] skillRequirements)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, lvl);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, lvl);
        SkillId = skillId;
        Level = skillNameData.Level;
        Name = skillNameData.Name;
        // Desc = string.IsNullOrEmpty(skillNameData.Desc) ? SkillNameTable.Instance.GetDescription(skillId) : skillNameData.Desc;
        Desc = skillNameData.Desc;
        Icon = skillgrp.Icon;
        IconPanel = skillgrp.IconPanel;
        MpCost = skillgrp.MpConsume;
        SpCost = spCost;
        Range = skillgrp.CastRange;
        IsMagic = skillgrp.IsMagic;
        Type = skillgrp.IconType;
        HitTime = skillgrp.HitTime;
        ReuseDelay = skillgrp.ReuseDelay;
        SkillRequirement = skillRequirements;
        SkillDescParams = skillNameData.DescParams;
    }

    public SkillWindowInfo(int skillId, int lvl)
    {
        Skillgrp skillgrp = SkillgrpTable.Instance.GetSkill(skillId, lvl);
        SkillNameData skillNameData = SkillNameTable.Instance.GetName(skillId, lvl);
        SkillId = skillId;
        Level = skillNameData.Level;
        Name = skillNameData.Name;
        // Desc = string.IsNullOrEmpty(skillNameData.Desc) ? SkillNameTable.Instance.GetDescription(skillId) : skillNameData.Desc;
        Desc = skillNameData.Desc;
        Icon = skillgrp.Icon;
        IconPanel = skillgrp.IconPanel;
        MpCost = skillgrp.MpConsume;
        Range = skillgrp.CastRange;
        IsMagic = skillgrp.IsMagic;
        Type = skillgrp.IconType;
        HitTime = skillgrp.HitTime;
        ReuseDelay = skillgrp.ReuseDelay;
        SkillDescParams = skillNameData.DescParams;
    }

    public string ComposeDescription()
    {
        for (var i = 0; i < SkillDescParams.Length; i++)
        {
            Desc = Desc.Replace($"$s{i + 1}", SkillDescParams[i]);
        }
        return Desc;
    }

    public string GetSkillType() =>
        Type switch
        {
            SkillType.Passive or SkillType.EquipmentPassive or SkillType.AbilityPassive or SkillType.Clan => "Passive Skill",
            _ when Type is SkillType.Physical or SkillType.Toggle or SkillType.CraftAndItems && IsMagic == SkillMagicType.None => "Active Skill",
            _ when Type is SkillType.Physical or SkillType.Magic && IsMagic != SkillMagicType.None => "Magic",
            _ when Type is SkillType.Buff && IsMagic == SkillMagicType.DamageBuffHeal => "Synergy/Song/Dance",
            _ => string.Empty
        };

    public bool IsMagicSkill() => IsMagic != SkillMagicType.None;

    public bool IsPassiveSkill() =>
        Type is SkillType.Passive or SkillType.EquipmentPassive or SkillType.AbilityPassive or SkillType.Clan;
}