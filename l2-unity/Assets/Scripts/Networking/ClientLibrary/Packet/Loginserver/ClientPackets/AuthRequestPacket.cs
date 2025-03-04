public class AuthRequestPacket : LoginClientPacket
{
    public AuthRequestPacket(byte[] rsaBlock) : base((byte)LoginClientPacketType.AuthRequest)
    {
        WriteB(rsaBlock);
        BuildPacket();
    }
}