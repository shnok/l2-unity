using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState;

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }

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

    public void LogIn() {
        DefaultClient.Instance.Connect(StringUtils.GenerateRandomString());
    }

    public void LogOut() {
        DefaultClient.Instance.Disconnect();
    }
}
