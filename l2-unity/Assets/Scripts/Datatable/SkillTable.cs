using System.Collections.Generic;
using UnityEngine;

public class SkillTable
{
    private static SkillTable _instance;
    public static SkillTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillTable();
            }

            return _instance;
        }
    }

    [SerializeField] private List<int> _skillToLoad = new List<int>();

    public Dictionary<int, Skill> Skills { get; private set; }

    public Dictionary<int, EffectEmitter> SkillEffects { get; private set; }

    private bool _loadAll = false;

    public void Initialize()
    {
        FillDataToLoad();
    }

    private void FillDataToLoad()
    {
        _skillToLoad = new List<int> {
            16, 56, 3, 1216, 1177, 1012, 1011, 1168, 4345, 1040, 1027, 1015,
            1147, 1164, 91, 77, 70, 29, 1090, 1100, 1097, 1010, 1095,
            2039, 2150, 2151, 2152, 2153, 2154, // Soulshots
            2061, 2160, 2161, 2162, 2163, 2164, // Blessed Spiritshots
            2047, 2155, 2156, 2157, 2158, 2159}; // Spiritshots
    }

    /*
    Mortal Blow 16
    Power Shot 56
    Power Strike 3
    Self Heal 1216
    Wind Strike 1177
    Cure Poison 1012
    Heal 1011
    Curse: Poison 1168
    Might 4345
    Shield 1040
    Group Heal 1027 
    Battle Heal 1015 
    Vampiric Touch 1147 
    Curse: Weakness 1164 
    Defense Aura 91 
    Attack Aura 77
    Drain Health 70
    Iron Punch 29
    Life Drain 1090
    Chill Flame 1100 
    Dreaming Spirit 1097 
    Soul Shield 1010
    */

    /*

    Spiritshots (cast):
    2047 NG
    2155 D
    2156 C
    2157 B
    2158 A
    2159 S

    Blessed Spiritshots (cast):
    2061 NG
    2160 D
    2161 C
    2162 B
    2163 A
    2164 S

    Soulshots (cast):
    2039 NG
    2150 D
    2151 C
    2152 B
    2153 A
    2154 S

    */

    public bool ShouldLoadSkill(int id)
    {
        if (_loadAll) return true;

        if (_skillToLoad.Count == 0)
        {
            return true;
        }

        if (_skillToLoad.Contains(id))
        {
            return true;
        }

        return false;
    }

    public void CacheSkills()
    {
        Skills = new Dictionary<int, Skill>();

        foreach (KeyValuePair<int, Dictionary<int, Skillgrp>> kvp in SkillgrpTable.Instance.Skills)
        {
            if (Skills.ContainsKey(kvp.Key))
            {
                continue;
            }

            Dictionary<int, SkillNameData> skillname = SkillNameTable.Instance.GetNames(kvp.Key);

            SkillNameData[] skillNameArray = new SkillNameData[skillname.Keys.Count];
            for (int i = 0; i < skillname.Keys.Count; i++)
            {
                skillNameArray[i] = skillname[i + 1];
            }

            Dictionary<int, Skillgrp> skillgrp = SkillgrpTable.Instance.GetSkillgrp(kvp.Key);

            Skillgrp[] skillgrpArray = new Skillgrp[skillgrp.Keys.Count];
            for (int i = 0; i < skillgrp.Keys.Count; i++)
            {
                skillgrpArray[i] = skillgrp[i + 1];
            }

            SkillSoundgrp skillSoundgrp = SkillSoundgrpTable.Instance.GetSkillSoundGrp(kvp.Key);

            Skill skill = new Skill(kvp.Key, skillNameArray, skillgrpArray, skillSoundgrp);
            Skills.Add(kvp.Key, skill);

            Debug.LogWarning($"ID:{skill.SkillNameDatas[0].Id} Name:{skill.SkillNameDatas[0].Name} Effect:{skill.EffectId}");
        }
    }


    public Skill GetSkill(int id)
    {
        Skill skill;
        Skills.TryGetValue(id, out skill);

        return skill;
    }
}
