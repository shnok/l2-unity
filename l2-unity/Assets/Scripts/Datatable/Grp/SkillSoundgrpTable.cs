using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FMODUnity;
using UnityEngine;

public class SkillSoundgrpTable
{
    private static SkillSoundgrpTable _instance;
    private Dictionary<int, SkillSoundgrp> _data;

    public static SkillSoundgrpTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillSoundgrpTable();
            }

            return _instance;
        }
    }

    public void Initialize()
    {
        ReadActions();
    }

    private void ReadActions()
    {
        _data = new Dictionary<int, SkillSoundgrp>();

        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SkillSoundgrp_Classic.txt");
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
                SkillSoundgrp grp = new SkillSoundgrp();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++)
                {
                    if (!keyvals[i].Contains("="))
                    {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1].Replace("[", "").Replace("]", "");

                    switch (key)
                    {
                        case "skill_id":
                            grp.SkillId = int.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "spelleffect_sound_1":
                        case "spelleffect_sound_2":
                        case "spelleffect_sound_3":
                            if (value != "None")
                            {
                                int index = int.Parse(key.Replace("spelleffect_sound_", ""), CultureInfo.InvariantCulture);
                                grp.SpellEffectSounds[--index] = new SkillSound();
                                grp.SpellEffectSounds[index].SoundEventName = value;
                            }
                            break;
                        case "spelleffect_sound_vol_1":
                        case "spelleffect_sound_vol_2":
                        case "spelleffect_sound_vol_3":
                            int volIndex = int.Parse(key.Replace("spelleffect_sound_vol_", ""));
                            if (grp.SpellEffectSounds[--volIndex] != null)
                            {
                                float vol = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.SpellEffectSounds[volIndex].SoundVolune = vol;
                            }
                            break;
                        case "spelleffect_sound_rad_1":
                        case "spelleffect_sound_rad_2":
                        case "spelleffect_sound_rad_3":
                            int radIndex = int.Parse(key.Replace("spelleffect_sound_rad_", ""));
                            if (grp.SpellEffectSounds[--radIndex] != null)
                            {
                                float rad = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.SpellEffectSounds[radIndex].SoundRadius = rad;
                            }
                            break;
                        case "spelleffect_sound_delay_1":
                        case "spelleffect_sound_delay_2":
                        case "spelleffect_sound_delay_3":
                            int delayIndex = int.Parse(key.Replace("spelleffect_sound_delay_", ""));
                            if (grp.SpellEffectSounds[--delayIndex] != null)
                            {
                                float delay = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.SpellEffectSounds[delayIndex].SoundDelay = delay;
                            }
                            break;
                        case "shoteffect_sound_1":
                        case "shoteffect_sound_2":
                        case "shoteffect_sound_3":
                            if (value != "None")
                            {
                                int index = int.Parse(key.Replace("shoteffect_sound_", ""), CultureInfo.InvariantCulture);
                                grp.ShotEffectSounds[--index] = new SkillSound();
                                grp.ShotEffectSounds[index].SoundEventName = value;
                            }
                            break;
                        case "shoteffect_sound_vol_1":
                        case "shoteffect_sound_vol_2":
                        case "shoteffect_sound_vol_3":
                            int volIndex2 = int.Parse(key.Replace("shoteffect_sound_vol_", ""));
                            if (grp.ShotEffectSounds[--volIndex2] != null)
                            {
                                float vol = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.ShotEffectSounds[volIndex2].SoundVolune = vol;
                            }
                            break;
                        case "shoteffect_sound_rad_1":
                        case "shoteffect_sound_rad_2":
                        case "shoteffect_sound_rad_3":
                            int radIndex2 = int.Parse(key.Replace("shoteffect_sound_rad_", ""));
                            if (grp.ShotEffectSounds[--radIndex2] != null)
                            {
                                float rad = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.ShotEffectSounds[radIndex2].SoundRadius = rad;
                            }
                            break;
                        case "shoteffect_sound_delay_1":
                        case "shoteffect_sound_delay_2":
                        case "shoteffect_sound_delay_3":
                            int delayIndex2 = int.Parse(key.Replace("shoteffect_sound_delay_", ""));
                            if (grp.ShotEffectSounds[--delayIndex2] != null)
                            {
                                float delay = float.Parse(value, CultureInfo.InvariantCulture);
                                grp.ShotEffectSounds[delayIndex2].SoundDelay = delay;
                            }
                            break;
                        case "mfighter_cast":
                        case "ffighter_cast":
                        case "mmagic_cast":
                        case "fmagic_cast":
                        case "melf_cast":
                        case "felf_cast":
                        case "mdarkelf_cast":
                        case "fdarkelf_cast":
                        case "mdwarf_cast":
                        case "fdwarf_cast":
                        case "morc_cast":
                        case "forc_cast":
                        case "mshaman_cast":
                        case "fshaman_cast":
                            // case "mkamael_cast":
                            // case "fkamael_cast":
                            // case "mertheia_cast":
                            // case "fertheia_cast":
                            if (value != "None")
                            {
                                CharacterModelType modelType = CharacterModelTypeParser.ParseRace(key.Replace("_cast", ""));
                                grp.CastingVoices[(int)modelType] = value;
                            }
                            break;
                        case "mfighter_magic":
                        case "ffighter_magic":
                        case "mmagic_magic":
                        case "fmagic_magic":
                        case "melf_magic":
                        case "felf_magic":
                        case "mdarkelf_magic":
                        case "fdarkelf_magic":
                        case "mdwarf_magic":
                        case "fdwarf_magic":
                        case "morc_magic":
                        case "forc_magic":
                        case "mshaman_magic":
                        case "fshaman_magic":
                            // case "mkamael_magic":
                            // case "fkamael_magic":
                            // case "mertheia_magic":
                            // case "fertheia_magic":
                            if (value != "None")
                            {
                                CharacterModelType modelType = CharacterModelTypeParser.ParseRace(key.Replace("_magic", ""));
                                grp.CastingEndVoices[(int)modelType] = value;
                            }
                            break;
                        case "cast_volune":
                            grp.CastVolume = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                        case "cast_rad":
                            grp.CastRadius = float.Parse(value, CultureInfo.InvariantCulture);
                            break;
                    }
                }

                UpdateSoundReferences(grp.SkillId, grp.SpellEffectSounds);
                UpdateSoundReferences(grp.SkillId, grp.ShotEffectSounds);

                _data.TryAdd(grp.SkillId, grp);
            }
            Debug.Log($"Successfully imported {_data.Count} SkillSoundGrp(s)");
        }
    }

    private void UpdateSoundReferences(int skillId, SkillSound[] skillSounds)
    {
        foreach (SkillSound skillSound in skillSounds)
        {
            if (skillSound == null || skillSound.SoundEventName == null || skillSound.SoundEventName.Length == 0)
            {
                continue;
            }

            string updatedPath = skillSound.SoundEventName.Replace(".", "/");
            EventReference er = RuntimeManager.PathToEventReference("event:/" + updatedPath);
            Debug.LogWarning($"Getting sound reference for sound: {skillSound.SoundEventName}");
            if (er.IsNull)
            {
                Debug.LogWarning($"Missing sound: {updatedPath} for skill: {skillId}.");
            }
            else
            {
                skillSound.SoundEvent = er;
            }
        }
    }

    public SkillSoundgrp GetSkillSoundGrp(int key)
    {
        SkillSoundgrp skillSoundgrp;
        _data.TryGetValue(key, out skillSoundgrp);

        if (skillSoundgrp == null)
        {
            Debug.LogWarning($"SkillsoundGrp is null for skillId: {key}.");
        }

        return skillSoundgrp;
    }
}
