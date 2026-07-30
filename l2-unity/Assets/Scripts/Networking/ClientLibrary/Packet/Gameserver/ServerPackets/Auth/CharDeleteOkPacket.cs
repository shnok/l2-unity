public class CharDeleteOkPacket : ServerPacket
{
    public CharDeleteOkPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
    }
}

