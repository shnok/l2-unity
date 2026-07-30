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
                        case "mfighter_cast": //mfighter
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MFighter] = value.Replace("chrsound.", ""); break;
                        case "ffighter_cast":  //ffighter
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FFighter] = value.Replace("chrsound.", ""); break;
                        case "mmagic_cast": //mdarkelf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MDarkElf] = value.Replace("chrsound.", ""); break;
                        case "fmagic_cast": //fdarkelf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FDarkElf] = value.Replace("chrsound.", ""); break;
                        case "melf_cast": //mdwarf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MDwarf] = value.Replace("chrsound.", ""); break;
                        case "felf_cast": //fdwarf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FDwarf] = value.Replace("chrsound.", ""); break;
                        case "mdarkelf_cast": //melf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MElf] = value.Replace("chrsound.", ""); break;
                        case "fdarkelf_cast": //felf
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FElf] = value.Replace("chrsound.", ""); break;
                        case "mdwarf_cast": //mmagic
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MMagic] = value.Replace("chrsound.", ""); break;
                        case "fdwarf_cast": //fmagic
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FMagic] = value.Replace("chrsound.", ""); break;
                        case "morc_cast": //morc
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MOrc] = value.Replace("chrsound.", ""); break;
                        case "forc_cast": //forc
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FOrc] = value.Replace("chrsound.", ""); break;
                        case "mshaman_cast": //mshaman
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.MShaman] = value.Replace("chrsound.", ""); break;
                        case "fshaman_cast": //fshaman
                            if (value != "None") grp.CastingVoices[(int)CharacterModelType.FShaman] = value.Replace("chrsound.", ""); break;
                        case "mfighter_magic": //mfighter
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MFighter] = value.Replace("chrsound.", ""); break;
                        case "ffighter_magic":  //ffighter
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FFighter] = value.Replace("chrsound.", ""); break;
                        case "mmagic_magic": //mdarkelf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MDarkElf] = value.Replace("chrsound.", ""); break;
                        case "fmagic_magic": //fdarkelf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FDarkElf] = value.Replace("chrsound.", ""); break;
                        case "melf_magic": //mdwarf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MDwarf] = value.Replace("chrsound.", ""); break;
                        case "felf_magic": //fdwarf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FDwarf] = value.Replace("chrsound.", ""); break;
                        case "mdarkelf_magic": //melf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MElf] = value.Replace("chrsound.", ""); break;
                        case "fdarkelf_magic": //felf
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FElf] = value.Replace("chrsound.", ""); break;
                        case "mdwarf_magic": //mmagic
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MMagic] = value.Replace("chrsound.", ""); break;
                        case "fdwarf_magic": //fmagic
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FMagic] = value.Replace("chrsound.", ""); break;
                        case "morc_magic": //morc
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MOrc] = value.Replace("chrsound.", ""); break;
                        case "forc_magic": //forc
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FOrc] = value.Replace("chrsound.", ""); break;
                        case "mshaman_magic": //mshaman
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.MShaman] = value.Replace("chrsound.", ""); break;
                        case "fshaman_magic": //fshaman
                            if (value != "None") grp.CastingEndVoices[(int)CharacterModelType.FShaman] = value.Replace("chrsound.", ""); break;
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
            // Debug.LogWarning($"Getting sound reference for sound: {skillSound.SoundEventName}");
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
            // Debug.LogWarning($"SkillsoundGrp is null for skillId: {key}.");
        }

        return skillSoundgrp;
    }
}
