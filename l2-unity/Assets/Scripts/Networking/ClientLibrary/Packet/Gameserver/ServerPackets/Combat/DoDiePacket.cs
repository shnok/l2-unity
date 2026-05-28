public class DoDiePacket : ServerPacket
{
    public int EntityId { get; private set; }
    public bool ToVillageAllowed { get; private set; }
    public bool ToClanHallAllowed { get; private set; }
    public bool ToCastleAllowed { get; private set; }
    public bool ToSiegeHQAllowed { get; private set; }
    public bool Sweepable { get; private set; }
    public bool FixedResAllowed { get; private set; }

    public DoDiePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        EntityId = ReadI();
        ToVillageAllowed = ReadI() == 1;
        ToClanHallAllowed = ReadI() == 1;
        ToCastleAllowed = ReadI() == 1;
        ToSiegeHQAllowed = ReadI() == 1;
        Sweepable = ReadI() == 1;
        FixedResAllowed = ReadI() == 1;
    }
}
