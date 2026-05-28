using System;
using UnityEngine;

public class FightStanceStartPacket : ServerPacket
{
    public int EntityId { get; private set; }

    public FightStanceStartPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
            EntityId = ReadI();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
