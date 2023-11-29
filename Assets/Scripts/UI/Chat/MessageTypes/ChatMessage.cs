using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage : Message
{
    private string user;
    private string message;

    public ChatMessage(string user, string message) : base(MessageType.ChatMessage) {
        this.user = user;
        this.message = message;
    }

    public override string ToString() {
        return "<color=#DDDDDD>" + user + ": " + message + "</color>";
    }
}
