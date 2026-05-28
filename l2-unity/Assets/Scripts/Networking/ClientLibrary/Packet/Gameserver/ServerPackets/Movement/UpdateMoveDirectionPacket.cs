using System;
using UnityEngine;

public class UpdateMoveDirectionPacket : ServerPacket
{
    public int Id { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    public float VerticalVelocity { get; private set; }
    public long Timestamp { get; private set; }
    public UpdateMoveDirectionPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        try
        {
            Id = ReadI();
            Vector3 dir = new Vector3();
            dir.x = ReadI() / 100f;
            dir.y = 0; //apply gravity
            dir.z = ReadI() / 100f;
            Direction = dir;
            VerticalVelocity = ReadI() / 100f; // vertical velocity
            Vector3 currentPos = new Vector3();
            currentPos.z = ReadI() / 52.5f;
            currentPos.x = ReadI() / 52.5f;
            currentPos.y = ReadI() / 52.5f;
            Position = currentPos;
            Timestamp = ReadL();


        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}