public class TellMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.TELL;
    public TellMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        // return "<color=#F428A7>" + _user + ": " + _message + "</color>";
        return "<color=#FF00FF>" + _user + ": " + _message + "</color>";
    }
}