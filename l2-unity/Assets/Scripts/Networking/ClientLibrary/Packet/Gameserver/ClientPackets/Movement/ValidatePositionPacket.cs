using UnityEngine;

public class ValidatePositionPacket : ClientPacket
{
    public ValidatePositionPacket(Vector3 pos, int heading) : base((byte)GameClientPacketType.ValidatePosition)
    {
        WriteI((int)(pos.z * 52.5f));
        WriteI((int)(pos.x * 52.5f));
        WriteI((int)(pos.y * 52.5f));
        WriteI(heading);

        BuildPacket();
    }
}