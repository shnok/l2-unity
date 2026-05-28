public class RestartResponsePacket : ServerPacket
{
    public bool Allowed { get; private set; }
    public RestartResponsePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Allowed = ReadI() == 1;
    }
}