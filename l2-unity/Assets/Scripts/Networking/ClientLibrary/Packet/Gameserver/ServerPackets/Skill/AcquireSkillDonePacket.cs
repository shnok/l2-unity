public class AcquireSkillDonePacket : ServerPacket
{

    public AcquireSkillDonePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
    }
}
