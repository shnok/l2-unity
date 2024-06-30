using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public abstract class DefaultClient : MonoBehaviour {
    [SerializeField] protected string _serverIp = "127.0.0.1";
    [SerializeField] protected int _serverPort = 11000;
    [SerializeField] protected AsynchronousClient _client;
    [SerializeField] protected int _connectionTimeoutMs = 10000;
    [SerializeField] protected bool _connected = false;
    [SerializeField] protected bool _logReceivedPackets = true;
    [SerializeField] protected bool _logSentPackets = true;
    [SerializeField] protected int _sessionKey1;
    [SerializeField] protected int _sessionKey2;

    private bool _connecting = false;
    public bool LogReceivedPackets { get { return _logReceivedPackets; } }
    public bool LogSentPackets { get { return _logSentPackets; } }
    public int ConnectionTimeoutMs { get { return _connectionTimeoutMs; } }
    public string ServerIp { get { return _serverIp; } set { _serverIp = value; } }
    public int ServerPort { get { return _serverPort; } set { _serverPort = value; } }
    public int SessionKey1 { get { return _sessionKey1; } set { _sessionKey1 = value; } }
    public int SessionKey2 { get { return _sessionKey2; } set { _sessionKey2 = value; } }
    public bool IsConnected { get { return _connected; } }

    private void Start() {
        if(World.Instance != null && World.Instance.OfflineMode) {
            this.enabled = false;
        }
    }

    public async void Connect() {
        _connected = false;
        if(_connecting) {
            return;
        }

        CreateAsyncClient();

        WhileConnecting();

        bool connected = await Task.Run(_client.Connect);
        if(connected) {  
            _connecting = false;

            EventProcessor.Instance.QueueEvent(() => OnConnectionSuccess());
        }
    }

    protected virtual void WhileConnecting() {
        _connecting = true;
    }

    protected abstract void CreateAsyncClient();

    protected virtual void OnConnectionSuccess() {
        _connected = true;
    }

    public virtual void OnConnectionFailed() {
        _connecting = false;
        _connected = false;
    }

    public abstract void OnAuthAllowed();

    public int GetPing() {
        return _client.Ping;
    }

    public void Disconnect() {
        _connected = false;

        if (_client != null) {
            _client.Disconnect();
        }
    }

    public virtual void OnDisconnect() {
        _client = null;
        GameManager.Instance.OnDisconnect();
    }

    void OnApplicationQuit() {
        if(_client != null) {
            _client.Disconnect();
        }   
    }
}
