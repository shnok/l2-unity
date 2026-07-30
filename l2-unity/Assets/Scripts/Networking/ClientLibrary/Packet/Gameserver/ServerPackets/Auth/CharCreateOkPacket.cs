public class CharCreateOkPacket : ServerPacket
{
    public CharCreateOkPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ReadI();
    }
}

