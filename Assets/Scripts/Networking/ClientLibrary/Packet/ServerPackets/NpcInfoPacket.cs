using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfoPacket : ServerPacket {

    private NetworkIdentity _identity = new NetworkIdentity();
    private NpcStatus _status = new NpcStatus();

    public NpcInfoPacket() { }
    public NpcInfoPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            _identity.Id = ReadI();
            _identity.NpcId = ReadI();
            _identity.SetPosX(ReadF());
            _identity.SetPosY(ReadF());
            _identity.SetPosZ(ReadF());
            _status.Level = ReadI();
            _status.Hp = ReadI();
            _status.MaxHp = ReadI();
            _identity.Owned = false;
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public NetworkIdentity GetIdentity() {
        return _identity;
    }

    public NpcStatus GetStatus() {
        return _status;
    }
}
