public class RevivePacket : ServerPacket
{
    public int EntityId { get; private set; }

    public RevivePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        EntityId = ReadI();
    }
}