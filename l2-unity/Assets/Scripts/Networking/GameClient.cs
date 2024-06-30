using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class GameClient : DefaultClient {
    [SerializeField] protected Entity _currentPlayer;
    [SerializeField] protected int _serverId;
    [SerializeField] private int _playKey1;
    [SerializeField] private int _playKey2;

    public string CurrentPlayer { get { return _currentPlayer.Identity.Name; } }
    public int ServerId { get { return _serverId; } set { _serverId = value; } }
    public int PlayKey1 { get { return _playKey1; } set { _playKey1 = value; } }
    public int PlayKey2 { get { return _playKey2; } set { _playKey2 = value; } }


    private GameClientPacketHandler clientPacketHandler;
    private GameServerPacketHandler serverPacketHandler;

    public GameClientPacketHandler ClientPacketHandler { get { return clientPacketHandler; } }
    public GameServerPacketHandler ServerPacketHandler { get { return serverPacketHandler; } }


    private static GameClient _instance;
    public static GameClient Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    protected override void CreateAsyncClient() {
        clientPacketHandler = new GameClientPacketHandler();
        serverPacketHandler = new GameServerPacketHandler();

        _client = new AsynchronousClient(_serverIp, _serverPort, this, clientPacketHandler, serverPacketHandler, false);
    }

    protected override void WhileConnecting() {
        base.WhileConnecting();

        GameManager.Instance.OnConnectingToGameServer();
    }

    protected override void OnConnectionSuccess() {
        base.OnConnectionSuccess();

        Debug.Log("Connected to GameServer");

        clientPacketHandler.SendProtocolVersion();
    }

    public override void OnConnectionFailed() {
        base.OnConnectionFailed();
    }

    public override void OnAuthAllowed() {
        Debug.Log("Authed to GameServer");

        GameManager.Instance.OnAuthAllowed();
    }

    public override void OnDisconnect() {
        base.OnDisconnect();

        Debug.Log("Disconnected from GameServer.");
    }
}
