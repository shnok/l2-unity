public class SocialActionPacket : ServerPacket
{
    public const int LEVELUP_ACTION = 15;

    public int ObjectId { get; private set; }
    public int Action { get; private set; }

    public SocialActionPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();
        Action = ReadB();
    }
}