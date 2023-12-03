using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMoveToPacket : ServerPacket {
    private int _id;
    private Vector3 _pos = new Vector3();

    public ObjectMoveToPacket() { }
    public ObjectMoveToPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            _id = ReadI();
            _pos.x = ReadF();
            _pos.y = ReadF();
            _pos.z = ReadF();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public int getId() {
        return _id;
    }

    public Vector3 getPosition() {
        return _pos;
    }
}