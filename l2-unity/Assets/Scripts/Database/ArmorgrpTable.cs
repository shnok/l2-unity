using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ArmorgrpTable {
    private static ArmorgrpTable _instance;
    public static ArmorgrpTable Instance {
        get {
            if (_instance == null) {
                _instance = new ArmorgrpTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, Armorgrp> _armorgrps;

    public Dictionary<int, Armorgrp> ArmorGrps { get { return _armorgrps; } }


    public void Initialize() {
        ReadArmorGrpDat();
    }

    private void ReadArmorGrpDat() {
        _armorgrps = new Dictionary<int, Armorgrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Armorgrp_Classic.txt");
        if (!File.Exists(dataPath)) {
            Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                Armorgrp armorgrp = new Armorgrp();
                armorgrp.Model = new string[ModelTable.RACE_COUNT];
                armorgrp.Texture = new string[ModelTable.RACE_COUNT];
                string[] modTex;

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if(!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    if(DatUtils.ParseBaseAbstractItemGrpDat(armorgrp, key, value)) {
                        continue;
                    }

                    switch (key) {                
                        case "body_part": //artifact_a1 = chest, artifact_a2 = legs, artifact_a3 = boots, head = head, artifactbook = gloves, rfinger, lfinger, rear, lear, onepiece,
                            armorgrp.BodyPart = ItemSlotParser.ParseBodyPart(value); //TODO for fullbody store 2 models and textures for one item
                            break;
                        case "m_HumnFigh": // {{[Fighter.MFighter_m002_g]};{[mfighter.mfighter_m002_t10_g]}}
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte) CharacterRaceAnimation.MFighter] = modTex[0];
                            armorgrp.Texture[(byte) CharacterRaceAnimation.MFighter] = modTex[1];
                            break;
                        case "f_HumnFigh":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FFighter] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FFighter] = modTex[1];
                            break;
                        case "m_DarkElf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MDarkElf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MDarkElf] = modTex[1];
                            break;
                        case "f_DarkElf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FDarkElf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FDarkElf] = modTex[1];
                            break;
                        case "m_Dorf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MDwarf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MDwarf] = modTex[1];
                            break;
                        case "f_Dorf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FDwarf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FDwarf] = modTex[1];
                            break;
                        case "m_Elf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MElf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MElf] = modTex[1];
                            break;
                        case "f_Elf":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FElf] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FElf] = modTex[1];
                            break;
                        case "m_HumnMyst":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MMagic] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MMagic] = modTex[1];
                            break;
                        case "f_HumnMyst":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FMagic] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FMagic] = modTex[1];
                            break;
                        case "m_OrcFigh":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MOrc] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MOrc] = modTex[1];
                            break;
                        case "f_OrcFigh":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FOrc] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FOrc] = modTex[1];
                            break;
                        case "m_OrcMage":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.MShaman] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.MShaman] = modTex[1];
                            break;
                        case "f_OrcMage":
                            modTex = DatUtils.ParseArray(value);
                            armorgrp.Model[(byte)CharacterRaceAnimation.FShaman] = modTex[0];
                            armorgrp.Texture[(byte)CharacterRaceAnimation.FShaman] = modTex[1];
                            break;
                        case "mp_bonus": //mp_bonus=0
                            armorgrp.MpBonus = int.Parse(value);
                            break;                     
                    }
                }

                if (!ItemTable.Instance.ShouldLoadItem(armorgrp.ObjectId)) {
                    continue;
                }

                _armorgrps.TryAdd(armorgrp.ObjectId, armorgrp);
            }

            Debug.Log($"Successfully imported {_armorgrps.Count} armorgrp(s)");
        }
    }
}
