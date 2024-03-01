using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    void Awake() {
        ItemTable.Instance.Initialize();
        ModelTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();

    }

    void Start() {
        SceneLoader.Instance.LoadDefaultScene();
    }

    void Update() {
        
    }
}
