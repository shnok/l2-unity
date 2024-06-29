using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public abstract class DefaultClient : MonoBehaviour {
    [SerializeField] protected string _serverIp = "127.0.0.1";
    [SerializeField] protected int _serverPort = 11000;
    [SerializeField] protected AsynchronousClient _client;
    [SerializeField] protected int _connectionTimeoutMs = 10000;
    [SerializeField] protected bool _logReceivedPackets = true;
    [SerializeField] protected bool _logSentPackets = true;

    private bool _connecting = false;
    public bool LogReceivedPackets { get { return _logReceivedPackets; } }
    public bool LogSentPackets { get { return _logSentPackets; } }
    public int ConnectionTimeoutMs { get { return _connectionTimeoutMs; } }
    public string ServerIp { get { return _serverIp; } set { _serverIp = value; } }
    public int ServerPort { get { return _serverPort; } set { _serverPort = value; } }

    private void Start() {
        if(World.Instance != null && World.Instance.OfflineMode) {
            this.enabled = false;
        }
    }

    public async void Connect() {
        if(_connecting) {
            return;
        }

        _connecting = true;


        CreateAsyncClient();

        bool connected = await Task.Run(_client.Connect);
        if(connected) {  
            _connecting = false;

            EventProcessor.Instance.QueueEvent(() => OnConnectionSuccess());
        }
    }

    protected abstract void CreateAsyncClient();

    protected abstract void OnConnectionSuccess();

    public virtual void OnConnectionFailed() {
        _connecting = false;
    }

    public abstract void OnAuthAllowed();

    public int GetPing() {
        return _client.Ping;
    }

    public void Disconnect() {
        if (_client != null) {
            _client.Disconnect();
        }
    }

    public virtual void OnDisconnect() {
        Debug.Log("Disconnected");
        _client = null;
        GameManager.Instance.OnDisconnect();
    }

    void OnApplicationQuit() {
        if(_client != null) {
            _client.Disconnect();
        }   
    }
}
