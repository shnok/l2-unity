public class RequestBypassToServerPacket : ClientPacket
{
    public RequestBypassToServerPacket(string htmlCommand) : base((byte)GameClientPacketType.RequestBypassToServer)
    {
        WriteS(htmlCommand);
        BuildPacket();
    }
}