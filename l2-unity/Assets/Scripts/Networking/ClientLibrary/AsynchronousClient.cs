using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class AsynchronousClient {
    private Socket _socket;
    private string _ipAddress;
    private string _username;
    private int _port;
    private bool _connected;
    private ClientPacketHandler _clientPacketHandler;
    private ServerPacketHandler _serverPacketHandler;
    private DefaultClient _client;

    public int Ping { get; set; }

    public AsynchronousClient(string ip, int port, DefaultClient client, ClientPacketHandler clientPacketHandler, ServerPacketHandler serverPacketHandler) {
        _ipAddress = ip;
        _port = port;
        _clientPacketHandler = clientPacketHandler;
        _serverPacketHandler = serverPacketHandler;
        _clientPacketHandler.SetClient(this);
        _serverPacketHandler.SetClient(this, _clientPacketHandler);
        _client = client;
    }

    public bool Connect() {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(_ipAddress);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Connecting...");

        IAsyncResult result = _socket.BeginConnect(ipAddress, _port, null, null);

        bool success = result.AsyncWaitHandle.WaitOne(5000, true);

        if (_socket.Connected) {
            Debug.Log("Connection success.");
            _socket.EndConnect( result );
            _connected = true;

            Task.Run(StartReceiving);
            return true;
        } else {
            Debug.Log("Connection failed.");
            EventProcessor.Instance.QueueEvent(() => _client.OnConnectionFailed());
            _socket.Close();
            return false;
        }
    }

    public void Disconnect() {
        try {
            _serverPacketHandler.CancelTokens();
            _connected = false;         
            _socket.Close();
            _socket.Dispose();

            EventProcessor.Instance.QueueEvent(() => _client.OnDisconnect());       
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    public void SendPacket(ClientPacket packet) {
        try {
            using (NetworkStream stream = new NetworkStream(_socket)) {
                stream.Write(packet.GetData(), 0, (int)packet.GetLength());
                stream.Flush();
            }
        } catch (IOException e) {
            Debug.Log(e.ToString());
        }
    }

    public void StartReceiving() {
        using (NetworkStream stream = new NetworkStream(_socket)) {
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

                Task.Run(() => _serverPacketHandler.HandlePacketAsync(packet));        
            }
        }
    }
}
