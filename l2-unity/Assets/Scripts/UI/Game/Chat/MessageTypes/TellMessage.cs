public class TellMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.TELL;
#pragma warning restore 0414
    public TellMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        // return "<color=#F428A7>" + _user + ": " + _message + "</color>";
        return "<color=#FF00FF>" + _user + ": " + _message + "</color>";
    }
}