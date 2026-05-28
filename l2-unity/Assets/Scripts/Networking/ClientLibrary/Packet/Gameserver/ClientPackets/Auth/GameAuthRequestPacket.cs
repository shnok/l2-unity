public class GameAuthRequestPacket : ClientPacket
{
    public GameAuthRequestPacket(string account, int playKey1, int playKey2, int loginKey1, int loginKey2) : base((byte)GameClientPacketType.AuthRequest)
    {
        WriteS(account);
        WriteI(loginKey2);
        WriteI(loginKey1);
        WriteI(playKey1);
        WriteI(playKey2);

        BuildPacket();
    }
}
