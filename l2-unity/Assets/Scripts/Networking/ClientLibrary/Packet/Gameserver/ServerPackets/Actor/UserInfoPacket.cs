using UnityEngine;
using System;

public class UserInfoPacket : ServerPacket
{
    public NetworkIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }
    public Stats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }
    public EntityActionInfo EntityActionInfo { get; set; }

    public UserInfoPacket(byte[] d) : base(d)
    {
        Identity = new NetworkIdentity();
        Status = new PlayerStatus();
        Stats = new Stats();
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

            ReadI(); // boat info

            Identity.Id = ReadI();
            Identity.Name = ReadS();
            Appearance.Race = (byte)ReadI();
            Appearance.Sex = (byte)ReadI();
            Identity.PlayerClass = (byte)ReadI();

            ReadI(); //HairAll?
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
            ReadI(); //rhand augmentationid
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI();
            ReadI(); //lhand augmentationid
            ReadI();
            ReadI();

            Identity.PvpFlag = ReadI(); // pvp flag
            Stats.Karma = ReadI(); // karma

            Stats.MAtkSpd = ReadI();
            Stats.PAtkSpd = ReadI();

            Identity.PvpFlag = ReadI(); // pvp flag
            Stats.Karma = ReadI(); // karma

            Stats.RunSpeed = ReadI();
            Stats.WalkSpeed = ReadI();
            ReadI(); // swim speed
            ReadI(); // swim speed
            ReadI(); //RunSpeed
            ReadI(); //WalkSpeed
            ReadI(); //RunSpeed
            ReadI(); //WalkSpeed

            Stats.MoveSpeedMultiplier = (float)ReadD();
            Stats.AttackSpeedMultiplier = (float)ReadD();

            Appearance.CollisionRadius = (float)ReadD() / 52.5f;
            Appearance.CollisionHeight = (float)ReadD() / 52.5f;

            Appearance.HairStyle = (byte)ReadI();
            Appearance.HairColor = (byte)ReadI();
            Appearance.Face = (byte)ReadI();

            Identity.Title = ReadS();

            ReadI(); //ClanId
            ReadI(); //ClanCrest
            ReadI(); //Ally
            ReadI(); //AllyCrest

            ReadI();

            EntityActionInfo.Sitting = ReadB() == 0;
            EntityActionInfo.Running = ReadB() == 1;
            EntityActionInfo.InCombat = ReadB() == 1;
            EntityActionInfo.AlikeDead = ReadB() == 1;
            EntityActionInfo.Invisible = ReadB() == 1;

            ReadB(); //MountType
            ReadB(); //OperateType

            int cubicCount = ReadH();
            for (int i = 0; i < cubicCount; i++)
            {
                ReadH(); //cubic id
            }

            ReadB(); //IsInPartyMatchRoom
            ReadI(); //AbnormalEffect
            ReadB(); //Reco left
            ReadH(); //Reco have

            Identity.PlayerClass = (byte)ReadI();

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

            Identity.Heading = ReadI();

            ReadI(); //Pledge class
            ReadI(); //Pledge type

            Appearance.ServerTitleColor = ReadI(); //Title Color

            ReadI(); //Cursed weapon

            Identity.IsMage = CharacterClassParser.IsMage((CharacterClass)Identity.PlayerClass);

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
        return $"UserInfoPacket: {{ " +
               $"Identity: {{ ID: {Identity.Id},, Position: {Identity.Position},\n " +
               $"  Name: {Identity.Name}\n" +
               $"  NameColor: {Appearance.ServerNameColor}\n" +
               $"  Title: {Identity.Title}\n" +
               $"  TitleColor: {Appearance.ServerTitleColor}\n" +
               $"  FlagTime: {Identity.PvpFlag}\n" +
               $"Class: {Identity.PlayerClass}, IsMage: {Identity.IsMage}, Heading: {Identity.Heading} }}, " +
               $"Status: {{ CP: {Status.Cp} }}, " +
               $"Stats: {{ Karma: {Stats.Karma}, PAtkSpd: {Stats.PAtkSpd}, MAtkSpd: {Stats.MAtkSpd}, RunSpeed: {Stats.RunSpeed}, " +
               $"WalkSpeed: {Stats.WalkSpeed}, MoveSpeedMultiplier: {Stats.MoveSpeedMultiplier}, AttackSpeedMultiplier: {Stats.AttackSpeedMultiplier}, " +
               $"MaxCp: {Stats.MaxCp}, AttackRange: {Stats.AttackRange} }}, " +
               $"Appearance: {{ Race: {Appearance.Race}, Sex: {Appearance.Sex}, HairStyle: {Appearance.HairStyle}, HairColor: {Appearance.HairColor}, " +
               $"Face: {Appearance.Face}, CollisionRadius: {Appearance.CollisionRadius}, CollisionHeight: {Appearance.CollisionHeight}, " +
               $"RHand: {Appearance.RHand}, LHand: {Appearance.LHand}, Gloves: {Appearance.Gloves}, Chest: {Appearance.Chest}, " +
               $"Legs: {Appearance.Legs}, Feet: {Appearance.Feet} }}, " +
               $"Running: {EntityActionInfo.Running}, Sitting: {EntityActionInfo.Sitting}, InCombat: {EntityActionInfo.InCombat}, AlikeDead: {EntityActionInfo.AlikeDead}, Invisible: {EntityActionInfo.Invisible} " +
               $"}}";
    }
}