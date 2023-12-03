using UnityEngine;
using System;
public class UpdateRotationPacket : ServerPacket {
    private int _id;
    private float _angle;

    public UpdateRotationPacket(){}
    public UpdateRotationPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            _id = ReadI();
            _angle = ReadF();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return _id;
    }

    public float getAngle() {
        return _angle;
    }
}