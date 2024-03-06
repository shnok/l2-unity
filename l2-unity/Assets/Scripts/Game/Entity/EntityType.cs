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
        if(type.Contains("LineageNPC")) {
            return EntityType.NPC;
        } else {
            return EntityType.Monster;
        }
    }
}
