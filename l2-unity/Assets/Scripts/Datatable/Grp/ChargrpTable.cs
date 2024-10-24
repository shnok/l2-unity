using System.Collections.Generic;
using System.IO;
using FMODUnity;
using UnityEngine;

public class ChargrpTable
{
    private static ChargrpTable _instance;
    public static ChargrpTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChargrpTable();
            }

            return _instance;
        }
    }

    public Dictionary<CharacterModelType, Chargrp> CharGrps { get; private set; }

    public void Initialize()
    {
        Readgrps();
    }

    private void Readgrps()
    {
        CharGrps = new Dictionary<CharacterModelType, Chargrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Chargrp_Lindvior.txt");
        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int lineIndex = 0;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Replace("[", "").Replace("]", "").Replace("}", "").Replace("{", "");

                Chargrp grp = new Chargrp();

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
                        case "hair":
                            string[] hairVals = value.Split(";");

                            int maxHairCount = 5;
                            int hairCount = 1;
                            List<HairStyle> hairStyles = new List<HairStyle>();

                            for (int x = 0; x < hairVals.Length; x += 4)
                            {
                                if (hairVals[x + 2].Length == 0)
                                {
                                    continue;
                                }

                                HairStyle hairStyle = new HairStyle();
                                hairStyle.AhModel = hairVals[x];
                                hairStyle.BhModel = hairVals[x + 2];


                                if (hairStyle.AhModel != null && hairStyle.AhModel.Length > 0)
                                {
                                    hairStyle.AhTextures = new string[4];
                                    hairStyle.AhTextures[0] = hairVals[x + 1];
                                }
                                hairStyle.BhTextures = new string[4];
                                hairStyle.BhTextures[0] = hairVals[x + 3];

                                for (int colors = 1; colors < 4; colors++)
                                {
                                    hairStyle.BhTextures[colors] = hairStyle.BhTextures[0].Replace("t00", "t0" + colors);
                                    if (hairStyle.AhModel != null && hairStyle.AhModel.Length > 0)
                                    {
                                        hairStyle.AhTextures[colors] = hairStyle.AhTextures[0].Replace("t00", "t0" + colors);
                                    }
                                }

                                hairStyles.Add(hairStyle);
                                if (hairCount++ > maxHairCount)
                                {
                                    break;
                                }
                            }

                            grp.HairStyles = hairStyles.ToArray();
                            break;
                        case "face_mesh":
                            grp.FaceMesh = value.Split(";")[0];
                            break;
                        case "face_texture":
                            grp.FaceTextures = new string[6];
                            grp.FaceTextures[0] = value.Split(";")[0];
                            for (int x = 1; x < 6; x++)
                            {
                                grp.FaceTextures[x] = grp.FaceTextures[0].Replace("t00", "t0" + x);
                            }
                            break;
                        case "class_name":
                            grp.ClassName = value;
                            break;
                        case "race":
                            grp.Race = int.Parse(value);
                            break;
                        case "class_type":
                            grp.ClassType = int.Parse(value);
                            break;
                        case "attack_effect":
                            grp.AttackEffect = value;
                            break;
                        case "damage_sound":
                            grp.DamageSounds = DatUtils.ParseArray(value);
                            grp.DamageSoundsEvents = GetSoundReference(grp.ClassId, grp.DamageSounds);
                            break;
                        case "defense_sound":
                            grp.DefenseSounds = DatUtils.ParseArray(value);
                            grp.DefenseSoundsEvents = GetSoundReference(grp.ClassId, grp.DefenseSounds);
                            break;
                        case "attack_sound":
                            grp.AttackSounds = DatUtils.ParseArray(value);
                            grp.AttackSoundsEvents = GetSoundReference(grp.ClassId, grp.AttackSounds);
                            break;
                    }

                }

                CharGrps.Add(FromIndexToModelType(lineIndex++), grp);

                if (lineIndex > 13)
                {
                    break;
                }

            }

            Debug.Log($"Successfully imported {CharGrps.Count} chargrps(s)");
        }
    }

    private List<EventReference> GetSoundReference(int raceId, string[] eventNames)
    {
        List<EventReference> eventList = new List<EventReference>();
        foreach (var sound in eventNames)
        {
            string updatedPath = sound.Replace(".", "/");
            EventReference er = RuntimeManager.PathToEventReference("event:/" + updatedPath);
            Debug.LogWarning($"Getting sound reference for sound: {sound} - {er.IsNull}");
            if (er.IsNull)
            {
                Debug.LogWarning($"Missing sound: {sound} for chargrp: {raceId}.");
            }
            else
            {
                eventList.Add(er);
            }
        }

        return eventList;
    }

    public CharacterModelType FromIndexToModelType(int index)
    {
        switch (index)
        {
            case 0:
                return CharacterModelType.MFighter;
            case 1:
                return CharacterModelType.FFighter;
            case 2:
                return CharacterModelType.MDarkElf;
            case 3:
                return CharacterModelType.FDarkElf;
            case 4:
                return CharacterModelType.MDwarf;
            case 5:
                return CharacterModelType.FDwarf;
            case 6:
                return CharacterModelType.MElf;
            case 7:
                return CharacterModelType.FElf;
            case 8:
                return CharacterModelType.MMagic;
            case 9:
                return CharacterModelType.FMagic;
            case 10:
                return CharacterModelType.MOrc;
            case 11:
                return CharacterModelType.FOrc;
            case 12:
                return CharacterModelType.MShaman;
            case 13:
                return CharacterModelType.FShaman;
            default:
                return CharacterModelType.MFighter;
        }
    }
}