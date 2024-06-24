using UnityEngine;
using System;

public class UpdateAnimationPacket : ServerPacket {
    public int Id { get; private set; }
    public byte AnimId { get; private set; }
    public float Value { get; private set; }

    public UpdateAnimationPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Id = ReadI();
            AnimId = ReadB();
            Value = ReadF();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}