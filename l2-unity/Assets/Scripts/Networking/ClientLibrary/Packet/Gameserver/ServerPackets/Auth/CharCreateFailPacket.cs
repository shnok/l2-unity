public class CharCreateFailPacket : ServerPacket
{
    public enum CreateFailReason
    {
        REASON_CREATION_FAILED = 0x00,
        REASON_TOO_MANY_CHARACTERS = 0x01,
        REASON_NAME_ALREADY_EXISTS = 0x02,
        REASON_INCORRECT_NAME = 0x04
    }

    public int Reason { get; private set; }
    public CharCreateFailPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Reason = ReadI();
    }
}
