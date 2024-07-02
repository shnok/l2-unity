using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] private int _selectedCharacterSlot;
    [SerializeField] private CharSelectionInfoPackage _selectedCharacter;
    [SerializeField] private List<CharSelectionInfoPackage> _characters;
    [SerializeField] private LayerMask _characterMask;
    [SerializeField] private Camera _charSelectCamera;
    private GameObject _container;
    private List<Logongrp> _pawnData;
    private List<GameObject> _characterGameObjects;

    public Camera Camera { get { return _charSelectCamera; } set { _charSelectCamera = value; } }
    public int SelectedSlot { get { return _selectedCharacterSlot; } }


    private static CharacterSelector _instance;
    public static CharacterSelector Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    public void SetCharacterList(List<CharSelectionInfoPackage> characters) {
        if(_container == null) {
            _container = new GameObject("Characters");
        }

        if (_characterGameObjects != null) {
            _characterGameObjects.ForEach((go) => {
                Destroy(go);
            });
        }

        _characters = characters;
        _pawnData = LogongrpTable.Instance.Logongrps;
        _characterGameObjects = new List<GameObject>();
        _selectedCharacterSlot = -1;

        for (int i = 0; i < characters.Count; i++) {
            SpawnCharacterSlot(i);
        }
    }

    public void SpawnCharacterSlot(int id) {
        GameObject pawnObject = CharacterCreator.Instance.CreatePawn(_characters[id].CharacterRaceAnimation, _characters[id].PlayerAppearance);
        pawnObject.GetComponent<SelectableCharacterEntity>().CharacterInfo = _characters[id];
        CharacterCreator.Instance.PlacePawn(pawnObject, _pawnData[id], _characters[id].Name, _container);
        _characterGameObjects.Add(pawnObject);
    }

    public void SelectCharacter(int slot) {
        if (slot >= 0 && slot < _characters.Count) {
            if (_selectedCharacterSlot == slot) {
                return;
            }

            if(_selectedCharacterSlot != -1) {
                CharacterCreator.Instance.UpdatePawnPosAndRot(_characterGameObjects[_selectedCharacterSlot], _pawnData[_selectedCharacterSlot]);
            }

            CharacterCreator.Instance.UpdatePawnPosAndRot(_characterGameObjects[slot], _pawnData[7]);

            _selectedCharacterSlot = slot;
            _selectedCharacter = _characters[slot];
        }
    }

    public void ConfirmSelection() {
        if (SelectedSlot == -1) {
            Debug.LogWarning("Please select a character");
            return;
        }

        GameClient.Instance.ClientPacketHandler.SendRequestSelectCharacter(SelectedSlot);
    }


    void Update() {
        if(_charSelectCamera == null) {
            return;
        }


        if(Input.GetMouseButtonDown(0)) {
            Ray ray = _charSelectCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f)) {
                int hitLayer = hit.collider.gameObject.layer;
                if (_characterMask == (_characterMask | (1 << hitLayer))) {
                    CharSelectionInfoPackage hitInfo = hit.transform.parent.GetComponent<SelectableCharacterEntity>().CharacterInfo;
                    SelectCharacter(hitInfo.Slot);
                }
            }
        }
    }
}
