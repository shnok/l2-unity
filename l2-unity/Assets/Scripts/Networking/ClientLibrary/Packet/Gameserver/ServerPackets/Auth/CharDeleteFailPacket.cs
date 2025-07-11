public class CharDeleteFailPacket : ServerPacket
{
    public enum CharDeleteFailReason : int
    {
        REASON_DELETION_FAILED = 0x01,
        REASON_YOU_MAY_NOT_DELETE_CLAN_MEMBER = 0x02,
        REASON_CLAN_LEADERS_MAY_NOT_BE_DELETED = 0x03,
    }

    public int Reason { get; private set; }
    public CharDeleteFailPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Reason = ReadI();
    }
}
