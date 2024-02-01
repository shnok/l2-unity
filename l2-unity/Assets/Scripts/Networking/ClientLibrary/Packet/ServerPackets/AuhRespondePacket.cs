using UnityEngine;
using System;

public enum AuthResponse {
    ALLOW,
    ALREADY_CONNECTED,
    INVALID_USERNAME
}

public class AuthResponsePacket : ServerPacket {
    public AuthResponse Response { get; private set; }

    public AuthResponsePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            Response = (AuthResponse)ReadB();
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}