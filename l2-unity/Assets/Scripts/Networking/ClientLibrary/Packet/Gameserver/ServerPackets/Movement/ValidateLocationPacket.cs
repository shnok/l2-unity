using System;
using UnityEngine;

public class ValidateLocationPacket : ServerPacket
{
    public int Id { get; private set; }
    public int Speed { get; private set; }
    public int Heading { get; private set; }
    public Vector3 Location { get; private set; }

    public ValidateLocationPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Id = ReadI();
        Vector3 location = new Vector3();
        location.z = ReadI() / 52.5f;
        location.x = ReadI() / 52.5f;
        location.y = ReadI() / 52.5f;
        Location = location;
        Heading = ReadI();
    }
}