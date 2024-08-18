using System;
using UnityEngine;

public class ActionAllowedPacket : ServerPacket {
    public PlayerAction PlayerAction { get; private set; }

    public ActionAllowedPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        try {
            PlayerAction = (PlayerAction)ReadB();
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
