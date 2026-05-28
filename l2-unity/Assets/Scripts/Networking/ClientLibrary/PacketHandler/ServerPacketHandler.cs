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
    protected bool _checksumEnabled = false;

    public void SetClient(AsynchronousClient client, ClientPacketHandler clientPacketHandler, bool enableChecksum)
    {
        _client = client;
        _tokenSource = new CancellationTokenSource();
        _eventProcessor = EventProcessor.Instance;
        _clientPacketHandler = clientPacketHandler;
        _checksumEnabled = enableChecksum;
    }

    public bool HandlePacketCrypto(byte[] data, bool init)
    {
        if (_client.CryptEnabled)
        {
            data = DecryptPacket(data);

            if (init)
            {
                if (!DecodeXOR(data))
                {
                    Debug.LogError("Packet XOR could not be decoded.");
                    return false;
                }
            }
            else if (_checksumEnabled && !NewCrypt.verifyChecksum(data)) // checksum verification is only enabled in loginserver
            {
                Debug.LogError("Packet checksum is wrong. Ignoring packet...");
                return false;
            }
        }
        else
        {
            if (GameClient.Instance.LogCryptography)
            {
                Debug.Log("<---- [GAME] CLEAR: " + StringUtils.ByteArrayToString(data));
            }
        }

        return true;
    }

    public async Task HandlePacketAsync(byte[] data)
    {
        await Task.Run(() => HandlePacket(data));
    }

    public void CancelTokens()
    {
        _tokenSource.Cancel();
    }

    public abstract void HandlePacket(byte[] data);

    protected abstract byte[] DecryptPacket(byte[] data);

    public bool DecodeXOR(byte[] packet)
    {
        if (NewCrypt.decXORPass(packet))
        {
            Debug.Log("CLEAR: " + StringUtils.ByteArrayToString(packet));
            return true;
        }

        return false;
    }
}
