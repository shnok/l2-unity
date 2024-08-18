using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skillgrp 
{
    [SerializeField] public int _id;
    [SerializeField] public int _level;
    [SerializeField] public int _subLevel;
    [SerializeField] public int _icon_type;
    [SerializeField] public int _magicType;
    [SerializeField] public int _operate_type;
    [SerializeField] public int _mp_consume;
    [SerializeField] public int _cast_range;
    [SerializeField] public int _cast_style;
    [SerializeField] public double _hit_time;
    [SerializeField] public double _cool_time;
    [SerializeField] public double _reuse_delay;
    [SerializeField] public int _effect_point;
    [SerializeField] public int _is_magic;
    [SerializeField] public int _is_double;
    [SerializeField] public string _animation;
    [SerializeField] public string _skill_visual_effect;
    [SerializeField] public string _icon;
    [SerializeField] public string _icon_panel;
    [SerializeField] public int _debuff;
    [SerializeField] public int _resist_cast;
    [SerializeField] public int _enchant_skill_level;
    [SerializeField] public string _enchant_icon;
    [SerializeField] public int _hp_consume;
    [SerializeField] public int _rumble_self;
    [SerializeField] public int _rumble_target;


    public int Id { get => _id; set => _id = value; }
    public int Level { get => _level; set => _level = value; }
    public int SubLevel { get => _subLevel; set => _subLevel = value; }
    public int Icon_type { get => _icon_type; set => _icon_type = value; }
    public int MagicType { get => _magicType; set => _magicType = value; }
    public int OperateType { get => _operate_type; set => _operate_type = value; }
    public int MpConsume { get => _mp_consume; set => _mp_consume = value; }
    public int CastRange { get => _cast_range; set => _cast_range = value; }
    public int CastStyle { get => _cast_style; set => _cast_style = value; }
    public double HitTime { get => _hit_time; set => _hit_time = value; }
    public double CoolTime { get => _cool_time; set => _cool_time = value; }
    public double ReuseDelay { get => _reuse_delay; set => _reuse_delay = value; }
    public int EffectPoint { get => _effect_point; set => _effect_point = value; }
    public int IsMagic { get => _is_magic; set => _is_magic = value; }
    public int IsDouble { get => _is_double; set => _is_double = value; }
    public string Animation { get => _animation; set => _animation = value; }
    public string SkillVisualEffect { get => _skill_visual_effect; set => _skill_visual_effect = value; }

    public string Icon { get => _icon; set => _icon = value; }
    public string IconPanel { get => _icon_panel; set => _icon_panel = value; }
    public int Debuff { get => _debuff; set => _debuff = value; }
    public int ResistCast { get => _resist_cast; set => _resist_cast = value; }
    public int EnchantSkillLevel { get => _enchant_skill_level; set => _enchant_skill_level = value; }
    public string EnchantIcon { get => _enchant_icon; set => _enchant_icon = value; }
    public int HpConsume { get => _hp_consume; set => _hp_consume = value; }
    public int RumbleSelf { get => _rumble_self; set => _rumble_self = value; }
    public int RumbleTarget { get => _rumble_target; set => _rumble_target = value; }
}
