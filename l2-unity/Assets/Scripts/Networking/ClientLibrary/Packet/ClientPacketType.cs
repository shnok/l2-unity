public enum ClientPacketType : byte
{
    Ping = 0,
    AuthRequest = 1,
    SendMessage = 2,
    RequestMove = 3,
    LoadWorld = 4,
    RequestRotate = 5,
    RequestAnim = 6,
    RequestAttack = 7,
    RequestMoveDirection = 8,
    RequestSetTarget = 9
}
