public class RestartResponsePacket : ServerPacket
{
    public RestartResponsePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
    }
}