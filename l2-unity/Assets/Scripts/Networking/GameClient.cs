using UnityEngine;
using L2_login;
using static CharSelectedPacket;
using System;

public class GameClient : DefaultClient
{
    [SerializeField] protected PlayerInfo _playerInfo;
    [SerializeField] protected int _serverId;
    [SerializeField] private int _playKey1;
    [SerializeField] private int _playKey2;
    [SerializeField] private float _serverEntityPositionSyncThreshold;
    [SerializeField] private float _playerPositionSyncThreshold;

    private GameCrypt _gameCrypt;

    public PlayerInfo PlayerInfo { get { return _playerInfo; } set { _playerInfo = value; } }
    public int CurrentPlayerId { get { return _playerInfo.Identity.Id; } }
    public int ServerId { get { return _serverId; } set { _serverId = value; } }
    public int PlayKey1 { get { return _playKey1; } set { _playKey1 = value; } }
    public int PlayKey2 { get { return _playKey2; } set { _playKey2 = value; } }
    public float ServerEntityPositionSyncThreshold { get { return _serverEntityPositionSyncThreshold; } set { _serverEntityPositionSyncThreshold = value; } }
    public float PlayerPositionSyncThreshold { get { return _playerPositionSyncThreshold; } set { _playerPositionSyncThreshold = value; } }

    public GameCrypt GameCrypt { get { return _gameCrypt; } }

    private GameClientPacketHandler clientPacketHandler;
    private GameServerPacketHandler serverPacketHandler;

    public GameClientPacketHandler ClientPacketHandler { get { return clientPacketHandler; } }
    public GameServerPacketHandler ServerPacketHandler { get { return serverPacketHandler; } }

    private static GameClient _instance;
    public static GameClient Instance { get { return _instance; } }

    private void Awake()
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

    protected override void CreateAsyncClient()
    {
        clientPacketHandler = new GameClientPacketHandler();
        serverPacketHandler = new GameServerPacketHandler();

        _client = new AsynchronousClient(_serverIp, _serverPort, this, clientPacketHandler, serverPacketHandler, false, false);
    }

    public void EnableCrypt(byte[] key)
    {
        _gameCrypt = new GameCrypt();
        _gameCrypt.SetKey(key);
        _client.CryptEnabled = true;
    }

    protected override void WhileConnecting()
    {
        base.WhileConnecting();
    }

    protected override void OnConnectionSuccess()
    {
        base.OnConnectionSuccess();

        Debug.Log("Connected to GameServer");

        clientPacketHandler.SendProtocolVersion();
    }

    public override void OnConnectionFailed()
    {
        base.OnConnectionFailed();
    }

    public override void OnAuthAllowed()
    {
        Debug.Log("Authed to GameServer.");
    }

    public override void OnDisconnect()
    {
        base.OnDisconnect();

        L2ConfirmWindow.Instance.ShowWindow(127, () =>
        {
            GameManager.Instance.NotifyEvent(GameEvent.GAME_DISCONNECTED);
        }, null);

        Debug.Log("Disconnected from GameServer.");
    }
}
