using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestCharSelectPacket : ClientPacket {
    public RequestCharSelectPacket(int slot) : base((byte)GameClientPacketType.RequestCharSelect) {
        WriteB((byte)slot);
        BuildPacket();
    }
}
