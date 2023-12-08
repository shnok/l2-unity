using UnityEngine;

public class RequestAnimPacket : ClientPacket {

    public RequestAnimPacket(byte anim, float value) : base((byte)ClientPacketType.RequestAnim) {
        WriteB(anim);
        WriteF(value);

        BuildPacket();
    }
}
