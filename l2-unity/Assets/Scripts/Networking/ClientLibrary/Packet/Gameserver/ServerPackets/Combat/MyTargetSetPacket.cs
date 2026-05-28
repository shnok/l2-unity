using System;
using UnityEngine;

public class MyTargetSetPacket : ServerPacket
{
    public int TargetId { get; private set; }
    public int Color { get; private set; }

    public MyTargetSetPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        TargetId = ReadI();
        Color = ReadH();
    }
}
