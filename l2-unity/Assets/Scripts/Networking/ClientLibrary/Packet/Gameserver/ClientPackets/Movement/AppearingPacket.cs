using UnityEngine;

public class AppearingPacket : ClientPacket
{
    public AppearingPacket() : base((byte)GameClientPacketType.Appearing)
    {
        BuildPacket();
    }
}