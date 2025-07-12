using UnityEngine;
using System;
public class RequestMoveDirectionPacket : ClientPacket
{

    public RequestMoveDirectionPacket(Vector3 direction, int heading, float verticalVelocity, Vector3 position, bool requireResponse) : base((byte)GameClientPacketType.RequestMoveDirection)
    {
        WriteB(requireResponse);
        WriteD(direction.x);
        WriteD(direction.z);
        WriteI(heading);
        WriteD(verticalVelocity);
        WriteI((int)(position.z * 52.5f));
        WriteI((int)(position.x * 52.5f));
        WriteI((int)(position.y * 52.5f));
        WriteL(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        // Debug.Log($"RequestMoveDirectionPacket: {direction} {heading} {verticalVelocity} {position} {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        BuildPacket();
    }
}
