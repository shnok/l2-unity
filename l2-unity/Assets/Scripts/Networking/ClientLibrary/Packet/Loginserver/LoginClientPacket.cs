public abstract class LoginClientPacket : ClientPacket
{
    public LoginClientPacket(byte type) : base(type) { }

    public LoginClientPacket(byte[] data) : base(data)
    {
    }

    protected override void PadBuffer()
    {
        byte paddingLength = (byte)(_buffer.Count % 8);
        if (paddingLength > 0)
        {

            paddingLength = (byte)(8 - paddingLength);

            //Debug.Log($"Packet needs a padding of {paddingLength} bytes.");

            for (int i = 0; i < paddingLength; i++)
            {
                _buffer.Add((byte)0);
            }
        }
    }
}