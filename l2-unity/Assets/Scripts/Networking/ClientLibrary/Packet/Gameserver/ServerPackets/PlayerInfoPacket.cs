using UnityEngine;
using System;

public class PlayerInfoPacket : ServerPacket {
    public struct PlayerInfo {
        public NetworkIdentity Identity { get; set; }
        public PlayerStatus Status { get; set; }
        public PlayerStats Stats { get; set; }
        public PlayerAppearance Appearance { get; set; }
    }

    private PlayerInfo _info;
    public PlayerInfo PacketPlayerInfo { get { return _info; } }


    public PlayerInfoPacket(byte[] d) : base(d) {
        _info = new PlayerInfo();
        _info.Identity = new NetworkIdentity();
        _info.Status = new PlayerStatus();
        _info.Stats = new PlayerStats();
        _info.Appearance = new PlayerAppearance();
        Parse();
    }

    public override void Parse() {    
        try {
            _info.Identity.Id = ReadI();
            _info.Identity.Name = ReadS();
            _info.Identity.PlayerClass = ReadB();
            _info.Identity.IsMage = ReadB() == 1;
            _info.Identity.Heading = ReadF();
            _info.Identity.SetPosX(ReadF());
            _info.Identity.SetPosY(ReadF());
            _info.Identity.SetPosZ(ReadF());
            _info.Identity.Owned = true;
            // Status
            _info.Stats.Level = ReadI();
            _info.Status.Hp = ReadI();
            _info.Stats.MaxHp = ReadI();
            _info.Status.Mp = ReadI(); 
            _info.Stats.MaxMp = ReadI();
            _info.Status.Cp = ReadI();
            _info.Stats.MaxCp = ReadI();
            // Combat
            _info.Stats.Speed = ReadI();
            _info.Stats.PAtkSpd = ReadI();
            _info.Stats.MAtkSpd = ReadI();
            _info.Stats.AttackRange = ReadF();
            _info.Stats.PAtk = ReadI();
            _info.Stats.PDef = ReadI();
            _info.Stats.PEvasion = ReadI();
            _info.Stats.PAccuracy = ReadI();
            _info.Stats.MEvasion = ReadI();
            _info.Stats.MAccuracy = ReadI();
            _info.Stats.PCritical = ReadI();
            _info.Stats.MCritical = ReadI();
            _info.Stats.MAtk = ReadI();
            _info.Stats.MDef = ReadI();
            // Stats
            _info.Stats.Con = ReadB();
            _info.Stats.Dex = ReadB();
            _info.Stats.Str = ReadB();
            _info.Stats.Men = ReadB();
            _info.Stats.Wit = ReadB();
            _info.Stats.Int = ReadB();
            _info.Stats.Sp = ReadI();
            _info.Stats.Exp = ReadI();
            _info.Stats.MaxExp = ReadI();
            _info.Stats.CurrWeight = ReadI();
            _info.Stats.MaxWeight = ReadI();
            // Social
            _info.Stats.PkKills = ReadI();
            _info.Stats.PvpKills = ReadI();
            _info.Status.PvpFlag = ReadL();
            _info.Stats.Karma = ReadI();
            // Appearance
            _info.Appearance.CollisionHeight = ReadF();
            _info.Appearance.CollisionRadius = ReadF();
            _info.Appearance.Race = ReadB();
            _info.Appearance.Sex = ReadB();
            _info.Appearance.Face = ReadB();
            _info.Appearance.HairStyle = ReadB();
            _info.Appearance.HairColor = ReadB();
            // Gear
            _info.Appearance.LHand = ReadI();
            _info.Appearance.RHand = ReadI();
            _info.Appearance.Chest = ReadI();
            _info.Appearance.Legs = ReadI();
            _info.Appearance.Gloves = ReadI();
            _info.Appearance.Feet = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}