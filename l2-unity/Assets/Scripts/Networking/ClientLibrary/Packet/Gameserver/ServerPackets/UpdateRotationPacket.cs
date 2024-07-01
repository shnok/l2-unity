using UnityEngine;
using System;
public class UpdateRotationPacket : ServerPacket {
    public int Id { get; private set; }
    public float Angle { get; private set; }

    public UpdateRotationPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Id = ReadI();
            Angle = ReadF();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}