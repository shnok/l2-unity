using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class AsynchronousClient
{
    private Socket _socket;
    private string _ipAddress;
    private int _port;
    private bool _connected;
    private ClientPacketHandler _clientPacketHandler;
    private ServerPacketHandler _serverPacketHandler;
    private DefaultClient _client;
    private bool _cryptEnabled = false;
    private bool _initPacket = true;
    private bool _initPacketEnabled;

    public bool InitPacket { get { return _initPacket; } set { _initPacket = value; } }
    public bool CryptEnabled
    {
        get { return _cryptEnabled; }
        set
        {
            Debug.Log("Crypt" + (value ? " enabled." : " disabled."));
            _cryptEnabled = value;
        }
    }

    public AsynchronousClient(string ip, int port, DefaultClient client, ClientPacketHandler clientPacketHandler,
        ServerPacketHandler serverPacketHandler, bool enableInitPacket)
    {
        _ipAddress = ip;
        _port = port;
        _clientPacketHandler = clientPacketHandler;
        _serverPacketHandler = serverPacketHandler;
        _clientPacketHandler.SetClient(this);
        _serverPacketHandler.SetClient(this, _clientPacketHandler);
        _client = client;
        _initPacketEnabled = enableInitPacket;
        _initPacket = enableInitPacket;
    }

    public bool Connect()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(_ipAddress);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        if (_initPacketEnabled)
        {
            _initPacket = true;
        }

        Debug.Log("Connecting...");

        IAsyncResult result = _socket.BeginConnect(ipAddress, _port, null, null);

        bool success = result.AsyncWaitHandle.WaitOne(5000, true);

        if (_socket.Connected)
        {
            Debug.Log("Connection success.");
            _socket.EndConnect(result);
            _connected = true;

            Task.Run(StartReceiving);
            return true;
        }
        else
        {
            Debug.Log("Connection failed.");
            EventProcessor.Instance.QueueEvent(() => _client.OnConnectionFailed());
            _socket.Close();
            return false;
        }
    }

    public void Disconnect()
    {
        if (!_connected)
        {
            return;
        }

        Debug.Log("Disconnect");

        ClientCleanup();

        EventProcessor.Instance.QueueEvent(() => _client.OnDisconnect());
    }

    private void ClientCleanup()
    {
        if (!_connected)
        {
            return;
        }

        try
        {
            _serverPacketHandler.CancelTokens();
            _connected = false;
            _socket.Close();
            _socket.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SendPacket(ClientPacket packet)
    {
        try
        {
            using (NetworkStream stream = new NetworkStream(_socket))
            {
                stream.WriteByte((byte)(packet.GetData().Length & 0xff));

                // Write the higher 8 bits
                stream.WriteByte((byte)((packet.GetData().Length >> 8) & 0xff));


                stream.Write(packet.GetData(), 0, (int)packet.GetData().Length);
                stream.Flush();
            }
        }
        catch (IOException e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void StartReceiving()
    {
        Debug.Log("Start receiving");

        using (NetworkStream stream = new NetworkStream(_socket))
        {
            int lengthHi;
            int lengthLo;
            int length;

            for (; ; )
            {
                if (!_connected)
                {
                    Debug.LogWarning("Disconnected.");
                    break;
                }

                lengthLo = stream.ReadByte();
                lengthHi = stream.ReadByte();
                length = (lengthHi * 256) + lengthLo;

                if (lengthHi == -1 || !_connected)
                {
                    Debug.Log("Server terminated the connection.");
                    Disconnect();
                    break;
                }

                //Debug.Log("Packet length: " + length);

                byte[] data = new byte[length];

                int receivedBytes = 0;
                int newBytes = 0;
                while ((newBytes != -1) && (receivedBytes < length))
                {
                    newBytes = stream.Read(data, 0, length);
                    receivedBytes += newBytes;
                }

                if (_serverPacketHandler.HandlePacketCrypto(data, _initPacket))
                {
                    Task.Run(() => _serverPacketHandler.HandlePacketAsync(data));
                }
            }
        }
    }
}
