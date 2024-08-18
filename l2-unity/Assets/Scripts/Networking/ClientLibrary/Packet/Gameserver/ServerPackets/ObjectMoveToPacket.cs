using System;
using UnityEngine;

public class ObjectMoveToPacket : ServerPacket {
    public int Id { get; private set; }
    public Vector3 Pos { get; private set; }
    public int Speed { get; private set; }
    public bool Walking { get; private set; }

    public ObjectMoveToPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            Id = ReadI();
            Vector3 newPos = new Vector3();
            newPos.x = ReadF();
            newPos.y = ReadF();
            newPos.z = ReadF();
            Pos = newPos;
            Speed = ReadI();
            Walking = ReadB() == 1;
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}