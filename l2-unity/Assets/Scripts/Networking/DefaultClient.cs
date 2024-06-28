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
    [SerializeField] protected string _account;
    [SerializeField] protected string _password;

    private bool _connecting = false;
    public string Account { get { return _account; } set { _account = value; } }
    public string Password { get { return _password; } set { _password = value; } }
    public bool LogReceivedPackets { get { return _logReceivedPackets; } }
    public bool LogSentPackets { get { return _logSentPackets; } }
    public int ConnectionTimeoutMs { get { return _connectionTimeoutMs; } }

    private void Start() {
        if(World.Instance != null && World.Instance.OfflineMode) {
            this.enabled = false;
        }
    }

    public async void Connect(string account, string password) {
        if(_connecting) {
            return;
        }

        _connecting = true;
        _account = account;
        _password = password;


        CreateAsyncClient();

        bool connected = await Task.Run(_client.Connect);
        if(connected) {  
            _connecting = false;

            OnConnectionSuccess();   
        }
    }

    protected abstract void CreateAsyncClient();

    protected abstract void OnConnectionSuccess();

    public virtual void OnConnectionFailed() {
        _connecting = false;
    }

    public virtual void OnAuthAllowed() {
        Debug.Log("Connected");
        GameManager.Instance.OnAuthAllowed();
    }

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
