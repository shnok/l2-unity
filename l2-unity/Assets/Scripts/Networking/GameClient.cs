using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class GameClient : DefaultClient {
    [SerializeField] protected Entity _currentPlayer;

    public string CurrentPlayer { get { return _currentPlayer.Identity.Name; } }
    public int SessionKey1 { get { return _client.SessionKey1; } set { _client.SessionKey1 = value; } }
    public int SessionKey2 { get { return _client.SessionKey2; } set { _client.SessionKey2 = value; } }

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

        _client = new AsynchronousClient(_serverIp, _serverPort, this, clientPacketHandler, serverPacketHandler);
    }

    protected override void OnConnectionSuccess() {
        clientPacketHandler.SendPing();
        clientPacketHandler.SendAuth("1234");   
    }

    public override void OnConnectionFailed() {
        base.OnConnectionFailed();
    }

    public override void OnAuthAllowed() {
        Debug.Log("Connected");
        GameManager.Instance.OnAuthAllowed();
    }

    public override void OnDisconnect() {
        base.OnDisconnect();
    }
}
