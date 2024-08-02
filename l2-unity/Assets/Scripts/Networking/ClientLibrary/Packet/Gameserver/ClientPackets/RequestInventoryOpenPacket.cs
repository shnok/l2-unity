using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestInventoryOpenPacket : ClientPacket
{
    public RequestInventoryOpenPacket() : base((byte)GameClientPacketType.RequestInventoryOpen) {
        BuildPacket();
    }
}
