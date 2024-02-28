using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfoPacket : ServerPacket {

    public NetworkIdentity Identity { get; private set; }
    public NpcStatus Status { get; private set; }
    public Stats Stats { get; private set; }
    public Appearance Appearance { get; private set; }

    public NpcInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new NpcStatus();
        Stats = new Stats();
        Appearance = new Appearance();
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
            // Appearance
            Appearance.CollisionHeight = ReadF();
            Appearance.CollisionRadius = ReadF();
            Appearance.LHand = ReadI();
            Appearance.RHand = ReadI();
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
