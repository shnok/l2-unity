public class NormalMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.ALL;
    public NormalMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#DDDDDD>" + _user + ": " + _message + "</color>";
    }
}