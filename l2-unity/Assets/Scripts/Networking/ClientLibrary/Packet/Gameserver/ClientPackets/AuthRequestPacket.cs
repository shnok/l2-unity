using System.Text;

public class AuthRequestPacket : ClientPacket {
    public AuthRequestPacket(string username) : base((byte)GameClientPacketType.AuthRequest) {
        WriteS(username);
        BuildPacket();
    }
}