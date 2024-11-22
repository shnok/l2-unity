public class CreatureSayPacket : ServerPacket
{
    public int ObjectId { get; private set; }
    public L2MessageType MessageType { get; private set; }
    public string Sender { get; private set; }
    public string Text { get; private set; }
    public int SystemStringId { get; private set; }
    public int SystemMessageId { get; private set; }

    public CreatureSayPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();
        MessageType = (L2MessageType)ReadI();
        if (MessageType != L2MessageType.BOAT)
        {
            Sender = ReadS();
            Text = ReadS();
        }
        else
        {
            SystemStringId = ReadI();
            SystemMessageId = ReadI();
        }
    }
}