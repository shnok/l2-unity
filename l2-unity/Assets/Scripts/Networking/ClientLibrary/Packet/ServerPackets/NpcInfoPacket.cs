using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfoPacket : ServerPacket {

    public NetworkIdentity Identity { get; private set; }
    public NpcStatus Status { get; private set; }

    public NpcInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new NpcStatus();
        Parse();
    }

    public override void Parse() {
        try {
            Identity.Id = ReadI();
            Identity.NpcId = ReadI();
            Identity.NpcClass = ReadS();
            Identity.Type = ReadS();
            Identity.Name = ReadS();
            Identity.Title = ReadS();
            Identity.Heading = ReadF();
            Identity.SetPosX(ReadF());
            Identity.SetPosY(ReadF());
            Identity.SetPosZ(ReadF());
            Identity.LeftHandId = ReadI();
            Identity.RightHandId = ReadI();
            Identity.CollisionHeight = ReadF();
            Status.Speed = ReadI();
            Status.PAtkSpd = ReadI();
            Status.MAtkSpd = ReadI();
            Status.Level = ReadI();
            Status.Hp = ReadI();
            Status.MaxHp = ReadI();
            Identity.Owned = false;
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}
