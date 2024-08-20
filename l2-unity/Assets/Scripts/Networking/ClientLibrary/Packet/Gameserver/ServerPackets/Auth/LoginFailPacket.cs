using static LoginServerFailPacket;

public class LoginFailPacket : ServerPacket
{
    private LoginFailedReason _loginFailedReason;
    public LoginFailedReason FailedReason { get { return _loginFailedReason; } }

    public LoginFailPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        _loginFailedReason = (LoginFailedReason)ReadB();
    }
}
