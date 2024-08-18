public class RequestAttackPacket : ClientPacket {
    public RequestAttackPacket(int targetId, AttackType type) : base((byte)GameClientPacketType.RequestAttack) {
        WriteI(targetId);
        WriteB((byte)type);

        BuildPacket();
    }
}

