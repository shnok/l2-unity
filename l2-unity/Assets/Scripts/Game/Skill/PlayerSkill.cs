using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private static PlayerSkill _instance;
    public static PlayerSkill Instance => _instance;
    private Dictionary<int, SkillInfo> _skills;
    private List<SkillInfo> _skillsOnCd;
    private Coroutine _cooltimeManagementCoroutine;

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
        _skillsOnCd = new List<SkillInfo>();
    }

    private void OnDestroy()
    {
        if (_cooltimeManagementCoroutine != null)
        {
            StopCoroutine(_cooltimeManagementCoroutine);
        }
    }

    public void SetSkills(SkillInfo[] skills)
    {
        Debug.LogWarning($"Received {skills.Length} skill(s) from the server.");

        _skills = new Dictionary<int, SkillInfo>();

        for (var i = 0; i < skills.Length; i++)
        {
            SkillInfo si = skills[i];
            _skills[si.Id] = si;
            // Debug.LogWarning($"Adding skill with id: {si.Id}.");
        }

        SkillWindow.Instance.SetSkills(GetSkillsForWindow());
        if (_cooltimeManagementCoroutine != null)
        {
            StopCoroutine(_cooltimeManagementCoroutine);
        }

        _cooltimeManagementCoroutine = StartCoroutine(ManageCooldowns());
    }

    public SkillInfo GetSkillInfo(int skillId)
    {
        if (_skills == null)
        {
            Debug.LogError("Skills are not yet loaded.");
            return null;
        }

        if (_skills.TryGetValue(skillId, out SkillInfo skill))
        {
            return skill;
        }

        Debug.Log($"Character doesnt know the skill with id: {skillId}.");

        return null;
    }

    // public List<SkillInfo> GetAllSkills()
    // {
    //     if (_skills == null)
    //     {
    //         return new List<SkillInfo>();
    //     }

    //     return _skills.Values.ToList();
    // }

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

    // public void UpdateSkill()
    // {
    //     GameClient.Instance.ClientPacketHandler.SendRequestSkillList();
    // }

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

            _skillsOnCd.Add(skillInfo);
            PlayerShortcuts.Instance.OnSkillUsed(skillInfo);
        }
    }

    private IEnumerator ManageCooldowns()
    {
        while (true)
        {
            for (int i = _skillsOnCd.Count - 1; i >= 0; i--)
            {
                if (i < 0 || i >= _skillsOnCd.Count)
                {
                    continue;
                }

                if (_skillsOnCd[i].CooldownEndTime <= Time.time)
                {
                    AudioManager.Instance.PlayUISound("cooltime_end");
                    _skillsOnCd.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(1 / 30f); // 30 fps
        }
    }

    // public bool IsSkillOnCooldown(int skillId)
    // {
    //     if (_skills.TryGetValue(skillId, out SkillInfo skillInfo))
    //     {
    //         return skillInfo.IsSkillOnCooldown;
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Skill not found.");
    //         return false;
    //     }
    // }

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

                _skillsOnCd.Add(skillInfo);
            }
            else
            {
                Debug.LogWarning("Skill not found.");
            }
        }
    }
}