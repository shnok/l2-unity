using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private static PlayerSkill _instance;
    public static PlayerSkill Instance => _instance;
    private List<SkillInfo> _skills;
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

        _skills = new List<SkillInfo>();
    }

    public void SetSkills(SkillInfo[] skills)
    {
        _skills = skills.ToList();
        SkillWindow.Instance.SetSkills(GetSkillsForWindow());
    }

    public Skill[] GetSkills()
    {
        if (_skills == null) return Array.Empty<Skill>();

        Skill[] skillDetails = new Skill[_skills.Count];
        for (var i = 0; i < _skills.Count; i++)
        {
            Skill skill = SkillTable.Instance.GetSkill(_skills[i].Id);
            skillDetails[i] = skill;
        }

        Initialized = true;
        return skillDetails;
    }

    public List<SkillWindowInfo>[] GetSkillsForWindow()
    {
        if (_skills == null) return new List<SkillWindowInfo>[2];

        List<SkillWindowInfo>[] result = new List<SkillWindowInfo>[2] { new(10), new(10) };

        for (var i = 0; i < _skills.Count; i++)
        {
            SkillWindowInfo skillwInfo = new SkillWindowInfo(_skills[i].Id, _skills[i].Level, 0, null);
            if (_skills[i].IsPassive)
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
        SkillInfo skill = _skills.FirstOrDefault(s => s.Id == skillId);
        if (skill is not null)
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SKILL, skill);
        }
        else
        {
            Debug.LogWarning("Skill not found.");
        }
    }
}