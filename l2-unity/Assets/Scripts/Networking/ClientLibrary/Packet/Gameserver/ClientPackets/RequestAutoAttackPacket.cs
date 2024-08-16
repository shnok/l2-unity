public class RequestAutoAttackPacket : ClientPacket {
    public RequestAutoAttackPacket() : base((byte)GameClientPacketType.RequestAutoAttack) {
        BuildPacket();
    }
}
