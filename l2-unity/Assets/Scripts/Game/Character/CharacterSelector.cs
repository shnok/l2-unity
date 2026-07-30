using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int _selectedCharacterSlot;
    [SerializeField] private int _defaultSelectedCharacterSlot;
    [SerializeField] private CharSelectionInfoPackage _selectedCharacter;
    [SerializeField] private List<CharSelectionInfoPackage> _characters;
    [SerializeField] private LayerMask _characterMask;
    [SerializeField] private Camera _charSelectCamera;
    private GameObject _container;
    private List<Logongrp> _pawnData;
    private List<GameObject> _characterGameObjects;

    public Camera Camera { get { return _charSelectCamera; } set { _charSelectCamera = value; } }
    public int SelectedSlot { get { return _selectedCharacterSlot; } set { _selectedCharacterSlot = value; } }
    public int DefaultSelectedSlot { get { return _defaultSelectedCharacterSlot; } set { _defaultSelectedCharacterSlot = value; } }
    public List<CharSelectionInfoPackage> Characters { get { return _characters; } set { _characters = value; } }


    private static CharacterSelector _instance;
    public static CharacterSelector Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    public void ApplyCharacterList()
    {
        if (_container == null)
        {
            _container = new GameObject("Characters");
        }

        if (_characters == null)
        {
            Debug.LogWarning("Character list is empty.");
            return;
        }

        if (_characterGameObjects != null)
        {
            _characterGameObjects.ForEach((go) =>
            {
                Destroy(go);
            });
        }

        _pawnData = LogongrpTable.Instance.Logongrps;
        _characterGameObjects = new List<GameObject>();
        _selectedCharacterSlot = -1;

        for (int i = 0; i < _characters.Count; i++)
        {
            SpawnCharacterList(i);
        }
    }

    public void SpawnCharacterList(int id)
    {
        GameObject pawnObject = CharacterBuilder.Instance.BuildCharacterBase(_characters[id].CharacterRaceAnimation, _characters[id].PlayerAppearance, EntityType.Pawn);

        EntityReferenceHolder referenceHolder = pawnObject.GetComponent<EntityReferenceHolder>();
        NewHumanoidAnimationController animController = (NewHumanoidAnimationController)referenceHolder.NewAnimationController;

        // referenceHolder.Entity.Appearance = _characters[id].PlayerAppearance;
        referenceHolder.Entity.Stats = _characters[id].PlayerStats;
        referenceHolder.Entity.Status = _characters[id].PlayerStatus;
        referenceHolder.Entity.Identity.Name = _characters[id].Name;
        referenceHolder.Entity.Identity.Id = id;

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

        gear.Initialize(-1);

        referenceHolder.Entity.UpdateAppearance(_characters[id].PlayerAppearance);

        pawnObject.GetComponent<SelectableCharacterEntity>().CharacterInfo = _characters[id];

        CharacterCreator.Instance.PlacePawn(pawnObject, _pawnData[id], _characters[id].Name, _container, animController, gear);

        if (_characters[id].DeleteTimer > 0)
        {
            referenceHolder.Entity.UpdateWaitType(ChangeWaitTypePacket.WaitType.WT_SITTING);
            animController.SitWait();
        }

        _characterGameObjects.Add(pawnObject);
    }

    public void SelectDefaultCharacter()
    {
        Debug.Log("Selecting default slot " + DefaultSelectedSlot);

        // Select first character if default selected slot is not set
        if (DefaultSelectedSlot == -1 && Characters.Count > 0)
        {
            DefaultSelectedSlot = 0;
        }

        // Select do not select anything if character list is empty
        if (DefaultSelectedSlot >= 0 && Characters.Count == 0)
        {
            DefaultSelectedSlot = -1;
        }

        SelectCharacter(DefaultSelectedSlot);
    }

    public void SelectCharacter(int slot)
    {
        if (slot >= 0 && slot < _characters.Count)
        {
            if (_selectedCharacterSlot == slot)
            {
                return;
            }

            if (_selectedCharacterSlot != -1)
            {
                _characterGameObjects[_selectedCharacterSlot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[_selectedCharacterSlot]);
            }

            _characterGameObjects[slot].GetComponent<SelectableCharacterEntity>().SetDestination(_pawnData[7]);

            _selectedCharacterSlot = slot;
            _selectedCharacter = _characters[slot];

            CharSelectWindow.Instance.SelectSlot(slot);

            if (_selectedCharacter.DeleteTimer > 0)
            {
                L2ConfirmWindow.Instance.ShowWindow(1555, () =>
                {
                    GameClient.Instance.ClientPacketHandler.SendRequestRestoreCharacter(slot);
                },
                () =>
                {
                    SelectCharacter(DefaultSelectedSlot);
                });
            }
            else
            {
                L2ConfirmWindow.Instance.HideWindow(false);
            }
        }
    }

    public void ConfirmSelection()
    {
        if (SelectedSlot == -1 || _characters[SelectedSlot].DeleteTimer > 0)
        {
            Debug.LogWarning("Please select a character");
            return;
        }

        AudioManager.Instance.PlayUISound("game_start");

        GameClient.Instance.ClientPacketHandler.SendRequestSelectCharacter(SelectedSlot);
    }


    void Update()
    {
        if (_charSelectCamera == null)
        {
            return;
        }


        if (Input.GetMouseButtonDown(0) && !L2LoginUI.Instance.MouseOverUI)
        {
            Ray ray = _charSelectCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                int hitLayer = hit.collider.gameObject.layer;
                if (_characterMask == (_characterMask | (1 << hitLayer)))
                {
                    CharSelectionInfoPackage hitInfo = hit.transform.parent.parent.GetComponent<SelectableCharacterEntity>().CharacterInfo;
                    SelectCharacter(hitInfo.Slot);
                }
            }
        }
    }

    public void DeleteCharacter()
    {
        if (_selectedCharacter.DeleteTimer > 0)
        {
            return;
        }

        SystemMessageDat sm = SystemMessageTable.Instance.GetSystemMessage(4076);
        string message = sm.Message;
        string characterName = _selectedCharacter.Name;
        message = message.Replace("$s1", characterName);

        L2ConfirmWindow.Instance.ShowWindow(message, () =>
        {
            ConfirmDelete();
        }, () =>
        {

        });
    }

    private void ConfirmDelete()
    {
        if (SelectedSlot == -1)
        {
            Debug.LogWarning("Please select a character");
            return;
        }

        GameClient.Instance.ClientPacketHandler.SendRequestDeleteCharacter(SelectedSlot);
    }
}
