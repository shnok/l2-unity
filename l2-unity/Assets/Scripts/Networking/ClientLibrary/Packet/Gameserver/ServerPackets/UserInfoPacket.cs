using UnityEngine;
using System;

public class UserInfoPacket : ServerPacket
{
    public NetworkIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }
    public Stats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }
    public bool Running { get; set; }

    public UserInfoPacket(byte[] d) : base(d)
    {
        Identity = new NetworkIdentity();
        Status = new PlayerStatus();
        Stats = new Stats();
        Appearance = new PlayerAppearance();
        Parse();
    }

    public override void Parse()
    {
        try
        {
            Identity.Id = ReadI();
            Identity.Name = ReadS();
            Identity.PlayerClass = ReadB();
            Identity.IsMage = ReadB() == 1;
            Identity.Heading = ReadF();
            Identity.SetPosX(ReadF());
            Identity.SetPosY(ReadF());
            Identity.SetPosZ(ReadF());
            Identity.Owned = Identity.Name == GameClient.Instance.CurrentPlayer;
            // Status
            Stats.Level = ReadI();
            Status.Hp = ReadI();
            Stats.MaxHp = ReadI();
            // Stats
            Stats.RunSpeed = ReadI();
            Stats.WalkSpeed = ReadI();
            Stats.PAtkSpd = ReadI();
            Stats.MAtkSpd = ReadI();
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

            Running = ReadI() == 1;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}