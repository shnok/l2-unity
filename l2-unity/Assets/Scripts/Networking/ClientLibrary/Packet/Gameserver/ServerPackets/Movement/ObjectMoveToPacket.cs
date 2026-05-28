using System;
using UnityEngine;

public class ObjectMoveToPacket : ServerPacket
{
    public int Id { get; private set; }
    public Vector3 CurrentPosition { get; private set; }
    public Vector3 Destination { get; private set; }

    public ObjectMoveToPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
            Id = ReadI();

            Vector3 destination = new Vector3();
            destination.z = ReadI() / 52.5f;
            destination.x = ReadI() / 52.5f;
            destination.y = ReadI() / 52.5f;

            Vector3 currentPos = new Vector3();
            currentPos.z = ReadI() / 52.5f;
            currentPos.x = ReadI() / 52.5f;
            currentPos.y = ReadI() / 52.5f;

            CurrentPosition = currentPos;
            Destination = destination;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}