using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginOkPacket : ServerPacket {
    public LoginOkPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
    }
}