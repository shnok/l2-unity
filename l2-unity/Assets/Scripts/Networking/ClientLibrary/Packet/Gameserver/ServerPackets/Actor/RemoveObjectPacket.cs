public class RemoveObjectPacket : ServerPacket
{
    public int Id { get; private set; }

    public RemoveObjectPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Id = ReadI();
        ReadI(); //is seated?
    }
}