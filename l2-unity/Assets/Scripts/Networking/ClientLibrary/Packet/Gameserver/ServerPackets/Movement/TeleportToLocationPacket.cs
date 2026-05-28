using UnityEngine;

public class TeleportToLocationPacket : ServerPacket
{
    public int EntityId { get; private set; }
    public Vector3 TeleportTo { get; private set; }
    public bool LoadingScreen { get; private set; }

    public TeleportToLocationPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        EntityId = ReadI();

        Vector3 position = new Vector3();
        position.z = ReadI() / 52.5f;
        position.x = ReadI() / 52.5f;
        position.y = ReadI() / 52.5f;
        TeleportTo = position;

        LoadingScreen = ReadI() == 1;
    }
}