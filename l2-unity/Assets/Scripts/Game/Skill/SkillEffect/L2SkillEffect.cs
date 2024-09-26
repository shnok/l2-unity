using System.Collections.Generic;

public class L2SkillEffect
{
    public int SkillId { get; set; }
    public List<L2SkillEffectEmitter> CastingActions { get; set; }
    public List<L2SkillEffectEmitter> ShotActions { get; set; }
}
