using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfoPacket : ServerPacket {

    private NetworkIdentity identity = new NetworkIdentity();
    private NpcStatus status = new NpcStatus();

    public NpcInfoPacket() { }
    public NpcInfoPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            identity.Id = ReadI();
            identity.NpcId = ReadI();
            identity.NpcClass = ReadS();
            identity.Type = ReadS();
            identity.Name = ReadS();
            identity.Title = ReadS();
            identity.Heading = ReadF();
            identity.SetPosX(ReadF());
            identity.SetPosY(ReadF());
            identity.SetPosZ(ReadF());
            identity.CollisionHeight = ReadF();
            status.Level = ReadI();
            status.Hp = ReadI();
            status.MaxHp = ReadI();
            identity.Owned = false;
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public NetworkIdentity GetIdentity() {
        return identity;
    }

    public NpcStatus GetStatus() {
        return status;
    }
}
