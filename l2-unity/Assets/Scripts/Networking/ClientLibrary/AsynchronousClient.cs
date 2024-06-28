using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using L2_login;

public class AsynchronousClient {
    public static byte[] STATIC_BLOWFISH_KEY = {
        (byte) 0x6b,
        (byte) 0x60,
        (byte) 0xcb,
        (byte) 0x5b,
        (byte) 0x82,
        (byte) 0xce,
        (byte) 0x90,
        (byte) 0xb1,
        (byte) 0xcc,
        (byte) 0x2b,
        (byte) 0x6c,
        (byte) 0x55,
        (byte) 0x6c,
        (byte) 0x6c,
        (byte) 0x6c,
        (byte) 0x6c
    };

    private BlowfishEngine _blowfish;
    private Socket _socket;
    private string _ipAddress;
    private int _port;
    private bool _connected;
    private ClientPacketHandler _clientPacketHandler;
    private ServerPacketHandler _serverPacketHandler;
    private DefaultClient _client;
    private bool _initPacket = true;

    private RSACrypt _rsa;
    public RSACrypt RSACrypt { get { return _rsa; } }

    private byte[] _blowfishKey;
    public byte[] BlowfishKey { get { return _blowfishKey; } set { SetBlowFishKey(value); } }
    public string Account { get { return _client.Account; } }
    public string Password { get { return _client.Password; } }
    public int Ping { get; set; }

    public AsynchronousClient(string ip, int port, DefaultClient client, ClientPacketHandler clientPacketHandler, ServerPacketHandler serverPacketHandler) {
        SetBlowFishKey(STATIC_BLOWFISH_KEY);
        _ipAddress = ip;
        _port = port;
        _clientPacketHandler = clientPacketHandler;
        _serverPacketHandler = serverPacketHandler;
        _clientPacketHandler.SetClient(this);
        _serverPacketHandler.SetClient(this, _clientPacketHandler);
        _client = client;

    }

    public void SetBlowFishKey(byte[] blowfishKey) {
        _blowfishKey = blowfishKey;
        _blowfish = new BlowfishEngine();
        _blowfish.init(false, blowfishKey);

        Debug.Log("Blowfish key set.");
    }

    public void SetRSAKey(byte[] rsaKey) {
        _rsa = new RSACrypt(rsaKey, true);
        Debug.Log("RSA Key set.");
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
        Debug.Log("Start receiving");

        using (NetworkStream stream = new NetworkStream(_socket)) {
            int lengthHi;
            int lengthLo;
            int length;

            for (;;) {
                if(!_connected) {
                    Debug.LogWarning("Disconnected.");
                    break;
                }

                lengthLo = stream.ReadByte();
                lengthHi = stream.ReadByte();
                length = (lengthHi * 256) + lengthLo;

                if (lengthHi == -1 || !_connected) {
                    Debug.Log("Server terminated the connection.");
                    Disconnect();
                    break;
                }

                Debug.Log("lengthLo: " + lengthLo);
                Debug.Log("lengthHi: " + lengthHi);
                Debug.Log("Packet length: " + length);

                byte[] data = new byte[length];

                int receivedBytes = 0;
                int newBytes = 0;
                while ((newBytes != -1) && (receivedBytes < (length))) {
                    newBytes = stream.Read(data, 0, length);
                    receivedBytes = receivedBytes + newBytes;
                }

                
                Task.Run(() => _serverPacketHandler.HandlePacketAsync(data, _blowfish, _initPacket));        
            }
        }
    }
}
