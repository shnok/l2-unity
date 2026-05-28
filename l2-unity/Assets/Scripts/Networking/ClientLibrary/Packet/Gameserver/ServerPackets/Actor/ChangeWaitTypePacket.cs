using UnityEngine;

public class ChangeWaitTypePacket : ServerPacket
{
    public int Owner { get; private set; }
    public WaitType MoveType { get; private set; }
    public Vector3 EntityPosition { get; private set; }

    public enum WaitType
    {
        WT_SITTING = 0,
        WT_STANDING = 1,
        WT_START_FAKEDEATH = 2,
        WT_STOP_FAKEDEATH = 3
    }

    public int EntityId { get; private set; }

    public ChangeWaitTypePacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Owner = ReadI();
        MoveType = (WaitType)ReadI();
        Vector3 currentPos = new Vector3();
        currentPos.z = ReadI() / 52.5f;
        currentPos.x = ReadI() / 52.5f;
        currentPos.y = ReadI() / 52.5f;
        EntityPosition = currentPos;
    }
}