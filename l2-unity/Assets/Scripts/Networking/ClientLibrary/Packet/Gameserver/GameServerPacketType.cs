public enum GameServerPacketType : byte
{
    Ping = 0x00,
    Key = 0x01,
    CharSelectionInfo = 0x02,
    MessagePacket = 0x03,
    SystemMessage = 0x04,
    PlayerInfo = 0x05,
    ObjectPosition = 0x06,
    RemoveObject = 0x07,
    ObjectRotation = 0x08,
    ObjectAnimation = 0x09,
    ApplyDamage = 0x0A,
    NpcInfo = 0x0B,
    ObjectMoveTo = 0x0C,
    UserInfo = 0x0D,
    ObjectMoveDirection = 0x0E,
    GameTime = 0x0F,
    EntitySetTarget = 0x10,
    AutoAttackStart = 0x11,
    AutoAttackStop = 0x12,
    ActionFailed = 0x13
}
