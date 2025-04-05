using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private static PlayerSkill _instance;
    public static PlayerSkill Instance => _instance;
    private Dictionary<int, SkillInfo> _skills;
    private (int SkillId, int Lvl, int SpCost)? _skillToLearn;

    public bool Initialized { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _skills = new Dictionary<int, SkillInfo>();
    }

    public void SetSkills(SkillInfo[] skills)
    {
        _skills = new Dictionary<int, SkillInfo>();

        for (var i = 0; i < skills.Length; i++)
        {
            SkillInfo si = skills[i];
            _skills[si.Id] = si;
        }

        SkillWindow.Instance.SetSkills(GetSkillsForWindow());
    }

    // public Skill[] GetSkills()
    // {
    //     if (_skills == null) return Array.Empty<Skill>();

    //     Skill[] skillDetails = new Skill[_skills.Count];
    //     for (var i = 0; i < _skills.Count; i++)
    //     {
    //         Skill skill = SkillTable.Instance.GetSkill(_skills[i].Id);
    //         skillDetails[i] = skill;
    //     }

    //     Initialized = true;
    //     return skillDetails;
    // }

    public SkillInfo GetSkillInfo(int skillId)
    {
        if (_skills == null)
        {
            return null;
        }

        if (_skills.TryGetValue(skillId, out SkillInfo skill))
        {
            return skill;
        }

        return null;
    }

    public List<SkillWindowInfo>[] GetSkillsForWindow()
    {
        if (_skills == null) return new List<SkillWindowInfo>[2];

        List<SkillWindowInfo>[] result = new List<SkillWindowInfo>[2] { new(10), new(10) };

        foreach (KeyValuePair<int, SkillInfo> kvp in _skills)
        {
            SkillWindowInfo skillwInfo = new SkillWindowInfo(kvp.Value.Id, kvp.Value.Level, 0, null);
            if (kvp.Value.IsPassive)
            {
                result[1].Add(skillwInfo);
            }
            else
            {
                result[0].Add(skillwInfo);
            }
        }

        Initialized = true;
        return result;
    }

    public void UpdateSkill()
    {
        GameClient.Instance.ClientPacketHandler.SendRequestSkillList();
    }

    public void UseSkill(int skillId)
    {
        if (_skills.TryGetValue(skillId, out SkillInfo skill))
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SKILL, skill);
        }
        else
        {
            Debug.LogWarning("Skill not found.");
        }
    }

    public void OnSkillUsed(int skillId, int reuseDelay)
    {
        SkillInfo skillInfo = GetSkillInfo(skillId);
        if (skillInfo != null)
        {
            skillInfo.CooldownStartTime = Time.time;
            skillInfo.CooldownEndTime = Time.time + reuseDelay / 1000f;

            PlayerShortcuts.Instance.OnSkillUsed();
        }
    }

    public bool IsSkillOnCooldown(int skillId)
    {
        if (_skills.TryGetValue(skillId, out SkillInfo skillInfo))
        {
            return skillInfo.IsSkillOnCooldown;
        }
        else
        {
            Debug.LogWarning("Skill not found.");
            return false;
        }
    }

    public void UpdateSkillCoolTimes(SkillCoolTimePacket.SkillCoolTimeInfo[] cooltimes)
    {
        for (var i = 0; i < cooltimes.Length; i++)
        {
            if (_skills.TryGetValue(cooltimes[i].SKillId, out SkillInfo skillInfo))
            {
                int reuseTimeSec = cooltimes[i].SkillReuseTimeSec;
                int cooldownRemainingTimeSec = cooltimes[i].SkillCooldownRemainingTimeSec;

                skillInfo.CooldownStartTime = Time.time - (reuseTimeSec - cooldownRemainingTimeSec);
                skillInfo.CooldownEndTime = Time.time + cooldownRemainingTimeSec;
            }
            else
            {
                Debug.LogWarning("Skill not found.");
            }
        }
    }
}