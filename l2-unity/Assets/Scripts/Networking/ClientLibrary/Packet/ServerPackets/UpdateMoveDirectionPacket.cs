using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMoveDirectionPacket : ServerPacket {
    public int Id { get; private set; }
    public int Speed { get; private set; }
    public Vector3 Direction { get; private set; }

    public UpdateMoveDirectionPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            Id = ReadI();
            Speed = ReadI();
            Vector3 dir = new Vector3();
            dir.x = ReadF();
            dir.y = ReadF();
            dir.z = ReadF();
            Direction = dir;
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}