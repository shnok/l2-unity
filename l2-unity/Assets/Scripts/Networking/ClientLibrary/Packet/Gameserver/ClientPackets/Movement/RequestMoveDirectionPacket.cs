using UnityEngine;

public class RequestMoveDirectionPacket : ClientPacket
{

    public RequestMoveDirectionPacket(Vector3 direction, int heading, float verticalVelocity, bool sharePosition, Vector3 position) : base((byte)GameClientPacketType.RequestMoveDirection)
    {
        WriteD(direction.x);
        WriteD(direction.z);
        WriteI(heading);
        WriteD(verticalVelocity);
        WriteB(sharePosition ? (byte)1 : (byte)0);
        if (sharePosition)
        {
            WriteI((int)(pos.z * 52.5f));
            WriteI((int)(pos.x * 52.5f));
            WriteI((int)(pos.y * 52.5f));
        }
        BuildPacket();
    }
}
