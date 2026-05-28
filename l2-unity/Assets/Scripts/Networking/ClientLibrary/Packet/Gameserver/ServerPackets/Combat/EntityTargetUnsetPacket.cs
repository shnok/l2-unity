using System;
using UnityEngine;

public class EntityTargetUnsetPacket : ServerPacket
{
    public int EntityId { get; private set; }
    public Vector3 EntityPosition { get; private set; }

    public EntityTargetUnsetPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        EntityId = ReadI();
        Vector3 currentPos = new Vector3();
        currentPos.z = ReadI() / 52.5f;
        currentPos.x = ReadI() / 52.5f;
        currentPos.y = ReadI() / 52.5f;
        EntityPosition = currentPos;
    }
}
