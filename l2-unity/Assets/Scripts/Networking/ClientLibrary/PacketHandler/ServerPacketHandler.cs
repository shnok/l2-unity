using L2_login;
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

    public async Task HandlePacketAsync(byte[] data, bool init) {
        await Task.Run(() => {
            if (_client.CryptEnabled) {
                data = DecryptPacket(data);

                if (init) {
                    if (!DecodeXOR(data)) {
                        Debug.LogError("Packet XOR could not be decoded.");
                        return;
                    }
                } else if(!NewCrypt.verifyChecksum(data)) {
                    Debug.LogError("Packet checksum is wrong.");
                }
            }

            HandlePacket(data);
        });
    }

    public void CancelTokens() {
        _tokenSource.Cancel();
    }

    public abstract void HandlePacket(byte[] data);

    protected abstract byte[] DecryptPacket(byte[] data);

    public bool DecodeXOR(byte[] packet) {
        if(NewCrypt.decXORPass(packet)) {
            Debug.Log("CLEAR: " + StringUtils.ByteArrayToString(packet));
            return true;
        }

        return false;
    }
}
