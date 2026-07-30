public class RequestMagicSkillUsePacket : ClientPacket
{
    public RequestMagicSkillUsePacket(int skillId, bool ctrlPressed, bool shiftPressed) : base((byte)GameClientPacketType.RequestMagicSkillUse)
    {
        WriteI(skillId);
        WriteI(ctrlPressed ? 1 : 0);
        WriteB(shiftPressed ? (byte)1 : (byte)0);
        BuildPacket();
    }
}

