using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class DefaultClient : MonoBehaviour {
    [SerializeField] private static AsynchronousClient _client;
    [SerializeField] private Entity _currentPlayer;
    [SerializeField] private string _username;
    [SerializeField] private int _connectionTimeoutMs = 10000;
    [SerializeField] private string _serverIp = "127.0.0.1";
    [SerializeField] private int _serverPort = 11000;
    [SerializeField] private bool _logReceivedPackets = true;
    [SerializeField] private bool _logSentPackets = true;
    public string Username { get { return _username; } private set { _username = value; } }
    public bool LogReceivedPackets { get { return _logReceivedPackets; } private set { _logReceivedPackets = value; } }
    public bool LogSentPackets { get { return _logSentPackets; } private set { _logSentPackets = value; } }
    public int ConnectionTimeoutMs { get { return _connectionTimeoutMs; } private set { _connectionTimeoutMs = value; } }

    private static DefaultClient _instance;
    public static DefaultClient Instance { get { return _instance; } }

    private void Awake() {
        if(_instance == null) {
            _instance = this;
        }
    }

    private void Start() {
        if(World.GetInstance().offlineMode) {
            this.enabled = false;
        }
    }

    public async void Connect(string user) {
        _username = user; 
        _client = new AsynchronousClient(_serverIp, _serverPort);
        bool connected = await Task.Run(_client.Connect);
        if(connected) {  
            ServerPacketHandler.Instance.SetClient(_client);
            ClientPacketHandler.Instance.SetClient(_client);         
            ClientPacketHandler.Instance.SendPing();
            ClientPacketHandler.Instance.SendAuth(user);                                   
        }
    }

    public void OnConnectionAllowed() {
        Debug.Log("Connected");
        SceneLoader.GetInstance().SwitchScene("Game");
    }

    public void OnWorldSceneLoaded() {
        ClientPacketHandler.Instance.SendLoadWorld();
    }

    public int GetPing() {
        return _client.Ping;
    }

    public void SendChatMessage(string message) {
        ClientPacketHandler.Instance.SendMessage(message);
    }
 
    public void Disconnect() {
        Debug.Log("Disconnected");
        SceneLoader.GetInstance().SwitchScene("Menu");
    }

    void OnApplicationQuit() {
        if(_client != null) {
            _client.Disconnect();
        }   
    }

    public void OnDisconnectReady() {
        if(_client != null) {
            _client.Disconnect();
            _client = null;
        }

        World.GetInstance().objects.Clear();
        World.GetInstance().players.Clear();
        World.GetInstance().npcs.Clear();
        ChatWindow.GetInstance().ClearChat();     
    }
}
