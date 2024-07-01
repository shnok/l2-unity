using System;
using UnityEngine;

public class AutoAttackStopPacket : ServerPacket {
    public int EntityId { get; private set; }

    public AutoAttackStopPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            EntityId = ReadI();
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
