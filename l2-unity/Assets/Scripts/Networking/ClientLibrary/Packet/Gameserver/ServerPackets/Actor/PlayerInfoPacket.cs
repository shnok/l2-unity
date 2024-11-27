using UnityEngine;
using System;

public class PlayerInfoPacket : ServerPacket
{
    public NetworkIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }
    public PlayerStats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }
    public EntityActionInfo EntityActionInfo { get; private set; }
    public int InventorySpace { get; private set; }

    public PlayerInfoPacket(byte[] d) : base(d)
    {
        Identity = new NetworkIdentity();
        Status = new PlayerStatus();
        Stats = new PlayerStats();
        Appearance = new PlayerAppearance();
        EntityActionInfo = new EntityActionInfo();
        Parse();
    }

    public override void Parse()
    {
        try
        {
            Identity.SetPosZ(ReadI() / 52.5f);
            Identity.SetPosX(ReadI() / 52.5f);
            Identity.SetPosY(ReadI() / 52.5f);
            Identity.Heading = ReadI();
            Identity.Id = ReadI();
            Identity.Name = ReadS();
            Appearance.Race = (byte)ReadI();
            Appearance.Sex = (byte)ReadI();
            Identity.PlayerClass = (byte)ReadI();
            Stats.Level = ReadI();
            Stats.Exp = (int)ReadL();
            Stats.ExpPercent = (float)ReadD();
            Stats.Str = (byte)ReadI();
            Stats.Dex = (byte)ReadI();
            Stats.Con = (byte)ReadI();
            Stats.Int = (byte)ReadI();
            Stats.Wit = (byte)ReadI();
            Stats.Men = (byte)ReadI();
            Stats.MaxHp = ReadI();
            Status.Hp = ReadI();
            Stats.MaxMp = ReadI();
            Status.Mp = ReadI();
            Stats.Sp = ReadI();
            Stats.CurrWeight = ReadI();
            Stats.MaxWeight = ReadI();
            ReadI(); //Active Weapon Item

            ReadI(); //HairAll?
            ReadI(); //Rear
            ReadI(); //Lear
            ReadI(); //Neck
            ReadI(); //Rfinger
            ReadI(); //Lfinger
            ReadI(); //Head
            Appearance.RHand = ReadI();
            Appearance.LHand = ReadI();
            Appearance.Gloves = ReadI();
            Appearance.Chest = ReadI();
            Appearance.Legs = ReadI();
            Appearance.Feet = ReadI();
            ReadI(); //Cloak
            Appearance.RHand = ReadI();
            ReadI(); //Hair
            ReadI(); //Face

            ReadI(); //HairAll?
            ReadI(); //Rear
            ReadI(); //Lear
            ReadI(); //Neck
            ReadI(); //Rfinger
            ReadI(); //Lfinger
            ReadI(); //Head
            Appearance.RHand = ReadI();
            Appearance.LHand = ReadI();
            Appearance.Gloves = ReadI();
            Appearance.Chest = ReadI();
            Appearance.Legs = ReadI();
            Appearance.Feet = ReadI();
            ReadI(); //Cloak
            Appearance.RHand = ReadI();
            ReadI(); //Hair
            ReadI(); //Face

            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI(); // AugmentationId RHAND
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI(); // AugmentationId LHAND
            ReadI();
            ReadI();

            Stats.PAtk = ReadI();
            Stats.PAtkSpd = ReadI();
            Stats.PDef = ReadI();
            Stats.PEvasion = ReadI();
            Stats.PAccuracy = ReadI();
            Stats.PCritical = ReadI();
            Stats.MAtk = ReadI();
            Stats.MAtkSpd = ReadI();
            Stats.PAtkSpd = ReadI();
            Stats.MDef = ReadI();
            Identity.PvpFlag = ReadI(); // pvp flag
            Stats.Karma = ReadI(); // karma

            Stats.RunSpeed = ReadI();
            Stats.WalkSpeed = ReadI();
            ReadI(); // swim speed
            ReadI(); // swim speed
            ReadI();
            ReadI();
            ReadI(); //RunSpeed
            ReadI(); //WalkSpeed

            Stats.MoveSpeedMultiplier = (float)ReadD();
            Stats.AttackSpeedMultiplier = (float)ReadD();

            Appearance.CollisionRadius = (float)ReadD() / 52.5f;
            Appearance.CollisionHeight = (float)ReadD() / 52.5f;

            Appearance.HairStyle = (byte)ReadI();
            Appearance.HairColor = (byte)ReadI();
            Appearance.Face = (byte)ReadI();
            ReadI(); // GM

            Identity.Title = ReadS();

            ReadI(); //ClanId
            ReadI(); //ClanCrest
            ReadI(); //Ally
            ReadI(); //AllyCrest
            ReadI(); //Relation
            ReadB(); //MountType
            ReadB(); //OperateType
            ReadB(); //HasCrystallize
            Stats.PkKills = ReadI();
            Stats.PvpKills = ReadI();

            int cubicCount = ReadH();
            for (int i = 0; i < cubicCount; i++)
            {
                ReadH(); //cubic id
            }

            ReadB(); //IsInPartyMatchRoom
            ReadI(); //AbnormalEffect
            ReadB();
            ReadI(); //ClanPrivileges
            ReadH(); //Reco left
            ReadH(); //Reco have
            ReadI(); //MountId
            InventorySpace = ReadH(); //Inventory space
            Identity.PlayerClass = (byte)ReadI();
            ReadI();
            Stats.MaxCp = ReadI();
            Status.Cp = ReadI();
            ReadB(); //EnchantEffect
            ReadB(); //TeamId (Event?)
            ReadI(); //Clan Crest LongId
            ReadB(); //IsNoble
            ReadB(); //Hero/GM Aura
            ReadB(); //IsFishing
            ReadI(); // Fishing Loc X
            ReadI(); // Fishing Loc Y
            ReadI(); // Fishing Loc Z
            Appearance.ServerNameColor = ReadI(); //NameColor
            EntityActionInfo.Running = ReadB() == 1;
            ReadI(); //Pledge class
            ReadI(); //Pledge type
            Appearance.ServerTitleColor = ReadI(); //Title Color
            ReadI(); //Cursed weapon


            Identity.IsMage = CharacterClassParser.IsMage((CharacterClass)Identity.PlayerClass);
            Identity.Owned = Identity.Id == GameClient.Instance.CurrentPlayerId;

            Stats.RunSpeed = (int)(Stats.MoveSpeedMultiplier > 0 ? Stats.RunSpeed * Stats.MoveSpeedMultiplier : Stats.RunSpeed);
            Stats.WalkSpeed = (int)(Stats.MoveSpeedMultiplier > 0 ? Stats.WalkSpeed * Stats.MoveSpeedMultiplier : Stats.WalkSpeed);
            // Stats.PAtkSpd = (int)(Stats.AttackSpeedMultiplier > 0 ? Stats.PAtkSpd * Stats.AttackSpeedMultiplier : Stats.PAtkSpd);
            Stats.AttackRange = ReadI() / 52.5f;

            Debug.LogWarning(ToString());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public override string ToString()
    {
        return $"Identity:\n" +
               $"  Id: {Identity.Id}\n" +
               $"  Position: ({Identity.Position})\n" +
               $"  Heading: {Identity.Heading}\n" +
               $"  Name: {Identity.Name}\n" +
               $"  NameColor: {Appearance.ServerNameColor}\n" +
               $"  Title: {Identity.Title}\n" +
               $"  TitleColor: {Appearance.ServerTitleColor}\n" +
               $"  PlayerClass: {Identity.PlayerClass} (Mage: {Identity.IsMage})\n" +
               $"  Flag: {Identity.PvpFlag}\n" +
               $"  Owned: {Identity.Owned}\n\n" +

               $"Appearance:\n" +
               $"  Race: {Appearance.Race}\n" +
               $"  Sex: {Appearance.Sex}\n" +
               $"  HairStyle: {Appearance.HairStyle}\n" +
               $"  HairColor: {Appearance.HairColor}\n" +
               $"  Face: {Appearance.Face}\n" +
               $"  Collision Radius: {Appearance.CollisionRadius}\n" +
               $"  Collision Height: {Appearance.CollisionHeight}\n" +
               $"  Equipment:\n" +
               $"    RHand: {Appearance.RHand}\n" +
               $"    LHand: {Appearance.LHand}\n" +
               $"    Gloves: {Appearance.Gloves}\n" +
               $"    Chest: {Appearance.Chest}\n" +
               $"    Legs: {Appearance.Legs}\n" +
               $"    Feet: {Appearance.Feet}\n\n" +

               $"Stats:\n" +
               $"  Level: {Stats.Level}\n" +
               $"  Exp: {Stats.Exp}\n" +
               $"  Attributes:\n" +
               $"    Str: {Stats.Str}, Dex: {Stats.Dex}, Con: {Stats.Con}\n" +
               $"    Int: {Stats.Int}, Wit: {Stats.Wit}, Men: {Stats.Men}\n" +
               $"  HP: {Status.Hp}/{Stats.MaxHp}\n" +
               $"  MP: {Status.Mp}/{Stats.MaxMp}\n" +
               $"  CP: {Status.Cp}/{Stats.MaxCp}\n" +
               $"  PAtk: {Stats.PAtk}, PAtkSpd: {Stats.PAtkSpd}\n" +
               $"  PDef: {Stats.PDef}, PEvasion: {Stats.PEvasion}\n" +
               $"  PAccuracy: {Stats.PAccuracy}, PCritical: {Stats.PCritical}\n" +
               $"  MAtk: {Stats.MAtk}, MAtkSpd: {Stats.MAtkSpd}, MDef: {Stats.MDef}\n" +
               $"  RunSpeed: {Stats.RunSpeed}, WalkSpeed: {Stats.WalkSpeed}\n" +
               $"  Speed Multipliers:\n" +
               $"    MoveSpeedMultiplier: {Stats.MoveSpeedMultiplier}\n" +
               $"    AttackSpeedMultiplier: {Stats.AttackSpeedMultiplier}\n" +
               $"  Karma: {Stats.Karma}, PkKills: {Stats.PkKills}, PvpKills: {Stats.PvpKills}\n" +
               $"  CurrWeight: {Stats.CurrWeight}, MaxWeight: {Stats.MaxWeight}\n\n" +

               $"Other:\n" +
               $"  Running: {EntityActionInfo.Running}\n";
    }
}