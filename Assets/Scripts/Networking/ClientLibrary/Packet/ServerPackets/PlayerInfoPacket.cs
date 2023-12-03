using UnityEngine;
using System;

public class PlayerInfoPacket : ServerPacket {

    private NetworkIdentity identity = new NetworkIdentity();
    private PlayerStatus status = new PlayerStatus();

    public PlayerInfoPacket(){}
    public PlayerInfoPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            identity.Id = ReadI();
            identity.Name = ReadS();
            identity.SetPosX(ReadF());
            identity.SetPosY(ReadF());
            identity.SetPosZ(ReadF());
            identity.Owned = identity.Name == DefaultClient.GetInstance().username;
            status.Level = ReadI();
            status.Hp = ReadI();
            status.MaxHp = ReadI();
            status.Mp = ReadI(); 
            status.MaxMp = ReadI();
            status.Cp = ReadI();
            status.MaxCp = ReadI();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public NetworkIdentity GetIdentity() {
        return identity;
    }

    public PlayerStatus GetStatus() {
        return status;
    }
}