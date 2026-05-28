using System;
using UnityEngine;

[Obsolete]
public class GameTimePacket : ServerPacket
{
    public long GameTicks { get; private set; }
    public int TickDurationMs { get; private set; }
    public int DayDurationMins { get; private set; }

    public GameTimePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
            GameTicks = ReadL();
            TickDurationMs = ReadI();
            DayDurationMins = ReadI();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
