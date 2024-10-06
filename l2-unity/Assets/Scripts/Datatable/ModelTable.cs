using System.Collections.Generic;
using UnityEngine;

public class ModelTable
{
    private static ModelTable _instance;
    public static ModelTable Instance
    {
        get
        {
            if (_instance == null)
            {
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
    private GameObject[] _pawnContainers;
    private Material[,] _faceMaterials;
    private GameObject[] _faceModels;
    private Material[,,] _hairMaterials;
    private GameObject[,] _hairModels;
    private Dictionary<string, GameObject> _weapons;
    private Dictionary<string, GameObject> _npcs;
    private Dictionary<string, L2Armor> _armors;
    private class L2Armor
    {
        public GameObject baseModel;
        public Dictionary<string, Material> materials;
    }

    public class L2ArmorPiece
    {
        public GameObject baseArmorModel;
        public Material material;

        public L2ArmorPiece(GameObject baseArmorModel, Material material)
        {
            this.baseArmorModel = baseArmorModel;
            this.material = material;
        }
    }

    public void Initialize()
    {
        CachePlayerContainers();
        CacheFaces();
        CacheHair();
        CacheWeapons();
        CacheArmors();
        CacheNpcs();
    }

    // private void OnDestroy() {
    //     _faces = null;
    //     _hair = null;
    //     _instance = null;
    //     _playerContainers = null;
    //     _userContainers = null;
    //     _weapons.Clear();
    //     _armors.Clear();
    //     _instance = null;
    // }

    // -------
    // CACHE
    // -------
    private void CachePlayerContainers()
    {
        _playerContainers = new GameObject[RACE_COUNT];
        _userContainers = new GameObject[RACE_COUNT];
        _pawnContainers = new GameObject[RACE_COUNT];

        // Player Containers
        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterModelType raceId = (CharacterModelType)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            string path = $"Data/Animations/{race}/{raceId}/Player_{raceId}";
            _playerContainers[r] = Resources.Load<GameObject>(path);
            //   Debug.Log($"Loading player container {r} [{path}]");
        }

        // User Containers
        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterModelType raceId = (CharacterModelType)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            string path = $"Data/Animations/{race}/{raceId}/User_{raceId}";
            _userContainers[r] = Resources.Load<GameObject>(path);
            //  Debug.Log($"Loading user container {r} [{path}]");
        }

        // Pawn Containers
        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterModelType raceId = (CharacterModelType)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            string path = $"Data/Animations/{race}/{raceId}/Pawn_{raceId}";
            _pawnContainers[r] = Resources.Load<GameObject>(path);
            //  Debug.Log($"Loading user container {r} [{path}]");
        }
    }

