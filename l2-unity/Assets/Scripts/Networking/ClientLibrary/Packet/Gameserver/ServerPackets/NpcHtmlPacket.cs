public class NpcHtmlPacket : ServerPacket
{
    public int ObjectId { get; private set; }
    public string Html { get; private set; }
    public int ItemId { get; private set; }

    public NpcHtmlPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();
        Html = ReadS();
        ItemId = ReadI();
    }
}