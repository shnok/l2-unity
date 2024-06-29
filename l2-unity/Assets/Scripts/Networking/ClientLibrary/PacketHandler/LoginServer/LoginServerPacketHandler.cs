using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            case LoginServerPacketType.LoginFail:
                OnLoginFail(data);
                break;
            case LoginServerPacketType.AccountKicked:
                OnAccountKicked(data);
                break;
            case LoginServerPacketType.LoginOk:
                OnLoginOk(data);
                break;
            case LoginServerPacketType.ServerList:
                OnServerListReceived(data);
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
        Debug.Log("On init receive");
        InitPacket packet = new InitPacket(data);

        byte[] rsaKey = packet.PublicKey;
        byte[] blowfishKey = packet.BlowfishKey;

        _client.SetRSAKey(rsaKey);
        _client.SetBlowFishKey(blowfishKey);

        _client.InitPacket = false;

        ((LoginClientPacketHandler)_clientPacketHandler).SendAuth();
    }

    private void OnLoginFail(byte[] data) {
        LoginFailPacket packet = new LoginFailPacket(data);

        LoginFailPacket.LoginFailedReason failedReason = packet.FailedReason;

        Debug.LogWarning($"Login failed reason: {Enum.GetName(typeof(LoginFailPacket.LoginFailedReason), failedReason)}");

        _client.Disconnect();
    }

    private void OnAccountKicked(byte[] data) {
        AccountKickedPacket packet = new AccountKickedPacket(data);

        AccountKickedPacket.AccountKickedReason kickedReason = packet.KickedReason;

        Debug.LogWarning($"Account kicked reason: {Enum.GetName(typeof(AccountKickedPacket.AccountKickedReason), kickedReason)}");

        _client.Disconnect();
    }

    private void OnLoginOk(byte[] data) {
        LoginOkPacket packet = new LoginOkPacket(data);

        _client.SessionKey1 = packet.SessionKey1;
        _client.SessionKey2 = packet.SessionKey2;

        EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.OnAuthAllowed());
    }

    private void OnServerListReceived(byte[] data) {
        ServerListPacket packet = new ServerListPacket(data);

        EventProcessor.Instance.QueueEvent(
            () => LoginClient.Instance.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers));
    }
}
