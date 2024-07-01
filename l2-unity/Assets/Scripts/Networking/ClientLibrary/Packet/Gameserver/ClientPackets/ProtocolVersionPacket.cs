using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtocolVersionPacket : ClientPacket
{
    public ProtocolVersionPacket(int version) : base((byte)GameClientPacketType.ProtocolVersion) {
        WriteI(version);

        BuildPacket();
    }
}
