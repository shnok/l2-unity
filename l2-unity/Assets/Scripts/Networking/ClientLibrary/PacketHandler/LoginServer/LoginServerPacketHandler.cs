using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class LoginServerPacketHandler : ServerPacketHandler
{
    public override void HandlePacket(byte[] data) {
        LoginServerPacketType packetType = (LoginServerPacketType)data[0];
        if (LoginClient.Instance.LogReceivedPackets && packetType != LoginServerPacketType.Ping) {
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Received packet:" + packetType);
        }

        switch (packetType) {
            case LoginServerPacketType.Ping:
                OnPingReceive();
                break;
            case LoginServerPacketType.Init:
                OnInitReceive(data);
                break;        
        }
    }

    private void OnPingReceive() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int ping = _timestamp != 0 ? (int)(now - _timestamp) : 0;
        //Debug.Log("Ping: " + ping + "ms");
        _client.Ping = ping;

        Task.Delay(1000).ContinueWith(t => {
            if (!_tokenSource.IsCancellationRequested) {
                ((LoginClientPacketHandler)_clientPacketHandler).SendPing();
                _timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            Task.Delay(LoginClient.Instance.ConnectionTimeoutMs).ContinueWith(t => {
                if (!_tokenSource.IsCancellationRequested) {
                    long now2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if (now2 - _timestamp >= LoginClient.Instance.ConnectionTimeoutMs) {
                        Debug.Log("Connection timed out");
                        _client.Disconnect();
                    }
                }
            }, _tokenSource.Token);
        }, _tokenSource.Token);
    }

    private void OnInitReceive(byte[] data) {
        InitPacket packet = new InitPacket(data);
        



    }

}
