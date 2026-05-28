public class LoginOkPacket : LoginServerPacket
{
    private int _sessionKey1;
    private int _sessionKey2;

    public int SessionKey1 { get { return _sessionKey1; } }
    public int SessionKey2 { get { return _sessionKey2; } }

    public LoginOkPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        _sessionKey1 = ReadI();
        _sessionKey2 = ReadI();
    }
}