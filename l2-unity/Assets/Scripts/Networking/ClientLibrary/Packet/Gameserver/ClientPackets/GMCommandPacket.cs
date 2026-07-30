public class GMCommandPacket : ClientPacket
{

    public GMCommandPacket(string command) : base((byte)GameClientPacketType.GMCommand)
    {
        WriteS(command);

        BuildPacket();
    }
}
