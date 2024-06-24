using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginClientPacketType : byte {
    Ping = 0x00,
    AuthRequest = 0x01
}
