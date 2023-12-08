using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Player,
    User,
    NPC,
    Monster,
    Item

}

public static class EntityTypeParser {
    public static EntityType ParseEntityType(string type) {
        switch(type) {
            case "L2Monster":
                return EntityType.Monster;
            case "L2Npc":
                return EntityType.NPC;
            default:
                return EntityType.NPC;
        }
    }
}
