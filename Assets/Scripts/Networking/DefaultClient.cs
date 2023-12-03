using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

public class DefaultClient : MonoBehaviour {
    static AsynchronousClient client;
    public Entity currentPlayer;
    public string username;
    public int connectionTimeoutMs = 10000;

    public string serverIp = "127.0.0.1";
    public int serverPort = 11000;
    public bool logReceivedPackets = true;
    public bool logSentPackets = true;

    private static DefaultClient instance;
    public static DefaultClient GetInstance() {
        return instance;
    }

    void Awake(){                
        if (instance == null) {
            instance = this;
        } 
    }

    public async void Connect(string user) {
        username = user; 
        client = new AsynchronousClient(serverIp, serverPort);
        bool connected = await Task.Run(client.Connect);
        if(connected) {  
            ServerPacketHandler.GetInstance().SetClient(client);
            ClientPacketHandler.GetInstance().SetClient(client);         
            ClientPacketHandler.GetInstance().SendPing();
            ClientPacketHandler.GetInstance().SendAuth(user);                                   
        }
    }

    public void OnConnectionAllowed() {
        Debug.Log("Connected");
        SceneLoader.GetInstance().SwitchScene("Game");
    }

    public void OnWorldSceneLoaded() {
        ClientPacketHandler.GetInstance().SendLoadWorld();
    }

    public int GetPing() {
        return client.GetPing();
    }

    public void SendChatMessage(string message) {
        ClientPacketHandler.GetInstance().SendMessage(message);
    }
 
    public void Disconnect() {
        Debug.Log("Disconnected");
        SceneLoader.GetInstance().SwitchScene("Menu");
    }

    void OnApplicationQuit() {
        if(client != null) {
            client.Disconnect();
        }   
    }

    public void OnDisconnectReady() {
        if(client != null) {
            client.Disconnect();
            client = null;
        }

        World.GetInstance().objects.Clear();
        World.GetInstance().players.Clear();
        World.GetInstance().npcs.Clear();
        ChatWindow.GetInstance().ClearChat();     
    }
}
