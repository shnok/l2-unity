using UnityEngine;

public class RequestRotatePacket : ClientPacket {
    public RequestRotatePacket(float angle) : base((byte)ClientPacketType.RequestRotate) {
        WriteF(angle); 
        BuildPacket();
    }
}