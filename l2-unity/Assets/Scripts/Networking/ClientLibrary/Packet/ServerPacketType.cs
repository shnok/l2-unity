public enum ServerPacketType : byte
{
    Ping = 0,
    AuthResponse = 1,
    MessagePacket = 2,
    SystemMessage = 3,
    PlayerInfo = 4,
    ObjectPosition = 5,
    RemoveObject = 6,
    ObjectRotation = 7,
    ObjectAnimation = 8,
    ApplyDamage = 9,
    NpcInfo = 0x0A,
    ObjectMoveTo = 0x0B,
    UserInfo = 0x0C,
    ObjectMoveDirection = 0x0D,
    GameTime = 0x0E,
    EntitySetTarget = 0x0F,
    AutoAttackStart = 0x10,
    AutoAttackStop = 0x11
}
