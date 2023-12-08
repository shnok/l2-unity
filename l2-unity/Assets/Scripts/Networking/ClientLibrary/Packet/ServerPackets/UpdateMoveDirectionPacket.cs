using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMoveDirectionPacket : ServerPacket {
    private int id;
    private float speed;
    private Vector3 direction = new Vector3();

    public UpdateMoveDirectionPacket() { }
    public UpdateMoveDirectionPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            id = ReadI();
            speed = ReadF();
            direction.x = ReadF();
            direction.y = ReadF();
            direction.z = ReadF();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return id;
    }

    public float getSpeed() {
        return speed;
    }
    public Vector3 getDirection() {
        return direction;
    }
}