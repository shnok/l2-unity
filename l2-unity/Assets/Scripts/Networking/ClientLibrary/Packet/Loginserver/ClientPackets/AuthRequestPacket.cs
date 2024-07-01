using System.Text;

public class AuthRequestPacket : ClientPacket {
    public AuthRequestPacket(byte[] rsaBlock) : base((byte)LoginClientPacketType.AuthRequest) {
        WriteB(rsaBlock);
        BuildPacket();
    }
}