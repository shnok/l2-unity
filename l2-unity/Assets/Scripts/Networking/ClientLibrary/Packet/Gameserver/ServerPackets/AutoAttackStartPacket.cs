using System;
using UnityEngine;

public class AutoAttackStartPacket : ServerPacket {
    public int EntityId { get; private set; }

    public AutoAttackStartPacket(byte[] d) : base(d) {
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
