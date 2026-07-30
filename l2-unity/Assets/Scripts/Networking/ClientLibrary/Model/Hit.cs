using System.Collections.Generic;

public class Hit
{
    private static int HITFLAG_USESS = 0x10;
    private static int HITFLAG_CRIT = 0x20;
    private static int HITFLAG_SHLD = 0x40;
    private static int HITFLAG_MISS = 0x80;

    private int _targetId;
    private int _damage;
    private int _flags = 0;

    public int TargetId { get { return _targetId; } }
    public int Damage { get { return _damage; } }
    public Entity Attacker { get; set; }
    public Entity Target { get; set; }
    public float HitTime { get; set; }

    public Hit(int targetId, int damage, int flags)
    {
        this._targetId = targetId;
        this._damage = damage;
        this._flags = flags;
    }

    public bool isCrit()
    {
        return (_flags & HITFLAG_CRIT) != 0;
    }

    public bool isMiss()
    {
        return (_flags & HITFLAG_MISS) != 0;
    }

    public bool hasSoulshot()
    {
        return (_flags & HITFLAG_USESS) != 0;
    }

    public bool isShielded()
    {
        return (_flags & HITFLAG_SHLD) != 0;
    }

    public int getSsGrade()
    {
        return _flags & 0x0F;
    }

    public override string ToString()
    {
        var flagDescriptions = new List<string>();
        if (isCrit()) flagDescriptions.Add("Critical");
        if (isMiss()) flagDescriptions.Add("Missed");
        if (hasSoulshot()) flagDescriptions.Add($"Soulshot Grade: {getSsGrade()}");
        if (isShielded()) flagDescriptions.Add("Shielded");

        var flagsSummary = flagDescriptions.Count > 0
            ? string.Join(", ", flagDescriptions)
            : "No flags";

        return $"TargetId: {_targetId}, Damage: {_damage}, Flags: [{flagsSummary}]";
    }
}
