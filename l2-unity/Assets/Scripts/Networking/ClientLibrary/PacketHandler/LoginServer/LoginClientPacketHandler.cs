using L2_login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class LoginClientPacketHandler : ClientPacketHandler {
    public void SendPing() {
        PingPacket packet = new PingPacket();
        _client.SendPacket(packet);
    }

    public void SendAuth() {
        string account = _client.Account;
        string password = _client.Password;

        account = account.ToLower();

        byte[] accountBytes = Encoding.UTF8.GetBytes(account);

        Debug.Log($"Account bytes length: {accountBytes.Length}");
        Debug.Log($"Account bytes hex: {StringUtils.ByteArrayToString(accountBytes)}");

        byte[] shaPass = SHACrypt.ComputeSha256HashToBytes(password);
        Debug.Log($"SHA-256 hash length: {shaPass.Length}");
        Debug.Log($"SHA-256 hash hex: {StringUtils.ByteArrayToString(shaPass)}");

        // Create combined byte array with length indicators and data
        byte[] rsaBlock = new byte[accountBytes.Length + shaPass.Length + 2];

        // Copy accountBytes length into combined at index 0
        rsaBlock[0] = (byte)accountBytes.Length;

        // Copy accountBytes into combined starting at index 1
        Array.Copy(accountBytes, 0, rsaBlock, 1, accountBytes.Length);

        // Copy shaPass length into combined at index accountBytes.Length + 1
        rsaBlock[accountBytes.Length + 1] = (byte)shaPass.Length;

        // Copy shaPass into combined starting after the length indicator and accountBytes
        Array.Copy(shaPass, 0, rsaBlock, accountBytes.Length + 2, shaPass.Length);

        // Debug output for combined byte array
        Debug.Log($"Combined byte array length: {rsaBlock.Length}");
        Debug.Log($"Clear RSA block: {StringUtils.ByteArrayToString(rsaBlock)}");

        rsaBlock = _client.RSACrypt.EncryptRSANoPadding(rsaBlock);

        Debug.Log($"Encrypted RSA block: {StringUtils.ByteArrayToString(rsaBlock)}");

        AuthRequestPacket packet = new AuthRequestPacket(rsaBlock);

        SendPacket(packet);
    }

    public override void SendPacket(ClientPacket packet) {
        if (LoginClient.Instance.LogSentPackets) {
            LoginClientPacketType packetType = (LoginClientPacketType)packet.GetPacketType();
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Sending packet:" + packetType);
        }

        EncryptPacket(packet);

        _client.SendPacket(packet);
    }
}
