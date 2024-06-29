using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using static ServerListPacket;

public class LoginClient : DefaultClient {

    [SerializeField] protected string _account;
    [SerializeField] protected string _password;

    public string Account { get { return _account; } set { _account = value; } }
    public string Password { get { return _password; } set { _password = value; } }


    private LoginClientPacketHandler clientPacketHandler;
    private LoginServerPacketHandler serverPacketHandler;

    public LoginClientPacketHandler ClientPacketHandler { get { return clientPacketHandler; } }
    public LoginServerPacketHandler ServerPacketHandler { get { return serverPacketHandler; } }


    private static LoginClient _instance;
    public static LoginClient Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    protected override void CreateAsyncClient() {
        clientPacketHandler = new LoginClientPacketHandler();
        serverPacketHandler = new LoginServerPacketHandler();

        _client = new AsynchronousClient(_serverIp, _serverPort, this, clientPacketHandler, serverPacketHandler);
    }

    protected override void OnConnectionSuccess() {
        GameManager.Instance.OnLoginServerConnected();
    }

    public override void OnConnectionFailed() {
        base.OnConnectionFailed();
    }

    public override void OnAuthAllowed() {
        GameManager.Instance.OnLoginServerAuthAllowed();
    }

    public void OnServerListReceived(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers) {
        GameManager.Instance.OnReceivedServerList(lastServer, serverData, charsOnServers);
    }

    public void OnServerSelected(int serverId) {
        clientPacketHandler.SendRequestServerLogin(serverId);
    }

    public override void OnDisconnect() {
        base.OnDisconnect();
    }
}
