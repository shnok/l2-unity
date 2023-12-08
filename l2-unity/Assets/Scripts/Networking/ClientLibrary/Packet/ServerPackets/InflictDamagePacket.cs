using UnityEngine;
using System;
public class InflictDamagePacket : ServerPacket {
    private int _senderId;
    private int _targetId;
    private byte _attackId;
    private int _value;

    public InflictDamagePacket(){}
    public InflictDamagePacket(byte[] d) : base(d) {
        Parse();
    }

    public int SenderId { get => _senderId; set => _senderId = value; }
    public int TargetId { get => _targetId; set => _targetId = value; }
    public byte AttackId { get => _attackId; set => _attackId = value; }
    public int Value { get => _value; set => _value = value; }

    public override void Parse() {    
        try {
            _senderId = ReadI();
            _targetId = ReadI();
            _attackId = ReadB();
            _value = ReadI();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }
}