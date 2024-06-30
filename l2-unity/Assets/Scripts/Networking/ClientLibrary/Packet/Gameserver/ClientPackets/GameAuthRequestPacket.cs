using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAuthRequestPacket : ClientPacket
{
    public GameAuthRequestPacket(string account, int playKey1, int playKey2, int loginKey1, int loginKey2) : base((byte)GameClientPacketType.AuthRequest) {
        WriteS(account);
        WriteI(playKey1);
        WriteI(playKey2);
        WriteI(loginKey1);
        WriteI(loginKey2);

        BuildPacket();
    }
}
