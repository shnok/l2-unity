using UnityEngine;

public class SkillInfo
{
    public SkillInfo(int id, int level, bool isPassive, bool isDisabled)
    {
        Id = id;
        Level = level;
        IsPassive = isPassive;
        IsDisabled = isDisabled;
    }

    public int Id { get; }
    public int Level { get; }
    public bool IsPassive { get; }
    public bool IsDisabled { get; }
    public float CooldownStartTime { get; set; }
    public float CooldownEndTime { get; set; }
    public bool IsSkillOnCooldown { get => Time.time < CooldownEndTime; }
}
