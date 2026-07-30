using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    [SerializeField] private int _skillId;
    [SerializeField] private L2SkillEffect _skillEffect;
    public int SkillId { get => _skillId; }
    public int EffectId { get => Skillgrps[0].SkillVisualEffect; }
    public SkillNameData[] SkillNameDatas { get; private set; }
    public Skillgrp[] Skillgrps { get; private set; }
    public L2SkillEffect SkillEffect { get => _skillEffect; set => _skillEffect = value; }
    public SkillSoundgrp SkillSoundgrp { get; private set; }

    public Skill(int skillId, SkillNameData[] skillNameDatas, Skillgrp[] skillgrps, SkillSoundgrp skillSoundgrp)
    {
        _skillId = skillId;
        SkillNameDatas = skillNameDatas;
        Skillgrps = skillgrps;
        SkillSoundgrp = skillSoundgrp;
    }
}