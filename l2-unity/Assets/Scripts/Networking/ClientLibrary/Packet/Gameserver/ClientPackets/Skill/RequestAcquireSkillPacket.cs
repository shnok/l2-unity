public class RequestAcquireSkillPacket : ClientPacket
{
    public RequestAcquireSkillPacket(int skillId, int skillLvl, PacketSkillType skillType) : base((byte)GameClientPacketType.RequestAcquireSkill) {
        WriteI(skillId);
        WriteI(skillLvl);
        WriteI((int)skillType);
        BuildPacket();
    }
}
