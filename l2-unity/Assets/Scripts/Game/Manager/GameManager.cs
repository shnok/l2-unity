using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.LOGIN;

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
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

    public void OnWorldSceneLoaded() {
        ClientPacketHandler.Instance.SendLoadWorld();
    }

    public void OnPlayerInfoReceived() {
        L2GameUI.Instance.StopLoading();
    }

    public void OnConnectionAllowed() {
        _gameState = GameState.CHAR_SELECT;

        CameraManager.Instance.SwitchCamera("CharSelect");

        L2LoginUI.Instance.OnLogin();
    }

    public void OnCharacterSelect() {
        _gameState = GameState.IN_GAME;

        SceneLoader.Instance.LoadGame();
    }

    public void OnRelogin() {
        _gameState = GameState.LOGIN;

        CameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.OnRelogin();
    }

    public void OnDisconnect() {
        MusicManager.Instance.Clear();
        SceneLoader.Instance.LoadMenu();
    }

    public void OnGameLaunched() {
        L2LoginUI.Instance.StopLoading();
    }
}
