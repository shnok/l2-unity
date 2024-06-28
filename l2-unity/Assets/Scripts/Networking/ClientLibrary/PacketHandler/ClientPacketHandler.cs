using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class ClientPacketHandler
{
    protected AsynchronousClient _client;

    public void SetClient(AsynchronousClient client) {
        _client = client;
    }

    public abstract void SendPacket(ClientPacket packet);

    protected void EncryptPacket(ClientPacket packet) {
        byte[] data = packet.GetData();
        Debug.Log("CLEAR: " + StringUtils.ByteArrayToString(data));

        _client.EncryptBlowFish.processBigBlock(data, 0, data, 0, data.Length);

        Debug.Log("ENCRYPTED: " + StringUtils.ByteArrayToString(data));

        packet.SetData(data);
    }
}
