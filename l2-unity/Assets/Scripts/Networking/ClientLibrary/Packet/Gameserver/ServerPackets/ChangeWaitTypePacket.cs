public class ChangeWaitTypePacket : ServerPacket
{
    public int Owner { get; private set; }
    public WaitType MoveType { get; private set; }
    public float PosX { get; private set; }
    public float PosY { get; private set; }
    public float PosZ { get; private set; }

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
        PosX = ReadF();
        PosY = ReadF();
        PosZ = ReadF();
    }
}