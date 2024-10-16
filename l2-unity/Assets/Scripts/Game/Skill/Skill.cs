using System.Collections.Generic;

public class Skill
{
    public int SkillId { get; private set; }
    public int EffectId { get { return Skillgrps[0].SkillVisualEffect; } }
    public SkillNameData[] SkillNameDatas { get; private set; }
    public Skillgrp[] Skillgrps { get; private set; }
    public L2SkillEffect SkillEffect { get; set; }
    public SkillSoundgrp SkillSoundgrp { get; private set; }

    public Skill(int skillId, SkillNameData[] skillNameDatas, Skillgrp[] skillgrps, SkillSoundgrp skillSoundgrp)
    {
        SkillId = skillId;
        SkillNameDatas = skillNameDatas;
        Skillgrps = skillgrps;
        SkillSoundgrp = skillSoundgrp;
    }
}