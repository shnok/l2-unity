using UnityEngine;
using System;

public class CharSelectedPacket : ServerPacket
{
    public struct PlayerInfo
    {
        public int CurrentGameTime { get; set; } // Returns game time in minute format (0-1439).
        public NetworkIdentity Identity { get; set; }
        public PlayerStatus Status { get; set; }
        public PlayerStats Stats { get; set; }
        public PlayerAppearance Appearance { get; set; }
        public EntityActionInfo EntityActionInfo { get; set; }
    }

    private PlayerInfo _info;
    public PlayerInfo PacketPlayerInfo { get { return _info; } }


    public CharSelectedPacket(byte[] d) : base(d)
    {
        _info = new PlayerInfo();
        _info.Identity = new NetworkIdentity();
        _info.Status = new PlayerStatus();
        _info.Stats = new PlayerStats();
        _info.Appearance = new PlayerAppearance();
        _info.EntityActionInfo = new EntityActionInfo();

        Parse();
    }

    public override void Parse()
    {
        try
        {
            _info.Identity.Name = ReadS();
            _info.Identity.Id = ReadI();
            _info.Identity.Title = ReadS();
            ReadI(); //sessionId
            ReadI(); //clanId

            ReadI();

            _info.Appearance.Sex = (byte)ReadI();
            _info.Appearance.Race = (byte)ReadI();
            _info.Identity.PlayerClass = (byte)ReadI();

            ReadI();

            _info.Identity.SetPosZ(ReadI() / 52.5f);
            _info.Identity.SetPosX(ReadI() / 52.5f);
            _info.Identity.SetPosY(ReadI() / 52.5f);
            _info.Status.Hp = (int)ReadD();
            _info.Status.Mp = (int)ReadD();
            _info.Stats.Sp = ReadI();
            _info.Stats.Exp = (int)ReadL();
            _info.Stats.Level = ReadI();
            _info.Stats.Karma = ReadI();
            _info.Stats.PkKills = ReadI();
            _info.Stats.Int = (byte)ReadI();
            _info.Stats.Str = (byte)ReadI();
            _info.Stats.Con = (byte)ReadI();
            _info.Stats.Men = (byte)ReadI();
            _info.Stats.Dex = (byte)ReadI();
            _info.Stats.Wit = (byte)ReadI();

            for (int i = 0; i < 30; i++)
            {
                ReadI();
            }

            ReadI();
            ReadI();

            _info.CurrentGameTime = ReadI(); //Game time

            ReadI();

            _info.Identity.PlayerClass = (byte)ReadI();
            _info.Identity.IsMage = CharacterClassParser.IsMage((CharacterClass)_info.Identity.PlayerClass);
            // _info.Identity.Heading = ReadF();
            _info.Identity.Owned = true;

            ReadI();
            ReadI();
            ReadI();
            ReadI();


            // // Status
            // _info.Stats.MaxHp = ReadI();
            // _info.Stats.MaxMp = ReadI();
            // _info.Status.Cp = ReadI();
            // _info.Stats.MaxCp = ReadI();
            // // Combat
            // _info.Stats.RunSpeed = ReadI();
            // _info.Stats.WalkSpeed = ReadI();
            // _info.Stats.PAtkSpd = ReadI();
            // _info.Stats.MAtkSpd = ReadI();
            // _info.Stats.AttackRange = ReadF();
            // _info.Stats.PAtk = ReadI();
            // _info.Stats.PDef = ReadI();
            // _info.Stats.PEvasion = ReadI();
            // _info.Stats.PAccuracy = ReadI();
            // _info.Stats.MEvasion = ReadI();
            // _info.Stats.MAccuracy = ReadI();
            // _info.Stats.PCritical = ReadI();
            // _info.Stats.MCritical = ReadI();
            // _info.Stats.MAtk = ReadI();
            // _info.Stats.MDef = ReadI();
            // // Stats
            // _info.Stats.MaxExp = ReadI();
            // _info.Stats.CurrWeight = ReadI();
            // _info.Stats.MaxWeight = ReadI();
            // // Social
            // _info.Stats.PvpKills = ReadI();
            // _info.Status.PvpFlag = ReadL();
            // // Appearance
            // _info.Appearance.CollisionHeight = ReadF();
            // _info.Appearance.CollisionRadius = ReadF();
            // _info.Appearance.Face = ReadB();
            // _info.Appearance.HairStyle = ReadB();
            // _info.Appearance.HairColor = ReadB();
            // // Gear
            // _info.Appearance.LHand = ReadI();
            // _info.Appearance.RHand = ReadI();
            // _info.Appearance.Chest = ReadI();
            // _info.Appearance.Legs = ReadI();
            // _info.Appearance.Gloves = ReadI();
            // _info.Appearance.Feet = ReadI();

            // _info.Running = ReadI() == 1;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}