using UnityEngine;
using System;
public class UpdatePositionPacket : ServerPacket {
    private int id;
    private Vector3 pos = new Vector3();

    public UpdatePositionPacket(){}
    public UpdatePositionPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            id = ReadI();
            pos.x = ReadF();
            pos.y = ReadF();
            pos.z = ReadF();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return id;
    }

    public Vector3 getPosition() {
        return pos;
    }
}