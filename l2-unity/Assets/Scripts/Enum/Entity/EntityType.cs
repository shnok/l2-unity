public enum EntityType
{
    Player,
    User,
    NPC,
    Monster,
    Item,
    Pawn

}

public static class EntityTypeParser
{
    public static EntityType ParseEntityType(string type)
    {
        if (type.ToLower().Contains("lineagenpc"))
        {
            return EntityType.NPC;
        }
        else
        {
            return EntityType.Monster;
        }
    }
}
