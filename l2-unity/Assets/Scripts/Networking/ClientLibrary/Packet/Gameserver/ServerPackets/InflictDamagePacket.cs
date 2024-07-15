using UnityEngine;
using System;
public class InflictDamagePacket : ServerPacket {
    private Hit[] _hits;
    public int SenderId { get; private set; }
    public Hit[] Hits { get { return _hits; }}

    public InflictDamagePacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {    
        try {
            SenderId = ReadI();

            byte hitCount = ReadB();
            _hits = new Hit[hitCount];

            for (int i = 0; i < hitCount; i++) {
                int targetId = ReadI();
                int damage = ReadI();
                int hitFlags = ReadI();

                Hit hit = new Hit(targetId, damage, hitFlags);
                _hits[i] = hit;
            }

        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}