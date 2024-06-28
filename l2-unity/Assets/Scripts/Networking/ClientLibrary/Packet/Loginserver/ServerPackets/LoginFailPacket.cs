using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginFailPacket : ServerPacket {
    public enum LoginFailedReason : byte {
        REASON_INVALID_GAME_SERVER_VERSION = 0,
        REASON_IP_BANNED = 1,
        REASON_IP_RESERVED = 2,
        REASON_WRONG_HEXID = 3,
        REASON_ID_RESERVED = 4,
        REASON_NO_FREE_ID = 5,
        NOT_AUTHED = 6,
        REASON_ALREADY_LOGGED_IN = 7
    }

    private LoginFailedReason _loginFailedReason;
    public LoginFailedReason FailedReason { get { return _loginFailedReason; } }


    public LoginFailPacket(byte[] d) : base(d) {
        Parse();
    }

    public override void Parse() {
        _loginFailedReason = (LoginFailedReason)ReadB();
    }
}
