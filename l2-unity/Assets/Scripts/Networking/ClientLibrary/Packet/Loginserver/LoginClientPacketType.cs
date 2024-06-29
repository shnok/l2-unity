using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginClientPacketType : byte {
    Ping = 0x00,
    AuthRequest = 0x01,
    RequestServerList = 0x02,
    RequestServerLogin = 0x03
}
