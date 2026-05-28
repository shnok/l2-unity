using System.Collections.Generic;
using UnityEngine;

public class ServerListPacket : LoginServerPacket
{
    private byte _serverCount;
    private byte _lastServer;
    private List<ServerData> _serverData;
    private Dictionary<int, int> _charsOnServers;

    public byte LastServer { get { return _lastServer; } }
    public List<ServerData> ServersData { get { return _serverData; } }
    public Dictionary<int, int> CharsOnServers { get { return _charsOnServers; } }

    public class ServerData
    {
        public byte[] ip;
        public int port;
        public int currentPlayers;
        public int maxPlayers;
        public int status;
        public int serverId;

        public ServerData()
        {
            ip = new byte[4];
        }
    }

    public ServerListPacket(byte[] d) : base(d)
    {
        _serverData = new List<ServerData>();
        _charsOnServers = new Dictionary<int, int>();

        Parse();
    }

    public override void Parse()
    {
        _serverCount = ReadB();
        _lastServer = ReadB();

        for (int i = 0; i < _serverCount; i++)
        {
            ServerData serverData = new ServerData();
            serverData.serverId = ReadB();
            serverData.ip[0] = ReadB();
            serverData.ip[1] = ReadB();
            serverData.ip[2] = ReadB();
            serverData.ip[3] = ReadB();

            serverData.port = ReadI();
            serverData.currentPlayers = ReadI();
            serverData.maxPlayers = ReadI();
            serverData.status = ReadB();

            Debug.Log($"Server data received: Id:{serverData.serverId} Ip:{StringUtils.ByteArrayToString(serverData.ip)} Port:{serverData.port} OnlineCount:{serverData.currentPlayers} MaxPlayers:{serverData.maxPlayers} Status:{serverData.status}");
            _serverData.Add(serverData);
        }

        byte charsOnServerCount = ReadB();
        if (charsOnServerCount > 0)
        {
            for (int i = 0; i < charsOnServerCount; i++)
            {
                byte serverId = ReadB();
                byte charCount = ReadB();

                Debug.Log($"Account as {charCount} character(s) on server {serverId}");
                _charsOnServers[serverId] = charCount;
            }
        }

    }
}
