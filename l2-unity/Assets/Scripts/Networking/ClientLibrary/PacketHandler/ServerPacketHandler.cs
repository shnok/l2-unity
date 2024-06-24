using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ServerPacketHandler
{
    protected AsynchronousClient _client;
    protected long _timestamp;
    protected CancellationTokenSource _tokenSource;
    protected EventProcessor _eventProcessor;
    protected ClientPacketHandler _clientPacketHandler;

    public void SetClient(AsynchronousClient client, ClientPacketHandler clientPacketHandler) {
        _client = client;
        _tokenSource = new CancellationTokenSource();
        _eventProcessor = EventProcessor.Instance;
        _clientPacketHandler = clientPacketHandler;
    }

    public async Task HandlePacketAsync(byte[] data) {
        await Task.Run(() => {
            HandlePacket(data);
        });
    }

    public void CancelTokens() {
        _tokenSource.Cancel();
    }

    public abstract void HandlePacket(byte[] data);
}
