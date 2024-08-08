using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInfoPacket;
using static ServerListPacket;

public class GameManager : MonoBehaviour {
    [SerializeField] private int _protocolVersion = 1;
    [SerializeField] private GameState _gameState = GameState.LOGIN_SCREEN;
    private bool _gameReady = false;

    public GameState GameState {
        get { return _gameState; }
        set {
            _gameState = value;
            Debug.Log($"Game state is now {_gameState}.");
        }
    }
    public bool GameReady { get { return _gameReady; } set { _gameReady = value; } }
    public int ProtocolVersion { get { return _protocolVersion; } }

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
        ActionNameTable.Instance.Initialize();
        SysStringTable.Instance.Initialize();
        SkillNameTable.Instance.Initialize();
        SkillgrpTable.Instance.Initialize();
        ModelTable.Instance.Initialize();
        LogongrpTable.Instance.Initialize();
        SystemMessageTable.Instance.Initialize();
    }

    public void LogIn() {
    }

    public void LogOut() {
        LoginClient.Instance.Disconnect();
    }

    public void OnWorldSceneLoaded() {
        GameObject.Destroy(L2LoginUI.Instance.gameObject);

        PlayerInfo playerInfo = GameClient.Instance.PlayerInfo;

        World.Instance.SpawnPlayer(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance);

        L2GameUI.Instance.StopLoading();

        GameClient.Instance.ClientPacketHandler.SendLoadWorld();
    }

    public void OnLoginServerConnected() {
        GameState = GameState.LOGIN_CONNECTED;
    }

    public void OnLoginServerAuthAllowed() {
        GameState = GameState.READING_LICENSE;

        L2LoginUI.Instance.ShowLicenseWindow();
    }

    public void OnLoginServerPlayOk() {
        GameState = GameState.READY_TO_CONNECT;
    }

    public void OnConnectingToGameServer() {
        GameState = GameState.CONNECTING_TO_GAMESERVER;
    }

    public void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers) {
        GameState = GameState.SERVER_LIST;

        L2LoginUI.Instance.ShowServerSelectWindow();

        ServerSelectWindow.Instance.UpdateServerList(lastServer, serverData, charsOnServers);
    }

    public void OnAuthAllowed() {
        GameState = GameState.CHAR_SELECT;

        LoginCameraManager.Instance.SwitchCamera("CharSelect");

        L2LoginUI.Instance.ShowCharSelectWindow();
    }

    public void OnCharacterSelect() {
        GameState = GameState.IN_GAME;

        L2LoginUI.Instance.StartLoading();
        SceneLoader.Instance.LoadGame();
    }

    public void OnCreateUser() {
        GameState = GameState.CHAR_CREATION;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowCharCreationWindow();
    }

    public void OnWorldLoading() {
        MusicManager.Instance.Clear();
        L2GameUI.Instance.StartLoading();
    }

    public void OnRelogin() {
        GameState = GameState.LOGIN_SCREEN;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowLoginWindow();
    }

    public void OnDisconnect() {
        if (GameState > GameState.CHAR_CREATION) {
            MusicManager.Instance.Clear();
            SceneLoader.Instance.LoadMenu();
        } else if(GameState > GameState.LOGIN_SCREEN && !GameClient.Instance.IsConnected && !LoginClient.Instance.IsConnected) {
            OnRelogin();
        }
    }

    public void OnGameserverSelected() {
        Debug.Log("Gameserver selected, connecting...");

        //GameClient.Instance.Connect();
    }

    public void OnStartingGame() {
        Debug.Log("On Starting game");
        //L2LoginUI.Instance.StartLoading();
    }

    public void OnGameLaunched() {
        Debug.Log("On game launched");
        L2LoginUI.Instance.StopLoading();

        CharacterCreator.Instance.SpawnAllPawns();
    }
}
