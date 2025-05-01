using UnityEngine;

public class EtcStatusUpdatePacket : ServerPacket
{
    public int Charges { get; private set; }
    public int WeightPenalty { get; private set; }
    public bool IsBlockingAllPlayers { get; private set; }
    public bool IsInsideDangerZone { get; private set; }
    public bool HasPenalty { get; private set; }
    public bool HasCharmOfCourage { get; private set; }
    public int DeathPenaltyLvl { get; private set; }

    public EtcStatusUpdatePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Charges = ReadI();
        WeightPenalty = ReadI();
        IsBlockingAllPlayers = ReadI() != 0;
        IsInsideDangerZone = ReadI() != 0;
        HasPenalty = ReadI() != 0;
        HasCharmOfCourage = ReadI() != 0;
        DeathPenaltyLvl = ReadI();
        // Debug.LogError($"Received EtcStatusUpdatePacket WeightPenalty: {WeightPenalty}, IsBlockingAllPlayers: {IsBlockingAllPlayers} Charges: {Charges} HasPenalty: {HasPenalty} DeathPenaltyLvl: {DeathPenaltyLvl}");
    }
}