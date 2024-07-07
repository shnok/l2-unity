using UnityEngine;
using System;

public class UserInfoPacket : ServerPacket {
    public NetworkIdentity Identity { get; private set; }
    public Status Status { get; private set; }
    public Stats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }

    public UserInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new Status();
        Stats = new Stats();
        Appearance = new PlayerAppearance();
        Parse();
    }
    
    public override void Parse() {    
        try {
            Debug.Log(0);
            Identity.Id = ReadI();
            Debug.Log(0);
            Identity.Name = ReadS();
            Debug.Log(0);
            Identity.PlayerClass = ReadB();
            Debug.Log(0);
            Identity.IsMage = ReadB() == 1;
            Debug.Log(0);
            Identity.Heading = ReadF();
            Debug.Log(0);
            Identity.SetPosX(ReadF());
            Debug.Log(0);
            Identity.SetPosY(ReadF());
            Debug.Log(0);
            Identity.SetPosZ(ReadF());
            Debug.Log(0);
            Identity.Owned = Identity.Name == GameClient.Instance.CurrentPlayer;
            // Status
            Debug.Log(0);
            Status.Level = ReadI();
            Debug.Log(0);
            Status.Hp = ReadI();
            Debug.Log(0);
            Status.MaxHp = ReadI();
            // Stats
            Debug.Log(0);
            Stats.Speed = ReadI();
            Debug.Log(0);
            Stats.PAtkSpd = ReadI();
            Debug.Log(0);
            Stats.MAtkSpd = ReadI();
            // Appearance
            Debug.Log(0);
            Appearance.CollisionHeight = ReadF();
            Debug.Log(0);
            Appearance.CollisionRadius = ReadF();
            Debug.Log(0);
            Appearance.Race = ReadB();
            Debug.Log(0);
            Appearance.Sex = ReadB();
            Debug.Log(Appearance.Sex);
            Appearance.Face = ReadB();
            Debug.Log(Appearance.Face);
            Appearance.HairStyle = ReadB();
            Debug.Log(Appearance.HairStyle);
            Appearance.HairColor = ReadB();
            Debug.Log(Appearance.HairColor);
            // Gear
            Appearance.LHand = ReadI();
            Debug.Log(Appearance.LHand);
            Appearance.RHand = ReadI();
            Debug.Log(Appearance.RHand);
            Appearance.Chest = ReadI();
            Debug.Log(Appearance.Chest);
            Appearance.Legs = ReadI();
            Debug.Log(Appearance.Legs);
            Appearance.Gloves = ReadI();
            Debug.Log(Appearance.Gloves);
            Appearance.Feet = ReadI();
            Debug.Log(Appearance.Feet);

        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}