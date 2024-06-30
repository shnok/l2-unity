using UnityEngine;
using System;
public class ReceiveMessagePacket : ServerPacket {
    public string Text { get; private set; }
    public string Sender { get; private set; }

    public ReceiveMessagePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Sender = ReadS();
            Text = ReadS();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}