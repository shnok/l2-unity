using UnityEngine;
using System;

public class PlayerInfoPacket : ServerPacket {
    public NetworkIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }
    public PlayerStats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }

    public PlayerInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new PlayerStatus();
        Stats = new PlayerStats();
        Appearance = new PlayerAppearance();
        Parse();
    }

    public override void Parse() {    
        try {
            Identity.Id = ReadI();
            Debug.Log(Identity.Id);
            Identity.Name = ReadS();
            Debug.Log(Identity.Name);
            Identity.PlayerClass = ReadB();
            Debug.Log(Identity.PlayerClass);
            Identity.IsMage = ReadB() == 1;
            Debug.Log(Identity.IsMage);
            Identity.Heading = ReadF();
            Identity.SetPosX(ReadF());
            Identity.SetPosY(ReadF());
            Identity.SetPosZ(ReadF());
            Debug.Log(Identity.Position);
            Identity.Owned = true;
            // Status
            Status.Level = ReadI();
            Status.Hp = ReadI();
            Status.MaxHp = ReadI();
            Status.Mp = ReadI(); 
            Status.MaxMp = ReadI();
            Status.Cp = ReadI();
            Status.MaxCp = ReadI();
            // Stats
            Stats.Speed = ReadI();
            Stats.PAtkSpd = ReadI();
            Stats.MAtkSpd = ReadI();
            Stats.AttackRange = ReadF();
            Stats.Con = ReadB();
            Stats.Dex = ReadB();
            Stats.Str = ReadB();
            Stats.Men = ReadB();
            Stats.Wit = ReadB();
            Stats.Int = ReadB();
            // Appearance
            Appearance.CollisionHeight = ReadF();
            Appearance.CollisionRadius = ReadF();
            Appearance.Race = ReadB();
            Appearance.Sex = ReadB();
            Appearance.Face = ReadB();
            Appearance.HairStyle = ReadB();
            Appearance.HairColor = ReadB();
            // Gear
            Appearance.LHand = ReadI();
            Appearance.RHand = ReadI();
            Appearance.Chest = ReadI();
            Appearance.Legs = ReadI();
            Appearance.Gloves = ReadI();
            Appearance.Feet = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}