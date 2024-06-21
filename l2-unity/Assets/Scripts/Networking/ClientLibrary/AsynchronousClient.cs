using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class AsynchronousClient {
    private Socket _client;
    private string _ipAddress;
    private string _username;
    private int _port;
    private bool _connected;
    public int Ping { get; set; }

    public AsynchronousClient(string ip, int port) {
        _ipAddress = ip;
        _port = port;
    }
    public bool Connect() {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(_ipAddress);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Connecting...");

        IAsyncResult result = _client.BeginConnect(ipAddress, _port, null, null);

        bool success = result.AsyncWaitHandle.WaitOne(5000, true);

        if (_client.Connected) {
            Debug.Log("Connection success.");
            _client.EndConnect( result );
            _connected = true;

            Task.Run(StartReceiving);
            return true;
        } else {
            Debug.Log("Connection failed.");
            EventProcessor.Instance.QueueEvent(() => DefaultClient.Instance.OnConnectionFailed());
            _client.Close();
            return false;
        }
    }

    public void Disconnect() {
        try {
            ServerPacketHandler.Instance.CancelTokens();
            _connected = false;         
            _client.Close();
            _client.Dispose();

            EventProcessor.Instance.QueueEvent(() => DefaultClient.Instance.OnDisconnect());       
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    public void SendPacket(ClientPacket packet) {
        if(DefaultClient.Instance.LogSentPackets) {
            ClientPacketType packetType = (ClientPacketType)packet.GetPacketType();
            if(packetType != ClientPacketType.Ping && packetType != ClientPacketType.RequestRotate) {
                Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] Sending packet:" + packetType);
            }
        }
        try {
            using (NetworkStream stream = new NetworkStream(_client)) {
                stream.Write(packet.GetData(), 0, (int)packet.GetLength());
                stream.Flush();
            }
        } catch (IOException e) {
            Debug.Log(e.ToString());
        }
    }

    public void StartReceiving() {
        using (NetworkStream stream = new NetworkStream(_client)) {
            for(;;) {
                if(!_connected) {
                    Debug.LogWarning("Disconnected.");
                    break;
                }
                int packetType = stream.ReadByte();
                if (packetType == -1 || !_connected) {
                    Debug.Log("Server terminated the connection.");
                    Disconnect();
                    break;
                }

                int packetLength = stream.ReadByte();
                byte[] packet = new byte[packetLength];
                packet[0] = (byte)packetType;
                packet[1] = (byte)packetLength;
               
                int received = 0;
                int readCount = 0;
               
                while ((readCount != -1) && (received < packet.Length - 2)) {
                    readCount = stream.Read(packet, 2, packet.Length - 2);
                    received += readCount;
                }

                Task.Run(() => ServerPacketHandler.Instance.HandlePacketAsync(packet));        
            }
        }
    }
}
