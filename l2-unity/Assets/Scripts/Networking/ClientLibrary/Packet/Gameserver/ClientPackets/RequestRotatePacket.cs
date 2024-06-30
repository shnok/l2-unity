using UnityEngine;

public class RequestRotatePacket : ClientPacket {
    public RequestRotatePacket(float angle) : base((byte)GameClientPacketType.RequestRotate) {
        WriteF(angle); 
        BuildPacket();
    }
}