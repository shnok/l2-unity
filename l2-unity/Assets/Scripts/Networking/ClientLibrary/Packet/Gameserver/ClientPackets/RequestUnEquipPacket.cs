using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestUnEquipPacket : ClientPacket {
    public RequestUnEquipPacket(int slot) : base((byte)GameClientPacketType.RequestUnEquip) {
        WriteI(slot);
        BuildPacket();
    }
}