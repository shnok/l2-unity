public class VersionCheckPacket : ServerPacket
{
    public byte[] BlowFishKey { get; private set; }
    public bool UseBlowfishCipher { get; private set; }
    public bool AuthAllowed { get; private set; }

    public VersionCheckPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ReadB();
        BlowFishKey = ReadB(8);
        UseBlowfishCipher = ReadI() == 1;
        AuthAllowed = ReadI() == 1;
    }
}
