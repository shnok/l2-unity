using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState = GameState.LOGIN;
    private bool _gameReady = false;

    public bool GameReady { get { return _gameReady; } set { _gameReady = value; } }

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    private void Start() {
        LoadTables();
        SceneLoader.Instance.LoadMenu(); 
    }

    private void LoadTables() {
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
        LogongrpTable.Instance.Initialize();
    }

    public void LogIn() {
        LoginClient.Instance.Connect(StringUtils.GenerateRandomString());
    }

    public void LogOut() {
        LoginClient.Instance.Disconnect();
    }

    public void OnWorldSceneLoaded() {
        GameObject.Destroy(L2LoginUI.Instance.gameObject);
        GameClient.Instance.ClientPacketHandler.SendLoadWorld();
    }

    public void OnPlayerInfoReceived() {
        L2GameUI.Instance.StopLoading();
    }

    public void OnAuthAllowed() {
        _gameState = GameState.CHAR_SELECT;

        LoginCameraManager.Instance.SwitchCamera("CharSelect");

        L2LoginUI.Instance.ShowCharSelectWindow();
    }

    public void OnCharacterSelect() {
        _gameState = GameState.IN_GAME;

        L2LoginUI.Instance.StartLoading();
        SceneLoader.Instance.LoadGame();
    }

    public void OnCreateUser() {
        _gameState = GameState.CHAR_CREATION;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowCharCreationWindow();
    }

    public void OnWorldLoading() {
        MusicManager.Instance.Clear();
        L2GameUI.Instance.StartLoading();
    }

    public void OnRelogin() {
        _gameState = GameState.LOGIN;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowLoginWindow();
    }

    public void OnDisconnect() {
        MusicManager.Instance.Clear();
        SceneLoader.Instance.LoadMenu();
    }

    public void OnStartingGame() {
        Debug.Log("On Starting game");
        L2LoginUI.Instance.StartLoading();
    }

    public void OnGameLaunched() {
        Debug.Log("On game launched");
        L2LoginUI.Instance.StopLoading();

        CharacterCreator.Instance.SpawnAllPawns();
    }
}
