using UnityEngine;
using System;

public class UpdateAnimationPacket : ServerPacket {
    private int id;
    private byte animId;
    private float value;

    public UpdateAnimationPacket(){}
    public UpdateAnimationPacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            id = ReadI();
            animId = ReadB();
            value = ReadF();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return id;
    }

    public byte getAnimId() {
        return animId;
    }

    public float getValue() {
        return value;
    }
}