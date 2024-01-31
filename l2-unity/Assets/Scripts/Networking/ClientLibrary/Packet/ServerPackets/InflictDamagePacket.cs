using UnityEngine;
using System;
public class InflictDamagePacket : ServerPacket {
    public int SenderId { get; private set; }
    public int TargetId { get; private set; }
    public byte AttackId { get; private set; }
    public int Value { get; private set; }

    public InflictDamagePacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {    
        try {
            SenderId = ReadI();
            TargetId = ReadI();
            AttackId = ReadB();
            Value = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}