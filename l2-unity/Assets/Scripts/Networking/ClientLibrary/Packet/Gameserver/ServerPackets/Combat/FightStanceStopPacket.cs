using System;
using UnityEngine;

public class FightStanceStopPacket : ServerPacket
{
    public int EntityId { get; private set; }

    public FightStanceStopPacket(byte[] d) : base(d)
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
