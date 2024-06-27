using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LoginClientPacketHandler : ClientPacketHandler {
    public void SendPing() {
        PingPacket packet = new PingPacket();
        _client.SendPacket(packet);
    }

    public void SendAuth(string username) {
        AuthRequestPacket packet = new AuthRequestPacket(username);
        _client.SendPacket(packet);
    }

    public override void SendPacket(ClientPacket packet) {
        if (LoginClient.Instance.LogSentPackets) {
            LoginClientPacketType packetType = (LoginClientPacketType)packet.GetPacketType();
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Sending packet:" + packetType);
        }

        _client.SendPacket(packet);
    }
}
