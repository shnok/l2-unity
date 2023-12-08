using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

public class AsynchronousClient {
    private Socket client;
    private bool connected;
    private String _ipAddress;
    private int _port;
    private int _ping;
    private string _username;

    public int ping {get; set;}

    public AsynchronousClient(String ip, int port) {
        _ipAddress = ip;
        _port = port;
    }

    public bool Connect() {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(_ipAddress);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Debug.Log("Connecting...");

        IAsyncResult result = client.BeginConnect(ipAddress, _port, null, null);

        bool success = result.AsyncWaitHandle.WaitOne(5000, true);

        if (client.Connected) {
            Debug.Log("Connection success.");
            client.EndConnect( result );
            connected = true;

            Task.Run(StartReceiving);
            return true;
        } else {
            Debug.Log("Connection failed.");
            client.Close();
            return false;
        }
    }

    public void Disconnect() {
        //Debug.Log((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        try {
            ServerPacketHandler.GetInstance().CancelTokens();

            connected = false;         
            client.Close();
            client.Dispose();           
        } catch (Exception e) {
            Debug.Log(e.ToString());
        }
    }

    public void SendPacket(ClientPacket packet) {
        if(DefaultClient.GetInstance().logSentPackets) {
            ClientPacketType packetType = (ClientPacketType)packet.GetPacketType();
            if(packetType != ClientPacketType.Ping) {
                Debug.Log("Sending packet:" + packetType);
            }
        }
        try {
            using (NetworkStream stream = new NetworkStream(client)) {
                stream.Write(packet.GetData(), 0, (int)packet.GetLength());
                stream.Flush();
            }
        } catch (IOException e) {
            Debug.Log(e.ToString());
        }
    }

    public void StartReceiving() {
        using (NetworkStream stream = new NetworkStream(client)) {
            for(;;) {
                if(!connected) {
                    Debug.LogWarning("Disconnected.");
                    break;
                }
                int packetType = stream.ReadByte();
                if (packetType == -1 || !connected) {
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

                ServerPacketHandler.GetInstance().HandlePacket(packet);
            }
        }
    }

    public void SetPing(int ping) {
        _ping = ping;
    }

    public int GetPing() {
        return _ping;
    }
}
