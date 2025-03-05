public class RequestSkillListPacket : ClientPacket
{
    public RequestSkillListPacket() : base((byte)GameClientPacketType.RequestSkillList) {
        BuildPacket();
    }
}
