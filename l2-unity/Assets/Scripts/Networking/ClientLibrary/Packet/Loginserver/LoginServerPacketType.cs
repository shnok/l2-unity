using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoginServerPacketType : byte
{
    Ping = 0,
    Init = 1,
    LoginOk = 2,
    LoginFail = 3,
    ServerList = 4,
    AccountKicked = 5
}

