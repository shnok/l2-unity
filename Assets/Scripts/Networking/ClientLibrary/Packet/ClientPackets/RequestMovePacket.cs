using UnityEngine;

public class RequestMovePacket : ClientPacket {

    public RequestMovePacket(Vector3 pos) : base((byte)ClientPacketType.RequestMove) {
        WriteF(pos.x);
        WriteF(pos.y);
        WriteF(pos.z);
        
        BuildPacket();
    }
}