using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage
{
    private string _user;
    private string _message;

    public ChatMessage(string user, string message) {
        _user = user;
        _message = message;
    }

    public override string ToString() {
        return "<color=#DDDDDD>" + _user + ": " + _message + "</color>";
    }
}
