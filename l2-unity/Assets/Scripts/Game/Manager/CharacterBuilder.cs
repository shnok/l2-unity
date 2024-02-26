using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    private static CharacterBuilder _instance;
    public static CharacterBuilder Instance { get { return _instance; } }

    private static int RACE_COUNT = 14;
    private static int FACE_COUNT = 6;
    private static int HAIR_STYLE_COUNT = 6;
    private static int HAIR_COLOR_COUNT = 4;

    private GameObject[] _playerContainers;
    private GameObject[] _userContainers;
    private GameObject[,] _faces;
    private GameObject[,] _hair;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

        CacheGameObjects();
    }

    private void OnDestroy() {
        _faces = null;
        _hair = null;
        _instance = null;
        _playerContainers = null;
        _userContainers = null;
    }

    private void CacheGameObjects() {
        _playerContainers = new GameObject[RACE_COUNT];
        _userContainers = new GameObject[RACE_COUNT];
        _faces = new GameObject[RACE_COUNT, FACE_COUNT]; // there is 14 races, each race has 6 faces
        _hair = new GameObject[RACE_COUNT, HAIR_STYLE_COUNT * HAIR_COLOR_COUNT * 2]; // there is 14 races, each race has 6 hairstyle (2 models each) of 4 colors

        string path;

        // Faces
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);
            for (int f = 0; f < FACE_COUNT; f++) {
                path = $"Data/Animations/{race}/{raceId}/Faces/{raceId}_f_{f}";
                _faces[r,f] = Resources.Load<GameObject>(path); 
                Debug.Log($"Loading face {f} for race {raceId} [{path}]");
            }
        }

        // Hair
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);
            for (int hs = 0; hs < HAIR_STYLE_COUNT; hs++) {
                for (int hc = 0; hc < HAIR_COLOR_COUNT; hc++) {
                    path = $"Data/Animations/{race}/{raceId}/Hair/{raceId}_h_{hs}_{hc}_ah";
                    _hair[r, hs * hc + hc] = Resources.Load<GameObject>(path);
                    Debug.Log($"Loading hair {hs} color {hc} at {hs * hc + hc} for race {raceId} [{path}]");

                    path = $"Data/Animations/{race}/{raceId}/Hair/{raceId}_h_{hs}_{hc}_bh";
                    _hair[r, hs * hc + hc + 1] = Resources.Load<GameObject>(path);
                    Debug.Log($"Loading hair {hs} color {hc} at {hs * hc + hc + 1} for race {raceId} [{path}]");
                }
            }
        }

        // Player Containers
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation) r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            path = $"Data/Animations/{race}/{raceId}/Player_{raceId}";
            _playerContainers[r] = Resources.Load<GameObject>(path);
            Debug.Log($"Loading player container {r} [{path}]");   
        }

        // User Containers
        for (int r = 0; r < RACE_COUNT; r++) {
            CharacterRaceAnimation raceId = (CharacterRaceAnimation)r;
            CharacterRace race = CharacterRaceParser.ParseRace(raceId);

            path = $"Data/Animations/{race}/{raceId}/User_{raceId}";
            _userContainers[r] = Resources.Load<GameObject>(path);
            Debug.Log($"Loading user container {r} [{path}]");
        }
    }

    // Load player animations, face and hair
    public GameObject BuildCharacterBase(PlayerAppearance appearance, bool isMage, bool isPlayer) {
        CharacterRace race = (CharacterRace) appearance.Race;
        CharacterRaceAnimation raceId = CharacterRaceAnimationParser.ParseRace(race, appearance.Sex, isMage);

        Debug.Log($"Building character: {race} {raceId} {race} {appearance.Sex} {appearance.Face} {appearance.HairStyle} {appearance.HairColor}");

        GameObject entity = Instantiate(GetContainer(raceId, isPlayer));
        GameObject face = Instantiate(GetFace(raceId, appearance.Face));
        GameObject hair1 = Instantiate(GetHair(raceId, appearance.HairStyle, appearance.HairColor, false));
        GameObject hair2 = Instantiate(GetHair(raceId, appearance.HairStyle, appearance.HairColor, true));

        Transform container = entity.transform;
        if (!isPlayer) {
            container = entity.transform.GetChild(0);
        }

        face.transform.SetParent(container.transform, false);
        hair1.transform.SetParent(container.transform, false);
        hair2.transform.SetParent(container.transform, false);

        return entity;
    }

    private GameObject GetContainer(CharacterRaceAnimation raceId, bool isPlayer) {
        GameObject go = isPlayer ? _playerContainers[(byte)raceId] : _userContainers[(byte)raceId];
        if (go == null) {
            Debug.LogError($"Can't find container for race {raceId} isPlayer? {isPlayer}");
        }

        return go;
    }

    private GameObject GetFace(CharacterRaceAnimation raceId, byte face) {
        GameObject go = _faces[(byte)raceId, face];
        if (go == null) {
            Debug.LogError($"Can't find face {face} for race {raceId} at index {raceId},{face}");
        }

        return go;
    }

    private GameObject GetHair(CharacterRaceAnimation raceId, byte hairStyle, byte hairColor, bool bh) {
        byte index = (byte) (hairStyle * hairColor + hairColor);

        if(bh) {
            index += 1;
        }

        GameObject go = _hair[(byte)raceId, index];
        if (go == null) {
            Debug.LogError($"Can't find hairstyle {hairStyle} haircolor {hairColor} for race {raceId} at index {raceId},{index}");
        }

        return go;
    }
}
