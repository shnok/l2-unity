using System;
using UnityEngine;

public class ObjectStopMovePacket : ServerPacket
{
    public int Id { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    public int Heading { get; private set; }

    public ObjectStopMovePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
            Id = ReadI();

            Vector3 currentPos = new Vector3();
            currentPos.z = ReadI() / 52.5f;
            currentPos.x = ReadI() / 52.5f;
            currentPos.y = ReadI() / 52.5f;
            CurrentPosition = currentPos;

            Heading = ReadI();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}