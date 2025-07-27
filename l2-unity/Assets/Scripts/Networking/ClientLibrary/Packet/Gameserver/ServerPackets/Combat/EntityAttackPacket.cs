using UnityEngine;
using System;
using System.Linq;
public class InflictDamagePacket : ServerPacket
{
    private Hit[] _hits;
    public int SenderId { get; private set; }
    public Hit[] Hits { get { return _hits; } }
    public Vector3 AttackerPosition { get; private set; }

    public InflictDamagePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        SenderId = ReadI();
        int targetId = ReadI();
        int damage = ReadI();
        byte flags = ReadB();

        Vector3 currentPos = new Vector3();
        currentPos.z = ReadI() / 52.5f;
        currentPos.x = ReadI() / 52.5f;
        currentPos.y = ReadI() / 52.5f;
        AttackerPosition = currentPos;

        int hitCount = ReadH();
        _hits = new Hit[hitCount + 1];
        _hits[0] = new Hit(targetId, damage, flags);

        for (int i = 1; i < hitCount + 1; i++)
        {
            targetId = ReadI();
            damage = ReadI();
            flags = ReadB();
            _hits[i] = new Hit(targetId, damage, flags);
        }

        Debug.LogWarning(ToString());
    }

    public override string ToString()
    {
        var hitsDescription = _hits != null
            ? string.Join(", ", _hits.Select((hit, index) => $"Hit {index}: {hit}"))
            : "No hits";

        return $"InflictDamagePacket: \n" +
               $"  SenderId: {SenderId}\n" +
               $"  AttackerPosition: {AttackerPosition}\n" +
               $"  Hits: [{hitsDescription}]";
    }
}