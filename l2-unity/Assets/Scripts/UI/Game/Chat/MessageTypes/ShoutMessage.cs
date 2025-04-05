public class ShoutMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.SHOUT;
#pragma warning restore 0414
    public ShoutMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#FF7200>" + _user + ": " + _message + "</color>";
    }
}