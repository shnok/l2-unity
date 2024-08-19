public class KeyPacket : ServerPacket {
    public byte[] BlowFishKey { get; private set; }
    public bool AuthAllowed { get; private set; }

    public KeyPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        byte blowFishKeyLength = ReadB();
        BlowFishKey = ReadB(blowFishKeyLength);
        AuthAllowed = ReadB() == 1;
    }
}
