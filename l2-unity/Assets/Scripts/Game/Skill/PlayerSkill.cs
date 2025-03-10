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
    
    private void Start()
    {
        _skills.Clear();
    }

    public void SetSkills(SkillInfo[] skills)
    {
        _skills = skills.ToList();
        _skills.Add(new SkillInfo(141, 1, true, false));
        _skills.Add(new SkillInfo(142, 1, true, false));
        _skills.Add(new SkillInfo(1164, 1, false, false));
        _skills.Add(new SkillInfo(56, 1, false, false));
        _skills.Add(new SkillInfo(91, 1, false, false));
        _skills.Add(new SkillInfo(16, 1, false, false));
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

    public void AcquireSkill(int skillId, int spCost)
    {
        foreach (var s in _skills)
        {
            if (s.Id == skillId)
            {
                _skillToLearn = (s.Id, s.Level, spCost);
            }
        }
    }
    
    public void UpdateSkill()
    {
        for (var i = 0; i < _skills.Count; ++i)
        {
            if (_skills[i].Id == _skillToLearn!.Value.SkillId)
            {
                _skills[i] = new SkillInfo(_skillToLearn.Value.SkillId, _skillToLearn.Value.Lvl + 1, _skills[i].IsPassive, false);
            }
        }
        WorldCombat.Instance.StatusUpdate(PlayerEntity.Instance, new List<StatusUpdatePacket.Attribute> {
            new((int)StatusUpdatePacket.AttributeType.SP, ((PlayerStats)PlayerEntity.Instance.Stats).Sp - _skillToLearn!.Value.SpCost)
        });
        _skillToLearn = null;
    }
    
    public void UseSkill(int skillId)
    {
        // GameClient.Instance.ClientPacketHandler.UseSkill(skillId);
        SkillInfo skill = _skills.FirstOrDefault(s => s.Id == skillId);
        if (skill is not null)
        {
            // skill.UseSkill();
        }
        else
        {
            Debug.LogWarning("Skill not found.");
        }
    }
}