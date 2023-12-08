using UnityEngine;
using System;

public enum AuthResponse {
    ALLOW,
    ALREADY_CONNECTED,
    INVALID_USERNAME
}

public class AuthResponsePacket : ServerPacket {
    private AuthResponse response;

    public AuthResponsePacket(){}
    public AuthResponsePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            response = (AuthResponse)ReadB();
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public AuthResponse GetResponse() {
        return response;
    }
}