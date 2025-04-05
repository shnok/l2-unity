public class AcquireSkillLearnDonePacket : ServerPacket
{

    public AcquireSkillLearnDonePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
    }
}
