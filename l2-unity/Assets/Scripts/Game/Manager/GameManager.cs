using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    void Awake() {
        ItemTable.Instance.Initialize();
        ItemNameTable.Instance.Initialize();
        ItemStatDataTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        WeapongrpTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();
        NpcgrpTable.Instance.Initialize();
        NpcNameTable.Instance.Initialize();
        ModelTable.Instance.Initialize();
    }

    void Start() {
        SceneLoader.Instance.LoadDefaultScene();
    }

    void Update() {
        
    }
}
