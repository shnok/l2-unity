using System.Collections.Generic;
using UnityEngine;

public class PawnCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] pawns = new GameObject[26];
    [SerializeField] private int currentPawnIndex = -1;
    [SerializeField] private GameObject currentPawn = null;

    private bool _pawnRotating = false;
    private bool _pawnRotatingRight = true;

    private GameObject _pawnContainer;
    public int PawnIndex { get { return currentPawnIndex; } }

    private static PawnCreator _instance;
    public static PawnCreator Instance { get { return _instance; } }

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
            SpawnPawnWithId(i);
        }
    }

    public void SpawnPawnWithId(int id)
    {
        PlayerAppearance appearance = new PlayerAppearance();

        List<Logongrp> pawnData = LogongrpTable.Instance.Logongrps;

        CharacterModelType raceId = GetCharacterTypeFromPawnId(id);

        if (raceId != CharacterModelType.FDwarf &&
        raceId != CharacterModelType.MDwarf &&
        raceId != CharacterModelType.FDarkElf &&
        raceId != CharacterModelType.MDarkElf &&
        raceId != CharacterModelType.FElf &&
        raceId != CharacterModelType.MElf)
        {
            Debug.LogWarning($"Race {raceId} is not yet added to the game.");
            raceId = CharacterModelType.FDwarf;
        }

        GameObject pawnObject = CreatePawn(raceId, appearance);

        EntityReferenceHolder referenceHolder = pawnObject.GetComponent<EntityReferenceHolder>();
        NewHumanoidAnimationController animController = (NewHumanoidAnimationController)referenceHolder.NewAnimationController;

        if (animController == null)
        {
            Debug.LogError("Pawn object animation controller is null");
        }

        animController.Initialize();

        UserGear gear = (UserGear)referenceHolder.Gear;
        if (gear == null)
        {
            Debug.LogError("Pawn object UserGear is null");
        }

        gear.Initialize(-1, raceId);

        GearUpPawn(appearance, gear);

        PlacePawn(pawnObject, pawnData[id], "Pawn" + id, _pawnContainer, animController, gear);
    }

    public void SelectPawn(string race, string pawnClass, string gender)
    {
        int index = GetPawnIndex(race, pawnClass, gender);
        currentPawnIndex = index;
        currentPawn = pawns[index];
    }

    public int GetPawnIndex(string race, string pawnClass, string gender)
    {
        int index = 0;
        switch (race)
        {
            case "Human":
                index = 8;
                break;
            case "Elf":
                index = 12;
                break;
            case "Dark Elf":
                index = 16;
                break;
            case "Orc":
                index = 20;
                break;
            case "Dwarf":
                index = 24;
                break;
        }

        if (pawnClass == "Mystic")
        {
            index += 2;
        }

        if (gender == "Female")
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

            SpawnPawnWithId(currentPawnIndex);
        }

        currentPawn = null;
        currentPawnIndex = -1;
    }

    public GameObject CreatePawn(CharacterModelType raceId, PlayerAppearance appearance)
    {
        GameObject pawnObject = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, EntityType.Pawn);

        return pawnObject;
    }

    public void GearUpPawn(PlayerAppearance appearance, UserGear gear)
    {
        if (appearance.Chest != 0)
        {
            gear.EquipArmor(appearance.Chest, ItemSlot.SLOT_CHEST);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.SLOT_CHEST);
        }

        if (appearance.Legs != 0)
        {
            gear.EquipArmor(appearance.Legs, ItemSlot.SLOT_LEGS);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.SLOT_LEGS);
        }

        if (appearance.Gloves != 0)
        {
            gear.EquipArmor(appearance.Gloves, ItemSlot.SLOT_GLOVES);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.SLOT_GLOVES);
        }

        if (appearance.Feet != 0)
        {
            gear.EquipArmor(appearance.Feet, ItemSlot.SLOT_FEET);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.SLOT_FEET);
        }

        gear.EquipAllWeapons(appearance);
    }

    public void PlacePawn(GameObject pawnObject, Logongrp pawnData, string name, GameObject container, NewHumanoidAnimationController animController, UserGear gear)
    {
        UpdatePawnPosAndRot(pawnObject, pawnData);
        pawnObject.transform.name = name;

        pawnObject.transform.parent = container.transform;

        pawnObject.SetActive(true);

        animController.Wait();
        animController.SetWalkSpeed(2.5f);
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
}