    private void CacheFaces()
    {
        _faceModels = new GameObject[RACE_COUNT]; // there is 14 races, each race has 1 face model
        _faceMaterials = new Material[RACE_COUNT, FACE_COUNT]; // there is 14 races, each race has 6 face materials

        // Faces
        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterModelType raceId = (CharacterModelType)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            if (ChargrpTable.Instance.CharGrps.TryGetValue(raceId, out Chargrp chargrp))
            {
                GameObject faceModel = LoadFaceModel(chargrp.FaceMesh);
                if (faceModel == null)
                {
                    continue;
                }

                _faceModels[r] = faceModel;

                for (int i = 0; i < chargrp.FaceTextures.Length; i++)
                {
                    _faceMaterials[r, i] = LoadFaceMaterial(chargrp.FaceTextures[i]);
                }
            }
        }
    }


    private GameObject LoadFaceModel(string model)
    {
        string[] folderFile = model.Split(".");

        if (folderFile.Length < 2) return null;

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";

        GameObject face = (GameObject)Resources.Load(modelPath);
        if (face == null)
        {
            // Debug.LogWarning($"Can't find face model at {modelPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded face model at {modelPath}");
        }

        return face;
    }

    private Material LoadFaceMaterial(string texture)
    {
        string[] folderFile = texture.Split(".");

        if (folderFile.Length < 3) return null;

        string materialPath = $"Data/SysTextures/LineageFaceTex/Materials/{folderFile[2]}";

        Material material = (Material)Resources.Load(materialPath);
        if (material == null)
        {
            // Debug.LogWarning($"Can't find face material at {materialPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded face material at {materialPath}");
        }

        return material;
    }


    private void CacheHair()
    {
        _hairModels = new GameObject[RACE_COUNT, HAIR_STYLE_COUNT * 2]; // there is 14 races, each race has 6 hairstyle (2 models each) of 4 colors
        _hairMaterials = new Material[RACE_COUNT, HAIR_STYLE_COUNT * 2, HAIR_COLOR_COUNT]; // there is 14 races, each race has 6 hairstyle (2 models each) of 4 colors

        // Hair
        for (int r = 0; r < RACE_COUNT; r++)
        {
            CharacterModelType raceId = (CharacterModelType)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);


            if (ChargrpTable.Instance.CharGrps.TryGetValue(raceId, out Chargrp chargrp))
            {
                for (int hs = 0; hs < HAIR_STYLE_COUNT; hs++)
                {
                    GameObject ahModel = LoadHairModel(chargrp.HairStyles[hs].AhModel);
                    GameObject bhModel = LoadHairModel(chargrp.HairStyles[hs].BhModel);

                    if (ahModel == null && bhModel == null)
                    {
                        continue;
                    }

                    _hairModels[r, hs * 2] = ahModel;
                    _hairModels[r, hs * 2 + 1] = bhModel;

                    for (int hc = 0; hc < HAIR_COLOR_COUNT; hc++)
                    {
                        _hairMaterials[r, hs * 2, hc] = LoadHairMaterial(chargrp.HairStyles[hs].AhTextures[hc]);
                        _hairMaterials[r, hs * 2 + 1, hc] = LoadHairMaterial(chargrp.HairStyles[hs].BhTextures[hc]);
                    }
                }
            }

        }
    }

    private GameObject LoadHairModel(string model)
    {
        if (model == null)
        {
            return null;
        }

        string[] folderFile = model.Split(".");

        if (folderFile.Length < 2) return null;

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";

        GameObject face = (GameObject)Resources.Load(modelPath);
        if (face == null)
        {
            // Debug.LogWarning($"Can't find hair model at {modelPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded hair model at {modelPath}");
        }

        return face;
    }

    private Material LoadHairMaterial(string texture)
    {
        if (texture == null)
        {
            return null;
        }

        string[] folderFile = texture.Split(".");

        if (folderFile.Length < 2) return null;

        string materialPath = $"Data/SysTextures/{folderFile[0]}/Materials/{folderFile[1]}";

        Material material = (Material)Resources.Load(materialPath);
        if (material == null)
        {
            // Debug.LogWarning($"Can't find hair material at {materialPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded hair material at {materialPath}");
        }

        return material;
    }

    private void CacheWeapons()
    {
        _weapons = new Dictionary<string, GameObject>();
        int success = 0;
        foreach (KeyValuePair<int, Weapon> kvp in ItemTable.Instance.Weapons)
        {
            if (_weapons.ContainsKey(kvp.Value.Weapongrp.Model))
            {
                continue;
            }

            GameObject weapon = LoadWeaponModel(kvp.Value.Weapongrp.Model);
            if (weapon != null)
            {
                success++;
                _weapons[kvp.Value.Weapongrp.Model] = weapon;
            }
        }

        Debug.Log($"Successfully loaded {success}/{ItemTable.Instance.Weapons.Count} weapon model(s).");
    }

    private GameObject LoadWeaponModel(string model)
    {
        string[] folderFile = model.Split(".");

        if (folderFile.Length < 2) return null;

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";

        GameObject weapon = (GameObject)Resources.Load(modelPath);
        if (weapon == null)
        {
            //Debug.LogWarning($"Can't find weapon model at {modelPath}");
        }
        else
        {
            Debug.Log($"Successfully loaded weapon {model} model.");
        }

        return weapon;
    }

    private void CacheArmors()
    {
        _armors = new Dictionary<string, L2Armor>();

        int armorMaterials = 0;
        foreach (KeyValuePair<int, Armor> kvp in ItemTable.Instance.Armors)
        {
            for (int i = 0; i < RACE_COUNT; i++)
            {
                string model = kvp.Value.Armorgrp.Model[i];
                if (model == null)
                {
                    Debug.LogWarning($"Model string is null for race {(CharacterModelType)i} in armor {kvp.Key}");
                    continue;
                }

                L2Armor l2Model = null;
                if (!_armors.ContainsKey(model))
                {
                    l2Model = new L2Armor();
                    l2Model.baseModel = LoadArmorModel(model);
                    if (l2Model.baseModel != null)
                    {
                        l2Model.materials = new Dictionary<string, Material>();
                        _armors.Add(model, l2Model);
                    }
                }

                if (l2Model == null)
                {
                    _armors.TryGetValue(model, out l2Model);
                }

                if (l2Model == null || l2Model.baseModel == null)
                {
                    //Debug.LogWarning($"Armor {kvp.Key} model cannot be loaded for race {(CharacterRaceAnimation)i} at {model}");
                    continue;
                }

                string texture = kvp.Value.Armorgrp.Texture[i];

                if (l2Model.materials.ContainsKey(texture))
                {
                    continue;
                }

                Material armorMaterial = LoadArmorMaterial(texture);

                if (armorMaterial == null)
                {
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

    private GameObject LoadArmorModel(string model)
    {

        string[] folderFile = model.Split(".");

        if (folderFile.Length < 2) return null;

        string modelPath = $"Data/Animations/{folderFile[0]}/{folderFile[1]}";

        GameObject armorPiece = (GameObject)Resources.Load(modelPath);
        if (armorPiece == null)
        {
            //Debug.LogWarning($"Can't find armor model at {modelPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded armor model at {modelPath}");
        }

        return armorPiece;
    }

    private Material LoadArmorMaterial(string texture)
    {
        string[] folderFile = texture.Split(".");

        if (folderFile.Length < 2) return null;

        string materialPath = $"Data/SysTextures/{folderFile[0]}/Materials/{folderFile[1]}";

        Material material = (Material)Resources.Load(materialPath);
        if (material == null)
        {
            // Debug.LogWarning($"Can't find armor model at {materialPath}");
        }
        else
        {
            // Debug.Log($"Successfully loaded armor model at {materialPath}");
        }

        return material;
    }

    private void CacheNpcs()
    {
        _npcs = new Dictionary<string, GameObject>();
        int success = 0;
        foreach (KeyValuePair<int, Npcgrp> kvp in NpcgrpTable.Instance.Npcgrps)
        {
            if (_npcs.ContainsKey(kvp.Value.Mesh))
            {
                continue;
            }

            GameObject npc = LoadNpc(kvp.Value.Mesh);
            if (npc != null)
            {
                success++;
                _npcs[kvp.Value.Mesh] = npc;
            }
        }

        Debug.Log($"Loaded {success} npc model(s).");
    }

    private GameObject LoadNpc(string meshname)
    {
        string[] folderFile = meshname.Split(".");

        if (folderFile.Length < 2)
        {
            return null;
        }

        string path = $"Data/Animations/{folderFile[0]}/{folderFile[1]}/{folderFile[1]}";

        return Resources.Load<GameObject>(path);
    }

    // -------
    // Getters
    // -------
    public L2ArmorPiece GetArmorPiece(Armor armor, CharacterModelType raceId)
    {
        if (armor == null)
        {
            Debug.LogWarning($"Given armor is null");
            return null;
        }

        string model = armor.Armorgrp.Model[(byte)raceId];
        if (!_armors.ContainsKey(model))
        {
            Debug.LogWarning($"Can't find armor model {model} in ModelTable");
            return null;
        }

        GameObject baseModel = _armors[model].baseModel;
        if (baseModel == null)
        {
            Debug.LogWarning($"Can't find armor model {model} for {raceId} in ModelTable");
            return null;
        }

        if (_armors[model].materials == null)
        {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        if (armor.Armorgrp.Texture == null || armor.Armorgrp.Texture.Length < RACE_COUNT || armor.Armorgrp.Texture[(byte)raceId] == null)
        {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        Material material;
        _armors[model].materials.TryGetValue(armor.Armorgrp.Texture[(byte)raceId], out material);

        if (material == null)
        {
            Debug.LogWarning($"Can't find armor material for {model} and {raceId} in ModelTable");
            return null;
        }

        L2ArmorPiece armorModel = new L2ArmorPiece(baseModel, material);
        return armorModel;
    }

    public L2ArmorPiece GetArmorPieceByItemId(int itemId, CharacterModelType raceId)
    {
        Armor armor = ItemTable.Instance.GetArmor(itemId);

        return GetArmorPiece(armor, raceId);
    }

    public GameObject GetWeaponById(int itemId)
    {
        Weapon weapon = ItemTable.Instance.GetWeapon(itemId);
        if (weapon == null)
        {
            Debug.LogWarning($"Can't find weapon {itemId} in ItemTable");
        }

        return GetWeapon(weapon.Weapongrp.Model);
    }

    public GameObject GetWeapon(string model)
    {
        if (!_weapons.ContainsKey(model))
        {
            Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        GameObject go = _weapons[model];
        if (go == null)
        {
            Debug.LogWarning($"Can't find weapon model {model} in ModelTable");
            return null;
        }

        return go;
    }

    public GameObject GetContainer(CharacterModelType raceId, EntityType entityType)
    {
        GameObject go = null;

        switch (entityType)
        {
            case EntityType.User:
                go = _userContainers[(byte)raceId];
                break;
            case EntityType.Player:
                go = _playerContainers[(byte)raceId];
                break;
            case EntityType.Pawn:
                go = _pawnContainers[(byte)raceId];
                break;
        }

        if (go == null)
        {
            Debug.LogError($"Can't find container for race {raceId} and entity type {entityType}");
        }

        return go;
    }

    public GameObject GetFace(CharacterModelType raceId, byte face)
    {
        face = (byte)Mathf.Min(face, FACE_COUNT - 1);

        Debug.Log($"Loading Face[{face}] Race:{raceId}");
        GameObject prefab = _faceModels[(byte)raceId];
        if (prefab == null)
        {
            Debug.LogError($"Can't find face model for race {raceId} at index {raceId},{face}");
        }

        Material material = _faceMaterials[(byte)raceId, face];
        if (material == null)
        {
            Debug.LogError($"Can't find face material {face} for race {raceId} at index {raceId},{face}");
        }

        GameObject go = GameObject.Instantiate(prefab);
        go.GetComponentInChildren<SkinnedMeshRenderer>().material = material;

        return go;
    }

    public GameObject GetHair(CharacterModelType raceId, byte hairStyle, byte hairColor, bool bh)
    {
        hairStyle = (byte)Mathf.Min(hairStyle, HAIR_STYLE_COUNT - 1);
        hairColor = (byte)Mathf.Min(hairColor, HAIR_COLOR_COUNT - 1);

        hairStyle = (byte)(hairStyle * 2);

        if (bh)
        {
            hairStyle += 1;
        }

        // Debug.Log($"Loading hair[{hairStyle}:{hairColor}] Race:{raceId} Model:{hairStyle}_{hairColor}_{(bh ? "bh" : "ah")}");

        GameObject prefab = _hairModels[(byte)raceId, hairStyle];
        if (prefab == null)
        {
            Debug.LogError($"Can't find hairstyle model {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{hairStyle}");
        }

        Material material = _hairMaterials[(byte)raceId, hairStyle, hairColor];

        if (material == null)
        {
            Debug.LogError($"Can't find hairstyle material {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{hairColor}");
        }

        GameObject go = GameObject.Instantiate(prefab);
        go.GetComponentInChildren<SkinnedMeshRenderer>().material = material;

        return go;
    }

    public GameObject GetNpc(string meshname)
    {
        GameObject npc = null;
        _npcs.TryGetValue(meshname, out npc);
        if (npc == null)
        {
            Debug.LogError($"Can't find npc {meshname} model in ModelTable.");
        }

        return npc;
    }
}
