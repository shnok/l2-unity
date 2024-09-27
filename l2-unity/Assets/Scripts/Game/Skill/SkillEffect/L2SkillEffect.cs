using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L2SkillEffect
{
    [SerializeField] private int _skillId;
    [SerializeField] private List<L2SkillEffectEmitter> _castingActions;
    [SerializeField] private List<L2SkillEffectEmitter> _shotActions;

    public int SkillId { get { return _skillId; } set { _skillId = value; } }
    public List<L2SkillEffectEmitter> CastingActions { get { return _castingActions; } set { _castingActions = value; } }
    public List<L2SkillEffectEmitter> ShotActions { get { return _shotActions; } set { _shotActions = value; } }
}
