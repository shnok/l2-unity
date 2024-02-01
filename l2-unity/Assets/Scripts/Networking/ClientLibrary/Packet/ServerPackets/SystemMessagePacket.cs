using UnityEngine;
using System;

public enum SystemMessageType {
    USER_LOGGED_IN,
    USER_LOGGED_OFF
}

public class SystemMessagePacket : ServerPacket {

    private SystemMessageType _type;
    public SystemMessage Message { get; private set; }

    public SystemMessagePacket(byte[] d) : base(d) {
        Parse();
    }
    
    public override void Parse() {    
        try {
            _type = (SystemMessageType)ReadB();

            switch (_type) {
                case SystemMessageType.USER_LOGGED_IN:
                    Message = new MessageLoggedIn(ReadS());
                    break;
                case SystemMessageType.USER_LOGGED_OFF:
                    Message = new MessageLoggedOut(ReadS());
                    break;
            }            
        } catch(Exception e) {
            Debug.LogError(e);
        }
    }
}