public class ExAutoSoulshotPacket : ServerPacket
{
    public int ItemId { get; private set; }
    public bool Enable { get; private set; }

    public ExAutoSoulshotPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ReadB();
        ReadB();
        ItemId = ReadI();
        Enable = ReadI() == 1;
    }
}
