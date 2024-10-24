using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SkillSoundgrp
{
    public int SkillId { get; set; }
    public SkillSound[] SpellEffectSounds { get; set; } //0: Start cast sound 
                                                        //1: End cast 
                                                        //2: Projectile hit target
    public SkillSound[] ShotEffectSounds { get; set; } //Unused?
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
    public string SoundEventName { get; set; }
    public EventReference SoundEvent { get; set; }
    public float SoundVolune { get; set; }
    public float SoundRadius { get; set; }
    public float SoundDelay { get; set; }
}