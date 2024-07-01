using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfoPacket : ServerPacket {
    public NetworkIdentity Identity { get; private set; }
    public NpcStatus Status { get; private set; }
    public Stats Stats { get; private set; }

    public NpcInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new NpcStatus();
        Stats = new Stats();
        Parse();
    }

    public override void Parse() {
        try {
            Identity.Id = ReadI();
            Identity.NpcId = ReadI();
            Identity.Heading = ReadF();
            Identity.SetPosX(ReadF());
            Identity.SetPosY(ReadF());
            Identity.SetPosZ(ReadF());
            // Stats
            Stats.Speed = ReadI();
            Stats.PAtkSpd = ReadI();
            Stats.MAtkSpd = ReadI();
            // Status
            Status.Level = ReadI();
            Status.Hp = ReadI();
            Status.MaxHp = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}
