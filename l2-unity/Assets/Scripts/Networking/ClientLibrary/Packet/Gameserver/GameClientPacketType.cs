public enum GameClientPacketType : byte
{
    Ping = 0x00,
    AuthRequest = 0x01,
    SendMessage = 0x02,
    RequestMove = 0x03,
    LoadWorld = 0x04,
    RequestRotate = 0x05,
    RequestAnim = 0x06,
    RequestAttack = 0x07,
    RequestMoveDirection = 0x08,
    RequestSetTarget = 0x09,
    RequestAutoAttack = 0x0A
}
