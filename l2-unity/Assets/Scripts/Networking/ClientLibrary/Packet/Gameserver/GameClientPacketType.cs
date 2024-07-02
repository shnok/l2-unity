public enum GameClientPacketType : byte
{
    Ping = 0x00,
    ProtocolVersion = 0x01,
    AuthRequest = 0x02,
    SendMessage = 0x03,
    RequestMove = 0x04,
    LoadWorld = 0x05,
    RequestRotate = 0x06,
    RequestAnim = 0x07,
    RequestAttack = 0x08,
    RequestMoveDirection = 0x09,
    RequestSetTarget = 0x0A,
    RequestAutoAttack = 0x0B,
    RequestCharSelect = 0x0C
}
