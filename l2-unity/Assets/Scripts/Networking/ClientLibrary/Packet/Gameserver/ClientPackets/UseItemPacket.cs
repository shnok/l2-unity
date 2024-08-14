using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemPacket : ClientPacket {
    public UseItemPacket(int itemObjectId) : base((byte)GameClientPacketType.UseItem) {
        WriteI(itemObjectId);
        BuildPacket();
    }
}
