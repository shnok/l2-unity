public class RequestActionUsePacket : ClientPacket
{

    public RequestActionUsePacket(int action, bool isControlPressed, bool isShiftPressed) : base((byte)GameClientPacketType.RequestActionUse)
    {
        WriteI(action);
        WriteI(isControlPressed ? 1 : 0);
        WriteB(isShiftPressed ? (byte)1 : (byte)0);
        BuildPacket();
    }
}
