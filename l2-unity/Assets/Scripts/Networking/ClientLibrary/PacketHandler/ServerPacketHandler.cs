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

    public async Task HandlePacketAsync(byte[] data, BlowfishEngine blowfish, bool init) {
        await Task.Run(() => {
            data = DecryptPacket(data, blowfish);

            if (init) {
                DecodeXOR(data);
            }

            HandlePacket(data);
        });
    }

    public void CancelTokens() {
        _tokenSource.Cancel();
    }

    public abstract void HandlePacket(byte[] data);

    private byte[] DecryptPacket(byte[] data, BlowfishEngine blowfish) {
        Debug.Log("ENCRYPTED: " + StringUtils.ByteArrayToString(data));

        blowfish.processBigBlock(data, 0, data, 0, data.Length);

        Debug.Log("XORED: " + StringUtils.ByteArrayToString(data));

        return data;
    }

    public bool DecodeXOR(byte[] packet) {
        int blen = packet.Length;

        if (blen < 1 || packet == null)
            return false; // TODO: Handle error or throw exception

        // Get XOR key
        int xorOffset = 8;
        uint xorKey = 0;
        xorKey |= packet[blen - xorOffset];
        xorKey |= (uint)(packet[blen - xorOffset + 1] << 8);
        xorKey |= (uint)(packet[blen - xorOffset + 2] << 16);
        xorKey |= (uint)(packet[blen - xorOffset + 3] << 24);

        // Decrypt XOR encrypted portion
        int offset = blen - xorOffset - 4;
        uint ecx = xorKey;
        uint edx = 0;

        while (offset > 2) // Adjust this condition if needed
        {
            edx = (uint)(packet[offset + 0] & 0xFF);
            edx |= (uint)(packet[offset + 1] & 0xFF) << 8;
            edx |= (uint)(packet[offset + 2] & 0xFF) << 16;
            edx |= (uint)(packet[offset + 3] & 0xFF) << 24;

            edx ^= ecx;
            ecx -= edx;

            packet[offset + 0] = (byte)((edx) & 0xFF);
            packet[offset + 1] = (byte)((edx >> 8) & 0xFF);
            packet[offset + 2] = (byte)((edx >> 16) & 0xFF);
            packet[offset + 3] = (byte)((edx >> 24) & 0xFF);

            offset -= 4;
        }

        Debug.Log("CLEAR: " + StringUtils.ByteArrayToString(packet));

        return true;
    }
}
