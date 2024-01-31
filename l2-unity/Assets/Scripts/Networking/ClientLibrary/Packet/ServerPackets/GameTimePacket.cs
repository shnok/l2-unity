using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimePacket : ServerPacket {
    private long gameTicks;
    public long GameTicks { get { return gameTicks; } }
    private int tickDurationMs;
    public int TickDurationMs { get { return tickDurationMs; } }
    private int dayDurationMins;
    public int DayDurationMins { get { return dayDurationMins; } }

    public GameTimePacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            gameTicks = ReadL();
            tickDurationMs = ReadI();
            dayDurationMins = ReadI();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }
}
