using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

struct L2Model {
    public GameObject baseModel;
    public Dictionary<string, Material> materials;
}

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
    private Dictionary<int, L2Model[,]> _armors; // ModelID(..).Model[Race(14),bodypart(5)]

    public void Initialize() {
        CachePlayerContainers();
        CacheFaces();
        CacheHair();
        CacheWeapons();
      //  CacheNakedArmors();
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
        //_weapons = new Dictionary<int, GameObject>();
        //int success = 0;
        //foreach (KeyValuePair<int, Weapon> kvp in ItemTable.Instance.Weapons) {
        //    string clientName = kvp.Value.Name.Replace("'", string.Empty).Replace(" ", "_").ToLower();
        //    string path = Path.Combine("Data", "Animations", "LineageWeapons", clientName + "_m00_wp", clientName + "_m00_wp_prefab");
        //    GameObject prefab = (GameObject) Resources.Load(path);
        //    if (prefab == null) {
        //        //Debug.LogWarning($"Could not load prefab for item {kvp.Value.Name}.");
        //        continue;
        //    } else {
        //        _weapons[kvp.Key] = prefab;
        //        Debug.Log($"Successfully loaded weapon {kvp.Key}: {clientName} model.");
        //    }
        //    success++;
        //}

        //Debug.Log($"Successfully loaded {success} weapon model(s).");
    }

    //private void CacheNakedArmors() {
    //    _armors = new Dictionary<int, GameObject[,]>();
    //    if (!_armors.ContainsKey(0)) {
    //        _armors.Add(0, new GameObject[RACE_COUNT, 5]);
    //    }

    //    //Load naked armors
    //    for (int r = 0; r < RACE_COUNT; r++) {
    //        CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
    //        CharacterRace race = CharacterRaceParser.ParseRace(raceId);

    //        string path = $"Data/Animations/{race}/{raceId}/{raceId}_m{0.ToString("000")}";

    //        _armors[0][(byte) raceId, 0] = LoadArmorPiece(path, raceId, ItemSlot.chest);
    //        _armors[0][(byte) raceId, 1] = LoadArmorPiece(path, raceId, ItemSlot.legs);
    //        _armors[0][(byte) raceId, 3] = LoadArmorPiece(path, raceId, ItemSlot.gloves);
    //        _armors[0][(byte) raceId, 4] = LoadArmorPiece(path, raceId, ItemSlot.feet);
    //    }
    //}

    private void CacheArmors() {
        //foreach (KeyValuePair<int, Armor> kvp in ItemTable.Instance.Armors) {
        //    int modelIndex = kvp.Value.ClientModelId;

        //    if (!_armors.ContainsKey(modelIndex)) {
        //        _armors.Add(modelIndex, new GameObject[RACE_COUNT, 5]);
        //    }

        //    ItemSlot slot = kvp.Value.ItemSlot;
        //    byte slotIndex = (byte)(slot - 2);

        //    for (int r = 0; r < RACE_COUNT; r++) {
        //        CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
        //        CharacterRace race = CharacterRaceParser.ParseRace(raceId);

        //        string path = $"Data/Animations/{race}/{raceId}/{raceId}_m{modelIndex.ToString("000")}";

        //        _armors[modelIndex][(byte)raceId, slotIndex] = LoadArmorPiece(path, raceId, slot);
        //    }
        //}
    }

    private string UpdatePath(string path, ItemSlot slot) {
            switch (slot) {
                case ItemSlot.chest:
                    return path + "_u";
                case ItemSlot.legs:
                    return path + "_l";
                case ItemSlot.fullarmor:
                    return path + "_u"; //TODO to verify
                case ItemSlot.gloves:
                    return path + "_g";
                case ItemSlot.feet:
                    return path + "_b";
                default: return path + "_u";
            }
    }

    private GameObject LoadArmorPiece(string path, CharacterRaceAnimation raceId, ItemSlot slot) {
        string modelPath = UpdatePath(path, slot);

        GameObject armorPiece = (GameObject)Resources.Load(modelPath);
        if (armorPiece == null) {
           // Debug.LogWarning($"Can't find armor model at {modelPath} for {raceId}");
        } else {
            Debug.Log($"Successfully loaded armor model at {modelPath} for {raceId}");
        }

        return armorPiece;
    }

    // -------
    // Getters
    // -------
    public GameObject GetArmorPiece(Armor armor, CharacterRaceAnimation raceId) {
        //if (armor == null) {
        //    Debug.LogWarning($"Given armor is null");
        //    return null;
        //}

        //if (!_armors.ContainsKey(armor.ClientModelId)) {
        //    Debug.LogWarning($"Can't find armor model {armor.ClientModelId} in ModelTable");
        //    return null;
        //}

        //GameObject go = _armors[armor.ClientModelId][(byte)raceId, (byte)(armor.ItemSlot - 2)];
        //if (go == null) {
        //    Debug.LogWarning($"Can't find armor model {armor.ClientModelId} for {raceId} slot {armor.ItemSlot} in ModelTable");
        //    return null;
        //}

        //return go;
        return null;
    }

    public GameObject GetArmorPieceByItemId(int itemId, CharacterRaceAnimation raceId) {
        //Armor armor = ItemTable.Instance.GetArmor(itemId);
        //if (armor == null) {
        //    Debug.LogWarning($"Can't find armor {itemId} in ItemTable");
        //    return null;
        //}

        //if (!_armors.ContainsKey(armor.ClientModelId)) {
        //    Debug.LogWarning($"Can't find armor model {armor.ClientModelId} in ModelTable");
        //    return null;
        //}

        //byte slotId = (byte)(armor.ItemSlot - 2);
        //GameObject go = _armors[armor.ClientModelId][(byte)raceId, slotId];
        //if (go == null) {
        //    Debug.LogWarning($"Can't find armor model {armor.ClientModelId} for {raceId} slot {armor.ItemSlot} - {slotId} in ModelTable");
        //    return null;
        //}

        //return go;
        return null;
    }

    public GameObject GetArmorPieceByModelId(int modelId, CharacterRaceAnimation raceId, ItemSlot slot) {
        //if (!_armors.ContainsKey(modelId)) {
        //    Debug.LogWarning($"Can't find armor model {modelId} in ModelTable");
        //    return null;
        //}

        //byte slotId = (byte)(slot - 2);
        //Debug.Log("Slot:" + slot + " " + slotId);
        //GameObject go = _armors[modelId][(byte) raceId, slotId];
        //if (go == null) {
        //    Debug.LogWarning($"Can't find armor model {modelId} for {raceId} slot {slot} - {slotId} in ModelTable");
        //    return null;
        //}

        //return go;
        return null;
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
