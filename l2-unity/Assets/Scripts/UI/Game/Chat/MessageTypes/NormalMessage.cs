public class NormalMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.ALL;
#pragma warning restore 0414
    public NormalMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#DDDDDD>" + _user + ": " + _message + "</color>";
    }
}