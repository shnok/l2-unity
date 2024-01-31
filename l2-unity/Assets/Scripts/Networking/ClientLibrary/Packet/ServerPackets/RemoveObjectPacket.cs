using UnityEngine;
using System;
public class RemoveObjectPacket : ServerPacket {
    public int Id { get; private set; }

    public RemoveObjectPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Id = ReadI();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}