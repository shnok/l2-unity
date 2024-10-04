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
            SpawnPawnWithId(i);
        }
    }

    public void SpawnPawnWithId(int id)
    {
        CharacterModelType raceId = CharacterModelType.FDarkElf;
        PlayerAppearance appearance = new PlayerAppearance();

        List<Logongrp> pawnData = LogongrpTable.Instance.Logongrps;

        GameObject pawnObject = CreatePawn(raceId, appearance);

        EntityReferenceHolder referenceHolder = pawnObject.GetComponent<EntityReferenceHolder>();
        HumanoidAnimationController animController = (HumanoidAnimationController)referenceHolder.AnimationController;

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

        currentPawnIndex = index;
        currentPawn = pawns[index];
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
            gear.EquipArmor(appearance.Chest, ItemSlot.chest);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
        }

        if (appearance.Legs != 0)
        {
            gear.EquipArmor(appearance.Legs, ItemSlot.legs);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
        }

        if (appearance.Gloves != 0)
        {
            gear.EquipArmor(appearance.Gloves, ItemSlot.gloves);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.gloves);
        }

        if (appearance.Feet != 0)
        {
            gear.EquipArmor(appearance.Feet, ItemSlot.feet);
        }
        else
        {
            gear.EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.feet);
        }

        if (appearance.LHand != 0)
        {
            gear.EquipWeapon(appearance.LHand, true);
        }
        if (appearance.RHand != 0)
        {
            gear.EquipWeapon(appearance.RHand, false);
        }
    }

    public void PlacePawn(GameObject pawnObject, Logongrp pawnData, string name, GameObject container, HumanoidAnimationController animController, UserGear gear)
    {
        UpdatePawnPosAndRot(pawnObject, pawnData);
        pawnObject.transform.name = name;

        pawnObject.transform.parent = container.transform;

        pawnObject.SetActive(true);

        animController.SetBool(HumanoidAnimType.wait, true);
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
