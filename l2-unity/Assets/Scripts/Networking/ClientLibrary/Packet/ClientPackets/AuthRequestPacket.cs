using System.Text;

public class AuthRequestPacket : ClientPacket {
    public AuthRequestPacket(string username) : base((byte)ClientPacketType.AuthRequest) {
        WriteS(username);
        BuildPacket();
    }
}