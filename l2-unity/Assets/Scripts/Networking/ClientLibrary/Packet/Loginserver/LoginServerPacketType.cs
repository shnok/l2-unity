public enum LoginServerPacketType : byte
{
    Ping = 99,
    Init = 0,
    LoginFail = 1,
    AccountKicked = 2,
    LoginOk = 3,
    ServerList = 4,
    PlayFail = 6,
    PlayOk = 7
}

