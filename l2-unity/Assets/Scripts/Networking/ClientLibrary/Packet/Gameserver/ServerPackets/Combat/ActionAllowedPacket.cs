public class ActionAllowedPacket : ServerPacket
{
    public ActionAllowedPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
    }
}
