public class ShoutMessage : ChatMessage
{
    private static MessageType Type = MessageType.SHOUT;
    public ShoutMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#FF7200>" + _user + ": " + _message + "</color>";
    }
}