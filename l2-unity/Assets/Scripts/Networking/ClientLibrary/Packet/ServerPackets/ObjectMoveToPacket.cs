using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveToPacket : ServerPacket {
    private int id;
    private Vector3 pos = new Vector3();
    private float speed;

    public ObjectMoveToPacket() { }
    public ObjectMoveToPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            id = ReadI();
            pos.x = ReadF();
            pos.y = ReadF();
            pos.z = ReadF();
            speed = ReadF();
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

    public float getSpeed() {
        return speed;
    }
}