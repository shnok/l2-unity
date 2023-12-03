using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestMoveDirectionPacket : ClientPacket {

    public RequestMoveDirectionPacket(float speed, Vector3 pos) : base((byte)ClientPacketType.RequestMoveDirection) {
        WriteF(speed);
        WriteF(pos.x);
        WriteF(pos.y);
        WriteF(pos.z);

        BuildPacket();
    }
}
