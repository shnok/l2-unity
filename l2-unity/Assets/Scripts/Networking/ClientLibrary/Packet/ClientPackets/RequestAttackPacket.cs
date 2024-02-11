using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RequestAttackPacket : ClientPacket {
    public RequestAttackPacket(int targetId, AttackType type) : base((byte)ClientPacketType.RequestAttack) {
        WriteI(targetId);
        WriteB((byte)type);

        BuildPacket();
    }
}

