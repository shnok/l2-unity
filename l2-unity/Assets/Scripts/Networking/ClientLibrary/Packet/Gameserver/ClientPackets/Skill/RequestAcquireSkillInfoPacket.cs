public class RequestAcquireSkillInfoPacket : ClientPacket
{
    public RequestAcquireSkillInfoPacket(int skillId, int skillLvl, PacketSkillType skillType) : base((byte)GameClientPacketType.RequestAcquireSkillInfo) {
        WriteI(skillId);
        WriteI(skillLvl);
        WriteI((int)skillType);
        BuildPacket();
    }
}
