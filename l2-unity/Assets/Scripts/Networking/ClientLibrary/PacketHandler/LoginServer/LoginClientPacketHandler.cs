using System;
using System.Text;
using System.Threading;
using UnityEngine;

public class LoginClientPacketHandler : ClientPacketHandler
{
    protected override void EncryptPacket(ClientPacket packet)
    {
        base.EncryptPacket(packet);

        byte[] data = packet.GetData();

        LoginClient.Instance.EncryptBlowFish.processBigBlock(data, 0, data, 0, data.Length);

        if (LoginClient.Instance.LogCryptography)
        {
            Debug.Log("----> [LOGIN] ENCRYPTED: " + StringUtils.ByteArrayToString(data));
        }

        packet.SetData(data);
    }

    public void SendPing()
    {
        PingPacket packet = new PingPacket();
        SendPacket(packet);
    }

    public void SendAuth()
    {
        string account = LoginClient.Instance.Account;
        string password = LoginClient.Instance.Password;

        byte[] accountBytes = Encoding.UTF8.GetBytes(account);

        if (LoginClient.Instance.LogCryptography)
        {
            Debug.Log($"Account bytes hex [{accountBytes.Length}]: {StringUtils.ByteArrayToString(accountBytes)}");
        }

        byte[] shaPass = SHACrypt.ComputeSha256HashToBytes(password);

        if (LoginClient.Instance.LogCryptography)
        {
            Debug.Log($"SHA-256 hash hex [{shaPass.Length}]: {StringUtils.ByteArrayToString(shaPass)}");
        }

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

        if (LoginClient.Instance.LogCryptography)
        {
            // Debug output for combined byte array
            Debug.Log($"Clear RSA block [{rsaBlock.Length}]: {StringUtils.ByteArrayToString(rsaBlock)}");
        }

        rsaBlock = LoginClient.Instance.RSACrypt.EncryptRSAPskc1(rsaBlock);

        if (LoginClient.Instance.LogCryptography)
        {
            Debug.Log($"Encrypted RSA block: {StringUtils.ByteArrayToString(rsaBlock)}");
        }

        AuthRequestPacket packet = new AuthRequestPacket(rsaBlock);

        SendPacket(packet);
    }

    public void SendRequestServerList()
    {
        RequestServerListPacket packet = new RequestServerListPacket(LoginClient.Instance.SessionKey1, LoginClient.Instance.SessionKey2);

        SendPacket(packet);
    }

    public void SendRequestServerLogin(int serverId)
    {
        RequestServerLoginPacket packet = new RequestServerLoginPacket(serverId, LoginClient.Instance.SessionKey1, LoginClient.Instance.SessionKey2);

        SendPacket(packet);
    }

    public override void SendPacket(ClientPacket packet)
    {
        if (LoginClient.Instance.LogSentPackets)
        {
            LoginClientPacketType packetType = (LoginClientPacketType)packet.GetPacketType();
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [LoginServer] Sending packet:" + packetType);
        }

        if (LoginClient.Instance.LogCryptography)
        {
            Debug.Log("----> [LOGIN] CLEAR: " + StringUtils.ByteArrayToString(packet.GetData()));
        }

        EncryptPacket(packet);

        _client.SendPacket(packet);
    }
}
