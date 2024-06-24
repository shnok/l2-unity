using UnityEngine;

public class RequestMovePacket : ClientPacket {

    public RequestMovePacket(Vector3 pos) : base((byte)GameClientPacketType.RequestMove) {
        WriteF(pos.x);
        WriteF(pos.y);
        WriteF(pos.z);
        
        BuildPacket();
    }
}