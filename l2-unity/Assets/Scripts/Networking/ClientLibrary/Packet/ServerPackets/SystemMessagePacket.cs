using UnityEngine;
using System;

public enum SystemMessageType {
    USER_LOGGED_IN,
    USER_LOGGED_OFF
}

public class SystemMessagePacket : ServerPacket {

    private SystemMessageType type;
    private SystemMessage message;

    public SystemMessagePacket(){}
    public SystemMessagePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            type = (SystemMessageType)ReadB();

            switch (type) {
                case SystemMessageType.USER_LOGGED_IN:
                    message = new MessageLoggedIn(ReadS());
                    break;
                case SystemMessageType.USER_LOGGED_OFF:
                    message = new MessageLoggedOut(ReadS());
                    break;
            }            
        } catch(Exception e) {
            Debug.Log(e);
        }
    }

    public SystemMessage GetMessage() {
        return message;
    }
}