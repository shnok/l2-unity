using UnityEngine;
using System;

public class UserInfoPacket : ServerPacket {
    public NetworkIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }

    public UserInfoPacket(byte[] d) : base(d) {
        Identity = new NetworkIdentity();
        Status = new PlayerStatus();
        Parse();
    }
    
    public override void Parse() {    
        try {
            Identity.Id = ReadI();
            Identity.Name = ReadS();
            Identity.SetPosX(ReadF());
            Identity.SetPosY(ReadF());
            Identity.SetPosZ(ReadF());
            Identity.Owned = Identity.Name == DefaultClient.Instance.Username;
            Status.Speed = ReadI();
            Status.PAtkSpd = ReadI();
            Status.MAtkSpd = ReadI();
            Status.Level = ReadI();
            Status.Hp = ReadI();
            Status.MaxHp = ReadI();
            Status.Mp = ReadI();
            Status.MaxMp = ReadI();
            Status.Cp = ReadI();
            Status.MaxCp = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}