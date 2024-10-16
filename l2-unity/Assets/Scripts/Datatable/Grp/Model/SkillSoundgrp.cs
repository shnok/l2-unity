using System.Collections.Generic;
using UnityEngine;

public class SkillSoundgrp
{
    public int SkillId { get; set; }
    public SkillSound[] SpellEffectSounds { get; set; }
    public SkillSound[] ShotEffectSounds { get; set; }
    public string[] CastingVoices { get; set; }
    public string[] CastingEndVoices { get; set; }
    public float CastVolume { get; set; }
    public float CastRadius { get; set; }

    public SkillSoundgrp()
    {
        SpellEffectSounds = new SkillSound[3];
        ShotEffectSounds = new SkillSound[3];
        CastingVoices = new string[14];
        CastingEndVoices = new string[14];
    }
}

public class SkillSound
{
    public string SoundEvent { get; set; }
    public float SoundVolune { get; set; }
    public float SoundRadius { get; set; }
    public float SoundDelay { get; set; }
}