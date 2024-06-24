using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMoveDirectionPacket : ClientPacket {

    public RequestMoveDirectionPacket(Vector3 pos) : base((byte)GameClientPacketType.RequestMoveDirection) {
        WriteF(pos.x);
        WriteF(pos.y);
        WriteF(pos.z);

        BuildPacket();
    }
}
