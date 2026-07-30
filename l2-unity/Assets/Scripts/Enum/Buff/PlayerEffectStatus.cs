public class PlayerBuffStatus
{
    public int Charges { get; private set; }
    public int WeightPenalty { get; private set; }
    public bool IsBlockingAllPlayers { get; private set; }
    public bool IsInsideDangerZone { get; private set; }
    public bool HasGradePenalty { get; private set; }
    public bool HasCharmOfCourage { get; private set; }
    public int DeathPenaltyLvl { get; private set; }
    
    public PlayerBuffStatus(int charges, int weightPenalty, bool isBlockingAllPlayers, bool isInsideDangerZone, 
        bool hasGradePenalty, bool hasCharmOfCourage, int deathPenaltyLvl)
    {
        Charges = charges;
        WeightPenalty = weightPenalty;
        IsBlockingAllPlayers = isBlockingAllPlayers;
        IsInsideDangerZone = isInsideDangerZone;
        HasGradePenalty = hasGradePenalty;
        HasCharmOfCourage = hasCharmOfCourage;
        DeathPenaltyLvl = deathPenaltyLvl;
    }
}