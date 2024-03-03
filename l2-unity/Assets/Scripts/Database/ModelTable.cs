using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelTable
{
    private static ModelTable _instance;
    public static ModelTable Instance { 
        get { 
            if (_instance == null) {
                _instance = new ModelTable();
            }

            return _instance; 
        } 
    }

    public static int RACE_COUNT = 14;
    private static int FACE_COUNT = 6;
    private static int HAIR_STYLE_COUNT = 6;
    private static int HAIR_COLOR_COUNT = 4;

    private GameObject[] _playerContainers;
    private GameObject[] _userContainers;
    private GameObject[,] _faces;
    private GameObject[,] _hair;
    private Dictionary<int, GameObject> _weapons;
    //private Dictionary<int, GameObject[,]> _armors; // ModelID(..).Model[Race(14),bodypart(5)]
    private Dictionary<string, L2Armor> _armors;
    private class L2Armor {
        public GameObject baseModel;
        public Dictionary<string, Material> materials;
    }

    public class L2ArmorPiece {
        public GameObject baseArmorModel;
        public Material material;

        public L2ArmorPiece(GameObject baseArmorModel, Material material) {
            this.baseArmorModel = baseArmorModel;
            this.material = material;
        }
    }

    public void Initialize() {
        CachePlayerContainers();
        CacheFaces();
        CacheHair();
        CacheWeapons();
        CacheArmors();
    }

    private void OnDestroy() {
        _faces = null;
        _hair = null;
        _instance = null;
        _playerContainers = null;
        _userContainers = null;
        _weapons.Clear();
        _armors.Clear();
        _instance = null;
    }

    // -------
    // CACHE
    // -------
    private void CachePlayerContainers() {
        _playerContainers = new GameObject[RACE_COUNT];
        _userContainers = new GameObject[RACE_COUNT];
       
        // Player Containers
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            string path = $"Data/Animations/{race}/{raceId}/Player_{raceId}";
            _playerContainers[r] = Resources.Load<GameObject>(path);
         //   Debug.Log($"Loading player container {r} [{path}]");
        }

        // User Containers
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            string path = $"Data/Animations/{race}/{raceId}/User_{raceId}";
            _userContainers[r] = Resources.Load<GameObject>(path);
          //  Debug.Log($"Loading user container {r} [{path}]");
        }
    }

    private void CacheFaces() {   
        _faces = new GameObject[RACE_COUNT, FACE_COUNT]; // there is 14 races, each race has 6 faces

        // Faces
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);
            for (int f = 0; f < FACE_COUNT; f++) {
                string path = $"Data/Animations/{race}/{raceId}/Faces/{raceId}_f_{f}";
                _faces[r, f] = Resources.Load<GameObject>(path);
               // Debug.Log($"Loading face {f} for race {raceId} [{path}]");
            }
        }
    }

    private void CacheHair() {
        _hair = new GameObject[RACE_COUNT, HAIR_STYLE_COUNT * HAIR_COLOR_COUNT * 2]; // there is 14 races, each race has 6 hairstyle (2 models each) of 4 colors

        // Hair
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);
            for (int hs = 0; hs < HAIR_STYLE_COUNT; hs++) {
                for (int hc = 0; hc < HAIR_COLOR_COUNT; hc++) {
                    int index = hs * HAIR_STYLE_COUNT + (hc * 2);
                    string path = $"Data/Animations/{race}/{raceId}/Hair/{raceId}_h_{hs}_{hc}_ah";
                    _hair[r, index] = Resources.Load<GameObject>(path);
                   // Debug.Log($"Loading hair {hs} color {hc} at {index} for race {raceId} [{path}]");

                    path = $"Data/Animations/{race}/{raceId}/Hair/{raceId}_h_{hs}_{hc}_bh";
                    _hair[r, index + 1] = Resources.Load<GameObject>(path);
                    //Debug.Log($"Loading hair {hs} color {hc} at {index + 1} for race {raceId} [{path}]");
                }
            }
        }
    }

    private void CacheWeapons() {
        _weapons = new Dictionary<int, GameObject>();
        int success = 0;
        foreach (KeyValuePair<int, Weapon> kvp in ItemTable.Instance.Weapons) {
            GameObject weapon = LoadWeaponModel(kvp.Value.Weapongrp.Model);
            if (weapon != null) {
                success++;
                _weapons[kvp.Key] = weapon;
            }
        }

        Debug.Log($"Successfully loaded {success}/{ItemTable.Instance.Weapons.Count} weapon model(s).");
    }

    private GameObject LoadWeaponModel(string model) {
        string[] folderFile = model.Split(".");

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";
        Debug.Log("====> " + modelPath);

        GameObject weapon = (GameObject)Resources.Load(modelPath);
        if (weapon == null) {
            //Debug.LogWarning($"Can't find armor model at {modelPath}");
        } else {
            Debug.Log($"Successfully loaded weapon {model} model.");
        }

        return weapon;
    }

    private void CacheArmors() {
        _armors = new Dictionary<string, L2Armor>();

        int armorMaterials = 0;
        foreach (KeyValuePair<int, Armor> kvp in ItemTable.Instance.Armors) {
            for (int i = 0; i < RACE_COUNT; i++) {
                string model = kvp.Value.Armorgrp.Model[i];
                if (model == null) {
                    Debug.LogWarning($"Model string is null for race {(CharacterRaceAnimation)i} in armor {kvp.Key}");
                    continue;
                }

                L2Armor l2Model = null;
                if (!_armors.ContainsKey(model)) {
                    l2Model = new L2Armor();
                    l2Model.baseModel = LoadArmorModel(model);
                    if(l2Model.baseModel != null) {
                        l2Model.materials = new Dictionary<string, Material>();
                        _armors.Add(model, l2Model);
                    }
                }

                if (l2Model == null) {
                    _armors.TryGetValue(model, out l2Model);
                }

                if (l2Model == null || l2Model.baseModel == null) {
                    //Debug.LogWarning($"Armor {kvp.Key} model cannot be loaded for race {(CharacterRaceAnimation)i} at {model}");
                    continue;
                }

                string texture = kvp.Value.Armorgrp.Texture[i];

                if(l2Model.materials.ContainsKey(texture)) {
                    continue;
                }

                Material armorMaterial = LoadArmorMaterial(texture);

                if (armorMaterial == null) {
                   // Debug.LogWarning($"Armor {kvp.Key} material cannot be loaded for race {(CharacterRaceAnimation)i} at {texture}");
                    continue;
                }

                l2Model.materials.Add(texture, armorMaterial);
                armorMaterials++;
            }
        }

        Debug.Log($"Successfully loaded {_armors.Count} armor model(s).");
        Debug.Log($"Successfully loaded {armorMaterials} armor marerial(s).");
    }

    private GameObject LoadArmorModel(string model) {

        string[] folderFile = model.Split(".");

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";

        GameObject armorPiece = (GameObject)Resources.Load(modelPath);
        if (armorPiece == null) {
            //Debug.LogWarning($"Can't find armor model at {modelPath}");
        } else {
            Debug.Log($"Successfully loaded armor model at {modelPath}");
        }

        return armorPiece;
    }

    private Material LoadArmorMaterial(string texture) {
        string[] folderFile = texture.Split(".");

        string materialPath = $"Data/SysTextures/{folderFile[0]}/Materials/{folderFile[1]}";

        Material material = (Material)Resources.Load(materialPath);
        if (material == null) {
           // Debug.LogWarning($"Can't find armor model at {materialPath}");
        } else {
            Debug.Log($"Successfully loaded armor model at {materialPath}");
        }

        return material;
    }

    // -------
    // Getters
    // -------
    public L2ArmorPiece GetArmorPiece(Armor armor, CharacterRaceAnimation raceId) {
        if (armor == null) {
            Debug.LogWarning($"Given armor is null");
            return null;
        }

        string model = armor.Armorgrp.Model[(byte)raceId];
        if (!_armors.ContainsKey(model)) {
            Debug.LogWarning($"Can't find armor model {model} in ModelTable");
            return null;
        }

        GameObject baseModel = _armors[model].baseModel;
        if (baseModel == null) {
            Debug.LogWarning($"Can't find armor model {model} for {raceId} in ModelTable");
            return null;
        }

        if (_armors[model].materials == null) {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        if(armor.Armorgrp.Texture == null || armor.Armorgrp.Texture.Length < RACE_COUNT || armor.Armorgrp.Texture[(byte)raceId] == null) {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        Material material;
        _armors[model].materials.TryGetValue(armor.Armorgrp.Texture[(byte) raceId], out material);

        if (material == null) {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        L2ArmorPiece armorModel = new L2ArmorPiece(baseModel, material);
        return armorModel;
    }

    public L2ArmorPiece GetArmorPieceByItemId(int itemId, CharacterRaceAnimation raceId) {
        Armor armor = ItemTable.Instance.GetArmor(itemId);

        return GetArmorPiece(armor, raceId);
    }

    public GameObject GetWeapon(int itemId) {
        if (!_weapons.ContainsKey(itemId)) {
            Debug.LogWarning($"Can't find weapon model {itemId} in ModelTable");
            return null;
        }

        GameObject go = _weapons[itemId];
        if (go == null) {
            Debug.LogWarning($"Can't find weapon model {itemId} in ModelTable");
            return null;
        }

        return go;
    }

    public GameObject GetContainer(CharacterRaceAnimation raceId, bool isPlayer) {
        GameObject go = isPlayer ? _playerContainers[(byte)raceId] : _userContainers[(byte)raceId];
        if (go == null) {
            Debug.LogError($"Can't find container for race {raceId} isPlayer? {isPlayer}");
        }

        return go;
    }

    public GameObject GetFace(CharacterRaceAnimation raceId, byte face) {
        GameObject go = _faces[(byte)raceId, face];
        if (go == null) {
            Debug.LogError($"Can't find face {face} for race {raceId} at index {raceId},{face}");
        }

        return go;
    }

    public GameObject GetHair(CharacterRaceAnimation raceId, byte hairStyle, byte hairColor, bool bh) {
        byte index = (byte)(hairStyle * hairColor + hairColor);

        if (bh) {
            index += 1;
        }

        Debug.Log($"Loading hair[{index}] Race:{raceId} Model:{hairStyle}_{hairColor}_{(bh ? "bh" : "ah")}");

        GameObject go = _hair[(byte)raceId, index];
        if (go == null) {
            Debug.LogError($"Can't find hairstyle {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{index}");
        }

        return go;
    }
}
