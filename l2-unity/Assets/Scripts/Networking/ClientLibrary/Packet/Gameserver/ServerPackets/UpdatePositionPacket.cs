using UnityEngine;
using System;
public class UpdatePositionPacket : ServerPacket {
    public int Id { get; private set; }
    public Vector3 Position { get; private set; }

    public UpdatePositionPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Id = ReadI();
            Vector3 pos = new Vector3();
            pos.x = ReadF();
            pos.y = ReadF();
            pos.z = ReadF();
            Position = pos;
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}