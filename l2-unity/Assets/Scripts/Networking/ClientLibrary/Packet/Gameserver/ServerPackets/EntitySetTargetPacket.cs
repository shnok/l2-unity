using System;
using UnityEngine;

public class EntitySetTargetPacket : ServerPacket {
    public int EntityId { get; private set; }
    public int TargetId { get; private set; }

    public EntitySetTargetPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            EntityId = ReadI();
            TargetId = ReadI();
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
