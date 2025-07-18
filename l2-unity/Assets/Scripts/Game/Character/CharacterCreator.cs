using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] pawns = new GameObject[26];
    [SerializeField] private int currentPawnIndex = -1;
    [SerializeField] private GameObject currentPawn = null;

    private bool _pawnRotating = false;
    private bool _pawnRotatingRight = true;

    private GameObject _pawnContainer;
    public int PawnIndex { get { return currentPawnIndex; } }

    private static CharacterCreator _instance;
    public static CharacterCreator Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentPawn == null)
        {
            _pawnRotating = false;
            return;
        }

        if (_pawnRotating)
        {
            if (_pawnRotatingRight)
            {
                currentPawn.transform.eulerAngles = new Vector3(0, currentPawn.transform.eulerAngles.y + Time.deltaTime * 69f, 0);
            }
            else
            {
                currentPawn.transform.eulerAngles = new Vector3(0, currentPawn.transform.eulerAngles.y - Time.deltaTime * 69f, 0);
            }
        }
    }

    public void SpawnAllPawns()
    {
        List<Logongrp> pawnData = LogongrpTable.Instance.Logongrps;

        _pawnContainer = new GameObject("Pawns");

        for (var i = 8; i < pawnData.Count; i++)
        {
            SpawnCharacterCreationPawn(i);
        }
    }

    public void SpawnCharacterCreationPawn(int id)
    {
        PlayerAppearance appearance = new PlayerAppearance();

        List<Logongrp> pawnData = LogongrpTable.Instance.Logongrps;

        CharacterModelType raceId = GetCharacterTypeFromPawnId(id);

        if (raceId == CharacterModelType.MOrc || raceId == CharacterModelType.FOrc || raceId == CharacterModelType.MShaman || raceId == CharacterModelType.FShaman)
        {
            Debug.LogWarning($"Race {raceId} is not yet added to the game.");
            return;
        }

        GameObject pawnObject = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, EntityType.Pawn);
        pawns[id] = pawnObject;

        EntityReferenceHolder referenceHolder = pawnObject.GetComponent<EntityReferenceHolder>();
        NewHumanoidAnimationController animController = (NewHumanoidAnimationController)referenceHolder.NewAnimationController;

        if (animController == null)
        {
            Debug.LogError("Pawn object animation controller is null");
        }

        UserGear gear = (UserGear)referenceHolder.Gear;
        if (gear == null)
        {
            Debug.LogError("Pawn object UserGear is null");
        }

        gear.Initialize(-1);
        referenceHolder.Entity.UpdateAppearance(appearance);
        // referenceHolder.Entity.EquipAllArmors();
        // referenceHolder.Entity.EquipAllWeapons();
        // GearUpPawn(appearance, gear);

        PlacePawn(pawnObject, pawnData[id], "Pawn" + id, _pawnContainer, animController, gear);
    }

    public void SelectPawn(int raceIndex, int classIndex, int genderIndex)
    {
        int index = GetPawnIndex(raceIndex, classIndex, genderIndex);
        currentPawnIndex = index;
        currentPawn = pawns[index];
    }

    public int GetPawnIndex(int raceIndex, int classIndex, int genderIndex)
    {
        int index = 0;
        switch (raceIndex)
        {
            case 0: //"Human":
                index = 8;
                break;
            case 1: //"Elf":
                index = 12;
                break;
            case 2: //"Dark Elf":
                index = 16;
                break;
            case 3:// "Orc":
                index = 20;
                break;
            case 4: //"Dwarf":
                index = 24;
                break;
        }

        if (classIndex == 1)
        {
            index += 2;
        }

        if (genderIndex == 1)
        {
            index += 1;
        }

        return index;
    }

    private CharacterModelType GetCharacterTypeFromPawnId(int id)
    {
        if (id >= 24)
        {
            // Dwarf
            return (id % 2 == 0) ? CharacterModelType.MDwarf : CharacterModelType.FDwarf;
        }

        if (id >= 20 && id < 24)
        {
            // Orc
            if (id % 4 == 0) return CharacterModelType.MOrc;
            if (id % 4 == 1) return CharacterModelType.FOrc;
            if (id % 4 == 2) return CharacterModelType.MShaman;
            return CharacterModelType.FShaman;
        }

        if (id >= 16 && id < 20)
        {
            // Dark Elf
            return (id % 2 == 0) ? CharacterModelType.MDarkElf : CharacterModelType.FDarkElf;
        }

        if (id >= 12 && id < 16)
        {
            // Elf
            return (id % 2 == 0) ? CharacterModelType.MElf : CharacterModelType.FElf;
        }

        if (id >= 8 && id < 12)
        {
            // Human
            if (id % 4 == 0) return CharacterModelType.MFighter;
            if (id % 4 == 1) return CharacterModelType.FFighter;
            if (id % 4 == 2) return CharacterModelType.MMagic;
            return CharacterModelType.FMagic;
        }

        return CharacterModelType.FDwarf;
    }

    public void ResetPawnSelection()
    {
        if (currentPawn != null)
        {
            // Restore pawn appearance and rotation
            Destroy(currentPawn);

            SpawnCharacterCreationPawn(currentPawnIndex);
        }

        currentPawn = null;
        currentPawnIndex = -1;
    }

    public void PlacePawn(GameObject pawnObject, Logongrp pawnData, string name, GameObject container, NewHumanoidAnimationController animController, UserGear gear)
    {
        UpdatePawnPosAndRot(pawnObject, pawnData);
        pawnObject.transform.name = name;

        pawnObject.transform.parent = container.transform;

        pawnObject.SetActive(true);

        animController.Initialize();
        animController.Wait();
        animController.SetWalkSpeed(1.9f);
    }

    public void UpdatePawnPosAndRot(GameObject pawnObject, Logongrp pawnData)
    {
        Vector3 pawnPosition = new Vector3(pawnData.X, pawnData.Y, pawnData.Z);
        pawnPosition = VectorUtils.ConvertPosToUnity(pawnPosition);
        pawnObject.transform.position = pawnPosition;
        pawnObject.transform.eulerAngles = new Vector3(0, 360.00f * pawnData.Yaw / 65536, 0);
    }

    public void RotatePawn(bool right)
    {
        _pawnRotating = true;
        _pawnRotatingRight = right;
    }

    public void StopRotatingPawn()
    {
        _pawnRotating = false;
    }

    public void ChangeCharacterFace(int faceIndex)
    {
        PlayerAppearance newAppearance = CopyAppearance();
        newAppearance.Face = (byte)faceIndex;

        if (newAppearance != null)
        {
            currentPawn.GetComponent<Entity>().UpdateAppearance(newAppearance);
        }
    }

    public void ChangeCharacterHairStyle(int hairStyleIndex)
    {
        PlayerAppearance newAppearance = CopyAppearance();
        newAppearance.HairStyle = (byte)hairStyleIndex;

        if (newAppearance != null)
        {
            currentPawn.GetComponent<Entity>().UpdateAppearance(newAppearance);
        }
    }

    public void ChangeCharacterHairColor(int hairColorIndex)
    {
        PlayerAppearance newAppearance = CopyAppearance();
        newAppearance.HairColor = (byte)hairColorIndex;

        if (newAppearance != null)
        {
            currentPawn.GetComponent<Entity>().UpdateAppearance(newAppearance);
        }
    }

    private PlayerAppearance CopyAppearance()
    {
        if (currentPawnIndex == -1 || currentPawn == null)
        {
            Debug.LogWarning("Current pawn is null.");
            return null;
        }

        Entity entity = currentPawn.GetComponent<Entity>();

        PlayerAppearance oldAppearance = (PlayerAppearance)entity.Appearance;

        PlayerAppearance newAppearance = new PlayerAppearance();
        newAppearance.UpdateAppearance(oldAppearance);

        return newAppearance;
    }

    public void ValidateCharacterCreation(string characterName, bool isMage)
    {
        if (currentPawnIndex == -1 || currentPawn == null)
        {
            Debug.LogWarning("Current pawn is null.");
            return;
        }

        Entity entity = currentPawn.GetComponent<Entity>();

        GameClient.Instance.ClientPacketHandler.SendRequestCreateCharacter(
            characterName,
            CharacterRaceParser.ParseRaceBase(entity.RaceId),
            CharacterSexParser.ParseSex(entity.RaceId),
            CharacterClassParser.ParseClass(entity.RaceId, isMage),
            ((PlayerAppearance)entity.Appearance).HairStyle,
            ((PlayerAppearance)entity.Appearance).HairColor,
            ((PlayerAppearance)entity.Appearance).Face);
    }
}
