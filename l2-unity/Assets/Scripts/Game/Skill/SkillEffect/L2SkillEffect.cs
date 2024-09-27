using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L2SkillEffect
{
    [SerializeField] private int _skillId;
    [SerializeField] private List<EffectEmitter> _castingActions;
    [SerializeField] private List<EffectEmitter> _shotActions;

    public int SkillId { get { return _skillId; } set { _skillId = value; } }
    public List<EffectEmitter> CastingActions { get { return _castingActions; } set { _castingActions = value; } }
    public List<EffectEmitter> ShotActions { get { return _shotActions; } set { _shotActions = value; } }
}
