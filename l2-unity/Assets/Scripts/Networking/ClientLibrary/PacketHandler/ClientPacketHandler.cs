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
}
