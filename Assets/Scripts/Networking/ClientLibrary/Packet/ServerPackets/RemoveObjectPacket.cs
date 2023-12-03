using UnityEngine;
using System;
public class RemoveObjectPacket : ServerPacket {
    private int _id;
    public RemoveObjectPacket(){}
    public RemoveObjectPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            _id = ReadI();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return _id;
    }
}