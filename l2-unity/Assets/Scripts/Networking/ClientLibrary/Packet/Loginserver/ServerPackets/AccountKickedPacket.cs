public class AccountKickedPacket : LoginServerPacket
{
    public enum AccountKickedReason : byte
    {
        REASON_DATA_STEALER = 0x01,
        REASON_GENERIC_VIOLATION = 0x08,
        REASON_7_DAYS_SUSPENDED = 0x10,
        REASON_PERMANENTLY_BANNED = 0x20
    }

    private AccountKickedReason _kickedReason;
    public AccountKickedReason KickedReason { get { return _kickedReason; } }


    public AccountKickedPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        _kickedReason = (AccountKickedReason)ReadB();
    }
}