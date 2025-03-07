
public class MagicSkillLaunchedPacked : ServerPacket
{
    public int ObjectId { get; private set; }
    public int SkillId { get; private set; }
    public int SkillLevel { get; private set; }
    public int TargetCount { get; private set; }
    public int[] Targets { get; private set; }

    public MagicSkillLaunchedPacked(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        ObjectId = ReadI();
        SkillId = ReadI();
        SkillLevel = ReadI();

        TargetCount = ReadI();
        if (TargetCount == 0)
        {
            ReadI();
            return;
        }

        Targets = new int[TargetCount];
        for (int i = 0; i < TargetCount; i++)
        {
            Targets[i] = ReadI();
        }
    }

    public override string ToString()
    {
        return $"MagicSkillUsePacket:\n" +
               $"  ObjectId: {ObjectId}\n" +
               $"  SkillId: {SkillId}\n" +
               $"  SkillLevel: {SkillLevel}\n" +
               $"  TargetCount: {TargetCount}";
    }
}
