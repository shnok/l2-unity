using UnityEngine;

public class RelationChangedPacket : ServerPacket
{
    public static int RELATION_PVP_FLAG = 0x00002; // pvp ???
    public static int RELATION_HAS_KARMA = 0x00004; // karma ???
    public static int RELATION_LEADER = 0x00080; // leader
    public static int RELATION_INSIEGE = 0x00200; // true if in siege
    public static int RELATION_ATTACKER = 0x00400; // true when attacker
    public static int RELATION_ALLY = 0x00800; // blue siege icon, cannot have if red
    public static int RELATION_ENEMY = 0x01000; // true when red icon, doesn't matter with blue
    public static int RELATION_MUTUAL_WAR = 0x08000; // double fist
    public static int RELATION_1SIDED_WAR = 0x10000; // single fist

    public int Owner { get; private set; }
    public int Relation { get; private set; }
    public bool AutoAttackable { get; private set; }
    public int Karma { get; private set; }
    public int PvpFlag { get; private set; }

    public RelationChangedPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Owner = ReadI();
        Relation = ReadI();
        AutoAttackable = ReadI() == 1;
        Karma = ReadI();
        PvpFlag = ReadI();

        // Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        return $"RelationChanged\n Owner: {Owner}\n Relation: {Relation}\nAutoAttackable: {AutoAttackable}\nKarma: {Karma}\n PvpFlag: {PvpFlag}";
    }
}