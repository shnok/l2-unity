public class NormalMessage : ChatMessage
{
    private static MessageType Type = MessageType.ALL;
    public NormalMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#DDDDDD>" + _user + ": " + _message + "</color>";
    }
}