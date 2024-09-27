using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkillgrpTable
{
    private static SkillgrpTable _instance;

    public static SkillgrpTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillgrpTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, Dictionary<int, Skillgrp>> _skills;

    public void Initialize()
    {
        _skills = new Dictionary<int, Dictionary<int, Skillgrp>>();
        ReadActions();
    }

    public void ClearTable()
    {
        _skills.Clear();
        _skills = null;
        _instance = null;
    }

    public Skillgrp GetSkill(int id, int level)
    {
        _skills.TryGetValue(id, out Dictionary<int, Skillgrp> skillLevel);

        if (skillLevel == null) return null;

        skillLevel.TryGetValue(level, out Skillgrp skillgrp);

        return skillgrp;
    }

    public Dictionary<int, Skillgrp> GetSkillgrp(int id)
    {
        _skills.TryGetValue(id, out Dictionary<int, Skillgrp> skillLevel);

        return skillLevel;
    }

    public Dictionary<int, Dictionary<int, Skillgrp>> Skills { get { return _skills; } }

    private void ReadActions()
    {

        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Skillgrp_Classic.txt");
        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Skillgrp skillGrp = new Skillgrp();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++)
                {
                    if (!keyvals[i].Contains("="))
                    {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    switch (key)
                    {
                        case "skill_id":
                            skillGrp.Id = int.Parse(value);
                            break;
                        case "skill_level":
                            skillGrp.Level = int.Parse(value);
                            break;
                        case "skill_sublevel":
                            skillGrp.SubLevel = int.Parse(value);
                            break;
                        case "icon_type":
                            skillGrp.Icon_type = int.Parse(value);
                            break;
                        case "MagicType":
                            skillGrp.MagicType = int.Parse(value);
                            break;
                        case "operate_type":
                            skillGrp.OperateType = int.Parse(value);
                            break;
                        case "mp_consume":
                            skillGrp.MpConsume = int.Parse(value);
                            break;
                        case "cast_range":
                            skillGrp.CastRange = int.Parse(value);
                            break;
                        case "cast_style":
                            skillGrp.CastStyle = int.Parse(value);
                            break;
                        case "hit_time":
                            skillGrp.HitTime = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "cool_time":
                            skillGrp.CoolTime = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "reuse_delay":
                            skillGrp.ReuseDelay = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "effect_point":
                            skillGrp.EffectPoint = int.Parse(value);
                            break;
                        case "is_magic":
                            skillGrp.IsMagic = int.Parse(value);
                            break;
                        case "origin_skill":
                            skillGrp.IsDouble = int.Parse(value);
                            break;
                        case "animation":
                            skillGrp.Animation = DatUtils.CleanupString(value);
                            break;
                        case "skill_visual_effect":
                            string visualEffect = DatUtils.CleanupString(value); ;
                            if (int.TryParse(visualEffect, out int effectId))
                            {
                                skillGrp.SkillVisualEffect = effectId;
                            }
                            else
                            {
                                skillGrp.SkillVisualEffect = -1;
                            }
                            break;
                        case "icon":
                            skillGrp.Icon = DatUtils.CleanupString(value);
                            break;
                        case "icon_panel":
                            skillGrp.IconPanel = DatUtils.CleanupString(value);
                            break;
                        case "debuff":
                            skillGrp.Debuff = int.Parse(value);
                            break;
                        case "resist_cast":
                            skillGrp.ResistCast = int.Parse(value);
                            break;
                        case "enchant_skill_level":
                            skillGrp.EnchantSkillLevel = int.Parse(value);
                            break;
                        case "enchant_icon":
                            skillGrp.EnchantIcon = DatUtils.CleanupString(value);
                            break;
                        case "hp_consume":
                            skillGrp.HpConsume = int.Parse(value);
                            break;
                        case "rumble_self":
                            skillGrp.RumbleSelf = int.Parse(value);
                            break;
                        case "rumble_target":
                            skillGrp.RumbleTarget = int.Parse(value);
                            break;
                    }
                }

                if (!SkillTable.Instance.ShouldLoadSkill(skillGrp.Id))
                {
                    continue;
                }

                TryAdd(skillGrp);
            }

            Debug.Log($"Successfully imported {_skills.Count} skills(s)");
        }

    }

    private void TryAdd(Skillgrp skillGrp)
    {
        if (_skills.ContainsKey(skillGrp.Id))
        {
            Dictionary<int, Skillgrp> dataGrp = _skills[skillGrp.Id];
            dataGrp.TryAdd(skillGrp.Level, skillGrp);
        }
        else
        {
            Dictionary<int, Skillgrp> dataGrp = CreateDict();
            AddDict(dataGrp, skillGrp);
            _skills.TryAdd(skillGrp.Id, dataGrp);
        }
    }

    private Dictionary<int, Skillgrp> CreateDict()
    {
        return new Dictionary<int, Skillgrp>();
    }

    private void AddDict(Dictionary<int, Skillgrp> dataGrp, Skillgrp skillGrp)
    {
        dataGrp.TryAdd(skillGrp.Level, skillGrp);
    }
}
