using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class GameClient : DefaultClient {
    [SerializeField] private string _serverIp = "127.0.0.1";
    [SerializeField] private int _serverPort = 11000;

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
        base.OnAuthAllowed();
    }

    public override void OnDisconnect() {
        base.OnDisconnect();
    }
}
