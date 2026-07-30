using UnityEngine;
class InitPacket : LoginServerPacket
{
    private int sessionId;
    private int rsaKeyLength;
    private byte[] publicKey;
    private int blowfishKeyLength;
    private byte[] blowfishKey;

    public int SessionId { get { return sessionId; } }
    public int RSAKeyLength { get { return rsaKeyLength; } }
    public byte[] PublicKey { get { return publicKey; } }
    public byte[] BlowfishKey { get { return blowfishKey; } }

    public InitPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        sessionId = ReadI();
        rsaKeyLength = ReadI();
        publicKey = ReadB(rsaKeyLength);
        blowfishKeyLength = ReadI();
        blowfishKey = ReadB(blowfishKeyLength);

        Debug.Log($"sessionId [{sessionId}]");
        Debug.Log($"rsaKeyLength [{rsaKeyLength}]");
        Debug.Log($"publicKey [{StringUtils.ByteArrayToString(publicKey)}]");
        Debug.Log($"blowfishKeyLength [{blowfishKeyLength}]");
        Debug.Log($"blowfishKey [{StringUtils.ByteArrayToString(blowfishKey)}]");
    }
}