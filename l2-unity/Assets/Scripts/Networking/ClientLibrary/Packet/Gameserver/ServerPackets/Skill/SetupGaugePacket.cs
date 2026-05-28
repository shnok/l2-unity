using UnityEngine;

public class SetupGaugePacket : ServerPacket
{
    public enum GaugeColor
    {
        BLUE = 0,
        RED = 1,
        CYAN = 2,
        GREEN = 3
    }

    public GaugeColor Color { get; private set; }
    public int Time { get; private set; }
    public int MaxTime { get; private set; }

    public SetupGaugePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Color = (GaugeColor)ReadI();
        Time = ReadI();
        MaxTime = ReadI();

        Debug.Log(ToString());
    }

    public override string ToString()
    {
        return $"Setup Gauge \n Time: {Time} MaxTime: {MaxTime} Color: {Color}";
    }
}