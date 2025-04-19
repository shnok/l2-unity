public class PartyEffectPacket : ServerPacket
{
    public BuffEffect[] Effects;
    public TargetType Type;
    public int ObjectId;
    
    public PartyEffectPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Type = (TargetType)ReadI();
        ObjectId = ReadI();
        int size = ReadI();
        
        Effects = new BuffEffect[size];
        for (int i = 0; i < size; i++)
        {
            int skillId = ReadI();
            int skillLvl = ReadH();
            int duration = ReadI();
            Effects[i] = new BuffEffect(skillId, skillLvl, duration);
        }
    }
}
