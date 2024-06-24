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
        if (DefaultClient.Instance.LogReceivedPackets && packetType != LoginServerPacketType.Ping) {
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Received packet:" + packetType);
        }
        switch (packetType) {
            case LoginServerPacketType.Ping:
                OnPingReceive();
                break;
            case LoginServerPacketType.AuthResponse:
                OnAuthReceive(data);
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

            Task.Delay(DefaultClient.Instance.ConnectionTimeoutMs).ContinueWith(t => {
                if (!_tokenSource.IsCancellationRequested) {
                    long now2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if (now2 - _timestamp >= DefaultClient.Instance.ConnectionTimeoutMs) {
                        Debug.Log("Connection timed out");
                        _client.Disconnect();
                    }
                }
            }, _tokenSource.Token);
        }, _tokenSource.Token);
    }

    private void OnAuthReceive(byte[] data) {
        AuthResponsePacket packet = new AuthResponsePacket(data);
        AuthResponse response = packet.Response;

        switch (response) {
            case AuthResponse.ALLOW:
                _eventProcessor.QueueEvent(() => DefaultClient.Instance.OnConnectionAllowed());
                break;
            case AuthResponse.ALREADY_CONNECTED:
                Debug.Log("User already connected.");
                _client.Disconnect();
                break;
            case AuthResponse.INVALID_USERNAME:
                Debug.Log("Incorrect user name.");
                _client.Disconnect();
                break;
        }
    }

}
