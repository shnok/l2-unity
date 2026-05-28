using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _enableLogs;
    [SerializeField] private int _protocolVersion = 740;
    [SerializeField] private bool _autoLogin = false;
    [SerializeField] private Camera _loadingCamera;
    [SerializeField] private bool _loading;
    public bool Loading { get { return _loading; } }
    public int ProtocolVersion { get { return _protocolVersion; } }
    public bool AutoLogin { get { return _autoLogin; } }

    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private GameState _currentState;
    public GameState State { get { return _currentState; } }
    public bool LogsEnabled { get { return _enableLogs; } }

    private GameStateBase _stateInstance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        InitializeState();

        InitializeLoading();

        ChangeState(GameState.STARTING_GAME);
    }

    private void InitializeLoading()
    {
        GameObject camObject = GameObject.Find("LoadingCamera");

        if (camObject == null)
        {
            Debug.LogError("Can't find loading camera");
            return;
        }

        camObject.TryGetComponent(out _loadingCamera);
    }

    public void StartLoading()
    {
        _loading = true;

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
        StartCoroutine(StopLoadingCoroutine());
    }

    public IEnumerator StopLoadingCoroutine()
    {
        Debug.Log("Stop Loading!");
        yield return new WaitForSeconds(0.15f);

        _loading = false;

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

    public void LoadTables()
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
        SkillbarImageTable.Instance.Initialize();

        // Memory cleanup
        ArmorgrpTable.Instance.ClearTable();
        EtcItemgrpTable.Instance.ClearTable();
        ItemNameTable.Instance.ClearTable();
        // SkillgrpTable.Instance.ClearTable();
        // SkillNameTable.Instance.ClearTable();
    }

    private void Update()
    {
        _stateInstance?.Update();
    }

    public void ChangeState(GameState newState)
    {
        ChangeState(newState, null);
    }

    public void ChangeState(GameState newState, object arg0)
    {
        if (_enableLogs) Debug.Log("[GameStateMachine][STATE] " + newState);
        _stateInstance?.Exit();
        _currentState = newState;
        InitializeState();
        _stateInstance?.Enter(arg0);
    }

    private void InitializeState()
    {
        _stateInstance = _currentState switch
        {
            GameState.STARTING_GAME => new StartingGameState(this),
            GameState.LOGIN_SCREEN => new LoginState(this),
            GameState.LOGIN_CONNECTED => new LoginConnectedState(this),
            GameState.LOGIN_AUTHED => new LoginAuthedState(this),
            GameState.CHAR_SELECT => new CharSelectionState(this),
            GameState.CHAR_CREATION => new CharCreationState(this),
            GameState.ENTERING_WORLD => new EnteringWorldState(this),
            GameState.IN_GAME => new InGameState(this),
            GameState.RESTARTING => new RestartingState(this),
            GameState.DISONNECTING => new DisconnectingState(this),
            GameState.TELEPORTING => new TeleportingState(this),
            _ => throw new ArgumentException("Invalid state")
        };
    }

    public void NotifyEvent(GameEvent evt)
    {
        NotifyEvent(evt, null);
    }

    public void NotifyEvent(GameEvent evt, object arg0)
    {
        if (_enableLogs) Debug.Log("[GameStateMachine][EVENT] " + evt);
        _stateInstance?.HandleEvent(evt, arg0);
    }
}