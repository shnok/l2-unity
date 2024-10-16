using System;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInfoPacket;
using static ServerListPacket;

public class GameManager : MonoBehaviour
{
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    // TODO: SWITCH TO EVENT HANDLER AND STATE MACHINE
    [SerializeField] private int _protocolVersion = 1;
    [SerializeField] private GameState _gameState = GameState.LOGIN_SCREEN;
    private bool _gameReady = false;
    [SerializeField] private bool _autoLogin = false;
    [SerializeField] private Camera _loadingCamera;

    public bool AutoLogin { get { return _autoLogin; } }

    public GameState GameState
    {
        get { return _gameState; }
        set
        {
            _gameState = value;
            Debug.Log($"Game state is now {_gameState}.");
        }
    }

    public bool GameReady { get { return _gameReady; } set { _gameReady = value; } }
    public int ProtocolVersion { get { return _protocolVersion; } }

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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

        GameObject camObject = GameObject.Find("LoadingCamera");
        if (camObject == null)
        {
            Debug.LogError("Can't find loading camera");
            return;
        }

        camObject.TryGetComponent(out _loadingCamera);

        StartLoading();
    }

    private void Start()
    {
        LoadTables();
        SceneLoader.Instance.LoadMenu();
    }

    private void LoadTables()
    {
        SkillSoundgrpTable.Instance.Initialize();
        SkillTable.Instance.Initialize();
        ItemTable.Instance.Initialize();
        ItemNameTable.Instance.Initialize();
        ItemStatDataTable.Instance.Initialize();
        ArmorgrpTable.Instance.Initialize();
        EtcItemgrpTable.Instance.Initialize();
        WeapongrpTable.Instance.Initialize();
        NpcgrpTable.Instance.Initialize();
        NpcNameTable.Instance.Initialize();
        ActionNameTable.Instance.Initialize();
        SysStringTable.Instance.Initialize();
        SkillNameTable.Instance.Initialize();
        SkillgrpTable.Instance.Initialize();
        LogongrpTable.Instance.Initialize();
        SystemMessageTable.Instance.Initialize();
        ChargrpTable.Instance.Initialize();

        // Caching
        ItemTable.Instance.CacheItems();
        SkillTable.Instance.CacheSkills();
        ModelTable.Instance.Initialize();
        SkillEffectTable.Instance.Initialize();
        ParticleEffectTable.Instance.Initialize();
        IconTable.Instance.Initialize();
        KeyImageTable.Instance.Initialize();

        // Memory cleanup
        ArmorgrpTable.Instance.ClearTable();
        EtcItemgrpTable.Instance.ClearTable();
        ItemNameTable.Instance.ClearTable();
        SkillgrpTable.Instance.ClearTable();
        SkillNameTable.Instance.ClearTable();
    }

    public void OnWorldSceneLoaded()
    {
        GameObject.Destroy(L2LoginUI.Instance.gameObject);

        PlayerInfo playerInfo = GameClient.Instance.PlayerInfo;

        World.Instance.SpawnPlayer(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance, playerInfo.Running);

        PlayerStateMachine.Instance.enabled = true;

        StopLoading();

        GameClient.Instance.ClientPacketHandler.SendLoadWorld();
    }

    public void OnLoginServerConnected()
    {
        GameState = GameState.LOGIN_CONNECTED;
    }

    public void OnLoginServerAuthAllowed()
    {
        GameState = GameState.READING_LICENSE;

        L2LoginUI.Instance.ShowLicenseWindow();
    }

    public void OnLoginServerPlayOk()
    {
        GameState = GameState.READY_TO_CONNECT;
    }

    public void OnConnectingToGameServer()
    {
        GameState = GameState.CONNECTING_TO_GAMESERVER;
    }

    public void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers)
    {
        GameState = GameState.SERVER_LIST;

        L2LoginUI.Instance.ShowServerSelectWindow();

        ServerSelectWindow.Instance.UpdateServerList(lastServer, serverData, charsOnServers);
    }

    public void OnAuthAllowed()
    {
        GameState = GameState.CHAR_SELECT;

        SwitchToCharSelect();

        L2LoginUI.Instance.ShowCharSelectWindow();
    }

    public void OnCharacterSelect()
    {
        GameState = GameState.IN_GAME;

        StartLoading();
        SceneLoader.Instance.LoadGame();
    }

    public void OnCreateUser()
    {
        GameState = GameState.CHAR_CREATION;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowCharCreationWindow();
    }

    public void OnWorldLoading()
    {
        MusicManager.Instance.Clear();
        StartLoading();
    }

    public void OnRelogin()
    {
        GameState = GameState.LOGIN_SCREEN;

        LoginCameraManager.Instance.SwitchCamera("Login");

        L2LoginUI.Instance.ShowLoginWindow();
    }

    public void OnDisconnect()
    {
        if (GameState > GameState.CHAR_CREATION)
        {
            MusicManager.Instance.Clear();
            SceneLoader.Instance.LoadMenu();
        }
        else if (GameState > GameState.LOGIN_SCREEN && !GameClient.Instance.IsConnected && !LoginClient.Instance.IsConnected)
        {
            OnRelogin();
        }
    }

    public void OnGameserverSelected()
    {
        Debug.Log("Gameserver selected, connecting...");

        //GameClient.Instance.Connect();
    }

    public void OnStartingGame()
    {
        Debug.Log("On Starting game");
    }

    public void OnGameLaunched()
    {
        Debug.Log("On game launched");

        if (GameState == GameState.RESTARTING)
        {
            GameState = GameState.CHAR_SELECT;
            L2LoginUI.Instance.ShowCharSelectWindow();
        }

        StopLoading();
        CharacterCreator.Instance.SpawnAllPawns();
    }

    internal void OnCharSelectAllowed()
    {
        if (GameState > GameState.IN_GAME)
        {
            MusicManager.Instance.Clear();
            SceneLoader.Instance.LoadMenu();
            StartLoading();
        }
    }

    public void OnLoginCamerasInitialized()
    {
        if (GameState == GameState.CHAR_SELECT)
        {
            SwitchToCharSelect();
        }
        else
        {
            LoginCameraManager.Instance.SwitchCamera("Login");
        }
    }

    public void SwitchToCharSelect()
    {
        CharSelectWindow.Instance.SetCharacterList(CharacterSelector.Instance.Characters);

        CharacterSelector.Instance.ApplyCharacterList();
        CharacterSelector.Instance.SelectDefaultCharacter();

        CharSelectWindow.Instance.SelectSlot(CharacterSelector.Instance.SelectedSlot);

        LoginCameraManager.Instance.SwitchCamera("CharSelect");
    }

    public void OnLoginUILoaded()
    {
        if (GameState == GameState.RESTARTING)
        {
            LoginWindow.Instance.HideWindow();
        }
        else
        {
            LoginWindow.Instance.ShowLogo();
        }
    }

    public void StartLoading()
    {
        _loadingCamera.enabled = true;
        if (L2GameUI.Instance != null)
        {
            L2GameUI.Instance.StartLoading();
        }
        if (L2LoginUI.Instance != null)
        {
            L2LoginUI.Instance.StartLoading();
        }
    }

    public void StopLoading()
    {
        _loadingCamera.enabled = false;
        if (L2GameUI.Instance != null)
        {
            L2GameUI.Instance.StopLoading();
        }
        if (L2LoginUI.Instance != null)
        {
            L2LoginUI.Instance.StopLoading();
        }
    }
}
